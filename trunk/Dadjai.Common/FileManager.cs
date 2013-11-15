using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dadjai.Common
{
    public static class FileManager
    {
        public static IEnumerable<string> GetData(string fileName, Encoding encoding)
        {
            List<string> data = new List<string>();
            using (BinaryReader r = new BinaryReader(File.Open(string.Format(@"../../../DATA/{0}", fileName), FileMode.Open), encoding))
            {
                while (r.PeekChar() > 0)
                {
                    data.Add(r.ReadString());
                }
            }

            return data;
        }

        public static void StoreData(string fileName, IEnumerable<string> data, Encoding encoding)
        {
            using (BinaryWriter w = new BinaryWriter(File.Open(string.Format(@"../../../DATA/{0}", fileName), FileMode.Create), encoding))
            {
                foreach (var item in data)
                {
                    w.Write(item);
                }
            }
        }
    }
}