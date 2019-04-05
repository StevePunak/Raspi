using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Queueing;
using KanoopCommon.TCP.Clients;
using KanoopCommon.Threading;
using MQTT.Packets;

namespace MQTT
{
	public class MqttClient : ThreadBase
	{
		#region Events

		public event PublishedMessageReceivedHandler InboundSubscribedMessage;
		public event PacketReceivedHandler PacketReceived;
		public event ClientDisconnectedHandler ClientDisconnected;

		#endregion

		#region Constants

		const int BUFFER_QUANTUM = 16384;

		#endregion

		#region Local Enumerations (State etc)

		public enum ClientStates
		{
			Idle,
			Connecting,
			Connected
		}

		#endregion

		class ClientEvent
		{
			public enum EventType
			{
				InboundPacket,
				InboundData
			}

			public EventType Type { get; set; }
			public MqqtPacket Packet { get; set; }
			public byte[] Data { get; set; }
		}

		#region Public Properties and Backing Variables

		public IPv4AddressPort BrokerAddress { get; set; }
		public String ClientID { get; set; }
		public String Topic { get; set; }
		public bool Connected { get; private set; }
		public QOSTypes QOS { get; set; }
		public String UserName { get; set; }
		public String Password { get; set; }
		public TimeSpan KeepAliveInterval { get; set; }
		public bool PersistConenction { get; set; }

		#endregion

		#region Local Member Variables

		TimeSpan SendKeepAliveInterval
		{
			get
			{
				return KeepAliveInterval > TimeSpan.FromSeconds(20)
					? KeepAliveInterval - TimeSpan.FromSeconds(10)
					: TimeSpan.FromSeconds(KeepAliveInterval.TotalSeconds / 2);
			}
		}

		TcpClientClient _client;
		public ClientStates ClientState { get; private set; }

		MemoryQueue<ClientEvent> _eventQueue;

		byte[] _recvBuffer;
		int _bytesInRecvBuffer;

		ControlPacket.Header _receiveHeader;

		MutexEvent _connectEvent;

		MutexLock _listLock;
		Dictionary<UInt16, DateTime> _needsAcknowledgment;
		UInt16 _nextMessageID;

		DateTime _lastKeepAlive;

		#endregion

		#region Constructors

		public MqttClient(String hostAddress, String clientID)
			: this(new IPv4AddressPort(hostAddress), clientID) { }

		public MqttClient(IPAddress address, String clientID)
			: this(new IPv4AddressPort(address, Constants.DefaultBrokerPort), clientID) { }

		public MqttClient(IPv4AddressPort address, String clientID)
			: base(String.Format("MQTT Client: {0}", clientID))
		{
			BrokerAddress = address;
			if(BrokerAddress.Port == 0)
			{
				BrokerAddress.Port = 1883;
			}
			ClientID = clientID;

			Connected = false;
			ClientState = ClientStates.Idle;
			_eventQueue = new MemoryQueue<ClientEvent>();
			_recvBuffer = new byte[BUFFER_QUANTUM];
			_bytesInRecvBuffer = 0;

			_needsAcknowledgment = new Dictionary<UInt16, DateTime>();
			_listLock = new MutexLock();

			_connectEvent = new MutexEvent();

			_nextMessageID = 1;

			KeepAliveInterval = TimeSpan.FromSeconds(60);

			InboundSubscribedMessage += delegate {};
			PacketReceived += delegate {};
			ClientDisconnected += delegate {};
		}

		#endregion

		#region Public Access Methods

		public void Connect()
		{
			Connect(TimeSpan.Zero);
		}

		public bool Connect(TimeSpan waitForAck)
		{
			bool result = true;

			if(Connected)
			{
				throw new MqttException("Already connected");
			}

			// connect transport
			_client = new TcpClientClient();
			_client.DataReceived += OnTcpDataReceived;
			_client.SocketDisconnected += OnSocketDisconnected;
			_client.Connect(BrokerAddress);
			Connected = true;

			// set state
			ClientState = ClientStates.Connecting;

			// start our thread
			Start();

			// send mqtt connect
			ConnectCommand packet = new ConnectCommand(this)
			{
				UserName = UserName,
				Password = Password,
				QOS = QOSTypes.Qos1,
				KeepAliveInterval = KeepAliveInterval
			};

			// if we are blocking, clear the event
			if(waitForAck != TimeSpan.Zero)
			{
				_connectEvent.Clear();
			}

			SendControlPacket(packet);

			// if blocking, wait for allotted time
			if(waitForAck != TimeSpan.Zero)
			{
				result = _connectEvent.Wait(waitForAck);
			}

			return result;
		}

		public void Disconnect()
		{
			if(!Connected)
			{
				throw new MqttException("Can not disconnected while not connected in the first place");
			}

			SendControlPacket(new DisconnectRequest(this));

			// stop our thread
			Stop();

			if(_client != null)
			{
				_client.Disconnect();
			}
			Connected = false;

			ClientState = ClientStates.Idle;
		}

		public void Publish(String topic, String message = null, bool retain = false)
		{
			Publish(topic, ASCIIEncoding.UTF8.GetBytes(message), retain);
		}

		public void Publish(String topic, byte[] message = null, bool retain = false)
		{
			if(!Connected)
			{
				throw new MqttException("MQQT client not connected");
			}

			PublishMessage packet = new PublishMessage(this)
			{
				Topic = topic,
				Message = message == null ? new byte[0] : message,
				QOS = QOS,
				Retain = retain
			};

			if(packet.QOS != QOSTypes.Qos0)
			{
				packet.MessageID = AddNeedsAcknowledgment();
			}

			SendControlPacket(packet);
		}

		public void Subscribe(String topic, QOSTypes qos)
		{
			Subscribe(
				new TopicFilterList()
				{
					new TopicFilter()
					{
						Topic = topic,
						QOS = qos
					}
				}
			);
		}

		public void Subscribe(TopicFilterList topics)
		{
			if(!Connected)
			{
				throw new MqttException("MQQT client not connected");
			}

			SubscribeRequest packet = new SubscribeRequest(this)
			{
				Topics = topics,
			};

			packet.MessageID = AddNeedsAcknowledgment();

			SendControlPacket(packet);
		}

		public void Unsubscribe(String topic, QOSTypes qos)
		{
			Unsubscribe(new List<String>() { topic } );
		}

		public void Unsubscribe(List<String> topics)
		{
			if(!Connected)
			{
				throw new MqttException("MQQT client not connected");
			}

			UnsubscribeRequest packet = new UnsubscribeRequest(this)
			{
				Topics = topics,
			};

			packet.MessageID = AddNeedsAcknowledgment();

			SendControlPacket(packet);
		}

		#endregion

		#region Packet Processors

		private void ProcessConnectAck(ConnectAcknowledgment packet)
		{
			if(packet.ReturnCode == ConnectReturnCode.Accepted)
			{
				ClientState = ClientStates.Connected;
			}
			else
			{
				Disconnect();
			}

			_connectEvent.Set();
		}

		private void ProcessPublishAck(PublishAcknowledgment packet)
		{
			ClearNeedsAcknowledgment(packet.MessageID);
			Log.LogText(LogLevel.DEBUG, "Received {0}. There are {1} outstanding acknowledgments", packet, _needsAcknowledgment.Count);
		}

		private void ProcessUnsubscribeAcknowledment(UnsubscribeAcknowledgment packet)
		{
			ClearNeedsAcknowledgment(packet.MessageID);
			Log.LogText(LogLevel.DEBUG, "Received {0}. There are {1} outstanding acknowledgments", packet, _needsAcknowledgment.Count);
		}

		private void ProcessPingResponse(PingResponse packet)
		{
		}

		private void ProcessSubscribeAcknowledgment(SubscribeAcknowledgment packet)
		{
			ClearNeedsAcknowledgment(packet.MessageID);
		}

		private void ProcessPublish(PublishMessage packet)
		{
			ControlPacket ack = null;
			switch(packet.QOS)
			{
				case QOSTypes.Qos1:
					ack = new PublishAcknowledgment(this)
					{
						MessageID = packet.MessageID,
					};
					break;
				case QOSTypes.Qos2:
					ack = new PublishReceived(this)
					{
						MessageID = packet.MessageID,
					};
					break;
				case QOSTypes.Qos0:
				default:
					break;
			}
			if(ack != null)
			{ 
				SendControlPacket(ack);
			}

			InboundSubscribedMessage(this, packet);
		}

		private void ProcessPublishRelease(PublishRelease packet)
		{
			PublishComplete ack = new PublishComplete(this)
			{
				MessageID = packet.MessageID,
			};
			SendControlPacket(ack);
		}

		#endregion

		#region Socket Handlers

		private void SendControlPacket(ControlPacket packet)
		{
			if(!Connected || _client == null)
			{
				throw new MqttException("Can't send while disconnected");
			}

			byte[] serialized = packet.Serialize();
			_client.Send(serialized);
			_lastKeepAlive = DateTime.UtcNow;
		}

		private void OnTcpDataReceived(byte[] data)
		{
			_eventQueue.Enqueue(
				new ClientEvent()
				{
					Type = ClientEvent.EventType.InboundData,
					Data = data
				}
			);
		}

		void ProcessInboundData(byte[] data)
		{
			if(data.Length + _bytesInRecvBuffer > _recvBuffer.Length)
			{
				int sizeNeeded = (((data.Length + _bytesInRecvBuffer) / BUFFER_QUANTUM) + 1) * BUFFER_QUANTUM;
				Log.SysLogText(LogLevel.ERROR, "Would overflow buffer... increasing size to {0}", sizeNeeded);
				byte[] newBuffer = new byte[sizeNeeded];
				Array.Copy(_recvBuffer, 0, newBuffer, 0, _bytesInRecvBuffer);
				_recvBuffer = newBuffer;
			}

			Array.Copy(data, 0, _recvBuffer, _bytesInRecvBuffer, data.Length);
			_bytesInRecvBuffer += data.Length;

			while(_bytesInRecvBuffer >= 2)
			{
				ControlPacket packet;
				ControlPacket.Header header;
				if(ControlPacket.TryParse(this, _recvBuffer, _bytesInRecvBuffer, out packet, out int bytesParsed, out header))
				{
					RemoveHeadBytes(bytesParsed);
					_eventQueue.Enqueue(
						new ClientEvent()
						{
							Type = ClientEvent.EventType.InboundPacket,
							Packet = packet
						});
				}
				else if(header != null)
				{
					_receiveHeader = header;
					break;
				}
				else
				{
					Log.SysLogText(LogLevel.WARNING, "Removing {0:X2} from buffer", _recvBuffer[0]);
					RemoveHeadBytes(1);
				}
			}

		}

		private void OnSocketDisconnected(TcpClientClient client)
		{
			Log.LogText(LogLevel.INFO, "Client disconnected");
			ClientState = ClientStates.Idle;
			ClientDisconnected(this);
			_client = null;
		}

		private void RemoveHeadBytes(int count)
		{
			Array.Copy(_recvBuffer, count, _recvBuffer, 0, _bytesInRecvBuffer - count);
			_bytesInRecvBuffer -= count;
		}

		#endregion

		#region Thread Overrides

		protected override bool OnRun()
		{
			ClientEvent clientEvent = _eventQueue.BlockDequeue(1000);
			if(clientEvent != null)
			{
				if(clientEvent.Type == ClientEvent.EventType.InboundPacket && clientEvent.Packet is ControlPacket)
				{
					ControlPacket controlPacket = clientEvent.Packet as ControlPacket;
					switch(controlPacket.Type)
					{
						case ControlPacketType.ConnectAcknowledgment:
							ProcessConnectAck(controlPacket as ConnectAcknowledgment);
							break;
						case ControlPacketType.PublishAcknowledgment:
							ProcessPublishAck(controlPacket as PublishAcknowledgment);
							break;
						case ControlPacketType.PingResponse:
							ProcessPingResponse(controlPacket as PingResponse);
							break;
						case ControlPacketType.SubscribeAcknowledgment:
							ProcessSubscribeAcknowledgment(controlPacket as SubscribeAcknowledgment);
							break;
						case ControlPacketType.Publish:
							ProcessPublish(controlPacket as PublishMessage);
							break;
						case ControlPacketType.PublishRelease:
							ProcessPublishRelease(controlPacket as PublishRelease);
							break;
						case ControlPacketType.UnsubscribeAcknowledment:
							ProcessUnsubscribeAcknowledment(controlPacket as UnsubscribeAcknowledgment);
							break;
						case ControlPacketType.Reserved1:
						case ControlPacketType.Connect:
						case ControlPacketType.PublishReceived:
						case ControlPacketType.PublishComplete:
						case ControlPacketType.Subscribe:
						case ControlPacketType.Unsubscribe:
						case ControlPacketType.PingRequest:
						case ControlPacketType.DisconnectRequest:
						case ControlPacketType.Reserved2:
						default:
							Log.SysLogText(LogLevel.DEBUG, "Unprocessed control packet type {0}", controlPacket.Type);
							break;
					}
					PacketReceived(this, clientEvent.Packet);
				}
				else if(clientEvent.Type == ClientEvent.EventType.InboundData)
				{
					ProcessInboundData(clientEvent.Data);
				}

			}

			if(Connected)
			{
				// keep-alive if necessary
				if(Connected && KeepAliveInterval != TimeSpan.Zero && DateTime.UtcNow > _lastKeepAlive + SendKeepAliveInterval)
				{
					SendControlPacket(new PingRequest(this));
					_lastKeepAlive = DateTime.UtcNow;
				}
			}
			else
			{
				if(PersistConenction && Connected == false)
				{
					Connect(TimeSpan.FromSeconds(10));
				}
			}

			return true;
		}

		#endregion

		#region List Handlers

		private UInt16 AddNeedsAcknowledgment()
		{
			UInt16 messageID = 0;
			try
			{
				_listLock.Lock();

				messageID = ++_nextMessageID;
				if(_needsAcknowledgment.ContainsKey(messageID) == false)
				{
					_needsAcknowledgment.Add(messageID, DateTime.UtcNow);
				}
				else
				{
					_needsAcknowledgment[messageID] = DateTime.UtcNow;
				}
			}
			finally
			{
				_listLock.Unlock();
			}
			return messageID;
		}

		private void SetNeedsAcknowledgment(UInt16 messageID)
		{
			try
			{
				_listLock.Lock();

				if(_needsAcknowledgment.ContainsKey(messageID) == false)
				{
					_needsAcknowledgment.Add(messageID, DateTime.UtcNow);
				}
				else
				{
					_needsAcknowledgment[messageID] = DateTime.UtcNow;
				}
			}
			finally
			{
				_listLock.Unlock();
			}
		}

		private void ClearNeedsAcknowledgment(UInt16 messageID)
		{
			try
			{
				_listLock.Lock();

				if(_needsAcknowledgment.ContainsKey(messageID) == true)
				{
					_needsAcknowledgment.Remove(messageID);
				}
			}
			finally
			{
				_listLock.Unlock();
			}
		}

		#endregion

		#region Utility

		public override string ToString()
		{
			return String.Format("{0} {1}", BrokerAddress, Connected ? "Connected" : "Not connected");
		}

		#endregion
	}
}
