namespace KrazenLabs.LogConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ChatLogParser
    {
        public static List<ChatEntry> Parse(string filePath)
        {
            var entries = new List<ChatEntry>();

            using var stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                try
                {
                    // Timestamp: 4 bytes, little-endian
                    uint timestamp = reader.ReadUInt32();
                    DateTime date = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

                    // Flag: 1 byte (0 = message, 1 = emote)
                    bool isEmote = reader.ReadByte() == 1;

                    // Speaker name length and value
                    byte nameLength = reader.ReadByte();
                    string speaker = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

                    // Message length and value
                    ushort messageLength = reader.ReadUInt16();
                    string message = Encoding.UTF8.GetString(reader.ReadBytes(messageLength));

                    // Skip next 2 bytes (separator or unknown metadata)
                    if (reader.BaseStream.Position + 2 <= reader.BaseStream.Length)
                    {
                        reader.ReadUInt16();
                    }

                    entries.Add(new ChatEntry
                    {
                        Timestamp = date,
                        Speaker = speaker,
                        Message = message,
                        IsEmote = isEmote
                    });
                }
                catch (EndOfStreamException)
                {
                    // Incomplete message at the end — safely ignore
                    break;
                }
            }

            return entries;
        }
    }
}
