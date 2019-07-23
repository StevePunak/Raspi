using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.Analog
{
	public class BMP280
	{
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiSetup();

		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CRead(int fd) ;

		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CReadReg8(int fd, int reg) ;
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CReadReg16(int fd, int reg) ;

		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CWrite(int fd, int data) ;
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CWriteReg8(int fd, int reg, int data) ;
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CWriteReg16(int fd, int reg, int data) ;

		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CSetupInterface(IntPtr device, int devId) ;
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiI2CSetup(int devId) ;

		/* BMP280 default address */
		const byte BMP280_I2CADDR 			=	0x77;
		const byte BMP280_CHIPID  			=	0xD0;

		/* BMP280 Registers */
		const byte BMP280_DIG_T1 			= 0x88;  /* R   Unsigned Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_T2 			= 0x8A;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_T3 			= 0x8C;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P1 			= 0x8E;  /* R   Unsigned Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P2 			= 0x90;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P3 			= 0x92;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P4 			= 0x94;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P5 			= 0x96;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P6 			= 0x98;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P7 			= 0x9A;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P8 			= 0x9C;  /* R   Signed Calibration data (16 bits			) =	*/
		const byte BMP280_DIG_P9 			= 0x9E;  /* R   Signed Calibration data (16 bits			) =	*/

		const byte BMP280_CONTROL      			=	0xF4;
		const byte BMP280_RESET        			=	0xE0;
		const byte BMP280_CONFIG       			=	0xF5;
		const byte BMP280_PRESSUREDATA 			=	0xF7;
		const byte BMP280_TEMPDATA     			=	0xFA;

		UInt16 cal_t1 							= 27504;
		Int16 cal_t2 							= 26435;
		Int16 cal_t3 							= -1000;
		UInt16 cal_p1 							= 36477;
		Int16 cal_p2 							= -10685;
		Int16 cal_p3 							= 3024;
		Int16 cal_p4 							= 2855;
		Int16 cal_p5 							= 140;
		Int16 cal_p6 							= -7;
		Int16 cal_p7 							= 15500;
		Int16 cal_p8 							= -14500;
		Int16 cal_p9 							= 6000;

		public void GetTemperature()
		{
			int fd = 0;
			wiringPiSetup();

			if((fd = wiringPiI2CSetup(BMP280_I2CADDR)) < 0)
			{
				Log.SysLogText(LogLevel.ERROR, $"Unable to open I2C device: {fd}");
				return;
			}

			if(0x58 != wiringPiI2CReadReg8(fd, BMP280_CHIPID))
			{
				Log.SysLogText(LogLevel.ERROR, $"Unsupported chip");
				return;
			}

			load_calibration(fd);
			wiringPiI2CWriteReg8(fd, BMP280_CONTROL, 0x3F);

			Log.SysLogText(LogLevel.ERROR, $"Temp: {read_temperature(fd)}");
			Log.SysLogText(LogLevel.ERROR, $"{read_pressure(fd) / 100} hPa\n");
		}

		void load_calibration(int fd)
		{
			cal_t1 = (UInt16)wiringPiI2CReadReg16(fd, BMP280_DIG_T1);
			cal_t2 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_T2);
			// TO DO: double check the value for t3
			//cal_t3 = wiringPiI2CReadReg16(fd, BMP280_DIG_T3);
			cal_p1 = (UInt16)wiringPiI2CReadReg16(fd, BMP280_DIG_P1);
			cal_p2 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P2);
			cal_p3 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P3);
			cal_p4 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P4);
			cal_p5 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P5);
			cal_p6 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P6);
			cal_p7 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P7);
			cal_p8 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P8);
			cal_p9 = (Int16)wiringPiI2CReadReg16(fd, BMP280_DIG_P9);
		}

		int SWAP_2BYTES(int x)
		{
			return (((x & 0xFFFF) >> 8) | ((x & 0xFF) << 8));
		}

		int read_raw(int fd, int reg)
		{
			int raw = SWAP_2BYTES(wiringPiI2CReadReg16(fd, reg));
			raw <<= 8;
			raw = raw | wiringPiI2CReadReg8(fd, reg + 2);
			raw >>= 4;
			return raw;
		}

		int compensate_temp(int raw_temp)
		{
			int t1 = (((raw_temp >> 3) - (cal_t1 << 1)) * (cal_t2)) >> 11;
			int t2 = (((((raw_temp >> 4) - (cal_t1)) * ((raw_temp >> 4) - (cal_t1))) >> 12) * (cal_t3)) >> 14;
			return t1 + t2;
		}

		float read_temperature(int fd)
		{
			int raw_temp = read_raw(fd, BMP280_TEMPDATA);
			int compensated_temp = compensate_temp(raw_temp);
			return (float)((compensated_temp * 5 + 128) >> 8) / 100;
		}

		double read_pressure(int fd)
		{
			int raw_temp = read_raw(fd, BMP280_TEMPDATA);
			int compensated_temp = compensate_temp(raw_temp);
			int raw_pressure = read_raw(fd, BMP280_PRESSUREDATA);

			Int64 p1 = compensated_temp/2 - 64000;
			Int64 p2 = p1 * p1 * (Int64)cal_p6/32768;
			Int64 buf = (p1 * (Int64)cal_p5*2);
			p2 += buf << 17;
			p2 += (Int64)cal_p4 << 35;
			p1 = ((p1 * p1 * cal_p3) >> 8) + ((p1 * cal_p2) << 12);
			p1 = (((Int64)1 << 47) + p1) * ((Int64)cal_p1) >> 33;

			// Avoid exception caused by division by zero
			if(0 == p1)
			{
				return 0;
			}

			Int64 p = 1048576 - raw_pressure;
			p = (((p << 31) - p2) * 3125) / p1;
			p1 = ((Int64)cal_p9 * (p >> 13) * (p >> 13)) >> 25;
			p2 = ((Int64)cal_p8 * p) >> 19;
			p = ((p + p1 + p2) >> 8) + (((Int64)cal_p7) << 4);

			return (double)(p / 256);
		}

	}
}
