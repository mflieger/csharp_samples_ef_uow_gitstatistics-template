using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string Filename = "commits.txt";

        /// <summary>
        /// Liefert die Messwerte mit den dazugehörigen Sensoren
        /// </summary>
        public static Commit[] ReadFromCsv()
        {
            List<Commit> commits = new List<Commit>();
            string[] lines = File.ReadAllLines(MyFile.GetFullNameInApplicationTree(Filename));
            Dictionary<string, Developer> developers = new Dictionary<string, Developer>();

            bool isHeader = true;
            Developer dev = null;
            string hashcode = null;
            DateTime dateTime = DateTime.Now;
            string message = null;
            int fileChanges = 0;
            int inserts = 0;
            int deletes = 0;

            foreach (var line in lines)
            {
                string[] parts = line.Split(",");

                if (parts.Length > 1)
                {
                    if (parts.Length >= 4)
                    {
                        if(!isHeader)
                        {
                            Commit newCommit = new Commit()
                            {
                                HashCode = hashcode,
                                Developer = dev,
                                Date = dateTime,
                                Message = message,
                                FilesChanges = fileChanges,
                                Insertions = inserts,
                                Deletions = deletes
                            };

                            commits.Add(newCommit);
                            isHeader = true;
                        }
                        if (!developers.ContainsKey(parts[1]))
                        {
                            dev = new Developer() { Name = parts[1] };
                            developers.Add(parts[1], dev);
                        }
                        else
                        {
                            dev = developers.GetValueOrDefault(parts[1]);
                        }

                        hashcode = parts[0];
                        dateTime = Convert.ToDateTime(parts[2]);

                        for (int i = 3; i < parts.Length; i++)
                        {
                            message = $"{message}, {parts[i]}";
                        }

                        isHeader = false;
                    }

                    if(parts.Length <= 3)
                    {
                            fileChanges = GetFirstValueOfString(parts[0]);

                            if (parts.Length == 3)
                            {
                                inserts = GetFirstValueOfString(parts[1]);
                                deletes = GetFirstValueOfString(parts[2]);
                            }
                            else if (parts[1].Contains("insertion"))
                            {
                                inserts = GetFirstValueOfString(parts[1]);
                            }
                            else if (parts[1].Contains("deletion"))
                            {
                                deletes = GetFirstValueOfString(parts[1]);
                            }                        
                    }
                }
            }

            Commit lastCommit = new Commit()
            {
                HashCode = hashcode,
                Developer = dev,
                Date = dateTime,
                Message = message,
                FilesChanges = fileChanges,
                Insertions = inserts,
                Deletions = deletes
            };

            commits.Add(lastCommit);


            return commits.ToArray();
        }

        private static int GetFirstValueOfString(string input)
        {
            string[] parts = input.Split(" ");

            return Convert.ToInt32(parts[1]);
        }
    }
}
