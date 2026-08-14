using BLL.Interfaces;
using BLL.DTO;
using DAL;
using DAL.Entities;
using AutoMapper;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public PatientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PatientDto> AddPatientAsync(PatientDto patientDto)
        {
            var patientEntity = _mapper.Map<PatientEntity>(patientDto);
            await _unitOfWork.Patients.AddAsync(patientEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PatientDto>(patientEntity);
        }

        public async Task<PatientDto> DeletePatientAsync(PatientDto patientDto)
        {
            var patientEntity = await _unitOfWork.Patients.GetByIdAsync(patientDto.Id);

            if (patientEntity == null)
                throw new InvalidOperationException($"Пацієнта з ID {patientDto.Id} не знайдено.");

            _unitOfWork.Patients.Delete(patientEntity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PatientDto>(patientEntity);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patientEntities = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patientEntities);
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var patientEntity = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patientEntity == null)
                throw new InvalidOperationException($"Пацієнта з ID {id} не знайдено.");
            return _mapper.Map<PatientDto>(patientEntity);
        }

        public async Task<PatientDto> UpdatePatientAsync(PatientDto patientDto)
        {
            var patientEntity = await _unitOfWork.Patients.GetByIdAsync(patientDto.Id);
            if (patientEntity == null)
                throw new InvalidOperationException($"Пацієнта з ID {patientDto.Id} не знайдено.");
            _mapper.Map(patientDto, patientEntity);
            _unitOfWork.Patients.Update(patientEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PatientDto>(patientEntity);
        }
    }
}
