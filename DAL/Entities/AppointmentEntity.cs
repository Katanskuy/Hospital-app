using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("Візити")]
    public class AppointmentEntity
    {
        [Key]
        [Column("ID_візиту")]
        public int AppointmentId { get; set; }

        [Column("ID_доктора")]
        public int DoctorId { get; set; }

        [Column("ПІБ_доктора")]
        public string DoctorName { get; set; }

        [Column("ID_пацієнта")]
        public int PatientId { get; set; }

        [Column("ПІБ_пацієнта")]
        public string PatientName { get; set; }

        [Column("Дата_візиту")]
        public DateTime AppointmentDate { get; set; }

        [Column("Початок_візиту")]
        public DateTime AppointmentStart { get; set; }

        [Column("Кінець_візиту")]
        public DateTime AppointmentEnd { get; set; }
    }
}
