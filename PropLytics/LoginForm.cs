using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PropLytics
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkForgotPassword;
        private DatabaseConnection db = new DatabaseConnection();

        public LoginForm()
        {
            this.Text = "Information System - Secure Login";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            Label lblTitle = new Label { Text = "Library Info System", Location = new Point(130, 50), AutoSize = true, Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.LightGray, BackColor = Color.Black };
            
            Label lblUsername = new Label { Text = "Username:", Location = new Point(90, 140), ForeColor = Color.White, BackColor = Color.Black };
            txtUsername = new TextBox { Location = new Point(190, 140), Width = 150, BackColor = Color.Black, ForeColor = Color.White };

            Label lblPassword = new Label { Text = "Password:", Location = new Point(90, 180), ForeColor = Color.White, BackColor = Color.Black };
            txtPassword = new TextBox { Location = new Point(190, 180), Width = 150, PasswordChar = '*', BackColor = Color.Black, ForeColor = Color.White };

            btnLogin = new Button { Text = "Login", Location = new Point(190, 230), Width = 100, Height = 30, BackColor = Color.Black, ForeColor = Color.White };
            btnLogin.Click += BtnLogin_Click;

            lnkForgotPassword = new LinkLabel
            {
                Text = "Forgot Password?",
                Location = new Point(180, 270),
                AutoSize = true,
                ForeColor = Color.White,
                LinkColor = Color.White,
                ActiveLinkColor = Color.White
            };
            lnkForgotPassword.LinkClicked += (s, e) => { new RecoveryForm().ShowDialog(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lnkForgotPassword);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Status FROM users WHERE Username = @user AND Password = @pass";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        if (result.ToString() == "Active")
                        {
                            this.Hide();
                            new DashboardForm().ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Account is Inactive. Please contact the administrator.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Username or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }
    }
}