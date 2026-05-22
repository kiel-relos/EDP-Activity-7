using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace PropLytics
{
    public class DashboardForm : Form
    {
        public DashboardForm()
        {
            this.Text = "PropLytics - Main Dashboard";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            // Navigation Panel
            Panel navPanel = new Panel { Dock = DockStyle.Left, Width = 200, BackColor = Color.White };
            
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

            // Main Content Area
            Label lblWelcome = new Label { Text = "Welcome to the Information System", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(220, 30), AutoSize = true, ForeColor = Color.White, BackColor = Color.Black };
            
            // Initialize database and load graph data
           // Initialize hardcoded data for the UI graph
            List<(string Day, int Value)> performanceData = new List<(string Day, int Value)>
        {
            ("Mon", 85), ("Tue", 78), ("Wed", 92), ("Thu", 88), ("Fri", 95), ("Sat", 80), ("Sun", 75)
        };

            // System Performance Graph
            Label lblChart = new Label { Text = "System Performance (Weekly)", Location = new Point(220, 60), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, ForeColor = Color.White, BackColor = Color.Black };

            int[] performanceValues = performanceData.Select(item => item.Value).ToArray();
            string[] performanceLabels = performanceData.Select(item => item.Day).ToArray();

            Panel graphPanel = new Panel { Location = new Point(220, 90), Size = new Size(550, 250), BackColor = Color.FromArgb(20, 20, 20) };
            graphPanel.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle bounds = graphPanel.ClientRectangle;
                bounds.Inflate(-50, -40);

                using Pen axisPen = new Pen(Color.White, 2);
                using Pen gridPen = new Pen(Color.FromArgb(80, 80, 80), 1);
                using Pen linePen = new Pen(Color.LimeGreen, 3);
                using Brush labelBrush = new SolidBrush(Color.White);
                using Brush markerBrush = new SolidBrush(Color.LimeGreen);

                int count = performanceValues.Length;
                int minValue = performanceValues.Min();
                int maxValue = performanceValues.Max();
                int range = Math.Max(10, maxValue - minValue);

                // Draw gridlines and labels
                for (int row = 0; row <= 4; row++)
                {
                    int y = bounds.Top + row * bounds.Height / 4;
                    g.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);
                    int value = maxValue - row * range / 4;
                    g.DrawString(value.ToString(), new Font("Segoe UI", 8), labelBrush, bounds.Left - 40, y - 8);
                }

                g.DrawLine(axisPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                g.DrawLine(axisPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);

                PointF[] points = new PointF[count];
                for (int i = 0; i < count; i++)
                {
                    float x = bounds.Left + i * (bounds.Width / (float)(count - 1));
                    float y = bounds.Bottom - (performanceValues[i] - minValue) / (float)range * bounds.Height;
                    points[i] = new PointF(x, y);
                }

                if (points.Length > 1)
                {
                    g.DrawLines(linePen, points);
                }

                for (int i = 0; i < count; i++)
                {
                    g.FillEllipse(markerBrush, points[i].X - 5, points[i].Y - 5, 10, 10);
                    g.DrawString(performanceLabels[i], new Font("Segoe UI", 8), labelBrush, points[i].X - 12, bounds.Bottom + 4);
                }
            };

            // Mock Data Grids for UI structure
            DataGridView dgvRecentBookings = new DataGridView
            {
                Location = new Point(220, 360),
                Size = new Size(550, 150),
                BackgroundColor = Color.Black,
                BorderStyle = BorderStyle.None,
                GridColor = Color.Gray,
                ForeColor = Color.White,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    SelectionBackColor = Color.DimGray,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowTemplate = { Height = 30 }
            };
            dgvRecentBookings.DataSource = performanceData.Select(item => new { Day = item.Day, Performance = item.Value }).ToList();
            dgvRecentBookings.BackgroundColor = Color.Black;

            this.Controls.Add(navPanel);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(lblChart);
            this.Controls.Add(graphPanel);
            this.Controls.Add(dgvRecentBookings);
        }
    }
}