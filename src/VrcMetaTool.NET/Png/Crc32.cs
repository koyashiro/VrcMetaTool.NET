using System;

using Force.Crc32;

namespace KoyashiroKohaku.VrcMetaTool.Png
{
    public static class Crc32
    {

        public static uint Compute(ReadOnlySpan<byte> input)
        {
            return Crc32Algorithm.Compute(input.ToArray());
        }
    }
}
