namespace MapsWinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panel1 = new Panel();
            Update_IP = new Button();
            comboBox1 = new ComboBox();
            txtUsrname = new TextBox();
            txtPassword = new TextBox();
            btnDisconnect = new Button();
            btnConnect = new Button();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new Size(800, 450);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // panel1
            // 
            panel1.BackColor = Color.MediumSeaGreen;
            panel1.Controls.Add(Update_IP);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(txtUsrname);
            panel1.Controls.Add(txtPassword);
            panel1.Controls.Add(btnDisconnect);
            panel1.Controls.Add(btnConnect);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 125);
            panel1.TabIndex = 1;
            // 
            // Update_IP
            // 
            Update_IP.Location = new Point(153, 20);
            Update_IP.Name = "Update_IP";
            Update_IP.Size = new Size(94, 29);
            Update_IP.TabIndex = 4;
            Update_IP.Text = "Update";
            Update_IP.UseVisualStyleBackColor = true;
            Update_IP.Click += Update_IP_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(153, 74);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(151, 28);
            comboBox1.TabIndex = 3;
            // 
            // txtUsrname
            // 
            txtUsrname.Location = new Point(334, 74);
            txtUsrname.Name = "txtUsrname";
            txtUsrname.Size = new Size(125, 27);
            txtUsrname.TabIndex = 2;
            txtUsrname.Text = "vpnbook";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(482, 74);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(125, 27);
            txtPassword.TabIndex = 2;
            txtPassword.Text = "b49dzh6";
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(23, 74);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(94, 29);
            btnDisconnect.TabIndex = 1;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click_1;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(23, 20);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(94, 29);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(webView21);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Panel panel1;
        private Button btnDisconnect;
        private Button btnConnect;
        private ComboBox comboBox1;
        private TextBox txtUsrname;
        private TextBox txtPassword;
        private Button Update_IP;
    }
}
