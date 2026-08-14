using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> AddPatientAsync(PatientDto patientDto);
        Task<PatientDto> DeletePatientAsync(PatientDto patientDto);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<PatientDto> UpdatePatientAsync(PatientDto patientDto);
    }
}
