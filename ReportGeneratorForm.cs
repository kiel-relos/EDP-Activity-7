using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Diagnostics;

namespace PropLytics
{
    public partial class ReportGeneratorForm : Form
    {
        // Using Laragon's default blank password
        private string connectionString = "server=localhost;user=root;password=;database=info_sys;";
        private int currentUserId = 1; 
        
        private DataGridView dataGridView = null!;
        private ComboBox cmbBooks = null!;
        private ComboBox cmbMembers = null!;

        public ReportGeneratorForm()
        {
            SetupUI();
            LoadComboBoxes(); // Load the books and members from DB
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Information System Transactions & Reports";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- 1. Transactions Frame ---
            // Increased height to 100 to fit the dropdown menus
            GroupBox transGroup = new GroupBox { Text = "Primary Transactions", Dock = DockStyle.Top, Height = 100, Padding = new Padding(10) };
            
            // Dropdowns for selection
            Label lblBook = new Label { Text = "Select Book:", Location = new Point(20, 25), Width = 80 };
            cmbBooks = new ComboBox { Location = new Point(100, 22), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblMember = new Label { Text = "Member:", Location = new Point(320, 25), Width = 60 };
            cmbMembers = new ComboBox { Location = new Point(380, 22), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            // Buttons
            Button btnBorrow = new Button { Text = "1. Borrow Book", Width = 120, Location = new Point(20, 60), BackColor = Color.LightBlue, Cursor = Cursors.Hand };
            btnBorrow.Click += BtnBorrow_Click;

            Button btnReturn = new Button { Text = "2. Return Book", Width = 120, Location = new Point(150, 60), BackColor = Color.LightGreen, Cursor = Cursors.Hand };
            btnReturn.Click += BtnReturn_Click;

            Button btnAddStock = new Button { Text = "3. Add Inventory", Width = 120, Location = new Point(280, 60), BackColor = Color.LightYellow, Cursor = Cursors.Hand };
            btnAddStock.Click += BtnAddStock_Click;

            Button btnDelete = new Button { Text = "Delete Selected", Width = 120, Location = new Point(460, 60), BackColor = Color.IndianRed, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnDelete.Click += BtnDelete_Click;

            transGroup.Controls.Add(lblBook);
            transGroup.Controls.Add(cmbBooks);
            transGroup.Controls.Add(lblMember);
            transGroup.Controls.Add(cmbMembers);
            transGroup.Controls.Add(btnBorrow);
            transGroup.Controls.Add(btnReturn);
            transGroup.Controls.Add(btnAddStock);
            transGroup.Controls.Add(btnDelete);
            this.Controls.Add(transGroup);

            // --- 2. Data Grid Frame ---
            GroupBox gridGroup = new GroupBox { Text = "Transaction Records", Dock = DockStyle.Fill, Padding = new Padding(10) };
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.WhiteSmoke
            };
            gridGroup.Controls.Add(dataGridView);
            this.Controls.Add(gridGroup);
            gridGroup.BringToFront();

            // --- 3. Export Module Frame ---
            Panel exportPanel = new Panel { Dock = DockStyle.Bottom, Height = 70, Padding = new Padding(10) };
            Button btnExport = new Button { 
                Text = "Generate Excel Report", 
                Width = 220, 
                Dock = DockStyle.Right, 
                BackColor = Color.SeaGreen, 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.Click += BtnExport_Click;
            exportPanel.Controls.Add(btnExport);
            this.Controls.Add(exportPanel);
        }

        // --- FETCH DATA FOR DROPDOWNS ---
        private void LoadComboBoxes()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Load Books
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT BookID, Title FROM Books", conn))
                    {
                        DataTable dtBooks = new DataTable();
                        da.Fill(dtBooks);
                        cmbBooks.DataSource = dtBooks;
                        cmbBooks.DisplayMember = "Title";
                        cmbBooks.ValueMember = "BookID";
                    }
                    // Load Members
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT MemberID, FullName FROM Members", conn))
                    {
                        DataTable dtMembers = new DataTable();
                        da.Fill(dtMembers);
                        cmbMembers.DataSource = dtMembers;
                        cmbMembers.DisplayMember = "FullName";
                        cmbMembers.ValueMember = "MemberID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load dropdowns: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BUTTON CLICK EVENTS ---
        private void BtnBorrow_Click(object? sender, EventArgs e)
        {
            if (cmbBooks.SelectedValue != null && cmbMembers.SelectedValue != null)
            {
                int bookId = Convert.ToInt32(cmbBooks.SelectedValue);
                int memberId = Convert.ToInt32(cmbMembers.SelectedValue);
                ExecuteTransaction(bookId, memberId, "Borrow", "Book borrowed successfully!");
            }
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (cmbBooks.SelectedValue != null && cmbMembers.SelectedValue != null)
            {
                int bookId = Convert.ToInt32(cmbBooks.SelectedValue);
                int memberId = Convert.ToInt32(cmbMembers.SelectedValue);
                ExecuteTransaction(bookId, memberId, "Return", "Book returned successfully!");
            }
        }

        private void BtnAddStock_Click(object? sender, EventArgs e)
        {
            if (cmbBooks.SelectedValue != null)
            {
                int bookId = Convert.ToInt32(cmbBooks.SelectedValue);
                ExecuteTransaction(bookId, null, "Add Stock", "Inventory updated successfully!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                // Grab the Transaction ID from the first column of the selected row
                int transId = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Trans ID"].Value);
                
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this transaction record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM Transactions WHERE TransactionID = @id";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", transId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Transaction deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Refresh grid
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row in the table to delete.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- DATABASE EXECUTION ---
        private void ExecuteTransaction(int bookId, int? memberId, string transType, string successMsg)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Transactions (BookID, MemberID, TransactionType, ProcessedBy) VALUES (@bookId, @memberId, @transType, @userId)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@bookId", bookId);
                        cmd.Parameters.AddWithValue("@memberId", memberId.HasValue ? (object)memberId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@transType", transType);
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show(successMsg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT t.TransactionID as 'Trans ID', 
                               b.Title as 'Book Title', 
                               IFNULL(m.FullName, 'N/A') as 'Member', 
                               t.TransactionType as 'Trans Type', 
                               t.TransactionDate as 'Date'
                        FROM Transactions t
                        LEFT JOIN Books b ON t.BookID = b.BookID
                        LEFT JOIN Members m ON t.MemberID = m.MemberID
                        ORDER BY t.TransactionDate DESC";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- REPORT GENERATION ---
private void BtnExport_Click(object? sender, EventArgs e)
{
    if (dataGridView.Rows.Count == 0)
    {
        MessageBox.Show("No data to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    try
    {
        ExcelPackage.License.SetNonCommercialPersonal("Kiel Hedrix");

        string fileName = $"Activity5_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        FileInfo file = new FileInfo(filePath);

        using (ExcelPackage package = new ExcelPackage(file))
        {
            // --- Sheet 1: Report Data ---
            ExcelWorksheet ws1 = package.Workbook.Worksheets.Add("Transaction Report");

            // 1. Company Name Header
            ws1.Cells["A1:E1"].Merge = true;
            ws1.Cells["A1"].Value = "BOOKSPHERE LIBRARY SYSTEM";
            ws1.Cells["A1"].Style.Font.Size = 18;
            ws1.Cells["A1"].Style.Font.Bold = true;
            ws1.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Make Row 2 tall enough to hold the image (Height of 80)
            ws1.Row(2).Height = 80;

            // 2. Insert the Logo Image
            // IMPORTANT: Change "R:\" to "C:\" if your laragon is on your C drive!
            string logoPath = @"R:\laragon\www\Information System - EDP Activity\PropLytics\library_logo.png"; 

            if (File.Exists(logoPath))
            {
                FileInfo logoFile = new FileInfo(logoPath);
                var picture = ws1.Drawings.AddPicture("CompanyLogo", logoFile);
                
                // Resize the logo to 100x100 pixels
                picture.SetSize(100, 100);
                
                // Position the image: Row 1 (which is Row 2 in Excel), 5 pixels down, Column 2 (Column C), 40 pixels over to center it
                picture.SetPosition(1, 5, 2, 40); 
            }
            else
            {
                ws1.Cells["A2:E2"].Merge = true;
                ws1.Cells["A2"].Value = "(Logo image not found at: " + logoPath + ")";
                ws1.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws1.Cells["A2"].Style.Font.Italic = true;
            }

            // 3. Data Grid Export
            int startRow = 4;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                ws1.Cells[startRow, i + 1].Value = dataGridView.Columns[i].HeaderText;
                ws1.Cells[startRow, i + 1].Style.Font.Bold = true;
            }

            Dictionary<string, int> dataCounts = new Dictionary<string, int>
            {
                { "Borrow", 0 }, { "Return", 0 }, { "Add Stock", 0 }
            };

            int rowIdx = startRow + 1;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                for (int colIdx = 0; colIdx < dataGridView.Columns.Count; colIdx++)
                {
                    ws1.Cells[rowIdx, colIdx + 1].Value = row.Cells[colIdx].Value?.ToString();
                }
                
                string transType = row.Cells[3].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(transType) && dataCounts.ContainsKey(transType))
                {
                    dataCounts[transType]++;
                }
                rowIdx++;
            }
            ws1.Cells[ws1.Dimension.Address].AutoFitColumns();

            // 4. Signature Placeholder
            int sigRow = rowIdx + 3;
            ws1.Cells[sigRow, 1].Value = "Prepared By:";
            ws1.Cells[sigRow + 2, 1].Value = "______________________________";
            ws1.Cells[sigRow + 3, 1].Value = "Kiel Hedrix V. Relos";
            ws1.Cells[sigRow + 4, 1].Value = "System Administrator";

            // --- Sheet 2: Graph ---
            ExcelWorksheet ws2 = package.Workbook.Worksheets.Add("Data Graph");
            ws2.Cells["A1"].Value = "Transaction Type";
            ws2.Cells["B1"].Value = "Count";

            int graphRow = 2;
            foreach (var kvp in dataCounts)
            {
                ws2.Cells[graphRow, 1].Value = kvp.Key;
                ws2.Cells[graphRow, 2].Value = kvp.Value;
                graphRow++;
            }

            var chart = ws2.Drawings.AddChart("TransChart", OfficeOpenXml.Drawing.Chart.eChartType.BarClustered);
            chart.Title.Text = "Transactions Summary";
            chart.SetPosition(1, 0, 15, 0); 
            chart.SetSize(600, 400);

            var yData = ws2.Cells[2, 2, graphRow - 1, 2];
            var xData = ws2.Cells[2, 1, graphRow - 1, 1];
            var series = chart.Series.Add(yData, xData);
            series.Header = "Total Count";

            package.Save();
        }

        MessageBox.Show($"Report generated successfully!\nSaved to Desktop: {fileName}", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
    }
}