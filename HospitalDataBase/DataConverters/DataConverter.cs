using HospitalDataBase.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDataBase.DataConverters
{
    public static class DataGridViewRowExtensions
    {
        public static Appointment ToAppointment(this DataGridViewRow row)
        {
            return new Appointment
            {
                AppointmentId = Convert.ToInt32(row.Cells["ID_візиту"].Value),
                DoctorId = Convert.ToInt32(row.Cells["ID_доктора"].Value),
                DoctorName = row.Cells["ПІБ_доктора"].Value?.ToString(),
                PatientId = Convert.ToInt32(row.Cells["ID_пацієнта"].Value),
                PatientName = row.Cells["ПІБ_пацієнта"].Value?.ToString(),
                AppointmentDate = Convert.ToDateTime(row.Cells["Дата_візиту"].Value),
                AppointmentStart = Convert.ToDateTime(row.Cells["Початок_візиту"].Value),
                AppointmentEnd = Convert.ToDateTime(row.Cells["Кінець_візиту"].Value)
            };
        }

        public static Doctor ToDoctor(this DataGridViewRow row)
        {
            return new Doctor
            {
                Id = Convert.ToInt32(row.Cells["ID_доктора"].Value),
                FullName = row.Cells["ПІБ_доктора"].Value?.ToString(),
                Birthday = Convert.ToDateTime(row.Cells["Дата_народження"].Value),
                Age = Convert.ToInt32(row.Cells["Повних_років"].Value),
                Specialisation = row.Cells["Спеціалізація"].Value?.ToString()
            };
        }

        public static Patient ToPatient(this DataGridViewRow row)
        {
            return new Patient
            {
                Id = Convert.ToInt32(row.Cells["ID_пацієнта"].Value),
                FullName = row.Cells["ПІБ_пацієнта"].Value?.ToString(),
                Birthday = Convert.ToDateTime(row.Cells["Дата_народження"].Value),
                Age = Convert.ToInt32(row.Cells["Повних_років"].Value),
                SicknessDesc = row.Cells["Опис_хвороби"].Value?.ToString()
            };
        }

        public static Recipe ToRecipe(this DataGridViewRow row)
        {
            return new Recipe
            {
                RecipeId = Convert.ToInt32(row.Cells["ID_рецепту"].Value),
                FullName = row.Cells["ПІБ_пацієнта"].Value?.ToString(),
                Birthday = Convert.ToDateTime(row.Cells["Дата_народження"].Value),
                Age = Convert.ToInt32(row.Cells["Повних_років"].Value),
                StartDate = Convert.ToDateTime(row.Cells["Початок_рецепту"].Value),
                EndDate = Convert.ToDateTime(row.Cells["Кінець_рецепту"].Value),
                Medicine1 = row.Cells["Назва_Ліків_1"].Value?.ToString(),
                Medicine2 = row.Cells["Назва_Ліків_2"].Value?.ToString(),
                Medicine3 = row.Cells["Назва_Ліків_3"].Value?.ToString(),
                Medicine4 = row.Cells["Назва_Ліків_4"].Value?.ToString(),
                Medicine5 = row.Cells["Назва_Ліків_5"].Value?.ToString(),
            };
        }
    }
}
