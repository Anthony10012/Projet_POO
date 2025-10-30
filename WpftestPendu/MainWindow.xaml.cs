using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;


namespace WpftestPendu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentPseudo;
        public MainWindow(string pseudo)
        {
            InitializeComponent();
            currentPseudo = pseudo;
            this.Loaded += MainWindow_Loaded;
        }
    
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   
            // Recupere la base de données
            string connectionString = "server=localhost;user id=root;password=root;database=games_pendu;";

            List<string> themes = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT name FROM themes";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) 
                        { 
                       
                         themes.Add(reader.GetString("name"));

                        }
                    
                    }



                }
                listBoxThemes.ItemsSource = themes;

            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}");
            }
        }

        private void GetWordsByTheme(string themeName)
        {

            // Recupere la base de données
            string connectionString = "server=localhost;user id=root;password=root;database=games_pendu;";

            List<string> words = new List<string>();

            try
            {

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT value FROM words WHERE Themes_id = (SELECT id FROM themes WHERE name = @themeName)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    // Add the parameter to the command
                    cmd.Parameters.AddWithValue("@themeName", themeName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            words.Add(reader.GetString("value"));
                        }


                    }
                }




                if (words.Count > 0)
                {
                    Random random = new Random();
                    string selectedWord = words[random.Next(words.Count)];
                   
                    GameWindow gameWindow = new GameWindow(selectedWord,WpftestPendu.Player.Pseudo);
                    gameWindow.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Aucun mot trouvé pour ce thème");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}");
            }
        }

        private void listBoxThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           string selectedTheme = listBoxThemes.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedTheme)) 
            { 
            GetWordsByTheme(selectedTheme);
            }
        }
    }
   
}