using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Laboratory.Data
{
    public class LaboratoryDbContext : DbContext
    {
        public LaboratoryDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public DbSet<ResultEntity> Patients { get; set; }
    }
}
