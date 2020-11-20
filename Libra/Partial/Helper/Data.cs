using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Libra
{
    public partial class Helper
    {
        /*
         *  Partial Class Helper
         *  Data Function
         */

        /// <summary>
        /// Function For Convert Object To ByteArray
        /// </summary>
        /// <param name="obj">Object Want to Convert</param>
        /// <returns>Byte Array</returns>
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
