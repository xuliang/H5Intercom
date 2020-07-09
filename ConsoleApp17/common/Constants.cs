namespace ConsoleApp17
{
    public class Constants {

        /**
         * 包头分隔符
         */
        public static readonly byte[] DELIMITER = new byte[] { 0x30, 0x31, 0x63, 0x64 };

        /**
         * 每个包的最大长度
         */
        public static readonly int MAX_FRAME_LENGTH = 980;
    }
}