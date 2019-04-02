using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{

	public class PigCommand
	{
		public CommandType Type { get; set; }
		public UInt32 Parameter1 { get; set; }
		public UInt32 Parameter2 { get; set; }
		public UInt32 Parameter3 { get; set; }

		public PigCommand(CommandType type, GpioPin p1, UInt32 p2)
			: this(type, (UInt32)p1, p2, 0) { }

		public PigCommand(CommandType type, UInt32 p1, UInt32 p2)
			: this(type, p1, p2, 0) { }

		public PigCommand(CommandType type, UInt32 p1, UInt32 p2, UInt32 p3)
		{
			Type = type;
			Parameter1 = p1;
			Parameter2 = p2;
			Parameter3 = p3;
		}

		public byte[] Serialize()
		{
			byte[] buffer = new byte[sizeof(UInt32) * 4];

			using(MemoryStream ms = new MemoryStream(buffer))
			using(BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write((UInt32)Type);
				bw.Write(Parameter1);
				bw.Write(Parameter2);
				bw.Write(Parameter3);
			}

			return buffer;
		}

		public override string ToString()
		{
			return String.Format("{0} {1:X4} {2:X4} {3:X4}", Type, Parameter1, Parameter2, Parameter3);
		}

		public enum CommandType
		{
			MODES =0,
			MODEG =1,
			PUD =2,
			READ =3,
			WRITE =4,
			PWM =5,
			PRS =6,
			PFS =7,
			SERVO =8,
			WDOG =9,
			BR1 =10,
			BR2 =11,
			BC1 =12,
			BC2 =13,
			BS1 =14,
			BS2 =15,
			TICK =16,
			HWVER =17,
			NO =18,
			NB =19,
			NP =20,
			NC =21,
			PRG =22,
			PFG =23,
			PRRG =24,
			HELP =25,
			PIGPV =26,
			WVCLR =27,
			WVAG =28,
			WVAS =29,
			WVGO =30,
			WVGOR =31,
			WVBSY =32,
			WVHLT =33,
			WVSM =34,
			WVSP =35,
			WVSC =36,
			TRIG =37,
			PROC =38,
			PROCD =39,
			PROCR =40,
			PROCS =41,
			SLRO =42,
			SLR =43,
			SLRC =44,
			PROCP =45,
			MICS =46,
			MILS =47,
			PARSE =48,
			WVCRE =49,
			WVDEL =50,
			WVTX =51,
			WVTXR =52,
			WVNEW =53,
			I_CMD_I2CO =54,
			I2CC =55,
			I2CRD =56,
			I2CWD =57,
			I2CWQ =58,
			I2CRS =59,
			I2CWS =60,
			I2CRB =61,
			I2CWB =62,
			I2CRW =63,
			I2CWW =64,
			I2CRK =65,
			I2CWK =66,
			I2CRI =67,
			I2CWI =68,
			I2CPC =69,
			I2CPK =70,
			I_CMD_SPIO =71,
			SPIC =72,
			SPIR =73,
			SPIW =74,
			SPIX =75,
			I_CMD_SERO =76,
			SERC =77,
			SERRB =78,
			SERWB =79,
			SERR =80,
			SERW =81,
			SERDA =82,
			I_CMD_GDC =83,
			GPW =84,
			I_CMD_HC =85,
			HP =86,
			I_CMD_CF1 =87,
			CF2 =88,
			I_CMD_BI2CC =89,
			BI2CO =90,
			BI2CZ =91,
			I_CMD_I2CZ =92,
			I_CMD_WVCHA =93,
			I_CMD_SLRI =94,
			I_CMD_CGI =95,
			CSI =96,
			I_CMD_FG =97,
			FN =98,
			I_CMD_NOIB =99,
			I_CMD_WVTXM =100,
			WVTAT =101,
			I_CMD_PADS =102,
			PADG =103,
			I_CMD_FO =104,
			FC =105,
			FR =106,
			FW =107,
			FS =108,
			FL =109,
			I_CMD_SHELL =110,
			I_CMD_BSPIC =111,
			BSPIO =112,
			BSPIX =113,
			I_CMD_BSCX =114,
			I_CMD_EVM =115,
			EVT =116,
			I_CMD_PROCU =117,
		};

	}
}
