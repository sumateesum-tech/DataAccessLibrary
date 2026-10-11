using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataAccessLibrary.Examples;

namespace DataAccessLibrary.WinForms
{
    /// <summary>
    /// ตัวอย่าง Form สำหรับใช้งาน DataAccessLibrary
    /// </summary>
    public partial class DataAccessDemoForm : Form
    {
        private IDataAccess _dataAccess;
        private UserRepository _userRepository;
        private DepartmentRepository _departmentRepository;

        public DataAccessDemoForm()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            try
            {
                // สร้าง DatabaseConfig
                var config = new DatabaseConfig
                {
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;",
                    LogPath = Application.StartupPath + "\\logs\\dataaccess.log"
                };

                // สร้าง DataAccess instance
                _dataAccess = DataAccessFactory.Create(config);
                _dataAccess.Open();

                // สร้าง Repository instances
                _userRepository = new UserRepository(_dataAccess);
                _departmentRepository = new DepartmentRepository(_dataAccess);

                MessageBox.Show("Connected to database successfully", "Success");
                LoadDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
                LogHelper.WriteLog("Form_Load error", ex);
            }
        }

        private void LoadDepartments()
        {
            try
            {
                var departments = _departmentRepository.GetActiveDepartments();
                cmbDepartment.DataSource = departments;
                cmbDepartment.DisplayMember = "DepartmentName";
                cmbDepartment.ValueMember = "DepartmentId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading departments: " + ex.Message);
            }
        }

        private void LoadUsers()
        {
            try
            {
                var users = _userRepository.GetUsersWithDepartment();
                dgvUsers.DataSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text))
                    throw new ArgumentException("UserName is required");
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                    throw new ArgumentException("Email is required");

                var newUser = new User
                {
                    UserName = txtUserName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Status = "Active",
                    DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue),
                    CreatedDate = DateTime.Now
                };

                int result = _userRepository.Add(newUser);
                if (result > 0)
                {
                    MessageBox.Show("Insert successful");
                    LoadUsers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                LogHelper.WriteLog("btnInsert_Click error", ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count == 0)
                    throw new ArgumentException("Please select a user to update");

                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells[0].Value);
                var user = _userRepository.GetById(userId);

                if (user != null)
                {
                    user.UserName = txtUserName.Text.Trim();
                    user.Email = txtEmail.Text.Trim();
                    user.PhoneNumber = txtPhoneNumber.Text.Trim();

                    int result = _userRepository.Update(user);
                    if (result > 0)
                    {
                        MessageBox.Show("Update successful");
                        LoadUsers();
                        ClearInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                LogHelper.WriteLog("btnUpdate_Click error", ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count == 0)
                    throw new ArgumentException("Please select a user to delete");

                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells[0].Value);

                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int result = _userRepository.Delete(userId);
                    if (result > 0)
                    {
                        MessageBox.Show("Delete successful");
                        LoadUsers();
                        ClearInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                LogHelper.WriteLog("btnDelete_Click error", ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                    throw new ArgumentException("Please enter search text");

                var results = _userRepository.SearchByName(txtSearch.Text);
                dgvUsers.DataSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells[0].Value);
                    var user = _userRepository.GetById(userId);

                    if (user != null)
                    {
                        txtUserName.Text = user.UserName;
                        txtEmail.Text = user.Email;
                        txtPhoneNumber.Text = user.PhoneNumber;
                        cmbDepartment.SelectedValue = user.DepartmentId;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("dgvUsers_SelectionChanged error", ex);
            }
        }

        private void ClearInputs()
        {
            txtUserName.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtSearch.Clear();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_dataAccess != null)
                {
                    _dataAccess.Close();
                    _dataAccess.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Form_FormClosing error", ex);
            }
        }
    }
}
