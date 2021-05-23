using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FindCandidate.Data
{
    public class FindCandidateDbContext : DbContext
    {
        public FindCandidateDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public DbSet<PatientEntity> Patients { get; set; }
    }
}
