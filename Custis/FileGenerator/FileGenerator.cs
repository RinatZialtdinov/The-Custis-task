using System;
using System.IO;

namespace FileGenerator
{
    public static class FileGenerator
    {
        public static void Generator(string fileName, long rowCount, int maxLength)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                Random rnd = new Random();
                for (int i = 0; i < rowCount; i++)
                {
                    sw.WriteLine(RandomString(rnd.Next(1, maxLength + 1)));
                }
            }
        }

        private static string RandomString(int length)
        {
            var result = new char[length];
            var rand = new Random();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)rand.Next(65, 123);
            }
            return new string(result);
        }


    }
}
