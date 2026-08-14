using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDataBase.Objects
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentStart { get; set; }
        public DateTime AppointmentEnd { get; set; }
    }
}
