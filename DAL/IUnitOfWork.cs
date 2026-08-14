using DAL.Repositories;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<AppointmentEntity> Appointments { get; }
        IGenericRepository<DoctorEntity> Doctors { get; }
        IGenericRepository<PatientEntity> Patients { get; }
        IGenericRepository<RecipeEntity> Recipes { get; }


        Task<int> SaveAsync();
    }
}
