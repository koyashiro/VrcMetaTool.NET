using System;
using System.Buffers.Binary;
using System.Text;

namespace KoyashiroKohaku.VrcMetaTool.Png
{
    /// <summary>
    /// Chunk
    /// </summary>
    public struct Chunk : IEquatable<Chunk>
    {
        private static readonly Range LENGTH_RANGE = 0..4;
        private static readonly Range CHUNK_TYPE_RANGE = 4..8;
        private static readonly Range CHUNK_DATA_RANGE = 8..^4;
        private static readonly Range CRC_TARGET_RANGE = 4..^4;
        private static readonly Range CRC_RANGE = ^4..;


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
            BinaryPrimitives.WriteInt32BigEndian(buffer[LENGTH_RANGE], chunkData.Length);
            chunkType.CopyTo(buffer[CHUNK_TYPE_RANGE]);
            chunkData.CopyTo(buffer[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[CRC_RANGE], Crc32.Compute(buffer[CRC_TARGET_RANGE]));
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
            BinaryPrimitives.WriteInt32BigEndian(buffer[LENGTH_RANGE], chunkDataBytes.Length);
            chunkTypeBytes.CopyTo(buffer[CHUNK_TYPE_RANGE]);
            chunkDataBytes.CopyTo(buffer[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[CRC_RANGE], Crc32.Compute(buffer[CRC_TARGET_RANGE]));
        }

        public ReadOnlySpan<byte> Bytes => _buffer;
        public ReadOnlySpan<byte> LengthBytes => _buffer[LENGTH_RANGE];
        public ReadOnlySpan<byte> ChunkTypeBytes => _buffer[CHUNK_TYPE_RANGE];
        public ReadOnlySpan<byte> ChunkDataBytes => _buffer[CHUNK_DATA_RANGE];
        public ReadOnlySpan<byte> CrcBytes => _buffer[CRC_RANGE];

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

            if (Crc32.Compute(_buffer[CRC_TARGET_RANGE]) != Crc)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{ChunkType}: {ChunkData}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Chunk other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Chunk other)
        {
            return Bytes.SequenceEqual(other.Bytes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_buffer);
        }

        public static bool operator ==(Chunk left, Chunk right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Chunk left, Chunk right)
        {
            return !(left == right);
        }
    }
}
