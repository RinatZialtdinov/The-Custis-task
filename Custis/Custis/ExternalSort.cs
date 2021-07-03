using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Custis
{
    class ExternalSort
    {
        static public void Algorithm(string dir, string fileName)
        {
            Split(dir, fileName);

            Sort(dir);

            Merge(dir);
        }

        static void Merge(string dir)
        {

            string[] paths = Directory.GetFiles(dir, "sorted*.dat");
            int chunks = paths.Length;
            int recordsize = 100;
            int maxusage = 500000000;
            int buffersize = maxusage / chunks;
            double recordoverhead = 7.5;
            int bufferlen = (int)(buffersize / recordsize / recordoverhead);

            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            StreamWriter sw = new StreamWriter(dir + "\\BigFileSorted.txt");
            bool done = false;
            int lowestIndex;
            string lowestValue;
            while (!done)
            {
                lowestIndex = -1;
                lowestValue = "";
                for (int j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowestIndex < 0 || String.Compare(queues[j].Peek(), lowestValue, StringComparison.CurrentCulture) < 0)
                        {
                            lowestIndex = j;
                            lowestValue = queues[j].Peek();
                        }
                    }
                }

                if (lowestIndex == -1) { break; }

                sw.WriteLine(lowestValue);
                queues[lowestIndex].Dequeue();

                if (queues[lowestIndex].Count == 0)
                {
                    LoadQueue(queues[lowestIndex], readers[lowestIndex], bufferlen);
                    if (queues[lowestIndex].Count == 0)
                    {
                        queues[lowestIndex] = null;
                    }
                }
            }
            sw.Close();

            for (int i = 0; i < chunks; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }

        static void LoadQueue(Queue<string> queue, StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        static void Sort(string directory)
        {
            foreach (string path in Directory.GetFiles(directory, "split*.dat"))
            {
                string[] contents = File.ReadAllLines(path);
                Array.Sort(contents);
                string newpath = path.Replace("split", "sorted");
                File.WriteAllLines(newpath, contents);
                File.Delete(path);
                contents = null;
                GC.Collect();
            }
        }

        static void Split(string dir, string fileName)
        {
            int splitNum = 1;
            StreamWriter sw = new StreamWriter(string.Format(dir + "\\split{0:d5}.dat", splitNum));

            using (StreamReader sr = new StreamReader(dir + fileName))
            {
                while (sr.Peek() >= 0)
                {
                    sw.WriteLine(sr.ReadLine());

                    if (sw.BaseStream.Length > 100000000 && sr.Peek() >= 0)
                    {
                        sw.Close();
                        splitNum++;
                        sw = new StreamWriter(string.Format(dir + "\\split{0:d5}.dat", splitNum));
                    }
                }
            }
            sw.Close();
        }

    }
}
