using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("SickPeopleData")]
    public class PatientEntity
    {
        [Key]
        [Column("ID_пацієнта")]
        public int Id { get; set; }

        [Column("ПІБ_пацієнта")]
        public string FullName { get; set; }

        [Column("Дата_народження")]
        public DateTime Birthday { get; set; }

        [Column("Повних_років")]
        public int Age { get; set; }

        [Column("Опис_хвороби")]
        public string SicknessDesc { get; set; }
    }
}
