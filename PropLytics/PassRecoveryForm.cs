using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PropLytics
{
    public class RecoveryForm : Form
    {
        private DatabaseConnection db = new DatabaseConnection();

        public RecoveryForm()
        {
            this.Text = "Account Recovery";
            this.Size = new Size(400, 230);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.LightGray;

            int centeredLeft = (this.ClientSize.Width - 320) / 2;

            Label lblEmail = new Label { Text = "Enter Registered Email:", Location = new Point(centeredLeft, 30), Size = new Size(320, 24), TextAlign = ContentAlignment.MiddleCenter };
            TextBox txtEmail = new TextBox { Location = new Point(centeredLeft, 65), Size = new Size(320, 28) };

            Button btnRecover = new Button { Text = "Recover Password", Location = new Point(125, 105), Size = new Size(150, 34), BackColor = Color.LightGray };
            
            btnRecover.Click += (s, e) => 
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT Password FROM users WHERE Email = @email";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        
                        object result = cmd.ExecuteScalar();
                        if(result != null)
                        {
                            MessageBox.Show($"Your password is: {result.ToString()}", "Recovery Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Email not found in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                }
            };

            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnRecover);
        }
    }
}