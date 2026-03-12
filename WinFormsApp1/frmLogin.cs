namespace WinFormsApp1
{
    public partial class frmLogin : Form
    {
        // Heys, Values
        Dictionary<string, string> users = new()
            {
                { "Mario", "Password1" },
                { "Nori", "Abc123@"},
                { "Luca", "Passw0rd!"},
                { "Giulia", "Qwerty123$" }
            };
        frmHome home = new();

        public frmLogin()
        {
            InitializeComponent();

            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lkRegistrazione_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmRegestrazione reg = new();
            reg.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (users.ContainsKey(txtUserName.Text) && users.ContainsValue(txtPassword.Text))
            {
                home.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Credenziali errate. Riprova");
                CleanAllText();
            }
        }

        private void CleanAllText()
        {
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            //txtPassword.Visible = users.ContainsKey(txtUserName.Text) ? true : false;

            //txtPassword.Visible = users.ContainsKey(txtUserName.Text);

            if (users.ContainsKey(txtUserName.Text))
                txtPassword.Visible = true;
            else
                txtPassword.Visible = false;
        }
    }
}
