using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Encoding;
using KanoopCommon.Extensions;
using KanoopCommon.Reflection;
using KanoopCommon.Serialization;

namespace RaspiCommon.Devices.Optics
{
	[IsSerializable]
	public class RaspiCameraParameters
	{
		public TimeSpan SnapshotDelay { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ImageType ImageType { get; set; }
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public int Brightness { get; set; }
		public String ImageEffect { get; set; }
		public String ColorEffect { get; set; }
		public String Exposure { get; set; }
		public String MeteringMode { get; set; }
		public String AutoWhiteBalance { get; set; }
		public int Contrast { get; set; }
		public int Saturation { get; set; }

		public int ByteArraySize
		{
			get
			{
				return
					sizeof(UInt64) +
					sizeof(Int32) +
					sizeof(Int32) +
					sizeof(Int32) +
					sizeof(Boolean) +
					sizeof(Boolean) +
					sizeof(Int32) +
					UTF8.Length(ImageEffect) +
					UTF8.Length(ColorEffect) +
					UTF8.Length(Exposure) +
					UTF8.Length(MeteringMode) +
					UTF8.Length(AutoWhiteBalance) +
					sizeof(Int32) +
					sizeof(Int32);
			}
		}

		public RaspiCameraParameters()
		{
			SnapshotDelay = TimeSpan.Zero;
			Width = 800;
			Height = 600;
			ImageType = ImageType.Jpeg;
			FlipHorizontal = true;
			FlipVertical = true;
			Brightness = 0;
			ImageEffect = String.Empty;
			ColorEffect = String.Empty;
			Exposure = String.Empty;
			MeteringMode = String.Empty;
			AutoWhiteBalance = String.Empty;
			Contrast = 0;
			Saturation = 0;
		}

		public RaspiCameraParameters(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				SnapshotDelay = TimeSpanExtensions.Deserialize(br);
				Width = br.ReadInt32();
				Height = br.ReadInt32();
				ImageType = (ImageType)br.ReadInt32();
				FlipHorizontal = br.ReadBoolean();
				FlipVertical = br.ReadBoolean();
				Brightness = br.ReadInt32();
				ImageEffect = UTF8.ReadEncoded(br);
				ColorEffect = UTF8.ReadEncoded(br);
				Exposure = UTF8.ReadEncoded(br);
				MeteringMode = UTF8.ReadEncoded(br);
				AutoWhiteBalance = UTF8.ReadEncoded(br);
				Contrast = br.ReadInt32();
				Saturation = br.ReadInt32();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(SnapshotDelay.Ticks);
				bw.Write(Width);
				bw.Write(Height);
				bw.Write((Int32)ImageType);
				bw.Write(FlipHorizontal);
				bw.Write(FlipVertical);
				bw.Write(Brightness);
				bw.Write(UTF8.Encode(ImageEffect));
				bw.Write(UTF8.Encode(ColorEffect));
				bw.Write(UTF8.Encode(Exposure));
				bw.Write(UTF8.Encode(MeteringMode));
				bw.Write(UTF8.Encode(AutoWhiteBalance));
				bw.Write(Contrast);
				bw.Write(Saturation);
			}
			return serialized;
		}

		public override bool Equals(object obj)
		{
			if(obj is RaspiCameraParameters != true)
			{
				throw new ArgumentException("not equal object");
			}
			RaspiCameraParameters other = obj as RaspiCameraParameters;
			return
				SnapshotDelay == other.SnapshotDelay &&
				Width == other.Width &&
				Height == other.Height &&
				ImageType == other.ImageType &&
				FlipHorizontal == other.FlipHorizontal &&
				FlipVertical == other.FlipVertical &&
				Brightness == other.Brightness &&
				ImageEffect == other.ImageEffect &&
				ColorEffect == other.ColorEffect &&
				Exposure == other.Exposure &&
				MeteringMode == other.MeteringMode &&
				AutoWhiteBalance == other.AutoWhiteBalance &&
				Contrast == other.Contrast &&
				Saturation == other.Saturation;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("Delay: {0} {1}X{2} {3} FH: {4} FV: {5}  Br {6} Effect: {7} Color: {8} Ex {9} Mtr {10} AWB {11} Co {12} Sat {13}",
				SnapshotDelay, Width, Height, ImageType, FlipHorizontal, FlipVertical, Brightness, ImageEffect, ColorEffect, Exposure, MeteringMode, AutoWhiteBalance, Contrast, Saturation);
		}
	}
}
