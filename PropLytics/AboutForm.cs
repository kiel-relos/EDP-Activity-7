using System;
using System.Drawing;
using System.Windows.Forms;

namespace PropLytics
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            this.Text = "About";
            this.Size = new Size(420, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblAppName = new Label
            {
                Text = "Information System",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(35, 30),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblVersion = new Label
            {
                Text = "Version: 1.0.0",
                Font = new Font("Arial", 10),
                Location = new Point(35, 70),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblDesc = new Label
            {
                Text = "An integrated platform for efficient\ninformation system management.",
                Font = new Font("Arial", 10),
                Location = new Point(35, 100),
                Size = new Size(350, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnOk = new Button { Text = "OK", Location = new Point(165, 170), Width = 90, Height = 35 };
            btnOk.Click += (s, e) => this.Close();

            this.Controls.Add(lblAppName);
            this.Controls.Add(lblVersion);
            this.Controls.Add(lblDesc);
            this.Controls.Add(btnOk);
        }
    }
}