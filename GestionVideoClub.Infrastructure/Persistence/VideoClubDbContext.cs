using GestionVideoClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionVideoClub.Infrastructure.Persistence
{
    public class VideoClubDbContext : DbContext
    {
        public VideoClubDbContext(DbContextOptions<VideoClubDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Format> Formats => Set<Format>();
        public DbSet<Copy> Copies => Set<Copy>();
        public DbSet<Rent> Rents => Set<Rent>();
        public DbSet<Fine> Fines => Set<Fine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Rent <-> Fine es 1 a 1: un alquiler tiene a lo sumo una multa, y la FK vive en Fine.
            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Fine)
                .WithOne(f => f.Rent)
                .HasForeignKey<Fine>(f => f.RentId);

            // Restrict: no queremos que borrar un Client/Employee/Copy borre en cascada el historial de alquileres.
            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Copy)
                .WithMany()
                .HasForeignKey(r => r.CopyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Una copia física pertenece a una película y a un formato; ambos pueden tener muchas copias.
            modelBuilder.Entity<Copy>()
                .HasOne(c => c.Movie)
                .WithMany()
                .HasForeignKey(c => c.MovieId);

            modelBuilder.Entity<Copy>()
                .HasOne(c => c.Format)
                .WithMany()
                .HasForeignKey(c => c.FormatId);

            // AutoInclude: estas navegaciones se usan junto con la entidad principal en casi todos los casos.
            modelBuilder.Entity<Copy>().Navigation(c => c.Movie).AutoInclude();
            modelBuilder.Entity<Copy>().Navigation(c => c.Format).AutoInclude();
            modelBuilder.Entity<Rent>().Navigation(r => r.Copy).AutoInclude();
            modelBuilder.Entity<Rent>().Navigation(r => r.Client).AutoInclude();
            modelBuilder.Entity<Rent>().Navigation(r => r.Employee).AutoInclude();
            modelBuilder.Entity<Fine>().Navigation(f => f.Rent).AutoInclude();

            // Precisión explícita para montos monetarios: SQL Server necesita (precision, scale) definidos.
            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Fine>().Property(f => f.Amount).HasPrecision(18, 2);
        }
    }
}
