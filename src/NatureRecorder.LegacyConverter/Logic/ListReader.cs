using System.Collections.Generic;
using System.IO;
using NatureRecorder.LegacyConverter.Entities;

namespace NatureRecorder.LegacyConverter.Logic
{
    public class ListReader
    {
        private readonly Dictionary<string, List<ListEntry>> _lists = new Dictionary<string, List<ListEntry>>();

        /// <summary>
        /// Return the list of entries held in the specified LST file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public List<ListEntry> GetList(string filepath)
        {
            List<ListEntry> list;

            // Get the name of the file from the file path  as this is the
            // dictionary key used to cache lists
            string filename = Path.GetFileName(filepath);

            // If we've already cached that list, just return it. Otherwise,
            // read it from the file
            if (_lists.ContainsKey(filename))
            {
                list = _lists[filename];
            }
            else
            {
                list = ReadList(filepath);
                _lists.Add(filename, list);
            }

            return list;
        }

        /// <summary>
        /// Read a list file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<ListEntry> ReadList(string filepath)
        {
            List<ListEntry> list = new List<ListEntry>();

            using (FileStream stream = new FileStream(filepath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // The first 4 bytes are the version
                    int version = reader.ReadInt32();

                    // The remaining content are the tags for this list
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        // Each tag record consists of a 4-byte index number, an 80 character
                        // tag and a 1024 character "information sheet" path (if the file
                        // version is > 100
                        int index = reader.ReadInt32();
                        string tag = ReadString(reader, 80);
                        string sheet = "";
                        if (version > 100)
                        {
                            sheet = ReadString(reader, 1024);
                        }

                        // Construct a new list entry and add it to the list
                        list.Add(new ListEntry
                        {
                            Index = index,
                            Tag = tag,
                            Sheet = sheet
                        });
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Read a string from the binary reader and remove null characters
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        private string ReadString(BinaryReader reader, int characters)
        {
            return new string(reader.ReadChars(characters)).Replace("\0", "");
        }
    }
}
