// <copyright file="Base32Encoder.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Base32Encoder
    {
        private const string DefEncodingTable = "abcdefghijklmnopqrstuvwxyz234567";

        private const char DefPadding = '=';

        private readonly string _eTable;

        private readonly char _padding;

        private readonly byte[] _dTable;

        public Base32Encoder()
            : this("abcdefghijklmnopqrstuvwxyz234567", '=')
        {
        }

        public Base32Encoder(char padding)
            : this("abcdefghijklmnopqrstuvwxyz234567", padding)
        {
        }

        public Base32Encoder(string encodingTable)
            : this(encodingTable, '=')
        {
        }

        public Base32Encoder(string encodingTable, char padding)
        {
            _eTable = encodingTable;
            _padding = padding;
            _dTable = new byte[128];
            InitialiseDecodingTable();
        }

        public virtual string Encode(byte[] input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = input.Length % 5;
            int num2 = input.Length - num;
            for (int i = 0; i < num2; i += 5)
            {
                int num3 = input[i] & 0xFF;
                int num4 = input[i + 1] & 0xFF;
                int num5 = input[i + 2] & 0xFF;
                int num6 = input[i + 3] & 0xFF;
                int num7 = input[i + 4] & 0xFF;
                stringBuilder.Append(_eTable[(num3 >> 3) & 0x1F]);
                stringBuilder.Append(_eTable[((num3 << 2) | (num4 >> 6)) & 0x1F]);
                stringBuilder.Append(_eTable[(num4 >> 1) & 0x1F]);
                stringBuilder.Append(_eTable[((num4 << 4) | (num5 >> 4)) & 0x1F]);
                stringBuilder.Append(_eTable[((num5 << 1) | (num6 >> 7)) & 0x1F]);
                stringBuilder.Append(_eTable[(num6 >> 2) & 0x1F]);
                stringBuilder.Append(_eTable[((num6 << 3) | (num7 >> 5)) & 0x1F]);
                stringBuilder.Append(_eTable[num7 & 0x1F]);
            }

            switch (num)
            {
                case 1:
                    {
                        int num3 = input[num2] & 0xFF;
                        stringBuilder.Append(_eTable[(num3 >> 3) & 0x1F]);
                        stringBuilder.Append(_eTable[(num3 << 2) & 0x1F]);
                        stringBuilder.Append(_padding).Append(_padding).Append(_padding)
                            .Append(_padding)
                            .Append(_padding)
                            .Append(_padding);
                        break;
                    }
                case 2:
                    {
                        int num3 = input[num2] & 0xFF;
                        int num4 = input[num2 + 1] & 0xFF;
                        stringBuilder.Append(_eTable[(num3 >> 3) & 0x1F]);
                        stringBuilder.Append(_eTable[((num3 << 2) | (num4 >> 6)) & 0x1F]);
                        stringBuilder.Append(_eTable[(num4 >> 1) & 0x1F]);
                        stringBuilder.Append(_eTable[(num4 << 4) & 0x1F]);
                        stringBuilder.Append(_padding).Append(_padding).Append(_padding)
                            .Append(_padding);
                        break;
                    }
                case 3:
                    {
                        int num3 = input[num2] & 0xFF;
                        int num4 = input[num2 + 1] & 0xFF;
                        int num5 = input[num2 + 2] & 0xFF;
                        stringBuilder.Append(_eTable[(num3 >> 3) & 0x1F]);
                        stringBuilder.Append(_eTable[((num3 << 2) | (num4 >> 6)) & 0x1F]);
                        stringBuilder.Append(_eTable[(num4 >> 1) & 0x1F]);
                        stringBuilder.Append(_eTable[((num4 << 4) | (num5 >> 4)) & 0x1F]);
                        stringBuilder.Append(_eTable[(num5 << 1) & 0x1F]);
                        stringBuilder.Append(_padding).Append(_padding).Append(_padding);
                        break;
                    }
                case 4:
                    {
                        int num3 = input[num2] & 0xFF;
                        int num4 = input[num2 + 1] & 0xFF;
                        int num5 = input[num2 + 2] & 0xFF;
                        int num6 = input[num2 + 3] & 0xFF;
                        stringBuilder.Append(_eTable[(num3 >> 3) & 0x1F]);
                        stringBuilder.Append(_eTable[((num3 << 2) | (num4 >> 6)) & 0x1F]);
                        stringBuilder.Append(_eTable[(num4 >> 1) & 0x1F]);
                        stringBuilder.Append(_eTable[((num4 << 4) | (num5 >> 4)) & 0x1F]);
                        stringBuilder.Append(_eTable[((num5 << 1) | (num6 >> 7)) & 0x1F]);
                        stringBuilder.Append(_eTable[(num6 >> 2) & 0x1F]);
                        stringBuilder.Append(_eTable[(num6 << 3) & 0x1F]);
                        stringBuilder.Append(_padding);
                        break;
                    }
            }

            return stringBuilder.ToString();
        }

        public virtual byte[] Decode(string data)
        {
            List<byte> list = new List<byte>();
            int num = data.Length;
            while (num > 0 && Ignore(data[num - 1]))
            {
                num--;
            }

            int i = 0;
            int num2 = num - 8;
            for (i = NextI(data, i, num2); i < num2; i = NextI(data, i, num2))
            {
                byte b = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b2 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b3 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b4 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b5 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b6 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b7 = _dTable[(uint)data[i++]];
                i = NextI(data, i, num2);
                byte b8 = _dTable[(uint)data[i++]];
                list.Add((byte)((b << 3) | (b2 >> 2)));
                list.Add((byte)((b2 << 6) | (b3 << 1) | (b4 >> 4)));
                list.Add((byte)((b4 << 4) | (b5 >> 1)));
                list.Add((byte)((b5 << 7) | (b6 << 2) | (b7 >> 3)));
                list.Add((byte)((b7 << 5) | b8));
            }

            DecodeLastBlock(list, data[num - 8], data[num - 7], data[num - 6], data[num - 5], data[num - 4], data[num - 3], data[num - 2], data[num - 1]);
            return list.ToArray();
        }

        protected virtual int DecodeLastBlock(ICollection<byte> outStream, char c1, char c2, char c3, char c4, char c5, char c6, char c7, char c8)
        {
            byte b;
            byte b2;
            if (c3 == _padding)
            {
                b = _dTable[(uint)c1];
                b2 = _dTable[(uint)c2];
                outStream.Add((byte)((b << 3) | (b2 >> 2)));
                return 1;
            }

            byte b3;
            byte b4;
            if (c5 == _padding)
            {
                b = _dTable[(uint)c1];
                b2 = _dTable[(uint)c2];
                b3 = _dTable[(uint)c3];
                b4 = _dTable[(uint)c4];
                outStream.Add((byte)((b << 3) | (b2 >> 2)));
                outStream.Add((byte)((b2 << 6) | (b3 << 1) | (b4 >> 4)));
                return 2;
            }

            byte b5;
            if (c6 == _padding)
            {
                b = _dTable[(uint)c1];
                b2 = _dTable[(uint)c2];
                b3 = _dTable[(uint)c3];
                b4 = _dTable[(uint)c4];
                b5 = _dTable[(uint)c5];
                outStream.Add((byte)((b << 3) | (b2 >> 2)));
                outStream.Add((byte)((b2 << 6) | (b3 << 1) | (b4 >> 4)));
                outStream.Add((byte)((b4 << 4) | (b5 >> 1)));
                return 3;
            }

            byte b6;
            byte b7;
            if (c8 == _padding)
            {
                b = _dTable[(uint)c1];
                b2 = _dTable[(uint)c2];
                b3 = _dTable[(uint)c3];
                b4 = _dTable[(uint)c4];
                b5 = _dTable[(uint)c5];
                b6 = _dTable[(uint)c6];
                b7 = _dTable[(uint)c7];
                outStream.Add((byte)((b << 3) | (b2 >> 2)));
                outStream.Add((byte)((b2 << 6) | (b3 << 1) | (b4 >> 4)));
                outStream.Add((byte)((b4 << 4) | (b5 >> 1)));
                outStream.Add((byte)((b5 << 7) | (b6 << 2) | (b7 >> 3)));
                return 4;
            }

            b = _dTable[(uint)c1];
            b2 = _dTable[(uint)c2];
            b3 = _dTable[(uint)c3];
            b4 = _dTable[(uint)c4];
            b5 = _dTable[(uint)c5];
            b6 = _dTable[(uint)c6];
            b7 = _dTable[(uint)c7];
            byte b8 = _dTable[(uint)c8];
            outStream.Add((byte)((b << 3) | (b2 >> 2)));
            outStream.Add((byte)((b2 << 6) | (b3 << 1) | (b4 >> 4)));
            outStream.Add((byte)((b4 << 4) | (b5 >> 1)));
            outStream.Add((byte)((b5 << 7) | (b6 << 2) | (b7 >> 3)));
            outStream.Add((byte)((b7 << 5) | b8));
            return 5;
        }

        protected int NextI(string data, int i, int finish)
        {
            while (i < finish && Ignore(data[i]))
            {
                i++;
            }

            return i;
        }

        protected bool Ignore(char c)
        {
            return c == '\n' || c == '\r' || c == '\t' || c == ' ' || c == '-';
        }

        protected void InitialiseDecodingTable()
        {
            for (int i = 0; i < _eTable.Length; i++)
            {
                _dTable[(uint)_eTable[i]] = (byte)i;
            }
        }
    }
}
