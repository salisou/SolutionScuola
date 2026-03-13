namespace WinFormsApp1
{
    partial class frmRegestrazione
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSalva = new Button();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            pictureBox2 = new PictureBox();
            cmbRuolo = new ComboBox();
            txtEmail = new TextBox();
            txtConfermaPassword = new TextBox();
            lkRegistrazione = new LinkLabel();
            txtPassword = new TextBox();
            txtUserName = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btnSalva
            // 
            btnSalva.BackColor = Color.FromArgb(0, 192, 192);
            btnSalva.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            btnSalva.Location = new Point(90, 301);
            btnSalva.Name = "btnSalva";
            btnSalva.Size = new Size(99, 33);
            btnSalva.TabIndex = 14;
            btnSalva.Text = "Salva";
            btnSalva.UseVisualStyleBackColor = false;
            btnSalva.Click += btnSalva_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            label3.Location = new Point(18, 131);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 11;
            label3.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            label2.Location = new Point(18, 94);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 10;
            label2.Text = "User Name";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(217, -1);
            label1.Name = "label1";
            label1.Size = new Size(151, 30);
            label1.TabIndex = 9;
            label1.Text = "Registrazione ";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.close;
            pictureBox1.Location = new Point(588, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(30, 22);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            label4.Location = new Point(18, 170);
            label4.Name = "label4";
            label4.Size = new Size(117, 15);
            label4.TabIndex = 16;
            label4.Text = "Conferma Password";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            label5.Location = new Point(18, 216);
            label5.Name = "label5";
            label5.Size = new Size(37, 15);
            label5.TabIndex = 18;
            label5.Text = "Email";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            label6.Location = new Point(18, 248);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 20;
            label6.Text = "Ruolo";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.Registrazione;
            pictureBox2.Location = new Point(264, 58);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(341, 293);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 22;
            pictureBox2.TabStop = false;
            // 
            // cmbRuolo
            // 
            cmbRuolo.FormattingEnabled = true;
            cmbRuolo.Items.AddRange(new object[] { "Studente", "Docente" });
            cmbRuolo.Location = new Point(149, 245);
            cmbRuolo.Name = "cmbRuolo";
            cmbRuolo.Size = new Size(156, 23);
            cmbRuolo.TabIndex = 28;
            cmbRuolo.Text = "Seleziona un opzione";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(149, 208);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Inserisci la mail";
            txtEmail.Size = new Size(279, 24);
            txtEmail.TabIndex = 27;
            // 
            // txtConfermaPassword
            // 
            txtConfermaPassword.Location = new Point(149, 167);
            txtConfermaPassword.Name = "txtConfermaPassword";
            txtConfermaPassword.PlaceholderText = "Conferma la password";
            txtConfermaPassword.Size = new Size(279, 24);
            txtConfermaPassword.TabIndex = 26;
            // 
            // lkRegistrazione
            // 
            lkRegistrazione.AutoSize = true;
            lkRegistrazione.Location = new Point(373, 310);
            lkRegistrazione.Name = "lkRegistrazione";
            lkRegistrazione.Size = new Size(114, 15);
            lkRegistrazione.TabIndex = 25;
            lkRegistrazione.TabStop = true;
            lkRegistrazione.Text = "Fai clic per Accedere";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(149, 128);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Inserisci la password";
            txtPassword.Size = new Size(279, 24);
            txtPassword.TabIndex = 24;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(149, 91);
            txtUserName.Name = "txtUserName";
            txtUserName.PlaceholderText = "Inserisci user name";
            txtUserName.Size = new Size(279, 24);
            txtUserName.TabIndex = 23;
            // 
            // frmRegestrazione
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Beige;
            ClientSize = new Size(619, 363);
            Controls.Add(cmbRuolo);
            Controls.Add(txtEmail);
            Controls.Add(txtConfermaPassword);
            Controls.Add(lkRegistrazione);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(pictureBox2);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(btnSalva);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmRegestrazione";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmRegestrazione";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSalva;
        private Label label3;
        private Label label2;
        private Label label1;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private Label label4;
        private Label label5;
        private Label label6;
        private PictureBox pictureBox2;
        private ComboBox cmbRuolo;
        private TextBox txtEmail;
        private TextBox txtConfermaPassword;
        private LinkLabel lkRegistrazione;
        private TextBox txtPassword;
        private TextBox txtUserName;
    }
}