using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections;

namespace VerifyMD5Hashes
{
    class Program
    {
        
        static string fileFolderPath;
        static string md5FilePath;
        static Log log;

        static void Main(string[] args)
        {
            Console.WriteLine("Start...");

            if (args.Length != 3)
            {
                Console.WriteLine("Please enter three arguements, whether to VERIFY hashes or just MATCH file names (V/M), the fileFolderPath and md5FilePath, for example {0}",
                    @"VerifyMD5Hashes V, i:\NB\2010\2010-12, i:\NB\2010\2010-12\md5sums.txt");
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            else
            {
                fileFolderPath = args[1];
                md5FilePath = args[2];
            }
            
            log = new Log(fileFolderPath);

            // Get Files
            DirectoryInfo di = new DirectoryInfo(fileFolderPath);
            FileInfo[] fi = di.GetFiles();
            

            // Put fi into hashtable
            Hashtable files = new Hashtable();
            foreach (FileInfo file in fi)
            {
                if (file.Name != "md5sums.txt")
                    files.Add(file.Name,"");
            }

            int unMatchedFiles = 0;
            // Get Hashes
            Hashtable hashes = new Hashtable();
            using (TextReader tr = new StreamReader(md5FilePath))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    if (line.Contains("page"))
                    {
                        string[] strings = line.Split(' ');
                        string MD5 = strings[0];
                        string fileName = strings[2];
                        if (!files.Contains(fileName))
                        {
                            log.Write(fileName + " IS NOT CONTAINED IN SET!");
                            unMatchedFiles++;
                        }
                        hashes.Add(fileName, MD5);
                    }
                }
            }

            if (unMatchedFiles == 0)
                log.Write("All files are in set!");
            
            if(args[0] == "V")
                CheckMD5(fi, hashes);
        }

        private static void CheckMD5(FileInfo[] fi, Hashtable hashes)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                int i = 0;
                int WrongHashes = 0;
                DateTime start = DateTime.Now;
                foreach (FileInfo file in fi)
                {
                    if ((file.FullName != md5FilePath) && (file.FullName != log.logPath))
                    {
                        i++;
                        DateTime startDownload = DateTime.Now;
                        log.Write("Start verification of " + file.Name + " at " + startDownload);
                        log.Write("This is file # " + i + " out of " + fi.Length);
                        log.Write("You are " + ((float)i / (float)fi.Length) * 100 + "% complete.");
                        string hash = GetMd5Hash(md5Hash, file.FullName);
                        log.Write("The MD5 hash of " + file.Name + " is: " + hash + ".");
                        log.Write("Took " + (DateTime.Now - startDownload).TotalSeconds + " seconds.");
                        log.Write("Verifying the hash...");

                        if (VerifyMd5Hash(hash, hashes[file.Name].ToString()))
                        {
                            log.Write("**SAME**");
                            log.Write(hash);
                            log.Write(hashes[file.Name].ToString());
                        }
                        else
                        {
                            WrongHashes++;
                            log.Write("**********PROBLEM - HASHES ARE NOT THE SAME************");
                            log.Write("**********PROBLEM - HASHES ARE NOT THE SAME************");
                            log.Write("**********PROBLEM - HASHES ARE NOT THE SAME************");
                        }
                        log.Write((DateTime.Now - start).TotalMinutes + " minutes have elapsed since start of program.");
                        log.Write("There are " + WrongHashes + " wrong hashes so far.");
                        log.Write("-----------------------------------------------------------------------");
                        log.Write("");
                    }
                }
                log.Write("There were " + WrongHashes + " wrong hashes.");
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string filePath)
        {

            FileStream file = new FileStream(filePath, FileMode.Open);
            byte[] retVal = md5Hash.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(string source, string target)
        {
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(source, target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        

    }
}
