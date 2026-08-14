using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("Рецепт")]
    public class RecipeEntity
    {
        [Key]
        [Column("ID_рецепта")]
        public int RecipeId { get; set; }

        [Column("ID_пацієнта")]
        public int PatientId { get; set; }

        [Column("ПІБ_пацієнта")]
        public string FullName { get; set; }

        [Column("Дата_народження")]
        public DateTime Birthday { get; set; }

        [Column("Повних_років")]
        public int Age { get; set; }

        [Column("Дата_початку")]
        public DateTime StartDate { get; set; }

        [Column("Дата_кінця")]
        public DateTime EndDate { get; set; }

        [Column("Назва_ліків_1")]
        public string Medicine1 { get; set; }

        [Column("Назва_ліків_2")]
        public string? Medicine2 { get; set; }

        [Column("Назва_ліків_3")]
        public string? Medicine3 { get; set; }

        [Column("Назва_ліків_4")]
        public string? Medicine4 { get; set; }

        [Column("Назва_ліків_5")]
        public string? Medicine5 { get; set; }
    }
}
