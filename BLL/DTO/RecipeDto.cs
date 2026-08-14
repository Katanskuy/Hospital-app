using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Medicine1 { get; set; }
        public string? Medicine2 { get; set; }
        public string? Medicine3 { get; set; }
        public string? Medicine4 { get; set; }
        public string? Medicine5 { get; set; }
    }
}
