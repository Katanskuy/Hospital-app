using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL;
using HospitalDataBase.DataConverters;
using HospitalDataBase.Objects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;

namespace HospitalDataBase
{
    public partial class MainMenu : Form
    {
        SqlConnection con = new("Data Source=DESKTOP-RIUJSPQ\\SQLEXPRESS;Database=Hospital;trusted_connection=true;TrustServerCertificate=True");
        string query = "";
        int idVar;
        List<Patient> patientList = [];
        List<Doctor> doctorList = [];
        List<Recipe> recipeList = [];
        List<Appointment> appointmentList = [];

        private readonly HospitalDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;

        public MainMenu(HospitalDbContext context, IMapper mapper, IPatientService patientService)
        {
            InitializeComponent();
            _context = context;
            _mapper = mapper;
            _patientService = patientService;
        }
        private void MainMenu_Load(object sender, EventArgs e)
        {
            con.Open();
        }

        public static void ClearAll(Control control)
        {
            foreach (Control c in control.Controls)
            {
                TextBox? texbox = c as TextBox;
                ComboBox? comboBox = c as ComboBox;
                DateTimePicker? dateTimePicker = c as DateTimePicker;

                if (texbox != null)
                    texbox.Clear();
                if (comboBox != null)
                {
                    comboBox.SelectedIndex = -1;
                    comboBox.Items.Clear();
                }
                if (dateTimePicker != null)
                    dateTimePicker.Value = DateTime.Now;
                if (c.HasChildren)
                    ClearAll(c);
            }
        }

        private void PatientMenu_Click(object sender, EventArgs e)
        {
            ClearAll(DoctorPanel);
            ClearAll(RecipePanel);
            ClearAll(AppointmentPanel);
            query = "SELECT * FROM SickPeopleData";
            SqlDataAdapter da = new(query, con);
            DataTable dt = new();
            da.Fill(dt);
            MainPatientPanel.Visible = true;
            MainDoctorPanel.Visible = false;
            MainRecipePanel.Visible = false;
            MainAppointmentPanel.Visible = false;
            PatientDataTable.DataSource = dt;
        }

        private void PatientDataTable_MouseClick(object sender, MouseEventArgs e)
        {
            if (PatientDataTable.SelectedRows.Count > 0 && e.Button is MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Right)
                {
                    PatientContextStrip.Show(MousePosition);
                }
            }
        }

        private async void DeletePatientStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Чи ви впевнені, що хочете видалити", "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var row = PatientDataTable.SelectedRows[0];
                Patient patient = row.ToPatient();

                PatientDto patientDto = _mapper.Map<PatientDto>(patient);
                await _patientService.DeletePatientAsync(patientDto);
            }
        }

        private void ChangePatientStrip_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);
            ChangePatientPanel.Visible = true;
            AddPatientPanel.Visible = false;
            SearchPatientPanel.Visible = false;

            var row = PatientDataTable.SelectedRows[0];
            Patient patient = row.ToPatient();

            idVar = Convert.ToInt32(patient.Id);
            ChangedNameBox.Text = patient.FullName;
            ChangedBirthdayPicker.Value = patient.Birthday;
            ChangedDescBox.Text = patient.SicknessDesc;
        }

        private async void ChangePatientSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(idVar);

                var patientEntity = await _patientService.GetPatientByIdAsync(id);
                if (patientEntity == null)
                {
                    MessageBox.Show("Пацієнта не знайдено.");
                    return;
                }

                patientEntity.FullName = ChangedNameBox.Text;
                patientEntity.Birthday = ChangedBirthdayPicker.Value;
                patientEntity.SicknessDesc = ChangedDescBox.Text;


                await _patientService.UpdatePatientAsync(patientEntity);

                var updatedPatients = await _patientService.GetAllPatientsAsync();
                PatientDataTable.DataSource = updatedPatients.ToList();

                ClearAll(ChangePatientPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час оновлення пацієнта: " + ex.Message);
            }
        }

        private void AddPatient_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);
            AddPatientPanel.Visible = true;
            SearchPatientPanel.Visible = false;
            ChangePatientPanel.Visible = false;
        }

        private async void AddPatientSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AddPatientNameBox.Text))
            {
                var patientDto = new PatientDto
                {
                    FullName = AddPatientNameBox.Text,
                    Birthday = AddPatientBirthdayPicker.Value,
                    SicknessDesc = AddPatientDescBox.Text
                };

                try
                {
                    await _patientService.AddPatientAsync(patientDto);

                    var allPatients = await _patientService.GetAllPatientsAsync();
                    PatientDataTable.DataSource = allPatients;

                    ClearAll(AddPatientPanel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при додаванні пацієнта: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть ім'я пацієнта.");
            }
        }

        private async void FindPatient_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);

            SearchPatientPanel.Visible = true;
            AddPatientPanel.Visible = false;
            ChangePatientPanel.Visible = false;

            var patients = await _patientService.GetAllPatientsAsync();

            PatientDataTable.DataSource = patients;

            FindPatientNameBox.Items.Clear();
            patientList.Clear();

            foreach (var patient in patients)
            {
                FindPatientNameBox.Items.Add(patient.FullName);

                patientList.Add(new Patient
                {
                    Id = Convert.ToInt32(patient.Id),
                    FullName = patient.FullName
                });
            }
        }

        private async void FindPatientNameBox_TextChanged(object sender, EventArgs e)
        {
            var selectedName = FindPatientNameBox.Text;
            var patient = patientList.FirstOrDefault(x => x.FullName == selectedName);

            IEnumerable<PatientDto> patients;

            if (patient != null)
            {
                patients = new List<PatientDto>
                {
                    await _patientService.GetPatientByIdAsync(Convert.ToInt32(patient.Id))
                };
            }
            else
            {
                patients = await _patientService.GetAllPatientsAsync();
            }

            PatientDataTable.DataSource = patients.ToList();
        }

        private void DoctorMenu_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);
            ClearAll(RecipePanel);
            ClearAll(AppointmentPanel);
            query = "SELECT * FROM Доктори";
            SqlDataAdapter da = new(query, con);
            DataTable dt = new();
            da.Fill(dt);
            MainDoctorPanel.Visible = true;
            MainPatientPanel.Visible = false;
            MainRecipePanel.Visible = false;
            MainAppointmentPanel.Visible = false;
            DoctorDataTable.DataSource = dt;
        }

        private void DoctorDataTable_MouseClick(object sender, MouseEventArgs e)
        {
            if (DoctorDataTable.SelectedRows.Count > 0 && e.Button is MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DoctorContextStrip.Show(MousePosition);
                }
            }
        }

        private void DeleteDoctorStrip_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(DoctorDataTable.SelectedRows[0].Cells["ID_доктора"].Value);
            query = "DELETE FROM Доктори WHERE ID_доктора='" + id + "'";
            SqlCommand cmd = new(query, con);
            cmd.ExecuteNonQuery();
            query = "SELECT * FROM Доктори";
            SqlDataAdapter da = new(query, con);
            DataTable dt = new();
            da.Fill(dt);
            DoctorDataTable.DataSource = dt;
        }

        private void AddDoctor_Click(object sender, EventArgs e)
        {
            ClearAll(DoctorPanel);
            AddDoctorPanel.Visible = true;
            FindDoctorPanel.Visible = false;
            ChangeDoctorPanel.Visible = false;
        }

        private void AddDoctorSave_Click(object sender, EventArgs e)
        {
            if (AddDoctorNameBox.Text != "")
            {
                query = "INSERT INTO Доктори (ПІБ_доктора,Дата_народження,Повних_років,Спеціалізація) VALUES(N'" + AddDoctorNameBox.Text + "','" + AddDoctorBirthdayPicker.Value + "', dbo.fn_CalculateAge( '" + AddDoctorBirthdayPicker.Value + "', GETDATE()) , N'" + AddDoctorSpecialisationBox.Text + "')";
                SqlCommand cmd = new(query, con);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    query = "SELECT * FROM Доктори";
                    SqlDataAdapter da = new(query, con);
                    DataTable dt = new();
                    da.Fill(dt);
                    DoctorDataTable.DataSource = dt;
                    ClearAll(AddDoctorPanel);
                }
                else
                {
                    MessageBox.Show("SQL QUERY ERROR");
                }
            }
        }

        private void FindDoctor_Click(object sender, EventArgs e)
        {
            ClearAll(DoctorPanel);
            FindDoctorPanel.Visible = true;
            AddDoctorPanel.Visible = false;
            ChangeDoctorPanel.Visible = false;
            SqlDataAdapter da = new("SELECT * FROM Доктори", con);
            DataTable dt = new();
            da.Fill(dt);
            DoctorDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                FindDoctorNameBox.Items.Add(dr["ПІБ_доктора"].ToString());
                Doctor doctor = new()
                {
                    Id = Convert.ToInt32(dr["ID_доктора"]),
                    FullName = dr["ПІБ_доктора"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Specialisation = dr["Спеціалізація"].ToString(),
                };
                doctorList.Add(doctor);
            }
        }

        private void FindDoctorNameBox_TextChanged(object sender, EventArgs e)
        {
            Doctor doctor = doctorList.Find(x => x.FullName == FindDoctorNameBox.Text);
            DataTable dt = new();
            if (doctor != null)
            {
                int id = Convert.ToInt32(doctor.Id);
                SqlDataAdapter da = new("SELECT * FROM Доктори WHERE ID_доктора='" + id + "'", con);
                da.Fill(dt);
            }
            else
            {
                SqlDataAdapter da = new("SELECT * FROM Доктори", con);
                da.Fill(dt);
            }
            DoctorDataTable.DataSource = dt;
        }

        private void ChangeDoctorStrip_Click(object sender, EventArgs e)
        {
            ClearAll(DoctorPanel);
            ChangeDoctorPanel.Visible = true;
            AddDoctorPanel.Visible = false;
            FindDoctorPanel.Visible = false;

            idVar = Convert.ToInt32(DoctorDataTable.SelectedRows[0].Cells["ID_доктора"].Value);
            SqlDataAdapter da = new("SELECT * FROM Доктори WHERE ID_доктора = '" + idVar + "'", con);
            DataTable dt = new();
            da.Fill(dt);
            DoctorDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                Doctor doctor = new()
                {
                    Id = Convert.ToInt32(dr["ID_доктора"]),
                    FullName = dr["ПІБ_доктора"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Specialisation = dr["Спеціалізація"].ToString(),
                };
                ChangedDoctorNameBox.Text = doctor.FullName;
                ChangedDoctorBirthdayPicker.Value = doctor.Birthday;
                ChangedDoctorSpecialisationBox.Text = doctor.Specialisation;

                doctorList.Add(doctor);
            }
        }

        private void ChangeDoctorButton_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Доктори SET ПІБ_доктора=N'" + ChangedDoctorNameBox.Text + "',Дата_народження='" + ChangedBirthdayPicker.Value + "',Повних_років = dbo.fn_CalculateAge( '" + ChangedDoctorBirthdayPicker.Value + "', GETDATE()) ,Спеціалізація=N'" + ChangedDoctorSpecialisationBox.Text + "' WHERE ID_Доктора='" + idVar + "'";
            SqlCommand cmd = new(query, con);
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                query = "SELECT * FROM Доктори";
                SqlDataAdapter da = new(query, con);
                DataTable dt = new();
                da.Fill(dt);
                DoctorDataTable.DataSource = dt;
                ClearAll(ChangeDoctorPanel);
            }
            else
            {
                MessageBox.Show("SQL QUERY ERROR");
            }
        }

        private void RecipeMenu_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);
            ClearAll(DoctorPanel);
            ClearAll(AppointmentPanel);
            query = "SELECT * FROM Рецепт";
            SqlDataAdapter da = new(query, con);
            DataTable dt = new();
            da.Fill(dt);
            MainRecipePanel.Visible = true;
            MainPatientPanel.Visible = false;
            MainDoctorPanel.Visible = false;
            MainAppointmentPanel.Visible = false;
            RecipeDataTable.DataSource = dt;
        }

        private void RecipeDataTable_MouseClick(object sender, MouseEventArgs e)
        {
            if (RecipeDataTable.SelectedRows.Count > 0 && e.Button is MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Right)
                {
                    RecipeContextStrip.Show(MousePosition);
                }
            }
        }

        private void DeleteRecipeStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Чи ви впевнені, що хочете видалити", "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(RecipeDataTable.SelectedRows[0].Cells["ID_рецепту"].Value);
                query = "DELETE FROM Рецепт WHERE ID_рецепту='" + id + "'";
                SqlCommand cmd = new(query, con);
                cmd.ExecuteNonQuery();
                query = "SELECT * FROM Рецепт";
                SqlDataAdapter da = new(query, con);
                DataTable dt = new();
                da.Fill(dt);
                RecipeDataTable.DataSource = dt;
            }
        }

        private void AddRecipe_Click(object sender, EventArgs e)
        {
            ClearAll(DoctorPanel);
            AddRecipePanel.Visible = true;
            FindRecipePanel.Visible = false;
            ChangeRecipePanel.Visible = false;

            SqlDataAdapter da1 = new("SELECT * FROM SickPeopleData", con);
            DataTable dt1 = new();
            da1.Fill(dt1);

            foreach (DataRow dr in dt1.Rows)
            {
                AddPatientRecipeBox.Items.Add(dr["ПІБ_пацієнта"].ToString());
                Patient patient = new()
                {
                    Id = Convert.ToInt32(dr["ID_пацієнта"]),
                    FullName = dr["ПІБ_пацієнта"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Age = Convert.ToInt32(dr["Повних_років"]),
                    SicknessDesc = dr["Опис_хвороби"].ToString(),
                };
                patientList.Add(patient);
            }

            SqlDataAdapter da2 = new("SELECT Назва FROM Ліки", con);
            DataTable dt2 = new();
            da2.Fill(dt2);

            foreach (DataRow dr in dt2.Rows)
            {
                RecipeMedicine1.Items.Add(dr["Назва"].ToString());
                RecipeMedicine2.Items.Add(dr["Назва"].ToString());
                RecipeMedicine3.Items.Add(dr["Назва"].ToString());
                RecipeMedicine4.Items.Add(dr["Назва"].ToString());
                RecipeMedicine5.Items.Add(dr["Назва"].ToString());
            }
        }

        private void AddRecipeSave_Click(object sender, EventArgs e)
        {
            if (AddPatientRecipeBox.Text != "" && RecipeMedicine1.Text != "")
            {
                Recipe recipe = new()
                {
                    FullName = AddPatientRecipeBox.Text,
                    StartDate = RecipeStartDatePicker.Value,
                    EndDate = RecipeEndDatePicker.Value,
                    Medicine1 = RecipeMedicine1.Text,
                    Medicine2 = RecipeMedicine2.Text,
                    Medicine3 = RecipeMedicine3.Text,
                    Medicine4 = RecipeMedicine4.Text,
                    Medicine5 = RecipeMedicine5.Text,
                };

                Patient patient = patientList.Find(x => x.FullName == recipe.FullName);
                if (patient != null)
                {
                    int id = Convert.ToInt32(patient.Id);
                    int age = Convert.ToInt32(patient.Age);

                    string sql = "INSERT INTO Рецепт VALUES('" + id + "', N'" + recipe.FullName + "', '" + patient.Birthday + "', '" + age + "', N'" + patient.SicknessDesc + "', '" + recipe.StartDate + "', '" + recipe.EndDate + "', N'" + recipe.Medicine1 + "', N'" + recipe.Medicine2 + "', N'" + recipe.Medicine3 + "', N'" + recipe.Medicine4 + "', N'" + recipe.Medicine5 + "')";

                    SqlCommand cmd = new(sql, con);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        query = "SELECT * FROM Рецепт";
                        SqlDataAdapter da = new(query, con);
                        DataTable dt = new();
                        da.Fill(dt);
                        RecipeDataTable.DataSource = dt;
                        ClearAll(AddRecipePanel);
                    }
                    else
                    {
                        MessageBox.Show("SQL QUERY ERROR");
                    }
                }
            }
        }

        private void FindRecipe_Click(object sender, EventArgs e)
        {
            FindRecipePanel.Visible = true;
            AddRecipePanel.Visible = false;
            ChangeRecipePanel.Visible = false;

            SqlDataAdapter da = new("SELECT * FROM Рецепт", con);
            DataTable dt = new();
            da.Fill(dt);
            RecipeDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                FindRecipeIdBox.Items.Add(dr["ID_пацієнта"].ToString());
                Recipe recipe = new()
                {
                    RecipeId = Convert.ToInt32(dr["ID_рецепту"]),
                    PatientId = Convert.ToInt32(dr["ID_пацієнта"]),
                    FullName = dr["ПІБ_пацієнта"].ToString(),
                };
                recipeList.Add(recipe);
            }

            SqlDataAdapter da1 = new("SELECT * FROM SickPeopleData", con);
            DataTable dt1 = new();
            da1.Fill(dt1);

            foreach (DataRow dr in dt.Rows)
            {
                FindRecipeNameBox.Items.Add(dr["ПІБ_пацієнта"].ToString());
            }
        }

        private void FindRecipeNameBox_TextChanged(object sender, EventArgs e)
        {
            Recipe recipe = recipeList.Find(x => x.FullName == FindRecipeNameBox.Text);
            DataTable dt = new();
            if (recipe != null)
            {
                int id = Convert.ToInt32(recipe.PatientId);
                SqlDataAdapter da = new("SELECT * FROM Рецепт WHERE ID_пацієнта='" + id + "'", con);
                da.Fill(dt);
            }
            else
            {
                SqlDataAdapter da = new("SELECT * FROM Рецепт", con);
                da.Fill(dt);
            }
            RecipeDataTable.DataSource = dt;
        }

        private void FindRecipeIdBox_TextChanged(object sender, EventArgs e)
        {
            Recipe recipe = recipeList.Find(x => x.PatientId == Convert.ToInt32(FindRecipeIdBox.Text));
            DataTable dt = new();
            if (recipe != null)
            {
                int id = Convert.ToInt32(recipe.PatientId);
                SqlDataAdapter da = new("SELECT * FROM Рецепт WHERE ID_пацієнта='" + id + "'", con);
                da.Fill(dt);
            }
            else
            {
                SqlDataAdapter da = new("SELECT * FROM Рецепт", con);
                da.Fill(dt);
            }
            RecipeDataTable.DataSource = dt;
        }

        private void ChangeRecipeStrip_Click(object sender, EventArgs e)
        {
            ChangeRecipePanel.Visible = true;
            FindRecipePanel.Visible = false;
            AddRecipePanel.Visible = false;

            idVar = Convert.ToInt32(RecipeDataTable.SelectedRows[0].Cells["ID_рецепту"].Value);
            SqlDataAdapter da = new("SELECT * FROM Рецепт WHERE ID_рецепту='" + idVar + "'", con);
            DataTable dt = new();
            da.Fill(dt);
            RecipeDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                Recipe recipe = new()
                {
                    RecipeId = Convert.ToInt32(dr["ID_рецепту"]),
                    PatientId = Convert.ToInt32(dr["ID_пацієнта"]),
                    FullName = dr["ПІБ_пацієнта"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Age = Convert.ToInt32(dr["Повних_років"]),
                    StartDate = Convert.ToDateTime(dr["Дата_початку"]),
                    EndDate = Convert.ToDateTime(dr["Дата_кінця"]),
                    Medicine1 = dr["Назва_ліків_1"].ToString(),
                    Medicine2 = dr["Назва_ліків_2"].ToString(),
                    Medicine3 = dr["Назва_ліків_3"].ToString(),
                    Medicine4 = dr["Назва_ліків_4"].ToString(),
                    Medicine5 = dr["Назва_ліків_5"].ToString(),
                };
                recipeList.Add(recipe);
                ChangedRecipeStartPicker.Value = recipe.StartDate;
                ChangedRecipeEndPicker.Value = recipe.EndDate;
                ChangedMedicineBox1.Text = recipe.Medicine1;
                ChangedMedicineBox2.Text = recipe.Medicine2;
                ChangedMedicineBox3.Text = recipe.Medicine3;
                ChangedMedicineBox4.Text = recipe.Medicine4;
                ChangedMedicineBox5.Text = recipe.Medicine5;
            }

            SqlDataAdapter da2 = new("SELECT Назва FROM Ліки", con);
            DataTable dt2 = new();
            da2.Fill(dt2);

            foreach (DataRow dr in dt2.Rows)
            {
                ChangedMedicineBox1.Items.Add(dr["Назва"].ToString());
                ChangedMedicineBox2.Items.Add(dr["Назва"].ToString());
                ChangedMedicineBox3.Items.Add(dr["Назва"].ToString());
                ChangedMedicineBox4.Items.Add(dr["Назва"].ToString());
                ChangedMedicineBox5.Items.Add(dr["Назва"].ToString());
            }
        }

        private void ChangeRecipeSave_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Рецепт SET Дата_початку='" + ChangedRecipeStartPicker.Value + "',Дата_кінця='" + ChangedRecipeEndPicker.Value + "',Назва_ліків_1=N'" + ChangedMedicineBox1.Text + "',Назва_ліків_2 = N'" + ChangedMedicineBox2.Text + "',Назва_ліків_3=N'" + ChangedMedicineBox3.Text + "',Назва_ліків_4=N'" + ChangedMedicineBox4.Text + "',Назва_ліків_5=N'" + ChangedMedicineBox5.Text + "' WHERE ID_Рецепту='" + idVar + "'";
            SqlCommand cmd = new(query, con);
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                query = "SELECT * FROM Рецепт";
                SqlDataAdapter da = new(query, con);
                DataTable dt = new();
                da.Fill(dt);
                RecipeDataTable.DataSource = dt;
                ClearAll(ChangeRecipePanel);
            }
            else
            {
                MessageBox.Show("SQL QUERY ERROR");
            }
        }

        private void AppointmentMenu_Click(object sender, EventArgs e)
        {
            ClearAll(PatientPanel);
            ClearAll(DoctorPanel);
            ClearAll(RecipePanel);
            query = "SELECT * FROM Візити";
            SqlDataAdapter da = new(query, con);
            DataTable dt = new();
            da.Fill(dt);
            MainAppointmentPanel.Visible = true;
            MainPatientPanel.Visible = false;
            MainDoctorPanel.Visible = false;
            MainRecipePanel.Visible = false;
            AppointmentDataTable.DataSource = dt;
        }

        private void AppointmentDataTable_MouseClick(object sender, MouseEventArgs e)
        {
            if (AppointmentDataTable.SelectedRows.Count > 0 && e.Button is MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Right)
                {
                    AppointmentContextStrip.Show(MousePosition);
                }
            }
        }

        private void DeleteAppointmentStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Чи ви впевнені, що хочете видалити", "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(AppointmentDataTable.SelectedRows[0].Cells["ID_візиту"].Value);
                query = "DELETE FROM Візити WHERE ID_пацієнта='" + id + "'";
                SqlCommand cmd = new(query, con);
                cmd.ExecuteNonQuery();
                query = "SELECT * FROM Візити";
                SqlDataAdapter da = new(query, con);
                DataTable dt = new();
                da.Fill(dt);
                AppointmentDataTable.DataSource = dt;
            }
        }

        private void AddAppointment_Click(object sender, EventArgs e)
        {
            ClearAll(AppointmentPanel);
            AddAppointmentStartPicker.CustomFormat = "HH:mm";
            AddAppointmentEndPicker.CustomFormat = "HH:mm";

            AddAppointmentPanel.Visible = true;
            FindAppointmentPanel.Visible = false;
            ChangeAppointmentPanel.Visible = false;

            SqlDataAdapter da1 = new("SELECT * FROM SickPeopleData", con);
            DataTable dt1 = new();
            da1.Fill(dt1);

            foreach (DataRow dr in dt1.Rows)
            {
                AddAppointmentPatientBox.Items.Add(dr["ПІБ_пацієнта"].ToString());
                Patient patient = new()
                {
                    Id = Convert.ToInt32(dr["ID_пацієнта"]),
                    FullName = dr["ПІБ_пацієнта"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Age = Convert.ToInt32(dr["Повних_років"]),
                };
                patientList.Add(patient);
            }

            SqlDataAdapter da2 = new("SELECT * FROM Доктори", con);
            DataTable dt2 = new();
            da2.Fill(dt2);

            foreach (DataRow dr in dt2.Rows)
            {
                AddAppointmentDoctorBox.Items.Add(dr["ПІБ_доктора"].ToString());
                Doctor doctor = new()
                {
                    Id = Convert.ToInt32(dr["ID_доктора"]),
                    FullName = dr["ПІБ_доктора"].ToString(),
                    Birthday = Convert.ToDateTime(dr["Дата_народження"]),
                    Age = Convert.ToInt32(dr["Повних_років"]),
                };
                doctorList.Add(doctor);
            }
        }

        private void AddAppointmentSave_Click(object sender, EventArgs e)
        {
            if (AddAppointmentDoctorBox.Text != "" && AddAppointmentPatientBox.Text != "")
            {
                Appointment appointment = new()
                {
                    DoctorName = AddAppointmentDoctorBox.Text,
                    PatientName = AddAppointmentPatientBox.Text,
                    AppointmentDate = AddAppointmentDatePicker.Value,
                    AppointmentStart = AddAppointmentStartPicker.Value,
                    AppointmentEnd = AddAppointmentEndPicker.Value,
                };

                Patient patient = patientList.Find(x => x.FullName == appointment.PatientName);
                Doctor doctor = doctorList.Find(x => x.FullName == appointment.DoctorName);
                if (patient != null && doctor != null)
                {
                    int patientId = Convert.ToInt32(patient.Id);
                    int doctorId = Convert.ToInt32(doctor.Id);


                    string sql = "INSERT INTO Візити VALUES ('" + doctorId + "', N'" + appointment.DoctorName + "', '" + patientId + "', N'" + appointment.PatientName + "','" + appointment.AppointmentDate + "', '" + appointment.AppointmentStart + "', '" + appointment.AppointmentEnd + "')";

                    SqlCommand cmd = new(sql, con);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        query = "SELECT * FROM Візити";
                        SqlDataAdapter da = new(query, con);
                        DataTable dt = new();
                        da.Fill(dt);
                        AppointmentDataTable.DataSource = dt;
                        ClearAll(AddAppointmentPanel);
                    }
                    else
                    {
                        MessageBox.Show("SQL QUERY ERROR");
                    }
                }
            }
        }

        private void FindAppointment_Click(object sender, EventArgs e)
        {
            ClearAll(FindAppointmentPanel);
            FindAppointmentPanel.Visible = true;
            AddAppointmentPanel.Visible = false;
            ChangeAppointmentPanel.Visible = false;
            SqlDataAdapter da = new("SELECT * FROM Візити", con);
            DataTable dt = new();
            da.Fill(dt);
            AppointmentDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                Appointment appointment = new()
                {
                    AppointmentId = Convert.ToInt32(dr["ID_візиту"]),
                    PatientName = dr["ПІБ_пацієнта"].ToString(),
                    PatientId = Convert.ToInt32(dr["ID_пацієнта"]),
                    DoctorName = dr["ПІБ_доктора"].ToString(),
                    DoctorId = Convert.ToInt32(dr["ID_доктора"]),
                };
                FindAppointmentIdBox.Items.Add(Convert.ToInt32(dr["ID_пацієнта"]));
                appointmentList.Add(appointment);
            }

            SqlDataAdapter da1 = new("SELECT * FROM SickPeopleData", con);
            DataTable dt1 = new();
            da1.Fill(dt1);

            foreach (DataRow dr in dt1.Rows)
            {
                FindAppointmentPatientBox.Items.Add(dr["ПІБ_пацієнта"].ToString());
            }
        }

        private void FindAppointmentPatientBox_TextChanged(object sender, EventArgs e)
        {
            Appointment appointment = appointmentList.Find(x => x.PatientName == FindAppointmentPatientBox.Text);
            DataTable dt = new();
            if (appointment != null)
            {
                int id = Convert.ToInt32(appointment.PatientId);
                SqlDataAdapter da = new("SELECT * FROM Візити WHERE ID_пацієнта='" + id + "'", con);
                da.Fill(dt);
            }
            else
            {
                SqlDataAdapter da = new("SELECT * FROM Візити", con);
                da.Fill(dt);
            }
            AppointmentDataTable.DataSource = dt;
        }

        private void FindAppointmentIdBox_TextChanged(object sender, EventArgs e)
        {
            Appointment appointment = appointmentList.Find(x => x.PatientId == Convert.ToInt32(FindAppointmentIdBox.Text));
            DataTable dt = new();
            if (appointment != null)
            {
                int id = Convert.ToInt32(appointment.PatientId);
                SqlDataAdapter da = new("SELECT * FROM Візити WHERE ID_пацієнта='" + id + "'", con);
                da.Fill(dt);
            }
            else
            {
                SqlDataAdapter da = new("SELECT * FROM Візити", con);
                da.Fill(dt);
            }
            AppointmentDataTable.DataSource = dt;
        }

        private void ChangeAppointmentStrip_Click(object sender, EventArgs e)
        {

            ChangeAppointmentPanel.Visible = true;
            AddAppointmentPanel.Visible = false;
            FindAppointmentPanel.Visible = false;

            idVar = Convert.ToInt32(AppointmentDataTable.SelectedRows[0].Cells["ID_візиту"].Value);
            SqlDataAdapter da = new("SELECT * FROM Візити WHERE ID_візиту = '" + idVar + "'", con);
            DataTable dt = new();
            da.Fill(dt);
            AppointmentDataTable.DataSource = dt;

            foreach (DataRow dr in dt.Rows)
            {
                string start = dr["Початок_візиту"].ToString();
                string end = dr["Кінець_візиту"].ToString();
                Appointment appointment = new()
                {
                    AppointmentId = Convert.ToInt32(dr["ID_візиту"]),
                    PatientId = Convert.ToInt32(dr["ID_пацієнта"]),
                    PatientName = dr["ПІБ_пацієнта"].ToString(),
                    DoctorId = Convert.ToInt32(dr["ID_доктора"]),
                    DoctorName = dr["ПІБ_доктора"].ToString(),
                    AppointmentDate = Convert.ToDateTime(dr["Дата_візиту"]),
                    AppointmentStart = DateTime.Parse(start),
                    AppointmentEnd = DateTime.Parse(end),
                };
                ChangedDatePicker.Value = appointment.AppointmentDate;
                ChangedStartPicker.Value = appointment.AppointmentStart;
                ChangedEndPicker.Value = appointment.AppointmentEnd;
            }
        }

        private void ChangeAppointmentSave_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Візити SET Дата_візиту='" + ChangedDatePicker.Value + "',Початок_візиту = '" + ChangedStartPicker.Value + "',Кінець_візиту='" + ChangedEndPicker.Value + "' WHERE ID_Візиту='" + idVar + "'";
            SqlCommand cmd = new(query, con);
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                query = "SELECT * FROM Візити";
                SqlDataAdapter da = new(query, con);
                DataTable dt = new();
                da.Fill(dt);
                AppointmentDataTable.DataSource = dt;
                ClearAll(ChangeAppointmentPanel);
            }
            else
            {
                MessageBox.Show("SQL QUERY ERROR");
            }
        }
    }
}