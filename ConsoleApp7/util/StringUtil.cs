using DotNetty.Common.Utilities;
using System.Text;

namespace ConsoleApp7
{
    public class StringUtil
    {

        private static readonly char[] hex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static string ConvertByteToHexString(byte[] b)
        {
            if (b == null)
                return "null";
            else
                return ConvertByteToHexString(b, 0, b.Length);
        }

        public static string ConvertByteToHexString(byte[] b, int offset, int len)
        {
            if (b == null)
                return "null";
            int end = offset + len;
            if (end > b.Length)
                end = b.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = offset; i < end; i++)
                sb.Append(hex[RightMove((b[i] & 0xf0), 4)]).Append(hex[b[i] & 0xf]).Append(' ');

            if (sb.Length > 0)
                //sb.DeleteCharAt(sb.length() - 1);
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        /// <summary>
        /// 无符号右移, 相当于java里的 value>>>pos
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int RightMove(int value, int pos)
        {
            //移动 0 位时直接返回原值
            if (pos != 0)
            {
                // int.MaxValue = 0x7FFFFFFF 整数最大值
                int mask = int.MaxValue;
                //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                value = value >> 1;
                //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value = value & mask;
                //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
                value = value >> pos - 1;
            }

            return value;
        }
        public static string ConvertByteToHexStringWithoutSpace(byte[] b)
        {
            if (b == null)
                return "null";
            else
                return ConvertByteToHexStringWithoutSpace(b, 0, b.Length);
        }

        public static string ConvertByteToHexStringWithoutSpace(byte[] b, int offset, int len)
        {
            if (b == null)
                return "null";
            int end = offset + len;
            StringBuilder sb = new StringBuilder();
            if (end > b.Length)
                end = b.Length;
            for (int i = offset; i < end; i++)
                sb.Append(hex[RightMove((b[i] & 0xf0), 4)]).Append(hex[b[i] & 0xf]);

            return sb.ToString();
        }
    }
}