// Code modified from Danny Chen comment in http://stackoverflow.com/questions/3404199/how-to-find-out-the-encoding-of-a-file-c-sharp

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace netDxf.IO
{
    internal class EncodingType
    {
        public static Encoding GetType(Stream fs)
        {
            byte[] Unicode = {0xFF, 0xFE, 0x41};
            byte[] UnicodeBIG = {0xFE, 0xFF, 0x00};
            byte[] UTF8 = {0xEF, 0xBB, 0xBF}; //with BOM
            Encoding reVal = Encoding.ASCII;//.Default;
            
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(CultureInfo.InvariantCulture), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == UTF8[0] && ss[1] == UTF8[1] && ss[2] == UTF8[2]))
                reVal = Encoding.UTF8;
            else if (ss[0] == UnicodeBIG[0] && ss[1] == UnicodeBIG[1] && ss[2] == UnicodeBIG[2])
                reVal = Encoding.BigEndianUnicode;
            else if (ss[0] == Unicode[0] && ss[1] == Unicode[1] && ss[2] == Unicode[2])
                reVal = Encoding.Unicode;
            fs.Position = 0;           
            return reVal;
        }

        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;
            for (int i = 0; i < data.Length; i++)
            {
                byte curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        while (((curByte <<= 1) & 0x80) != 0)
                            charByteCounter++;

                        if (charByteCounter == 1 || charByteCounter > 6)
                            return false;
                    }
                }
                else
                {
                    if ((curByte & 0xC0) != 0x80)
                        return false;
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
                throw new Exception("Error byte format.");

            return true;
        }
    }
}