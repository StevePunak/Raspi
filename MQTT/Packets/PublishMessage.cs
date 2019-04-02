using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class PublishMessage : ControlPacket
	{
		public String Topic { get; set; }
		public QOSTypes QOS { get; set; }
		public bool Duplicate { get; set; }
		public bool Retain { get; set; }
		public UInt16 MessageID { get; set; }
		public byte[] Message { get; set; }

		public PublishMessage(MqttClient client)
			: base(client, ControlPacketType.Publish)
		{
			Message = new byte[0];
		}

		public static bool TryParse(MqttClient client, Header header, byte[] buffer, int index, int length, out ControlPacket packet, out int bytesParsed)
		{
			packet = null;
			bytesParsed = 0;

			if(length - index >= header.RemainingLength)
			{
				QOSTypes qos = (QOSTypes)((header.Flags >> 1) & 0x03);
				using(BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
				{
					br.ReadBytes(index);        // burn the header
					String topic = Utility.ReadUTF8Encoded(br);
					UInt16 messageID = 0;
					if(qos != QOSTypes.Qos0)
					{
						messageID = (UInt16)IPAddress.NetworkToHostOrder((Int16)br.ReadUInt16());
					}

					int messageLength = header.RemainingLength - Utility.UTF8Length(topic);
					if(qos != QOSTypes.Qos0)
					{
						messageLength -= sizeof(UInt16);
					}
					byte[] messageData = br.ReadBytes(messageLength);

					PublishMessage message = new PublishMessage(client)
					{
						PacketHeader = header,
						MessageID = messageID,
						Topic = topic,
						Message = messageData,
						QOS = qos
					};

					bytesParsed = (int)br.BaseStream.Position;
					packet = message;
				}
			}

			return packet != null;
		}

		public override byte[] Serialize()
		{
			RemainingLength =
				Utility.UTF8Length(Topic) +						// topic length
				(QOS != QOSTypes.Qos0 ? sizeof(UInt16) : 0) +	// 2 more bytes if QOS > 0
				Message.Length;									// payload length
			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			using(MemoryStream ms = new MemoryStream(serialized))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(fixedHeader);
				bw.Write(Utility.EncodeUTF8(Topic));
				if(QOS != QOSTypes.Qos0)
				{
					bw.Write((UInt16)(IPAddress.HostToNetworkOrder((Int16)MessageID)));
				}
				bw.Write(Message);
			}

			return serialized;
		}

		protected override byte MakeFlags()
		{
			int flags = 0;
			if(Duplicate)		flags |= 4;
			if(Retain)			flags |= 1;
			flags |= ((int)QOS << 1);
			return (byte)flags;
		}

		public override string ToString()
		{
			return String.Format("Publish: {0}", Topic);
		}
	}
}
