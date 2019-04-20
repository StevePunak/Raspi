using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Encoding;

namespace RaspiCommon.Devices.Optics
{
	public class ImageAnalysis
	{
		public int ByteArraySize { get { return sizeof(int) + sizeof(int) + UTF8.Length(FileNames) + LEDs.ByteArraySize; } }

		public List<String> FileNames { get; set; }
		public LEDPositionList LEDs { get; set; }

		public ImageAnalysis()
			: this(new List<string>(), new LEDPositionList())
		{
		}

		public ImageAnalysis(List<String> filenames, LEDPositionList leds)
		{
			FileNames = filenames;
			LEDs = leds;
		}

		public ImageAnalysis(byte[] serialized)
			: this()
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				int fileCount = br.ReadInt32();
				int ledCount = br.ReadInt32();
				FileNames = new List<String>();
				for(int x = 0;x < fileCount;x++)
				{
					String filename = UTF8.ReadEncoded(br);
					FileNames.Add(filename);
				}
				for(int x = 0;x < ledCount;x++)
				{
					LEDPosition led = new LEDPosition(br.ReadBytes(LEDPosition.ByteArraySize));
					LEDs.Add(led);
				}
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(FileNames.Count);
				bw.Write(LEDs.Count);
				bw.Write(UTF8.Encode(FileNames));
				bw.Write(LEDs.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} files {1} leds", FileNames.Count, LEDs.Count);
		}
	}
}
