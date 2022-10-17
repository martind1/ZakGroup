using Microsoft.EntityFrameworkCore;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Connection.DPE
{
    public partial class DpeContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // weil sonst Tablename='VORF_Tbl' !?!
            modelBuilder.Entity<VORF>(entity => entity.ToTable("VORF"));

            modelBuilder.Entity<FLTR>(entity => entity.ToTable("FLTR"));

            modelBuilder.Entity<ASWS>(entity => entity.ToTable("ASWS"));

            modelBuilder.Entity<R_INIT>(entity => entity.ToTable("R_INIT"));

            modelBuilder.Entity<V_LADEZETTEL>(entity => entity.ToTable("V_LADEZETTEL"));
            modelBuilder.Entity<V_LADEZETTEL>(entity =>
                entity.HasKey(e => e.vorf_nr));
            //.HasName("PK_NEUVORF")
            //.IsClustered(false));

            modelBuilder.Entity<DKAT>(entity => entity.ToTable("DKAT"));
            modelBuilder.Entity<VFUE>(entity => entity.ToTable("VFUE"));


            /* Precision:
            modelBuilder.Entity<VORF>(entity =>
            {
                //Problem: [v].[tagew] <= 8.1099999999999994E0 (statt 8.11)
                //entity.Property(e => e.tagew).HasPrecision(28, 2);  //.HasColumnType("double?");
                //bringt nix entity.Property(e => e.tagew).HasColumnType("decimal?").HasPrecision(28, 2); 
            });
            */
            //so nicht:
            //modelBuilder.Entity<VORF>().Property(p => p.cfHOFL_OK)
            //    .HasComputedColumnSql("case HOFL_OK when 'J' then true else false end");
        }
    }
}
