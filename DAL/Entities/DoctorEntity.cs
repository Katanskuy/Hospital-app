using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("Доктори")]
    public class DoctorEntity
    {
        [Key]
        [Column("ID_доктора")]
        public int Id { get; set; }

        [Column("ПІБ_доктора")]
        public string FullName { get; set; }

        [Column("Дата_народження")]
        public DateTime Birthday { get; set; }

        [Column("Повних_років")]
        public int Age { get; set; }

        [Column("Спеціалізація")]
        public string Specialisation { get; set; }
    }
}
