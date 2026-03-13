using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using WinFormsApp1.Database;

namespace WinFormsApp1
{
    public partial class frmRegestrazione : Form
    {
        public frmRegestrazione()
        {
            InitializeComponent();
        }

        private void btnSalva_Click(object sender, EventArgs e)
        {
            if (ScuolaDatabase.UserExists(txtUserName.Text))
            {
                MessageBox.Show("Utente già esistente");
                return;
            }

            using (SqliteConnection conn = ScuolaDatabase.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Users (UserName,Password,Email,Ruolo)
                         VALUES (@user,@pass,@mail,@ruolo)";

                SQLiteCommand cmd = new(query, conn);

                cmd.Parameters.AddWithValue("@user", txtUserName.Text);
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                cmd.Parameters.AddWithValue("@mail", txtEmail.Text);
                cmd.Parameters.AddWithValue("@ruolo", cmbRuolo.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Registrazione completata!");
            }
        }
    }
}
