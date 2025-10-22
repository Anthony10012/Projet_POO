using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

namespace WpftestPendu
{
    /// <summary>
    /// Logique d'interaction pour GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private string wordToGuess;
        private string currentHiddenWord;// le mot affiché avec les tirets

        private const int MAX_ATTEMPTS = 6;
        private int remainingAttempts =MAX_ATTEMPTS;

        private int currentScore = 0;

        public GameWindow(string selectedWord,string userPseudo)
        {
            InitializeComponent();
            wordToGuess = selectedWord.ToUpper();
            if (pseudoTextBlock != null) 
            { 
                pseudoTextBlock.Text = userPseudo;
            }
            InitializeGame();
        }

        private void InitializeGame()
        {
            // 1 Affichage initial du mot masqué
            DisplayHiddenWord();

            // 2 Génération des boutons de l'alphabet
            GenerateLetterButtons();

            // 3. Affichage initial du score
            UpdateScoreDisplay();

            // 4. Afficher l'image de départ (0 erreur)
            UpdateHangmanImage();
           
        }

        private void DisplayHiddenWord()
        {
            // Retour à l'espace standard, mais le XAML le forcera à être visible
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < wordToGuess.Length; i++)
            {
                sb.Append("_ ");

            }
            currentHiddenWord = sb.ToString().TrimEnd(); // Utilisez TrimEnd pour supprimer le dernier espace
            // Assurez-vous que le TextBlock a bien x:Name="hiddenWordTextBlock" dans le XAML
            if (hiddenWordTextBlock != null)
            {
                hiddenWordTextBlock.Text = currentHiddenWord;
                
            }
          
        }
        private void GenerateLetterButtons()
        {
            // Vide la grille au cas où elle contiendrait des éléments
            lettersGrid.Children.Clear();
            // Boucle de 'A' à 'Z'
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Button btn = new Button
                {
                    Content = c.ToString(),
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(4),
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Background = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                    Cursor = Cursors.Hand,
                    Tag = c// propriété pour stocker la lettre
                };
                //Attache la même methode d'événement à chaque bouton
                btn.Click += LetterButton_Click;
                // Ajoute le boutton à la grille
                lettersGrid.Children.Add(btn);
            }

        }

        private void DisableAllLetterButtons()
        {
            // Parcourt tous les enfants dans l'UniformGrid nommée 'lettersGrid'
            foreach (var control in lettersGrid.Children)
            {
                // Vérifie si l'enfant est un bouton
                if (control is Button button)
                {
                    // Désactive le bouton
                    button.IsEnabled = false;
                }
            }
        }
        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            char guessedLetter = (char)clickedButton.Tag;

            // Désactiver le bouton pour qu'il ne puisse pas être réutilisé
            clickedButton.IsEnabled = false;

            // Pour garantir l'espacement, nous réutilisons la configuration XAML (xml:space="preserve")
            const string SEPARATOR = " ";

            // Utilisé pour reconstruire le mot masqué
            StringBuilder newHiddenWord = new StringBuilder();
            
            int lettersRevealedCount = 0;

            // 1. On parcourt le mot secret (wordToGuess)
            for (int i = 0; i < wordToGuess.Length; i++)
            {
                // On vérifie si la lettre cliquée correspond à la lettre actuelle du mot secret
                if (wordToGuess[i] == guessedLetter)
                {
                    // La lettre a été trouvée : on l'ajoute à la nouvelle chaîne
                    newHiddenWord.Append(guessedLetter);
                    lettersRevealedCount++; // Incrementation pour chaque occuren e
                }
                else
                {
                    // Si la lettre n'est pas la bonne, on regarde ce qui était affiché avant (lettre déjà trouvée ou tiret)

                    // On récupère la lettre ou le tiret actuel dans la version affichée
                    // On doit ignorer les séparateurs (espaces) pour lire uniquement les tirets ou lettres
                    char charCurrentlyDisplayed = currentHiddenWord.Replace(SEPARATOR, "")[i];

                    newHiddenWord.Append(charCurrentlyDisplayed);
                }

                // Ajoute l'espace de séparation (sauf après la dernière lettre)
                if (i < wordToGuess.Length - 1)
                {
                    newHiddenWord.Append(SEPARATOR);
                }
            }

            // 2. Mettre à jour l'état du jeu

            // Mettre à jour la variable globale avec la nouvelle version affichée
            currentHiddenWord = newHiddenWord.ToString();
            hiddenWordTextBlock.Text = currentHiddenWord;

            if (lettersRevealedCount > 0)
            {
                // La lettre était bonne !
                clickedButton.Background = Brushes.LightGreen;

                currentScore += (10 * lettersRevealedCount); // 10 points pour chaque bonne réponse trouvé
                UpdateScoreDisplay();
                // **Vérifiez si le joueur a gagné ici !**
                if (!currentHiddenWord.Contains("_"))
                {
                    MessageBox.Show("Félicitations, vous avez trouvé le mot !", "Gagné");

                    // Logique de Score
                    currentScore += 100; //Bonus pour avoir gagné
                    UpdateScoreDisplay();

                    // Desactivation de toutes les lettres pour arreter le jeu
                    DisableAllLetterButtons();

                }
            }
            else
            {
                // La lettre n'était pas dans le mot
                clickedButton.Background = Brushes.Salmon;
                remainingAttempts--; // Décrémente les tentatives

                // **Mettre à jour l'image du pendu ici !**
                UpdateHangmanImage(); 

                // Vérification de la défaite
                if (remainingAttempts <= 0)
                {

                    // Revele le mot directement 
                    hiddenWordTextBlock.Text = wordToGuess;

                    // Logique de Score
                    currentScore = 0; 
                    UpdateScoreDisplay();


                    // Desactivation de toutes les lettres pour arreter le jeu
                    DisableAllLetterButtons();
                   
                }

   

            }
        }
        private void UpdateHangmanImage()
        {
            // Le nombre maximal de fautes est 6 (si vous commencez à 6 tentatives)
            // Le numéro d'étape est 6 (max. fautes) - remainingAttempts
            int mistakesCount = 6 - remainingAttempts;

            // Assurez-vous d'avoir une image correspondante pour chaque étape
            // Exemple : si remainingAttempts est 5 (1 faute), mistakesCount est 1. On charge hangman_1.png

            string imagePath = $"Image/1erreur{mistakesCount}.png"; // Ajustez le chemin de votre dossier Images

            try
            {
                // Crée l'URI pour trouver l'image
                Uri uri = new Uri(imagePath, UriKind.Relative);

                // Met à jour la source de l'Image (assurez-vous que le nom de l'Image est bien 'hangmanImage' dans votre XAML)
                hangmanImage.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
            }
            catch
            {
                // En cas d'erreur (si l'image n'est pas trouvée)
                // Vous pouvez laisser un message d'erreur ou ignorer.
                
            }
        }

        private void UpdateScoreDisplay()
        {
            // Assurez-vous que votre TextBlock a x:Name="scoreTextBlock" dans le XAML
            if (scoreTextBlock != null)
            {
                scoreTextBlock.Text = $"Score : {currentScore}";
            }
        }


       

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Créer une nouvelle instance de la fenêtre de connexion/accueil
            //    (Nous supposons que votre fenêtre de login/accueil s'appelle 'MainWindow')
            LoginWindow loginwindow = new LoginWindow();

            // 2. Afficher la fenêtre d'accueil
            loginwindow.Show();

            // 3. Fermer la fenêtre de jeu actuelle
            this.Close();
        }

    }
}
