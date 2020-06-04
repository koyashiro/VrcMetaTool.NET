using System;
using System.Buffers.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace KoyashiroKohaku.VrcMetaToolSharp
{
    public static class VrcMetaDataReader
    {
        private static readonly byte[] PngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        public static VrcMetaData Read(byte[] buffer)
        {
            #region Argument Check
            if (buffer is null)
            {
                throw new ArgumentNullException($"Argument error. argument: '{nameof(buffer)}' is null.");
            }

            var span = buffer.AsSpan();

            if (!span[..8].SequenceEqual(PngSignature.AsSpan()))
            {
                throw new ArgumentException($"Argument error. argument: '{nameof(buffer)}' is broken or no png image binary.");
            }
            #endregion

            var vrcMetaData = new VrcMetaData();

            var offset = 4;
            while (offset + 8 < span.Length)
            {
                var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(span.Slice(offset + 4, 4));

                var chunkTypeUint = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(offset + 8, 4));

                if (!Enum.IsDefined(typeof(ChunkType), chunkTypeUint))
                {
                    offset += 12 + chunkDataLength;
                    continue;
                }

                var chunkType = (ChunkType)chunkTypeUint;

                var chunkDataString = Encoding.UTF8.GetString(span.Slice(offset + 12, chunkDataLength));

                switch (chunkType)
                {
                    case ChunkType.Date:
                        if (vrcMetaData.World is null)
                        {
                            vrcMetaData.Date = DateTime.ParseExact(chunkDataString, "yyyyMMddHHmmssfff", null);
                        }
                        else
                        {
                            throw new ArgumentException("Duplication error. There are two or more chunk types of 'vrCd'.");
                        }
                        break;
                    case ChunkType.Photographer:
                        if (vrcMetaData.Photographer is null)
                        {
                            vrcMetaData.Photographer = chunkDataString;
                        }
                        else
                        {
                            throw new ArgumentException("Duplication error. There are two or more chunk types of 'vrCp'.");
                        }
                        break;
                    case ChunkType.World:
                        if (vrcMetaData.World is null)
                        {
                            vrcMetaData.World = chunkDataString;
                        }
                        else
                        {
                            throw new ArgumentException("Duplication error. There are two or more chunk types of 'vrCw'.");
                        }
                        break;
                    case ChunkType.User:
                        User user;
                        if (chunkDataString.Contains(':'))
                        {
                            var match = new Regex(@"(?<userName>.*) : (?<twitterScreenName>@[0-9a-zA-Z_]*)").Match(chunkDataString);

                            if (match.Success)
                            {
                                user = new User
                                {
                                    UserName = match.Groups["userName"].Value,
                                    TwitterScreenName = match.Groups["twitterScreenName"].Value
                                };
                            }
                            else
                            {
                                user = new User
                                {
                                    UserName = chunkDataString
                                };
                            }
                        }
                        else
                        {
                            user = new User
                            {
                                UserName = chunkDataString
                            };
                        }
                        vrcMetaData.Users.Add(user);
                        break;
                }

                offset += 12 + chunkDataLength;
            }

            return vrcMetaData;
        }
    }
}
