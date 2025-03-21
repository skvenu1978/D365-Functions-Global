// <copyright file="ZBase32Encoder.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ZBase32Encoder : Base32Encoder
    {
        private const string DefEncodingTable = "ybndrfg8ejkmcpqxot1uwisza345h769";

        private const char DefPadding = '0';

        public ZBase32Encoder()
            : base("ybndrfg8ejkmcpqxot1uwisza345h769", '0')
        {
        }

        public override string Encode(byte[] input)
        {
            string text = base.Encode(input);
            return text.TrimEnd('0');
        }

        public override byte[] Decode(string data)
        {
            int num = Convert.ToInt32(Math.Floor((double)data.Length / 1.6));
            int totalWidth = 8 * Convert.ToInt32(Math.Ceiling((double)num / 5.0));
            string data2 = data.PadRight(totalWidth, '0').ToLower();
            return base.Decode(data2);
        }
    }
}
