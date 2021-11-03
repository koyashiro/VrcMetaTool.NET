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

        public ReadOnlySpan<byte> Bytes => _buffer;
        public ReadOnlySpan<byte> LengthBytes => _buffer[LENGTH_RANGE];
        public ReadOnlySpan<byte> ChunkTypeBytes => _buffer[CHUNK_TYPE_RANGE];
        public ReadOnlySpan<byte> ChunkDataBytes => _buffer[CHUNK_DATA_RANGE];
        public ReadOnlySpan<byte> CrcBytes => _buffer[CRC_RANGE];

        public int ChunkDataLength => BinaryPrimitives.ReadInt32BigEndian(LengthBytes);
        public string ChunkType => Encoding.UTF8.GetString(ChunkTypeBytes);
        public string ChunkData => Encoding.UTF8.GetString(ChunkDataBytes);
        public int Crc => BinaryPrimitives.ReadInt32BigEndian(CrcBytes);

        public static Chunk Parse(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4)
            {
                throw new ArgumentException();
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(buffer[LENGTH_RANGE]);
            if (buffer.Length != chunkDataLength + 12)
            {
                throw new ArgumentException();
            }

            return new Chunk { _buffer = buffer.ToArray() };
        }

        public static bool TryParse(ReadOnlySpan<byte> buffer, out Chunk chunk)
        {
            if (buffer.Length < 4)
            {
                chunk = default;
                return false;
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(buffer[LENGTH_RANGE]);
            if (buffer.Length != chunkDataLength + 12)
            {
                chunk = default;
                return false;
            }

            chunk = new Chunk { _buffer = buffer.ToArray() };
            return true;
        }

        public static Chunk Create(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData)
        {
            if (chunkType.Length != 4)
            {
                throw new ArgumentException();
            }

            var buffer = new byte[12 + chunkData.Length];
            var span = buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(span[LENGTH_RANGE], chunkData.Length);
            chunkType.CopyTo(span[CHUNK_TYPE_RANGE]);
            chunkData.CopyTo(span[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(span[CRC_RANGE], Crc32.Compute(span[CRC_TARGET_RANGE]));

            return new Chunk { _buffer = buffer };
        }

        public static bool TryCreate(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData, out Chunk chunk)
        {
            if (chunkType.Length != 4)
            {
                chunk = default;
                return false;
            }

            var buffer = new byte[12 + chunkData.Length];
            var span = buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(span[LENGTH_RANGE], chunkData.Length);
            chunkType.CopyTo(span[CHUNK_TYPE_RANGE]);
            chunkData.CopyTo(span[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(span[CRC_RANGE], Crc32.Compute(span[CRC_TARGET_RANGE]));

            chunk = new Chunk { _buffer = buffer };
            return true;
        }

        public static Chunk Create(string chunkType, string chunkData)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = string.IsNullOrEmpty(chunkData) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(chunkData);

            return Create(chunkTypeBytes, chunkDataBytes);
        }

        public static bool TryCreate(string chunkType, string chunkData, out Chunk chunk)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = string.IsNullOrEmpty(chunkData) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(chunkData);

            return TryCreate(chunkTypeBytes, chunkDataBytes, out chunk);
        }

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
