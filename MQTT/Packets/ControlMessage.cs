using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace MQTT.Packets
{
	public abstract class ControlMessage : MqqtPacket
	{
		public Header PacketHeader { get; protected set; }

		public ControlMessageType Type { get { return PacketHeader.Type;  } }

		public class Header
		{
			static readonly Dictionary<byte, ControlMessageType> _controlPacketIndex;
			public ControlMessageType Type { get; set; }
			public Byte Flags { get; set; }
			public int RemainingLength { get; set; }
			public int Length { get; private set; }

			int _remainingLengthFieldSize;

			static MutexLock _lock;

			static Header()
			{
				_lock = new MutexLock();
				_controlPacketIndex = new Dictionary<byte, ControlMessageType>()
				{
//					{ (byte)ControlPacketType.Reserved1,				ControlPacketType.Reserved1 },
					{ (byte)ControlMessageType.Connect,					ControlMessageType.Connect },
					{ (byte)ControlMessageType.ConnectAcknowledgment,	ControlMessageType.ConnectAcknowledgment },
					{ (byte)ControlMessageType.Publish,					ControlMessageType.Publish },
					{ (byte)ControlMessageType.PublishAcknowledgment,    ControlMessageType.PublishAcknowledgment },
					{ (byte)ControlMessageType.PublishReceived,					ControlMessageType.PublishReceived },
					{ (byte)ControlMessageType.PublishRelease,					ControlMessageType.PublishRelease },
					{ (byte)ControlMessageType.PublishComplete,					ControlMessageType.PublishComplete },
					{ (byte)ControlMessageType.Subscribe,				ControlMessageType.Subscribe },
					{ (byte)ControlMessageType.SubscribeAcknowledgment,	ControlMessageType.SubscribeAcknowledgment },
					{ (byte)ControlMessageType.Unsubscribe,				ControlMessageType.Unsubscribe },
					{ (byte)ControlMessageType.UnsubscribeAcknowledment,					ControlMessageType.UnsubscribeAcknowledment },
					{ (byte)ControlMessageType.PingRequest,				ControlMessageType.PingRequest },
					{ (byte)ControlMessageType.PingResponse,				ControlMessageType.PingResponse },
					{ (byte)ControlMessageType.DisconnectRequest,				ControlMessageType.DisconnectRequest },
//					{ (byte)ControlPacketType.Reserved2,				ControlPacketType.Reserved2 },
				};
			}

			public Header(ControlMessageType type, Byte flags, int remainingLength = 0)
			{
				Type = type;
				Flags = (Byte)((int)flags & 0x0f);
				RemainingLength = remainingLength;
				Length = 2;
			}

			public static bool TryParse(byte[] buffer, int length, out Header header, out int bytesParsed)
			{
				header = null;
				bytesParsed = 0;

				ControlMessageType type;
				if(TryGetControlPacketType((byte)(buffer[0] >> 4), out type))
				{
					int shift = 0;
					int remainingLength = 0;
					int index = 1;
					byte encodedByte;
					do
					{
						encodedByte = buffer[index];
						remainingLength += ((encodedByte & 0x7f) << shift);
						shift += 7;

						if(++index > 1 + 4)
						{
							throw new MalformedPacketException("Invalid length");
						}

					} while((encodedByte & 0x80) > 0 && index < length);

					if((encodedByte & 0x80) == 0 || index < length)
					{
						header = new Header(type, (byte)(buffer[0] & 0x0f), remainingLength)
						{
							Length = index
						};
						bytesParsed = index;
					}
				}
				return header != null;
			}

			public byte[] Serialize(ControlMessage packet)
			{
				byte[] remainingLengthField = MakeRemainingLengthField();
				byte[] output = new byte[1 + _remainingLengthFieldSize];
				output[0] = (byte)(((int)Type << 4) | (int)packet.MakeFlags());

				for(int x = 0;x < _remainingLengthFieldSize;x++)
				{
					output[x + 1] = remainingLengthField[x];
				}

				return output;
			}

			public static bool TryGetControlPacketType(byte b, out ControlMessageType type)
			{
				bool result = false;
				try
				{
					_lock.Lock();
					result = _controlPacketIndex.TryGetValue(b, out type);
				}
				finally
				{
					_lock.Unlock();
				}
				return result;
			}

			private byte[] MakeRemainingLengthField()
			{
				int bytesOut = 0;
				byte[] output = new byte[4];
				int remainingLength = RemainingLength;
				while(remainingLength > 0)
				{
					int encodedByte = remainingLength & 0x7f;
					remainingLength >>= 7;
					if(remainingLength > 0)
					{
						encodedByte |= 0x80;
					}
					output[bytesOut++] = (byte)encodedByte;
				}

				_remainingLengthFieldSize = Math.Max(bytesOut, 1);
				return output;
			}

			public override string ToString()
			{
				return String.Format("{0} 0x{1:x2}", Type, Flags);
			}
		}

		public int RemainingLength { get; set; }
		public MqttClient Client { get; set; }

		protected ControlMessage(MqttClient client, ControlMessageType type, Byte flags = 0)
			: base(PacketType.MQTT)
		{
			Client = client;
			PacketHeader = new Header(type, flags);
		}

		public static bool TryParse(MqttClient client, byte[] buffer, int length, out ControlMessage packet, out int bytesParsed, out Header header)
		{
			header = null;
			packet = null;
			bool result = false;
			if(Header.TryParse(buffer, length, out header, out bytesParsed))
			{
				switch(header.Type)
				{
					case ControlMessageType.ConnectAcknowledgment:
						result = ConnectAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.PublishAcknowledgment:
						result = PublishAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.PingResponse:
						result = PingResponse.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.SubscribeAcknowledgment:
						result = SubscribeAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.Publish:
						result = PublishMessage.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.PublishRelease:
						result = PublishRelease.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.UnsubscribeAcknowledment:
						result = UnsubscribeAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlMessageType.Reserved1:
					case ControlMessageType.Connect:
					case ControlMessageType.PublishReceived:
					case ControlMessageType.PublishComplete:
					case ControlMessageType.Subscribe:
					case ControlMessageType.Unsubscribe:
					case ControlMessageType.PingRequest:
					case ControlMessageType.DisconnectRequest:
					case ControlMessageType.Reserved2:
					default:
						Log.SysLogText(LogLevel.WARNING, "Unparsable packet type {0}", header.Type);
						break;
				}
			}
			return result;
		}

		public virtual byte[] Serialize()
		{
			PacketHeader.RemainingLength = RemainingLength;
			return PacketHeader.Serialize(this);
		}

		protected abstract byte MakeFlags();
	}

}
