using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResCryption
{
    public class Helper
    {
        public static List<byte[]> ParseData(byte[] data)
        {
            List<byte[]> returnObject = new List<byte[]>();
            int blockSize = int.Parse(Encoding.UTF8.GetString(new byte[] { data.First() }));

            if (blockSize == 0)
            {
                returnObject.Add(data);
                return returnObject;
            }

            for (int i = 1; i < data.Length; i++)
            {
                if (blockSize <= returnObject.Count + 1)
                {
                    byte[] last_block = new byte[data.Length - i];
                    Array.Copy(data, i, last_block, 0, data.Length - i);
                    returnObject.Add(last_block);
                    break;
                }
                int size = int.Parse(Encoding.UTF8.GetString(new byte[] { data[i] }));
                byte[] block = new byte[size];
                Array.Copy(data, i + 1, block, 0, size);
                returnObject.Add(block);
                i = i + size;
            }
            return returnObject;
        }

        public static List<byte> ConvertData(List<byte[]> data)
        {
            List<byte> returnObject = new List<byte>();
            returnObject.AddRange(Encoding.UTF8.GetBytes(data.Count.ToString()));

            foreach (var item in data)
            {
                if (item == data.Last())
                {
                    returnObject.AddRange(item);
                    break;
                }
                returnObject.AddRange(Encoding.UTF8.GetBytes(item.Length.ToString()));
                returnObject.AddRange(item);
            }
            return returnObject;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
