using Microsoft.EntityFrameworkCore;
using DAL.Entities;

namespace DAL
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<AppointmentEntity> Appointments { get; set; }
        public DbSet<DoctorEntity> Doctors { get; set; }
        public DbSet<PatientEntity> Patients { get; set; }
        public DbSet<RecipeEntity> Recipies { get; set; }
    }
}
