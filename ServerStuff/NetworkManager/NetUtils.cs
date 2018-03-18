using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetworkManager
{
    public static class NetUtils
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static string ConvertByteToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
            {
                if (b == 0)
                    break;
                response += (Char)b;
            }
            return response;
        }
        public static byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }
        public static bool[] ConvertByteToBoolArray(byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) == 0 ? false : true;

            // reverse the array
            Array.Reverse(result);

            return result;
        }
        public static object[] FormCommand(byte[] data, string[] split)
        {
            var temp = new List<object>();
            int pos = 0;
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "string" || split[i] == "s") // first 2 bytes length
                {
                    Int16 len = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                    pos += 2;
                    temp.Add(ConvertByteToString(data.SubArray(pos, len)));
                    pos += len;
                }
                else if (split[i] == "byte" || split[i] == "b")
                {
                    temp.Add(data[pos++]);
                }
                else if (split[i] == "bool" || split[i] == "bo")
                {
                    temp.Add(data[pos++] == 255);
                }
                else if (split[i] == "int" || split[i] == "i") // first 2 bytes length
                {
                    temp.Add(BitConverter.ToInt32(data.SubArray(pos, 4), 0));
                    pos += 4;
                }
                else if (split[i] == "message" || split[i] == "m") // first 2 bytes length
                {
                    Int16 len = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                    pos += 2;
                    temp.Add(new Message(data.SubArray(pos, len)));
                    pos += len;
                }
                else if (split[i] == "pid" || split[i] == "p") // first 2 bytes length
                {
                    Int16 len = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                    pos += 2;
                    temp.Add(new PID(data.SubArray(pos, len)));
                    pos += len;
                }
                else if (split[i] == "room" || split[i] == "r") // first 2 bytes length
                {
                    Int16 len = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                    pos += 2;
                    temp.Add(new Room(data.SubArray(pos, len)));
                    pos += len;
                }
                else if (split[i] == "string[]" || split[i] == "s[]")
                {
                    var strArr = new List<string>();
                    Int16 slen = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                    pos += 2;
                    for (int j = 0; j < slen; j++)
                    {
                        Int16 len = BitConverter.ToInt16(data.SubArray(pos, 2), 0);
                        pos += 2;
                        strArr.Add(ConvertByteToString(data.SubArray(pos, len)));
                        pos += len;
                    }
                    temp.Add(strArr.ToArray());
                }
            }
            return temp.ToArray();
        }
        public static byte[] PieceCommand(object[] parts)
        {
            var temp = new List<byte>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].GetType().ToString().Contains("String[]"))
                {
                    string[] arr = (string[])parts[i];
                    int slen = arr.Length;
                    byte[] dat;
                    byte[] len;
                    len = BitConverter.GetBytes((Int16)slen);
                    temp.Add(len[0]);
                    temp.Add(len[1]);
                    for (int j = 0; j < slen; j++)
                    {
                        dat = ASCIIEncoding.ASCII.GetBytes((string)arr[j]);
                        len = BitConverter.GetBytes((Int16)dat.Length);
                        temp.Add(len[0]);
                        temp.Add(len[1]);
                        for (int k = 0; k < dat.Length; k++)
                        {
                            temp.Add(dat[k]);
                        }
                    }
                }
                else if (parts[i].GetType().ToString().Contains("String"))
                {
                    byte[] dat = ASCIIEncoding.ASCII.GetBytes((string)parts[i]);
                    byte[] len = BitConverter.GetBytes((Int16)dat.Length);
                    temp.Add(len[0]);
                    temp.Add(len[1]);
                    for (int j = 0; j < dat.Length; j++)
                    {
                        temp.Add(dat[j]);
                    }
                }
                else if (parts[i].GetType().ToString().Contains("Byte"))
                {
                    temp.Add((byte)parts[i]);
                }
                else if (parts[i].GetType().ToString().Contains("Bool"))
                {
                    bool _temp = (bool)parts[i];
                    if (_temp)
                    {
                        temp.Add(255);
                    } else
                    {
                        temp.Add(0);
                    }
                }
                else if (parts[i].GetType().ToString().Contains("Int"))
                {
                    byte[] dat = BitConverter.GetBytes((Int32)parts[i]);
                    for (int j = 0; j < dat.Length; j++)
                    {
                        temp.Add(dat[j]);
                    }
                }
                else if (parts[i].GetType().ToString().Contains("Message"))
                {
                    byte[] dat = ((Message)parts[i]).ToBytes();
                    byte[] len = BitConverter.GetBytes((Int16)dat.Length);
                    temp.Add(len[0]);
                    temp.Add(len[1]);
                    for (int j = 0; j < dat.Length; j++)
                    {
                        temp.Add(dat[j]);
                    }
                }
                else if (parts[i].GetType().ToString().Contains("PID"))
                {
                    byte[] dat = ((PID)parts[i]).ToBytes();
                    byte[] len = BitConverter.GetBytes((Int16)dat.Length);
                    temp.Add(len[0]);
                    temp.Add(len[1]);
                    for (int j = 0; j < dat.Length; j++)
                    {
                        temp.Add(dat[j]);
                    }
                }
                else if (parts[i].GetType().ToString().Contains("Room"))
                {
                    byte[] dat = ((Room)parts[i]).ToBytes();
                    byte[] len = BitConverter.GetBytes((Int16)dat.Length);
                    temp.Add(len[0]);
                    temp.Add(len[1]);
                    for (int j = 0; j < dat.Length; j++)
                    {
                        temp.Add(dat[j]);
                    }
                }
            }
            return temp.ToArray();
        }
    }
}
