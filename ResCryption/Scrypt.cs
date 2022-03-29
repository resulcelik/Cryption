using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ResCryption
{

    class Scrypt
    {
        private static int BYTE_SIZE = 8;

        public int blockSize;
        public int roundSize;
        public CipherMode mode;
        public byte[] IV = new byte[32];

        public Scrypt()
        {
            blockSize = 32;
            roundSize = 10;
            mode = CipherMode.CBC;
            for (int i = 0; i < 32; i++)
                IV[i] = (byte)i;
        }

        public byte[] Encrypt(byte[] data, string key)
        {
            HashAlgorithm sha = SHA256.Create();
            byte[] keyData = sha.ComputeHash(Encoding.UTF8.GetBytes(key));

            int overFlowSize = data.Length % blockSize;
            int dataLenght = data.Length;
            int extensionSize = overFlowSize == 0 ? 0 : blockSize - overFlowSize;

            if (extensionSize > 0)
            {
                Array.Resize(ref data, data.Length + extensionSize);

                var rand = new Random();
                var bytes = new byte[extensionSize];
                rand.NextBytes(bytes);

                for (int i = 0; i < extensionSize; i++)
                    data[i + dataLenght] = bytes[i];
            }

            List<byte[]> encryptedData = null;
            switch (mode)
            {
                case CipherMode.ECB:
                    encryptedData = EncryptECB(data, keyData);
                    break;
                case CipherMode.CBC:
                    encryptedData = EncryptCBC(data, keyData);
                    break;
                case CipherMode.OFB:
                    encryptedData = EncryptOFB(data, keyData);
                    break;
                case CipherMode.CFB:
                    encryptedData = EncryptCFB(data, keyData);
                    break;
                default:

                    throw new Exception("ERR_NOT_SUPPORTED");
            }

            var size = encryptedData.Count * blockSize;
            var result = new byte[size + 1];
            var cursor = 0;

            for (int i = 0; i < encryptedData.Count(); i++)
            {
                encryptedData[i].CopyTo(result, cursor);
                cursor += blockSize;
            }

            result[result.Length - 1] = (byte)extensionSize;
            return result;
        }

        public byte[] Decrypt(byte[] data, string key)
        {
            HashAlgorithm sha = SHA256.Create();
            byte[] keyData = sha.ComputeHash(Encoding.UTF8.GetBytes(key));

            List<byte[]> decryptedData = null;
            var encData = data.Take(data.Length - 1).ToArray();

            switch (mode)
            {
                case CipherMode.ECB:
                    decryptedData = DecryptECB(encData, keyData);
                    break;
                case CipherMode.CBC:
                    decryptedData = DecryptCBC(encData, keyData);
                    break;
                case CipherMode.OFB:
                    decryptedData = DecryptOFB(encData, keyData);
                    break;
                case CipherMode.CFB:
                    decryptedData = DecryptCFB(encData, keyData);
                    break;
                default:
                    throw new Exception("ERR_NOT_SUPPORTED");
            }

            var size = (decryptedData.Count * blockSize) - data.Last();
            var result = new byte[size];
            var cursor = 0;
            var tempCursor = 0;
            foreach (var block in decryptedData)
            {
                tempCursor = cursor;
                cursor += (size - cursor) > blockSize ? blockSize : (size - cursor) % blockSize;
                if ((size - tempCursor) / blockSize > 0)
                {
                    block.CopyTo(result, tempCursor);
                }
                else
                {
                    for (int i = tempCursor; i < cursor; i++)
                        result[i] = block[i - tempCursor];
                }
            }

            return result;
        }

        private List<byte[]> EncryptECB(byte[] data, byte[] key)
        {
            var lockObject = new object();
            var dataBlockList = Helper.Split(data, blockSize);
            List<byte[]> encryptedData = new byte[dataBlockList.Count()][].ToList();

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            Parallel.For(0, dataBlockList.Count(), index =>
            {
                byte[] temp = EncryptBlock(dataBlockList.ElementAt(index).ToArray(), roundKey);
                lock (lockObject)
                {
                    encryptedData[index] = temp;
                }
            });

            return encryptedData;
        }

        private List<byte[]> DecryptECB(byte[] data, byte[] key)
        {
            var lockObject = new object();
            var dataBlockList = Helper.Split(data, blockSize);
            var decryptedData = new byte[dataBlockList.Count()][].ToList();

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            Parallel.For(0, dataBlockList.Count(), index =>
            {
                byte[] temp = DecryptBlock(dataBlockList.ElementAt(index).ToArray(), roundKey);
                lock (lockObject)
                {
                    decryptedData[index] = temp;
                }
            });

            return decryptedData;
        }

        private List<byte[]> EncryptCBC(byte[] data, byte[] key)
        {
            var encryptedData = new List<byte[]>();

            for (int i = 0; i < blockSize; i++)
                data[i] ^= IV[i];

            var dataBlockList = Helper.Split(data, blockSize);

            byte[] bytes = dataBlockList.First().ToArray();
            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            for (int i = 0; i < dataBlockList.Count(); i++)
            {
                encryptedData.Add(EncryptBlock(bytes, roundKey));
                if (i < dataBlockList.Count() - 1)
                {
                    bytes = dataBlockList.ElementAt(i + 1).ToArray();
                    for (int j = 0; j < blockSize; j++)
                        bytes[j] ^= encryptedData.Last()[j];
                }   
            }

            return encryptedData;
        }

        private List<byte[]> DecryptCBC(byte[] data, byte[] key)
        {
            var decryptedData = new List<byte[]>();
            var dataBlockList = Helper.Split(data, blockSize);

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            byte[] de = DecryptBlock(dataBlockList.First().ToArray(), roundKey);
            
            for (int j = 0; j < blockSize; j++)
                de[j] ^= IV[j];
            
            decryptedData.Add(de);

            for (int i = 1; i < dataBlockList.Count(); i++)
            {
                de = DecryptBlock(dataBlockList.ElementAt(i).ToArray(), roundKey);

                for (int j = 0; j < blockSize; j++)
                    de[j] ^= dataBlockList.ElementAt(i - 1).ToArray()[j];

                decryptedData.Add(de);
            }

            return decryptedData;
        }

        private List<byte[]> EncryptOFB(byte[] data, byte[] key)
        {
            var encryptedData = new List<byte[]>();
            var dataBlockList = Helper.Split(data, blockSize);

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            byte[] en = null;
            for (int i = 0; i < dataBlockList.Count(); i++)
            {
                if (i == 0)
                {
                    en = EncryptBlock(IV, roundKey);
                }
                else
                {
                    en = EncryptBlock(en, roundKey);
                }

                byte[] result = new byte[blockSize];
                for (int j = 0; j < blockSize; j++)
                    result[j] = (byte)(dataBlockList.ElementAt(i).ToArray()[j] ^ en[j]);

                encryptedData.Add(result);
            }

            return encryptedData;
        }

        private List<byte[]> DecryptOFB(byte[] data, byte[] key)
        {
            var decryptedData = new List<byte[]>();
            var dataBlockList = Helper.Split(data, blockSize);

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            byte[] de = null;
            for (int i = 0; i < dataBlockList.Count(); i++)
            {
                if (i == 0)
                {
                    de = EncryptBlock(IV, roundKey);
                }
                else
                {
                    de = EncryptBlock(de, roundKey);
                }

                byte[] result = new byte[blockSize];
                for (int j = 0; j < blockSize; j++)
                    result[j] = (byte)(dataBlockList.ElementAt(i).ToArray()[j] ^ de[j]);

                decryptedData.Add(result);
            }

            return decryptedData;
        }

        private List<byte[]> EncryptCFB(byte[] data, byte[] key)
        {
            var encryptedData = new List<byte[]>();
            var dataBlockList = Helper.Split(data, blockSize);

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            byte[] en = null;
            for (int i = 0; i < dataBlockList.Count(); i++)
            {
                if (i == 0)
                {
                    en = EncryptBlock(IV, roundKey);
                }
                else
                {
                    en = EncryptBlock(encryptedData.Last(), roundKey);
                }

                byte[] result = new byte[blockSize];
                for (int j = 0; j < blockSize; j++)
                    result[j] = (byte)(dataBlockList.ElementAt(i).ToArray()[j] ^ en[j]);

                encryptedData.Add(result);
            }

            return encryptedData;
        }

        private List<byte[]> DecryptCFB(byte[] data, byte[] key)
        {
            var decryptedData = new List<byte[]>();
            var dataBlockList = Helper.Split(data, blockSize);

            byte[][] roundKey = new byte[roundSize][];
            roundKey[0] = (byte[])key.Clone();

            //keys
            for (int i = 1; i < roundSize; i++)
                roundKey[i] = GenerateKey((byte[])roundKey[i - 1].Clone(), i);

            byte[] de = null;
            for (int i = 0; i < dataBlockList.Count(); i++)
            {
                if (i == 0)
                {
                    de = EncryptBlock(IV, roundKey);
                }
                else
                {
                    de = EncryptBlock(dataBlockList.ElementAt(i - 1).ToArray(), roundKey);
                }

                byte[] result = new byte[blockSize];
                for (int j = 0; j < blockSize; j++)
                    result[j] = (byte)(dataBlockList.ElementAt(i).ToArray()[j] ^ de[j]);

                decryptedData.Add(result);
            }

            return decryptedData;
        }

        private byte[] EncryptBlock(byte[] dataBlock, byte[][] roundKey)
        {
            byte[] data = (byte[])dataBlock.Clone();

            for (int round = 1; round <= roundSize; round++)
            {
                //substitute
                for (int i = 0; i < blockSize - 8; i++)
                    if (i % 8 == 7)
                        Substitute(ref data[i], ref data[i + 1]);
                    else
                        Substitute(ref data[i], ref data[i + 9]);

                //xor
                for (int i = 0; i < blockSize - 8; i++)
                {
                    if (i % 8 != 7)
                        data[i + 1] ^= data[i];

                    data[i] ^= data[i + 8];

                    if (i >= blockSize - 16)
                    {
                        if (i % 8 != 7)
                            data[i + 9] ^= data[i + 8];
                        data[i + 8] ^= data[i - (blockSize - 16)];
                    }
                }

                //shift
                for (int i = 0; i < blockSize; i++)
                    if (i % 2 == 0)
                        ShiftLeft(ref data[i], (i * round));
                    else
                        ShiftRight(ref data[i], (i * round));

                //mix columns
                for (int i = 0; i < blockSize / 8; i++)
                {
                    Swap(ref data[(i * 8) + 0], ref data[(i * 8) + 7]);
                    Swap(ref data[(i * 8) + 1], ref data[(i * 8) + 6]);
                    Swap(ref data[(i * 8) + 2], ref data[(i * 8) + 5]);
                    Swap(ref data[(i * 8) + 3], ref data[(i * 8) + 4]);
                }

                //add key
                for (int i = 0; i < blockSize; i++)
                    data[i] ^= roundKey[round - 1][i];
            }

            return data;
        }

        private byte[] DecryptBlock(byte[] dataBlock, byte[][] roundKey)
        {
            byte[] data = (byte[])dataBlock.Clone();


            for (int round = roundSize; round > 0; round--)
            {
                //add key
                for (int i = 0; i < blockSize; i++)
                    data[i] ^= roundKey[round - 1][i];

                //mix columns
                for (int i = 0; i < blockSize / 8; i++)
                {
                    Swap(ref data[(i * 8) + 0], ref data[(i * 8) + 7]);
                    Swap(ref data[(i * 8) + 1], ref data[(i * 8) + 6]);
                    Swap(ref data[(i * 8) + 2], ref data[(i * 8) + 5]);
                    Swap(ref data[(i * 8) + 3], ref data[(i * 8) + 4]);
                }

                //shift
                for (int i = 0; i < blockSize; i++)
                    if (i % 2 == 0)
                        ShiftRight(ref data[i], (i * round));
                    else
                        ShiftLeft(ref data[i], (i * round));

                ////xor
                for (int i = blockSize - 9; i >= 0; i--)
                {
                    if (i >= blockSize - 16)
                    {
                        data[i + 8] ^= data[i - (blockSize - 16)];

                        if (i % 8 != 7)
                            data[i + 9] ^= data[i + 8];
                    }

                    data[i] ^= data[i + 8];

                    if (i % 8 != 7)
                        data[i + 1] ^= data[i];
                }

                //substitute
                for (int i = 0; i < blockSize - 8; i++)
                    if (i % 8 == 7)
                        Substitute(ref data[i], ref data[i + 1]);
                    else
                        Substitute(ref data[i], ref data[i + 9]);
            }

            return data;
        }

        private byte[] GenerateKey(byte[] keyData, int round)
        {
            byte[] key = (byte[])keyData.Clone();

            //shift
            for (int i = 0; i < blockSize; i++)
                if (i % 2 == 0)
                    ShiftLeft(ref key[i], round * i);
                else
                    ShiftRight(ref key[i], round * i);

            //xor
            for (int i = 0; i < blockSize - 8; i++)
            {
                key[i] ^= key[i + 8];
                if (i >= blockSize - 16)
                    key[i + 8] = key[i - (blockSize - 16)];
            }

            //swap
            for (int i = 0; i < 8; i++)
                Swap(ref key[i + 8], ref key[i + blockSize - 8]);

            for (int i = 0; i < blockSize / BYTE_SIZE; i++)
            {
                Swap(ref key[(i * 8) + 2], ref key[(i * 8) + 3]);
                Swap(ref key[(i * 8) + (round % 8)], ref key[(i * 8) + (round % 5) + 3]);
            }

            //bitwise not
            for (int i = 0; i < blockSize / BYTE_SIZE; i++)
            {
                key[(i * 8) + 0] = (byte)~key[(i * 8) + 7];
                key[(i * 8) + 6] = (byte)~key[(i * 8) + 1];
                key[(i * 8) + 2] = (byte)~key[(i * 8) + 5];
                key[(i * 8) + 4] = (byte)~key[(i * 8) + 3];
            }

            return key;
        }

        private void Substitute(ref byte data1, ref byte data2)
        {
            byte temp = data1;
            data1 = (byte)((byte)(data2 << 4) | (byte)(data1 & 0x0F));
            data2 = (byte)((byte)(temp >> 4) | (byte)(data2 & 0xF0));
        }

        private void Swap<T>(ref T object1, ref T object2)
        {
            T temp = object1;
            object1 = object2;
            object2 = temp;
        }

        private void ShiftLeft(ref byte data, int offset)
        {
            offset %= BYTE_SIZE;
            byte temp = data;
            temp >>= (BYTE_SIZE - offset);
            data <<= offset;
            data |= temp;
        }

        private void ShiftRight(ref byte data, int offset)
        {
            offset %= BYTE_SIZE;
            byte temp = data;
            temp <<= (BYTE_SIZE - offset);
            data >>= offset;
            data |= temp;
        }
    }
}
