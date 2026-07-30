using Microsoft.EntityFrameworkCore;
using SlojPodataka.Model;
using System.Collections.Generic;

namespace SlojPodataka.Kontekst
{
    public class AppDbContext : DbContext
    {
        public DbSet<TakmicenjeModel> TakmicenjaModelObjektiDBSet { get; set; }

        public DbSet<DiplomaModel> DiplomaModelObjektiDBSet { get; set; }

        public DbSet<KorisnikModel> KorisnikModelObjektiDBSet { get; set; }
        public DbSet<UcenikModel> UcenikModelObjektiDBSet { get; set; }

        public DbSet<PredmetModel> PredmetiModelObjektiDBSet { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opcije)
            : base(opcije)
        {
        }
    }
}
