using System;
using System.Text;

namespace Libra
{
    partial class Helper
    {
        /*
         *  Partial Class Helper
         *  Conversion Function
         *  Create Function with Keycode Name 'Conversion'
         *  Example 'ConversionNamefunction()';
         */


        /// <summary>
        /// Convert Drum to Percentage
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double ConversionDrumPercentage(double value = 200, int min = 0, int max = 395)
        {
            double deviation = max - min;
            double actual = value - min;
            double result = ((actual / deviation) * 100);

            return Math.Round(result, 0);
        }


        /// <summary>
        /// Convert Percentage to Drum Voltage
        /// </summary>
        /// <param name="CurrentPercent">Current Percentage</param>
        /// <param name="min">Minimum Drum Voltage</param>
        /// <param name="max">Maximum Drum Voltage</param>
        /// <returns>Voltage</returns>
        public static int ConversionPercentagetoVoltage(int CurrentPercent, int min = 0, int max = 395)
        {
            int totalvoltage = max - min;
            double result = (CurrentPercent / 100.00) * totalvoltage;
            return int.Parse(Math.Round(result, 0).ToString());
        }


        /// <summary>
        /// Convert Custom Minimum Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromsource"></param>
        /// <param name="tosource"></param>
        /// <param name="fromtarget"></param>
        /// <param name="totarget"></param>
        /// <returns>decimal</returns>
        public static decimal ConversionMap(decimal value,decimal fromsource, decimal tosource, decimal fromtarget, decimal totarget)
        {
            return (value - fromsource) / (tosource - fromsource) * (totarget - fromtarget) + fromtarget;
        }


        /// <summary>
        /// Convert Maps Algorithm
        /// </summary>
        /// <param name="Min">Minimum Value</param>
        /// <param name="Max">Maximum Value</param>
        /// <param name="Value">Value</param>
        /// <returns>Double Result</returns>
        public static double ConversionMaps(double Min, double Max, double Value)
        {
            double Deviation = Max - Min;
            double Result = (100 / Deviation) * (Value - Max) + 100;

            return Result;
        }


        /// <summary>
        /// Multi Convert 3 Steps
        /// </summary>
        /// <param name="value">Raw Value</param>
        /// <returns>SHA256 String</returns>
        public static string ConversionCheckDevice(int value)
        {
            /*
             *  Function Multi Convert 3 Steps
             *  Recommended for Serial Number
             *  Flow : RAW-Values --> Binary --> Byte --> SHA256 --> Validation DB
             */

            string ResultBinary = "";
            byte[] ResultBytes = null;
            string ResultByteString = "";
            string ResultEncrypt256 = "";

            try
            {
                // SerialNumber (Decimal) --> Binary
                while (value > 1)
                {
                    int remainder = value % 2;
                    ResultBinary = Convert.ToString(remainder) + ResultBinary;
                    value /= 2;
                }
                ResultBinary = Convert.ToString(value) + ResultBinary;

                // Binary --> Byte
                int numOfBytes = ResultBinary.Length / 8;
                ResultBytes = new byte[numOfBytes];
                for (int i = 0; i < numOfBytes; ++i)
                {
                    ResultBytes[i] = Convert.ToByte(ResultBinary.Substring(8 * i, 8), 2);
                }

                // Byte --> String
                int ResultBytesLength = ResultBytes.Length;
                for (int i = 0; i < ResultBytesLength; i++)
                {
                    ResultByteString += ResultBytes[i].ToString();
                }

                // String --> SHA256 --> String
                using (System.Security.Cryptography.SHA256 encrypt = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] bytes = encrypt.ComputeHash(Encoding.UTF8.GetBytes(ResultByteString));
                    StringBuilder strBuilder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        strBuilder.Append(bytes[i].ToString("x2"));
                    }
                    ResultEncrypt256 = strBuilder.ToString();
                }
            }
            catch { }
            return ResultEncrypt256;
        }


        /// <summary>
        /// Epoch Generator
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ConversionConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            TimeSpan diff = date.ToLocalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }


        /// <summary>
        /// Convert Measure
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConversionConvertForMeasure(int value)
        {
            return double.Parse(value.ToString()) / 100;
        }


        /// <summary>
        /// Convert Decimal -> Binary 16
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[] ConversionDecimalToBinary16(int input)
        {
            int num = Convert.ToInt32(input);
            int[] arr = new int[16];

            //for positive integers
            if (num > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (num > 0)
                    {
                        if ((num % 2) == 0)
                        {
                            num = num / 2;
                            arr[16 - (i + 1)] = 0;
                        }
                        else if ((num % 2) != 0)
                        {
                            num = num / 2;
                            arr[16 - (i + 1)] = 1;
                        }
                    }
                }
            }

            //for negative integers
            else if (num < 0)
            {
                num = (num + 1) * -1;

                for (int i = 0; i < 16; i++)
                {
                    if (num > 0)
                    {
                        if ((num % 2) == 0)
                        {
                            num = num / 2;
                            arr[16 - (i + 1)] = 0;
                        }
                        else if ((num % 2) != 0)
                        {
                            num = num / 2;
                            arr[16 - (i + 1)] = 1;
                        }
                    }
                }

                for (int y = 0; y < 16; y++)
                {
                    arr[y] = (arr[y] != 0) ? arr[y] = 0 : arr[y] = 1;
                    //Debug.Write(arr[y]);
                }
            }
            return arr;
        }


        /// <summary>
        /// Convert String with character Separate to String Array
        /// </summary>
        /// <param name="value">Value want to convert</param>
        /// <param name="splitchar">character</param>
        /// <returns></returns>
        public static string[] ConvertSplitString(string value, char splitchar = ',')
        {
            string[] result;
            result = Array.ConvertAll<string, string>(value.Split(splitchar), Convert.ToString); ;
            return result;
        }


        /// <summary>
        /// Convert String with Comma Separate to int32 Array
        /// </summary>
        /// <param name="value">Value want to convert</param>
        /// <param name="splitchar">character</param>
        /// <returns>Array Result</returns>
        public static int[] ConvertSplitInt(string value, char splitchar = ',')
        {
            int[] result;
            result = Array.ConvertAll<string, int>(value.Split(splitchar), Convert.ToInt32);
            return result;
        }


        /// <summary>
        /// Convert int16 int16 to int32
        /// </summary>
        /// <param name="firstvalue16">int16 value</param>
        /// <param name="secondvalue16">int16 value</param>
        /// <returns>int32 (uint)</returns>
        public static uint Int16toInt32(int firstvalue16, int secondvalue16)
        {
            byte[] ByteValueFirst16 = new byte[4];
            byte[] ByteValueSecond16 = new byte[4];
            byte[] ByteTotalResult = new byte[4];

            // Convert Int16 to Byte
            ByteValueFirst16 = BitConverter.GetBytes(firstvalue16);
            ByteValueSecond16 = BitConverter.GetBytes(secondvalue16);

            // Allocate Byte to ByteResult
            ByteTotalResult[0] = ByteValueFirst16[0];
            ByteTotalResult[1] = ByteValueFirst16[1];
            ByteTotalResult[2] = ByteValueSecond16[0];
            ByteTotalResult[3] = ByteValueSecond16[1];

            // Return
            return BitConverter.ToUInt32(ByteTotalResult,0);
        }
    }
}
