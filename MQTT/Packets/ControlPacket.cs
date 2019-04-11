using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace MQTT.Packets
{
	public abstract class ControlPacket : MqqtPacket
	{
		public Header PacketHeader { get; protected set; }

		public ControlPacketType Type { get { return PacketHeader.Type;  } }

		public class Header
		{
			static readonly Dictionary<byte, ControlPacketType> _controlPacketIndex;
			public ControlPacketType Type { get; set; }
			public Byte Flags { get; set; }
			public int RemainingLength { get; set; }
			public int Length { get; private set; }

			int _remainingLengthFieldSize;

			static MutexLock _lock;

			static Header()
			{
				_lock = new MutexLock();
				_controlPacketIndex = new Dictionary<byte, ControlPacketType>()
				{
//					{ (byte)ControlPacketType.Reserved1,				ControlPacketType.Reserved1 },
					{ (byte)ControlPacketType.Connect,					ControlPacketType.Connect },
					{ (byte)ControlPacketType.ConnectAcknowledgment,	ControlPacketType.ConnectAcknowledgment },
					{ (byte)ControlPacketType.Publish,					ControlPacketType.Publish },
					{ (byte)ControlPacketType.PublishAcknowledgment,    ControlPacketType.PublishAcknowledgment },
					{ (byte)ControlPacketType.PublishReceived,					ControlPacketType.PublishReceived },
					{ (byte)ControlPacketType.PublishRelease,					ControlPacketType.PublishRelease },
					{ (byte)ControlPacketType.PublishComplete,					ControlPacketType.PublishComplete },
					{ (byte)ControlPacketType.Subscribe,				ControlPacketType.Subscribe },
					{ (byte)ControlPacketType.SubscribeAcknowledgment,	ControlPacketType.SubscribeAcknowledgment },
					{ (byte)ControlPacketType.Unsubscribe,				ControlPacketType.Unsubscribe },
					{ (byte)ControlPacketType.UnsubscribeAcknowledment,					ControlPacketType.UnsubscribeAcknowledment },
					{ (byte)ControlPacketType.PingRequest,				ControlPacketType.PingRequest },
					{ (byte)ControlPacketType.PingResponse,				ControlPacketType.PingResponse },
					{ (byte)ControlPacketType.DisconnectRequest,				ControlPacketType.DisconnectRequest },
//					{ (byte)ControlPacketType.Reserved2,				ControlPacketType.Reserved2 },
				};
			}

			public Header(ControlPacketType type, Byte flags, int remainingLength = 0)
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

				ControlPacketType type;
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

			public byte[] Serialize(ControlPacket packet)
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

			public static bool TryGetControlPacketType(byte b, out ControlPacketType type)
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

		protected ControlPacket(MqttClient client, ControlPacketType type, Byte flags = 0)
			: base(PacketType.MQTT)
		{
			Client = client;
			PacketHeader = new Header(type, flags);
		}

		public static bool TryParse(MqttClient client, byte[] buffer, int length, out ControlPacket packet, out int bytesParsed, out Header header)
		{
			header = null;
			packet = null;
			bool result = false;
			if(Header.TryParse(buffer, length, out header, out bytesParsed))
			{
				switch(header.Type)
				{
					case ControlPacketType.ConnectAcknowledgment:
						result = ConnectAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.PublishAcknowledgment:
						result = PublishAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.PingResponse:
						result = PingResponse.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.SubscribeAcknowledgment:
						result = SubscribeAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.Publish:
						result = PublishMessage.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.PublishRelease:
						result = PublishRelease.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
						break;
					case ControlPacketType.UnsubscribeAcknowledment:
						result = UnsubscribeAcknowledgment.TryParse(client, header, buffer, bytesParsed, length, out packet, out bytesParsed);
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
