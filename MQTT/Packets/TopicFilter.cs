using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.Packets
{
	public class TopicFilterList : List<TopicFilter>
	{
		public int SerializedLength { get { return this.Sum(f => f.Length); } }
		public byte[] Serialize()
		{
			byte[] serialized = new byte[SerializedLength];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(TopicFilter filter in this)
				{
					bw.Write(filter.Serialize());
				}
			}
			return serialized;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(TopicFilter filter in this)
			{
				sb.AppendFormat("[{0}] ", filter.Topic);
			}

			return sb.ToString().Trim();
		}
	}

	public class TopicFilter
	{
		public String Topic { get; set; }
		public QOSTypes QOS { get; set; }
		public int Length {  get { return Topic.Length + 3;  } }
		public TopicFilter()
			: this(String.Empty, QOSTypes.Qos0) { }
		public TopicFilter(String topic, QOSTypes qos)
		{
			Topic = topic;
			QOS = qos;
		}
		public byte[] Serialize()
		{
			byte[] output = new byte[Topic.Length + 3];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				bw.Write(Utility.EncodeUTF8(Topic));
				bw.Write((byte)QOS);
			}
			return output;
		}
	}
}
