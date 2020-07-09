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
        /// �޷�������, �൱��java��� value>>>pos
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int RightMove(int value, int pos)
        {
            //�ƶ� 0 λʱֱ�ӷ���ԭֵ
            if (pos != 0)
            {
                // int.MaxValue = 0x7FFFFFFF �������ֵ
                int mask = int.MaxValue;
                //�޷����������λ����ʾ�����������������з��ŵģ��з���������1λ������ʱ��λ��0������ʱ��λ��1
                value = value >> 1;
                //���������ֵ�����߼������㣬�����Ľ��Ϊ���Ա�ʾ����ֵ�����λ
                value = value & mask;
                //�߼�������ֵ�޷��ţ����޷��ŵ�ֱֵ�����������㣬����ʣ�µ�λ
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