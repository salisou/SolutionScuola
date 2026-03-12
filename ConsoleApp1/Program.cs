namespace ConsoleApp1
{
    public class Studenti
    {
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public int Eta { get; set; } = 0;
    }
    internal class Program
    {
        static void Main(string[] args)
        {

            Scelta();
            List<Studenti> studente = new()
            {
                new Studenti { Nome = "Luca", Cognome = "Rossi", Eta = 20 },
                new Studenti { Nome = "Maria", Cognome = "Bianchi", Eta = 22 },
                new Studenti { Nome = "Giovanni", Cognome = "Verdi", Eta = 19 }
            };

            foreach (var stud in studente)
            {
                Console.WriteLine($"Nome: {stud.Nome}, Cognome: {stud.Cognome}, Età: {stud.Eta}");
            }

            for (int i = 0; i < studente.Count; i++)
            {
                Console.WriteLine($"Nome: {studente[i].Nome}, Cognome: {studente[i].Cognome}, Età: {studente[i].Eta}");
            }
        }

        // Creare un metodo menu di scelta 
        public static void Scelta()
        {
            Console.WriteLine("Benvenuto al ristorante, hai una prenotazione? 1 si, 0 no");
            string lista = Console.ReadLine();
            string nome = "Luca";



            switch (lista)
            {
                case "1":
                    Prenotazione(nome);
                    break;
                case "0":
                    Console.WriteLine("Mi spiace non abbiamo posti");
                    break;
                default:
                    Console.WriteLine("Scelta non valida");
                    break;
            }
        }

        private static void Prenotazione(string nome)
        {
            Console.WriteLine("A che nome è la prenotazione?");
            string np = Console.ReadLine()!;
            if (np == nome)
            {
                Console.WriteLine("Perfetto avete il tavolo 9");
                // Menù
                Console.WriteLine("Posso avere il menù?");
                // Prensentimo il menù
                Menu();
            }
            else
            {
                Console.WriteLine("Mi dispiace non abbiamo una prenotazione a questo nome");
            }
        }

        private static void Menu()
        {
            Dictionary<string, double> listaMenu = new()
            {
                {"Pizza", 15.50 },
                {"Pasta", 8.00 },
                {"Insalata", 13.50 },
                {"Acqua", 5 },
                {"Coca-Cola", 5   }
            };
            foreach (var list in listaMenu)
            {
                Console.WriteLine($"{list.Key} - {list.Value} euro");
            }

            Console.WriteLine("Scegli");
            string cliente = Console.ReadLine();

        }

        //    Dictionary<string, string> users = new()
        //        {
        //            { "user1", "password1" },
        //            { "user2", "password"}
        //        };
        //        // Verifica se le credenziali sono corrette che si trovano nel dizionario users
        //        if (users.ContainsKey(txtUserName.Text) && users[txtUserName.Text] == txtPassword.Text)
        //        {
        //            MessageBox.Show("Login effettuato con successo!");
        //            // Apri il form principale
        //            frmMain mainForm = new frmMain();
        //    mainForm.Show();
        //            this.Hide();
        //}
        //        else
        //        {
        //            MessageBox.Show("Credenziali errate. Riprova.");

        //            {

        //            }
        //        }
    }
}
