using System;
using System.IO;
using System.Linq;
using static FileGenerator.FileGenerator;

namespace Custis
{
    class Program
    {
        static void Main(string[] args)
        {
            long rowCount;
            int maxLength;

            Console.Write("Кол-во строк: ");
            Int64.TryParse(Console.ReadLine(), out rowCount);

            Console.Write("Максимальная длина строки: ");
            Int32.TryParse(Console.ReadLine(), out maxLength);

            Console.Write("Полный путь до файла (создавать файл не обязательно): ");
            var path = Console.ReadLine();

            if (Directory.Exists(path) || rowCount == 0 || maxLength == 0)
            {
                Console.WriteLine("Некорректные данные");
            }
            else
            {
                Generator(path, rowCount, maxLength);

                var untitledPath = path.Substring(0, path.Length - path.Split('\\').Last().Length);
                ExternalSort.Algorithm(untitledPath, path.Split('\\').Last());
            }
        }

    }
}
