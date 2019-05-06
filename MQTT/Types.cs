using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MQTT.Packets;

namespace MQTT
{
	public delegate void PublishedMessageReceivedHandler(MqttClient client, PublishMessage packet);
	public delegate void PacketReceivedHandler(MqttClient client, MqqtPacket packet);
	public delegate void ClientDisconnectedHandler(MqttClient client);

	public enum PacketType
	{
		MQTT
	}

	public enum ControlMessageType
	{
		Reserved1 = 0,
		Connect = 1,
		ConnectAcknowledgment = 2,
		Publish = 3,
		PublishAcknowledgment = 4,
		PublishReceived = 5,
		PublishRelease = 6,
		PublishComplete = 7,
		Subscribe = 8,
		SubscribeAcknowledgment = 9,
		Unsubscribe = 10,
		UnsubscribeAcknowledment = 11,
		PingRequest = 12,
		PingResponse = 13,
		DisconnectRequest = 14,
		Reserved2 = 15,
	}

	public enum ConnectReturnCode
	{
		Accepted = 0,
		UnacceptableVersion = 1,
		IdentifierReject = 2,
		ServerUnavailable = 3,
		BadUserPass = 4,
		NotAuthorized = 5
	}

	public enum QOSTypes
	{
		/// <summary>
		/// At most once delivery
		/// </summary>
		Qos0 = 0,
		/// <summary>
		/// At least once delivery
		/// </summary>
		Qos1 = 1,
		/// <summary>
		/// Exactly once delivery
		/// </summary>
		Qos2 = 2,
		/// <summary>
		/// Must not be used
		/// </summary>
		Reserved = 3,
	}
}
