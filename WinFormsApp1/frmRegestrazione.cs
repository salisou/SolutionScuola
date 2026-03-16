using Microsoft.Data.Sqlite;
using WinFormsApp1.Database;

namespace WinFormsApp1
{
    public partial class frmRegestrazione : Form
    {
        //private readonly ScuolaDatabase _database;

        //public frmRegestrazione(ScuolaDatabase database)
        //{
        //    _database = database;
        //}

        public frmRegestrazione()
        {
            InitializeComponent();
            NascondiPassword();
        }

        private void NascondiPassword()
        {
            txtPassword.UseSystemPasswordChar = true;
            txtConfermaPassword.UseSystemPasswordChar = true;
        }

        private void btnSalva_Click(object sender, EventArgs e)
        {
            VerificaCampi();
            VerificaPassword();
            VerificaUtente();
            InserimentoUtente();
        }

        private void InserimentoUtente()
        {
            using (var conn = ScuolaDatabase.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Users
                                 (UserName, Password, Email, Ruolo)
                                 Values
                                 (@UserName, @Password, @Email, @Ruolo)
                                ";

                var cmd = new SqliteCommand(query, conn);

                cmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Ruolo", cmbRuolo.Text);

                cmd.ExecuteNonQuery();
            }

            Messaggio("Registrazione completa!");

            PuliziaCampi();
        }
        private void PuliziaCampi()
        {
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtConfermaPassword.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cmbRuolo.SelectedIndex = -1;
        }
        public string Messaggio(string msg) => MessageBox.Show(msg).ToString();

        private void VerificaUtente()
        {
            if (ScuolaDatabase.UserExists(txtUserName.Text))
                Messaggio("⚠️Utente già esistente");
            return;
        }

        private void VerificaPassword()
        {
            if (txtPassword.Text != txtConfermaPassword.Text)
            {
                Messaggio("⚠️Le password non coincidono");
                return;
            }
        }

        private void VerificaCampi()
        {
            if (txtUserName.Text == "" ||
                txtPassword.Text == "" ||
                txtEmail.Text == "" ||
                cmbRuolo.Text == "")
            {
                Messaggio("⚠️Compila tutti i campi");
                return;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RedirectToLogin();
        }

        private void lkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RedirectToLogin();
        }

        private void RedirectToLogin()
        {
            frmLogin login = new();
            login.Show();
            this.Hide();
        }

        private void frmRegestrazione_Load(object sender, EventArgs e)
        {
            cmbRuolo.Items.Add("Admin");
            cmbRuolo.Items.Add("Docente");
            cmbRuolo.Items.Add("Studente");
        }
    }
}
