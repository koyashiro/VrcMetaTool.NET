using System;
using System.Buffers.Binary;
using System.Text;

using Force.Crc32;

namespace KoyashiroKohaku.PngChunkUtil
{
    /// <summary>
    /// Chunk
    /// </summary>
    public struct Chunk
    {
        private byte[] _buffer;

        public Chunk(ReadOnlySpan<byte> buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _buffer = buffer.ToArray();
        }

        public Chunk(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData)
        {
            if (chunkType.Length != 4)
            {
                throw new ArgumentException();
            }

            _buffer = new byte[12 + chunkData.Length];

            var buffer = _buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(buffer.Slice(0, 4), chunkData.Length);
            chunkType.CopyTo(buffer.Slice(4, 8));
            chunkData.CopyTo(buffer.Slice(8, (8 + chunkData.Length)));
            BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(8 + chunkData.Length), CalculateCrc(chunkType, chunkData));
        }

        public Chunk(string chunkType, string chunkData)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = string.IsNullOrEmpty(chunkData) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(chunkData);

            if (chunkTypeBytes.Length != 4)
            {
                throw new ArgumentException();
            }

            _buffer = new byte[12 + chunkDataBytes.Length];

            var buffer = _buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(buffer[0..4], chunkDataBytes.Length);
            chunkTypeBytes.CopyTo(buffer[4..8]);
            chunkDataBytes.CopyTo(buffer[8..(8 + chunkDataBytes.Length)]);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[(8 + chunkDataBytes.Length)..], CalculateCrc(chunkType, chunkData));
        }

        public ReadOnlySpan<byte> Bytes => _buffer;
        public ReadOnlySpan<byte> LengthBytes => _buffer[0..4];
        public ReadOnlySpan<byte> ChunkTypeBytes => _buffer[4..8];
        public ReadOnlySpan<byte> ChunkDataBytes => _buffer[8..(8 + ChunkDataLength)];
        public ReadOnlySpan<byte> CrcBytes => _buffer[(8 + ChunkDataLength)..];

        public int ChunkDataLength => BinaryPrimitives.ReadInt32BigEndian(LengthBytes);
        public string ChunkType => Encoding.UTF8.GetString(ChunkTypeBytes);
        public string ChunkData => Encoding.UTF8.GetString(ChunkDataBytes);
        public int Crc => BinaryPrimitives.ReadInt32BigEndian(CrcBytes);

        public bool IsValid()
        {
            if (ChunkDataLength < 4)
            {
                return false;
            }

            if (ChunkDataLength != 12 + ChunkDataLength)
            {
                return false;
            }

            if (CalculateCrc(ChunkTypeBytes, ChunkDataBytes) != Crc)
            {
                return false;
            }

            return true;
        }

        private static uint CalculateCrc(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData)
        {
            var source = new byte[chunkType.Length + chunkData.Length];
            chunkType.CopyTo(source.AsSpan().Slice(0, 4));
            chunkData.CopyTo(source.AsSpan().Slice(4, chunkData.Length));

            return Crc32Algorithm.Compute(source);
        }

        private uint CalculateCrc(string chunkType, string chunkData = "")
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = Encoding.UTF8.GetBytes(chunkData);

            return CalculateCrc(chunkTypeBytes, chunkDataBytes);
        }

        public override string ToString()
        {
            return $"{ChunkType}: {ChunkData}";
        }
    }
}
