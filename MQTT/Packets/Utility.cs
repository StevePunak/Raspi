using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	class Utility
	{
		public static byte[] EncodeUTF8(String value)
		{
			byte[] buffer = new byte[value.Length + 2];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(buffer)))
			{
				bw.Write((UInt16)(IPAddress.HostToNetworkOrder((Int16)value.Length)));
				bw.Write(value.ToCharArray(), 0, value.Length);
			}
			return buffer;
		}

		public static byte[] EncodeUTF8(List<String> values)
		{
			byte[] buffer = new byte[UTF8Length(values)];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(buffer)))
			{
				foreach(String value in values)
				{
					bw.Write((UInt16)(IPAddress.HostToNetworkOrder((Int16)value.Length)));
					bw.Write(value.ToCharArray(), 0, value.Length);
				}
			}
			return buffer;
		}

		public static String ReadUTF8Encoded(BinaryReader br)
		{
			int length = IPAddress.NetworkToHostOrder((Int16)br.ReadUInt16());
			String value = ASCIIEncoding.UTF8.GetString(br.ReadBytes(length));
			return value;
		}

		public static int UTF8Length(String value)
		{
			if(value == null)
				return 0;
			else
				return 2 + value.Length;
		}

		public static int UTF8Length(List<String> values)
		{
			int length = values.Sum(v => v.Length) + (values.Count * 2);
			return length;
		}

	}
}
