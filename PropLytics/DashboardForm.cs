using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PropLytics
{
    public class DashboardForm : Form
    {
        // GLOBAL CONTROLS
        private DataGridView dgvRecentBooks;

        public DashboardForm()
        {
            this.Text = "Library System - Main Dashboard";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            // Navigation Panel
            Panel navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.White
            };

            Button btnHome = new Button { Text = "Dashboard Home", Dock = DockStyle.Top, Height = 50, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnReports = new Button { Text = "Report Generator", Dock = DockStyle.Top, Height = 50, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnUsers = new Button { Text = "User Management", Dock = DockStyle.Top, Height = 50, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnAbout = new Button { Text = "About the Program", Dock = DockStyle.Top, Height = 50, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnLogout = new Button { Text = "Logout", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnReports.Click += (s, e) => new ReportGeneratorForm().ShowDialog();
            btnUsers.Click += (s, e) => new UserManagementForm().ShowDialog();
            btnAbout.Click += (s, e) => new AboutForm().ShowDialog();

            btnLogout.Click += (s, e) =>
            {
                this.Close();
                new LoginForm().Show();
            };

            navPanel.Controls.Add(btnAbout);
            navPanel.Controls.Add(btnUsers);
            navPanel.Controls.Add(btnReports);
            navPanel.Controls.Add(btnHome);
            navPanel.Controls.Add(btnLogout);

            // =========================
            // DATABASE VALUES
            // =========================
            int totalBooks = GetCount("SELECT COUNT(*) FROM Books");
            int borrowedBooks = GetCount("SELECT COUNT(*) FROM Transactions WHERE TransactionType = 'Borrow'");
            int returnedBooks = GetCount("SELECT COUNT(*) FROM Transactions WHERE TransactionType = 'Return'");

            // Labels
            Label lblWelcome = new Label
            {
                Text = "Welcome to the Library System Dashboard",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(220, 30),
                AutoSize = true,
                ForeColor = Color.White
            };

            Label lblTotalBooks = new Label
            {
                Text = $"Total Books: {totalBooks}",
                Location = new Point(220, 80),
                ForeColor = Color.White,
                AutoSize = true
            };

            Label lblBorrowed = new Label
            {
                Text = $"Borrowed Books: {borrowedBooks}",
                Location = new Point(220, 110),
                ForeColor = Color.Orange,
                AutoSize = true
            };

            Label lblReturned = new Label
            {
                Text = $"Returned Books: {returnedBooks}",
                Location = new Point(220, 140),
                ForeColor = Color.LimeGreen,
                AutoSize = true
            };

            // Refresh Button
            Button btnRefresh = new Button
            {
                Text = "Refresh Library Data",
                Location = new Point(220, 170),
                Width = 180,
                Height = 30,
                BackColor = Color.LimeGreen
            };

            btnRefresh.Click += (s, e) =>
            {
                lblBorrowed.Text = "Borrowed Books: " +
                    GetCount("SELECT COUNT(*) FROM Transactions WHERE TransactionType = 'Borrow'");

                lblReturned.Text = "Returned Books: " +
                    GetCount("SELECT COUNT(*) FROM Transactions WHERE TransactionType = 'Return'");
            };

            // RECENT BOOKS GRID
            dgvRecentBooks = new DataGridView
            {
                Location = new Point(220, 220),
                Size = new Size(550, 250),
                BackgroundColor = Color.Black,
                ForeColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.DodgerBlue,
                    SelectionForeColor = Color.White
                }
            };

            LoadRecentBooks();

            // ADD CONTROLS
            this.Controls.Add(navPanel);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(lblTotalBooks);
            this.Controls.Add(lblBorrowed);
            this.Controls.Add(lblReturned);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dgvRecentBooks);
        }

        // ================= DATABASE METHODS =================

        private int GetCount(string query)
        {
            int count = 0;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return count;
        }

        private void LoadRecentBooks()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT b.Title AS BookTitle,
                               t.TransactionType AS Status
                        FROM Transactions t
                        JOIN Books b ON t.BookID = b.BookID
                        ORDER BY t.TransactionDate DESC
                        LIMIT 10";

                    using (var adapter = new MySqlDataAdapter(query, conn))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        adapter.Fill(dt);
                        dgvRecentBooks.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}