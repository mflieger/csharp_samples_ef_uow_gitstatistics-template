using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using GitStat.Persistence;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.txt eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
            }
            Console.WriteLine("Datenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                var commits = unitOfWork.CommitRepository.GetFromLast4Weeks();
                Console.WriteLine("Commits der letzten 4 Wochen");
                Console.WriteLine("----------------------------");
                WriteCommits(commits);

                var commitById = unitOfWork.CommitRepository.GetById(4);
                Console.WriteLine("Commits mit der Id 4");
                Console.WriteLine("--------------------");
                Console.WriteLine($"{"Developer", -20} {"Date", -15} {"FileChanges", -15} {"Insertions", -15} Deletions");
                Console.WriteLine($"{commitById.Developer.Name, -20} {commitById.Date.ToShortDateString(), -15} {commitById.FilesChanges, -15} {commitById.Insertions, -15   } {commitById.Deletions}");

                Console.WriteLine("Statistik der Commits der Developer");

                Console.WriteLine("");
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static void WriteCommits(Commit[] commits)
        {
            Console.WriteLine($"{"Developer", -20} {"Date", -15} {"FileChanges", -15} {"Insertions", -15} Deletions");

            foreach (var item in commits)
            {
                Console.WriteLine($"{item.Developer.Name, -20} {item.Date.ToShortDateString(), -15} {item.FilesChanges, -15} {item.Insertions, -15} {item.Deletions}");
            }
        }
    }
}
