using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Encoding;
using RaspiCommon.GraphicalHelp;

namespace RaspiCommon.Devices.Optics
{
	public class ImageAnalysis
	{
		public int ByteArraySize { get { return sizeof(int) + sizeof(int) + sizeof(int) + UTF8.Length(FileNames) + LEDs.ByteArraySize + Candidates.ByteArraySize; } }

		public List<String> FileNames { get; set; }
		public ColoredObjectPositionList LEDs { get; set; }
		public ObjectCandidateList Candidates { get; set; }

		public ImageAnalysis()
			: this(new List<string>(), new ColoredObjectPositionList(), new ObjectCandidateList())
		{
		}

		public ImageAnalysis(List<String> filenames, ColoredObjectPositionList leds, ObjectCandidateList candidates)
		{
			FileNames = filenames;
			LEDs = leds;
			Candidates = candidates;
		}

		public ImageAnalysis(byte[] serialized)
			: this()
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				int fileCount = br.ReadInt32();
				int ledCount = br.ReadInt32();
				int candidateCount = br.ReadInt32();
				FileNames = new List<String>();
				for(int x = 0;x < fileCount;x++)
				{
					String filename = UTF8.ReadEncoded(br);
					FileNames.Add(filename);
				}
				for(int x = 0;x < ledCount;x++)
				{
					ColoredObjectPosition led = new ColoredObjectPosition(br.ReadBytes(ColoredObjectPosition.ByteArraySize));
					LEDs.Add(led);
				}
				for(int x = 0;x < candidateCount;x++)
				{
					ColoredObjectCandidate candidate = new ColoredObjectCandidate(br.ReadBytes(ColoredObjectCandidate.ByteArraySize));
					Candidates.Add(candidate);
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
				bw.Write(Candidates.Count);
				bw.Write(UTF8.Encode(FileNames));
				bw.Write(LEDs.Serialize());
				bw.Write(Candidates.Serialize());
			}
			return serialized;
		}

		public override string ToString()
		{
			return String.Format("{0} files {1} leds {2} candidates", FileNames.Count, LEDs.Count, Candidates.Count);
		}
	}
}
