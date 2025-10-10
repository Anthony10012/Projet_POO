using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace WpftestPendu
{
    public partial class LoginWindow : Window
    {
        private const string ConnectionString = "server=localhost;user id=root;password=root;database=games_pendu;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string pseudo = pseudoTextBox.Text.Trim();

            if (string.IsNullOrEmpty(pseudo))
            {
                MessageBox.Show("Veuillez entrer un pseudo.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifier si le pseudo existe et le créer si ce n'est pas le cas
            try
            {
                int playerId = GetOrCreatePlayer(pseudo);

                // Stocker l'ID et le pseudo pour le jeu
                Player.Id = playerId;
                Player.Pseudo = pseudo;

                // Ouvrir la fenêtre principale
                MainWindow mainWindow = new MainWindow(pseudo);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de connexion à la base de données : {ex.Message}");
            }
        }

        // Nouvelle méthode pour vérifier ou créer le joueur
        private int GetOrCreatePlayer(string pseudo)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                // 1. Chercher le joueur
                string selectSql = "SELECT id FROM players WHERE pseudo = @pseudo";
                MySqlCommand selectCmd = new MySqlCommand(selectSql, conn);
                selectCmd.Parameters.AddWithValue("@pseudo", pseudo);

                object result = selectCmd.ExecuteScalar();
                if (result != null)
                {
                    // Le joueur existe, retourne son ID
                    return Convert.ToInt32(result);
                }

                // 2. Le joueur n'existe pas, il faut le créer
                string insertSql = "INSERT INTO players (pseudo, totalscore) VALUES (@pseudo, 0)";
                MySqlCommand insertCmd = new MySqlCommand(insertSql, conn);
                insertCmd.Parameters.AddWithValue("@pseudo", pseudo);
                insertCmd.ExecuteNonQuery();

                // 3. Récupérer l'ID du nouveau joueur
                return (int)insertCmd.LastInsertedId;
            }
        }
    }

    // Mettez à jour la classe Player pour inclure l'ID du joueur
    public static class Player
    {
        public static int Id { get; set; }
        public static string Pseudo { get; set; }
    }
}