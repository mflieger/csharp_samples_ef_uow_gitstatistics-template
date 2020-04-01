using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
    public class CommitRepository : ICommitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Commit[] commits)
        {
            _dbContext.Commits.AddRange(commits);
        }

        public Commit GetById(int id) => _dbContext
            .Commits
            .Include(c => c.Developer)
            .SingleOrDefault(c => c.Id == id);

        public Commit[] GetFromLast4Weeks() => _dbContext
            .Commits
            .Include(c => c.Developer)
            .Where(c => c.Date.Year == 2019 && c.Date.Month == 3)
            .OrderBy(c => c.Date)
            .ToArray();
    }
}