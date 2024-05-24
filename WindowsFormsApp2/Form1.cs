using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dgvEmployees.DataSource = database.employees;
        }

        EmployeeDatabase database = new EmployeeDatabase();

        private void btnRemoveEmployee_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                Employee selectedEmployee = (Employee)dgvEmployees.SelectedRows[0].DataBoundItem;
                database.RemoveEmployee(selectedEmployee.LastName, selectedEmployee.FirstName, selectedEmployee.MiddleName);
                RefreshDataGridView();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            database.LoadData();
            RefreshDataGridView();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            database.SaveData();
        }

        private void RefreshDataGridView()
        {
            dgvEmployees.DataSource = null;
            dgvEmployees.DataSource = database.employees;
        }

        private void ClearInputFields()
        {
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtMiddleName.Text = "";
            txtPosition.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
        }

       
        [Serializable]
        public class Employee
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Position { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }

            public Employee(string lastName, string firstName, string middleName, string position, string phone, string email)
            {
                LastName = lastName;
                FirstName = firstName;
                MiddleName = middleName;
                Position = position;
                Phone = phone;
                Email = email;
            }

            public Employee() { }

            public override string ToString()
            {
                return $"{LastName} {FirstName} {MiddleName}, {Position}, tel.: {Phone}, e-mail: {Email}";
            }
        }

        
        class EmployeeDatabase
        {
            public List<Employee> employees = new List<Employee>();
            private string fileName = "employees.xml";

            public void AddEmployee(Employee employee)
            {
                employees.Add(employee);
                SaveData();
            }

            public void RemoveEmployee(string lastName, string firstName, string middleName)
            {
                employees.RemoveAll(e => e.LastName == lastName && e.FirstName == firstName && e.MiddleName == middleName);
                SaveData();
            }

            public List<Employee> FindEmployee(string query)
            {
                List<Employee> results = new List<Employee>();
                foreach (var employee in employees)
                {
                    if (employee.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        employee.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        employee.MiddleName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        employee.Position.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        employee.Phone.IndexOf(query) >= 0 ||
                        employee.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        results.Add(employee);
                    }
                }
                return results;
            }

            public void LoadData()
            {
                if (File.Exists(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        employees = (List<Employee>)serializer.Deserialize(reader);
                    }
                }
            }

            public void SaveData()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    serializer.Serialize(writer, employees);
                }
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string query = txtSearch.Text;
            List<Employee> searchResults = database.FindEmployee(query);

            dgvEmployees.DataSource = null;
            dgvEmployees.DataSource = searchResults;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string lastName = txtLastName.Text;
            string firstName = txtFirstName.Text;
            string middleName = txtMiddleName.Text;
            string position = txtPosition.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Пожалуйста, заполните поля 'Фамилия' и 'Имя'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Employee employee = new Employee(lastName, firstName, middleName, position, phone, email);
            database.AddEmployee(employee);

            RefreshDataGridView();
            ClearInputFields();
        }
    }
}