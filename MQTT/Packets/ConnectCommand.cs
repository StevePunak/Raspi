using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using KanoopCommon.Encoding;

namespace MQTT.Packets
{
	public class ConnectCommand : ControlMessage
	{
		const String PROTOCOL_NAME =        "MQTT";
		const byte PROTOCOL_VERSION =		0x04;

		const int HDR_FLAG_USER =			0x80;
		const int HDR_FLAG_PASS =			0x40;
		const int HDR_FLAG_WILL_RETAIN =    0x20;
		const int HDR_FLAG_WILL_FLAG =      0x04;
		const int HDR_FLAG_CLEAN_SESSION =  0x02;


		public String ClientID { get; set; }
		public String UserName { get; set; }
		public String Password { get; set; }
		public bool WillRetain { get; set; }
		public QOSTypes QOS { get; set; }
		public String WillTopic { get; set; }
		public String WillMessage { get; set; }
		public QOSTypes WillQOS { get; set; }
		public bool CleanSession { get; set; }
		public TimeSpan KeepAliveInterval { get; set; }

		public ConnectCommand()
			: this(null, null, null, null, false, QOSTypes.Qos0, true, TimeSpan.Zero) { }

		public ConnectCommand(MqttClient client)
			: this(client, null, null, null, false, QOSTypes.Qos0, true, TimeSpan.Zero) { }

		public ConnectCommand(MqttClient client, String willTopic, String user, String password, bool willRetain, QOSTypes willQOS, bool cleanSession, TimeSpan keepAliveInterval)
			: base(client, ControlMessageType.Connect, 0)
		{
			ClientID = Client != null ? Client.ClientID : String.Empty;
			WillTopic = String.IsNullOrEmpty(willTopic) == false ? willTopic : null;
			UserName = String.IsNullOrEmpty(user) == false ? user : null;
			Password = String.IsNullOrEmpty(password) == false ? password : null;
			WillRetain = willRetain;
			WillQOS = willQOS;
			QOS = client.QOS;
			CleanSession = cleanSession;
			KeepAliveInterval = keepAliveInterval;
		}

		protected override byte MakeFlags()
		{
			return 0;
		}

		protected byte MakeConnectFlags()
		{
			int flags = 0;
			if(UserName != null) flags |= HDR_FLAG_USER;
			if(Password != null) flags |= HDR_FLAG_PASS;
			if(WillRetain) flags |= HDR_FLAG_WILL_RETAIN;
			if(WillTopic != null) flags |= HDR_FLAG_WILL_FLAG;
			if(WillQOS != QOSTypes.Qos0) flags |= ((int)WillQOS << 3);
			if(CleanSession) flags |= HDR_FLAG_CLEAN_SESSION;
			return (byte)flags;
		}

		public override byte[] Serialize()
		{
			RemainingLength =
				UTF8.Length(PROTOCOL_NAME) +             // 6
				sizeof(byte) +                                  // 1 for protocol level
				sizeof(byte) +                                  // 1 for connect flags
				sizeof(UInt16) +                                // 2 for keep-alive time
				UTF8.Length(ClientID) +
				UTF8.Length(WillTopic) +
				UTF8.Length(WillMessage) +
				UTF8.Length(UserName) +
				UTF8.Length(Password);

			byte[] fixedHeader = base.Serialize();
			byte[] serialized = new byte[fixedHeader.Length + RemainingLength];

			using(MemoryStream ms = new MemoryStream(serialized))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(fixedHeader);
				bw.Write(UTF8.Encode(PROTOCOL_NAME));
				bw.Write((byte)PROTOCOL_VERSION);
				bw.Write((byte)MakeConnectFlags());
				UInt16 keepAlive = (UInt16)IPAddress.HostToNetworkOrder((Int16)KeepAliveInterval.TotalSeconds);
				bw.Write(keepAlive);

				if(String.IsNullOrEmpty(ClientID))
				{
					throw new MqttException("Client ID must be specified in connect");
				}
				bw.Write(UTF8.Encode(ClientID));

				if(String.IsNullOrEmpty(WillTopic) == false)
					bw.Write(UTF8.Encode(WillTopic));
				if(String.IsNullOrEmpty(WillMessage) == false)
					bw.Write(UTF8.Encode(WillMessage));
				if(String.IsNullOrEmpty(UserName) == false)
					bw.Write(UTF8.Encode(UserName));
				if(String.IsNullOrEmpty(Password) == false)
					bw.Write(UTF8.Encode(Password));
			}

			return serialized;
		}
	}
}
