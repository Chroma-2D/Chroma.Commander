using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Chroma.Commander
{
    public partial class DebugConsole
    {
        private void PushToOutputBuffer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                _scrollBuffer.Push(string.Empty);
                return;
            }

            var sb = new StringBuilder();
            var strings = new List<string>();

            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n')
                {
                    strings.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
                
                sb.Append(input[i]);

                if (sb.Length >= _target.Width / 8 || i == input.Length - 1)
                {
                    strings.Add(sb.ToString());
                    sb.Clear();
                }
            }

            foreach (var s in strings)
            {
                _scrollBuffer.Push(s);
            }
        }

        public void Print(string value)
            => PushToOutputBuffer(value);

        public void Print(object value)
            => Print(value.ToString());

        public void Print(bool value)
            => Print(value.ToString());

        public void Print(sbyte value)
            => Print(value.ToString());

        public void Print(byte value)
            => Print(value.ToString());

        public void Print(short value)
            => Print(value.ToString());

        public void Print(ushort value)
            => Print(value.ToString());

        public void Print(int value)
            => Print(value.ToString());

        public void Print(uint value)
            => Print(value.ToString());

        public void Print(long value)
            => Print(value.ToString());

        public void Print(ulong value)
            => Print(value.ToString());

        public void Print(float value)
            => Print(value, CultureInfo.InvariantCulture);

        public void Print(float value, IFormatProvider provider)
            => Print(value.ToString(provider));

        public void Print(double value)
            => Print(value.ToString(CultureInfo.InvariantCulture));

        public void Print(double value, IFormatProvider provider)
            => Print(value.ToString(provider));
    }
}