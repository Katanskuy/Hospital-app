using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalDbContext _context;

        public IGenericRepository<AppointmentEntity> Appointments { get; private set; }
        public IGenericRepository<DoctorEntity> Doctors { get; private set; }
        public IGenericRepository<PatientEntity> Patients { get; private set; }
        public IGenericRepository<RecipeEntity> Recipes { get; private set; }

        public UnitOfWork(HospitalDbContext context)
        {
            _context = context;
            Appointments = new GenericRepository<AppointmentEntity>(_context);
            Doctors = new GenericRepository<DoctorEntity>(_context);
            Patients = new GenericRepository<PatientEntity>(_context);
            Recipes = new GenericRepository<RecipeEntity>(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
