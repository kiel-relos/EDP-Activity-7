using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PropLytics
{
    public class UserManagementForm : Form
    {
        private DataGridView dgvUsers;
        private TextBox txtUsername, txtPassword, txtEmail, txtSearch;
        private ComboBox cmbStatus;
        private Label lblIDDisplay;
        private DatabaseConnection db = new DatabaseConnection();
        private string selectedUserID = "";

        public UserManagementForm()
        {
            this.Text = "User Management";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;

            // Inputs
            Label lblID = new Label { Text = "User ID: ", Location = new Point(20, 20), AutoSize = true };
            lblIDDisplay = new Label { Text = "N/A", Location = new Point(100, 20), AutoSize = true, ForeColor = Color.Cyan };
            
            Label lblUser = new Label { Text = "Username:", Location = new Point(20, 50), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(100, 47), Width = 150 };

            Label lblPass = new Label { Text = "Password:", Location = new Point(20, 80), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(100, 77), Width = 150 };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 110), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(100, 107), Width = 150 };

            Label lblStatus = new Label { Text = "Status:", Location = new Point(20, 140), AutoSize = true };
            cmbStatus = new ComboBox { Location = new Point(100, 137), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "Active", "Inactive" });
            cmbStatus.SelectedIndex = 0;

            // Buttons
            Button btnAdd = new Button { Text = "Add Account", Location = new Point(20, 180), Width = 130, Height = 35, BackColor = Color.DarkGreen, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            Button btnUpdate = new Button { Text = "Update Account", Location = new Point(160, 180), Width = 130, Height = 35, BackColor = Color.DarkGoldenrod, FlatStyle = FlatStyle.Flat };
            btnUpdate.Click += BtnUpdate_Click;

            Button btnClear = new Button { Text = "Clear Fields", Location = new Point(20, 225), Width = 270, Height = 35, BackColor = Color.DarkSlateGray, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => ClearFields();

            // Search
            Label lblSearch = new Label { Text = "Search:", Location = new Point(300, 20), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(360, 17), Width = 250 };
            Button btnSearch = new Button { Text = "Search", Location = new Point(620, 15), Width = 80, Height = 28, BackColor = Color.Gray, ForeColor = Color.Black };
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text);

            // Data Grid View
            dgvUsers = new DataGridView
            {
                Location = new Point(300, 50),
                Size = new Size(460, 390),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.Black
            };
            dgvUsers.CellClick += DgvUsers_CellClick;

            this.Controls.Add(lblID); this.Controls.Add(lblIDDisplay);
            this.Controls.Add(lblUser); this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass); this.Controls.Add(txtPassword);
            this.Controls.Add(lblEmail); this.Controls.Add(txtEmail);
            this.Controls.Add(lblStatus); this.Controls.Add(cmbStatus);
            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdate); this.Controls.Add(btnClear);
            this.Controls.Add(lblSearch); this.Controls.Add(txtSearch); this.Controls.Add(btnSearch);
            this.Controls.Add(dgvUsers);

            LoadData(); // Load initial data
        }

        private void LoadData(string searchTerm = "")
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserID, Username, Email, Status FROM users WHERE Username LIKE @search OR Email LIKE @search";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvUsers.DataSource = dt;
                }
                catch (Exception ex) { MessageBox.Show("Error loading data: " + ex.Message); }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO users (Username, Password, Email, Status) VALUES (@u, @p, @e, @s)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@s", cmbStatus.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Account Added Successfully!");
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedUserID)) { MessageBox.Show("Select a user to update."); return; }

            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE users SET Username=@u, Password=@p, Email=@e, Status=@s WHERE UserID=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@p", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@s", cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@id", selectedUserID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Account Updated Successfully!");
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                selectedUserID = row.Cells["UserID"].Value.ToString();
                lblIDDisplay.Text = selectedUserID;
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                cmbStatus.Text = row.Cells["Status"].Value.ToString();
                txtPassword.Text = ""; // Keep password hidden on fetch
            }
        }

        private void ClearFields()
        {
            selectedUserID = "";
            lblIDDisplay.Text = "N/A";
            txtUsername.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            cmbStatus.SelectedIndex = 0;
        }
    }
}