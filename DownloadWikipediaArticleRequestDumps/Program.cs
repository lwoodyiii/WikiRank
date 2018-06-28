using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DownloadWikipediaArticleRequestDumps
{
    class Program
    {
        static string downloadPath = @"f:\NB\2008\";
        static string url = @"http://dumps.wikimedia.org/other/pagecounts-raw/2008/";
        static string md5File = @"f:\NB\2008\";
        static ArrayList files = new ArrayList();

        static void Main(string[] args)
        {
            Console.WriteLine("Start...");

            // args[0] should be 2011-07 for example
            // Path would be F:\NB\2011-07
            if (args.Length > 0)
            {
                url = url + args[0] + "/";
                md5File = md5File + args[0] + @"\md5sums.txt";
                downloadPath = downloadPath + args[0] + @"\";
            }

            using (TextReader tr = new StreamReader(md5File))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    if (line.Contains("page"))
                        files.Add(line);
                }
            }

            WebClient client = new WebClient();
            int i = 0;
            DateTime start = DateTime.Now;
            double totalFileSizeInMB = 0;
            foreach (string s in files)
            {
                i++;
                string[] strings = s.Split(' ');
                string MD5 = strings[0];
                string fileName = strings[2];
                DateTime startDownload = DateTime.Now;
                Console.WriteLine("Start Download of " + fileName + " at " + startDownload);
                Console.WriteLine("This is file # " + i + " out of " + files.Count);
                Console.WriteLine("You are " + ((float)i/(float)files.Count) * 100 + "% complete.");

                try
                {
                    client.DownloadFile(url + fileName, downloadPath + fileName);
                }
                catch (WebException)
                {
                    System.Threading.Thread.Sleep(60*5*1000);
                    client.DownloadFile(url + fileName, downloadPath + fileName);
                }
                
                FileInfo fi = new FileInfo(downloadPath + fileName);
                double fileSizeInMB = fi.Length / Math.Pow(2, 20);
                totalFileSizeInMB += fileSizeInMB;
                DateTime endDownload = DateTime.Now;
                double delta = (endDownload - startDownload).TotalSeconds;
                Console.WriteLine("End Download of " + fileName + " at " + endDownload);
                Console.WriteLine("Took " + delta + " seconds to download " + fileSizeInMB + " MB");
                Console.WriteLine("Or " + fileSizeInMB / delta + " MB per second");
                Console.WriteLine((endDownload - start).TotalMinutes + " minutes have elapsed since start of program.");
                Console.WriteLine("Your avg. bandwidth is " + totalFileSizeInMB / (endDownload - start).TotalSeconds);
                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine();

            }
            Console.WriteLine("End...");
        }
    }
}
