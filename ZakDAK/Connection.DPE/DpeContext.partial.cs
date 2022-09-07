using Microsoft.EntityFrameworkCore;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Connection.DPE
{
    public partial class DpeContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VORF>(entity => entity.ToTable("VORF"));

            modelBuilder.Entity<FLTR>(entity => entity.ToTable("FLTR"));

            modelBuilder.Entity<ASWS>(entity => entity.ToTable("ASWS"));

            modelBuilder.Entity<R_INIT>(entity => entity.ToTable("R_INIT"));
        }

    }
}
