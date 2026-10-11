using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DataAccessLibrary.Examples;

namespace DataAccessLibrary.WinForms
{
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
                InitializeControls();
                InitializeDatabase();
                LoadDepartments();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialization Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogHelper.WriteLog("Form_Load error", ex);
            }
        }

        private void InitializeControls()
        {
            // Create main container
            var mainPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            this.Controls.Add(mainPanel);

            // Title Label
            var titleLabel = new Label
            {
                Text = "DataAccessLibrary - WinForms Demo",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(400, 30)
            };
            mainPanel.Controls.Add(titleLabel);

            int yPosition = 50;

            // Department ComboBox
            var deptLabel = new Label { Text = "Department:", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(100, 20) };
            cmbDepartment = new ComboBox { Location = new System.Drawing.Point(120, yPosition), Size = new System.Drawing.Size(200, 20) };
            mainPanel.Controls.Add(deptLabel);
            mainPanel.Controls.Add(cmbDepartment);

            yPosition += 30;

            // UserName TextBox
            var userNameLabel = new Label { Text = "User Name:", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(100, 20) };
            txtUserName = new TextBox { Location = new System.Drawing.Point(120, yPosition), Size = new System.Drawing.Size(200, 20) };
            mainPanel.Controls.Add(userNameLabel);
            mainPanel.Controls.Add(txtUserName);

            yPosition += 30;

            // Email TextBox
            var emailLabel = new Label { Text = "Email:", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(100, 20) };
            txtEmail = new TextBox { Location = new System.Drawing.Point(120, yPosition), Size = new System.Drawing.Size(200, 20) };
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(txtEmail);

            yPosition += 30;

            // Phone TextBox
            var phoneLabel = new Label { Text = "Phone:", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(100, 20) };
            txtPhoneNumber = new TextBox { Location = new System.Drawing.Point(120, yPosition), Size = new System.Drawing.Size(200, 20) };
            mainPanel.Controls.Add(phoneLabel);
            mainPanel.Controls.Add(txtPhoneNumber);

            yPosition += 30;

            // Search TextBox
            var searchLabel = new Label { Text = "Search Name:", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(100, 20) };
            txtSearch = new TextBox { Location = new System.Drawing.Point(120, yPosition), Size = new System.Drawing.Size(200, 20) };
            mainPanel.Controls.Add(searchLabel);
            mainPanel.Controls.Add(txtSearch);

            yPosition += 40;

            // Buttons
            btnLoad = new Button { Text = "Load All", Location = new System.Drawing.Point(10, yPosition), Size = new System.Drawing.Size(75, 23) };
            btnLoad.Click += BtnLoad_Click;
            mainPanel.Controls.Add(btnLoad);

            btnInsert = new Button { Text = "Insert", Location = new System.Drawing.Point(90, yPosition), Size = new System.Drawing.Size(75, 23) };
            btnInsert.Click += BtnInsert_Click;
            mainPanel.Controls.Add(btnInsert);

            btnUpdate = new Button { Text = "Update", Location = new System.Drawing.Point(170, yPosition), Size = new System.Drawing.Size(75, 23) };
            btnUpdate.Click += BtnUpdate_Click;
            mainPanel.Controls.Add(btnUpdate);

            btnDelete = new Button { Text = "Delete", Location = new System.Drawing.Point(250, yPosition), Size = new System.Drawing.Size(75, 23) };
            btnDelete.Click += BtnDelete_Click;
            mainPanel.Controls.Add(btnDelete);

            btnSearch = new Button { Text = "Search", Location = new System.Drawing.Point(330, yPosition), Size = new System.Drawing.Size(75, 23) };
            btnSearch.Click += BtnSearch_Click;
            mainPanel.Controls.Add(btnSearch);

            yPosition += 40;

            // DataGridView
            dgvUsers = new DataGridView
            {
                Location = new System.Drawing.Point(10, yPosition),
                Size = new System.Drawing.Size(750, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            mainPanel.Controls.Add(dgvUsers);
        }

        private void InitializeDatabase()
        {
            try
            {
                var config = new DatabaseConfig
                {
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;",
                    LogPath = Application.StartupPath + "\\logs\\dataaccess.log"
                };

                _dataAccess = DataAccessFactory.Create(config);
                _dataAccess.Open();
                _userRepository = new UserRepository(_dataAccess);
                _departmentRepository = new DepartmentRepository(_dataAccess);

                MessageBox.Show("Connected to database successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to connect to database", ex);
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

        private void BtnLoad_Click(object sender, EventArgs e) => LoadUsers();

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("UserName and Email are required");
                    return;
                }

                var newUser = new User
                {
                    UserName = txtUserName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Status = "Active",
                    DepartmentId = (int)cmbDepartment.SelectedValue,
                    CreatedDate = DateTime.Now
                };

                int result = _userRepository.Add(newUser);
                MessageBox.Show(result > 0 ? "Insert successful" : "Insert failed");
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a user to update");
                    return;
                }

                int userId = (int)dgvUsers.SelectedRows[0].Cells[0].Value;
                var user = _userRepository.GetById(userId);

                if (user != null)
                {
                    user.UserName = txtUserName.Text.Trim();
                    user.Email = txtEmail.Text.Trim();
                    user.PhoneNumber = txtPhoneNumber.Text.Trim();

                    int result = _userRepository.Update(user);
                    MessageBox.Show(result > 0 ? "Update successful" : "Update failed");
                    LoadUsers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a user to delete");
                    return;
                }

                if (MessageBox.Show("Are you sure?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int userId = (int)dgvUsers.SelectedRows[0].Cells[0].Value;
                    int result = _userRepository.Delete(userId);
                    MessageBox.Show(result > 0 ? "Delete successful" : "Delete failed");
                    LoadUsers();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    MessageBox.Show("Please enter search text");
                    return;
                }

                var results = _userRepository.SearchByName(txtSearch.Text);
                dgvUsers.DataSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    int userId = (int)dgvUsers.SelectedRows[0].Cells[0].Value;
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
                LogHelper.WriteLog("Selection changed error", ex);
            }
        }

        private void ClearInputs()
        {
            txtUserName.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtSearch.Clear();
        }

        private void DataAccessDemoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _dataAccess?.Close();
                _dataAccess?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Form closing error", ex);
            }
        }

        // Controls
        private ComboBox cmbDepartment;
        private TextBox txtUserName;
        private TextBox txtEmail;
        private TextBox txtPhoneNumber;
        private TextBox txtSearch;
        private Button btnLoad;
        private Button btnInsert;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private DataGridView dgvUsers;
    }

    public partial class DataAccessDemoForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "DataAccessDemoForm";
            this.Text = "DataAccessLibrary Demo - WinForms";
            this.Load += new System.EventHandler(this.Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataAccessDemoForm_FormClosing);
        }
    }
}
