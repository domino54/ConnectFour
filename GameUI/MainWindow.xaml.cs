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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameLibrary;

namespace GameUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum ApplicationState
        {
            InMainMenu,
            InHelpMenu,
            StartingGame,
            Playing
        }

        private ApplicationState state;
        private Game game;
        private ImageSource gridEmptyTexture = new BitmapImage(new Uri(@"/assets/GridEmpty.png", UriKind.Relative));

        public MainWindow()
        {
            InitializeComponent();

            this.game = new Game();
            this.SetState(ApplicationState.InMainMenu);
        }

        private void SetState(ApplicationState newState)
        {
            if (this.state == newState) return;

            this.state = newState;

            MainMenu.Visibility         = this.state == ApplicationState.InMainMenu     ? Visibility.Visible : Visibility.Hidden;
            HowToPlayMenu.Visibility    = this.state == ApplicationState.InHelpMenu     ? Visibility.Visible : Visibility.Hidden;
            StartingGameMenu.Visibility = this.state == ApplicationState.StartingGame   ? Visibility.Visible : Visibility.Hidden;
            GameBoardMenu.Visibility    = this.state == ApplicationState.Playing        ? Visibility.Visible : Visibility.Hidden;
        }

        private void StartNewGame(Game.BoardSizes size)
        {
            try
            {
                this.game.StartNewGame(size);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                this.SetState(ApplicationState.Playing);
                this.DrawBoard();
            }
        }

        private void DrawBoard()
        {
            BoardFrame.Children.Clear();

            int[,] layout = this.game.CurrentBoard.Layout;
            int nbColumns = layout.GetLength(0);
            int nbRows = layout.GetLength(1);
            
            for (int col = 0; col < nbColumns; col++)
            {
                Grid columnGrid = new Grid();
                BoardFrame.Children.Add(columnGrid);

                columnGrid.Margin = new Thickness(col * 100, 0, 0, 0);

                for (int row = 0; row < nbRows; row++)
                {
                    Image rowImage = new Image();

                    rowImage.Source = gridEmptyTexture;
                    rowImage.Margin = new Thickness(0, row * 100, 0, 0);
                    rowImage.Width = rowImage.Height = 50;

                    columnGrid.Children.Add(rowImage);
                }
            }
        }

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            if (game.CurrentBoard != null) return;

            this.SetState(ApplicationState.StartingGame);
        }
        
        private void ButtonHowToPlay_Click(object sender, RoutedEventArgs e)
        {
            this.SetState(ApplicationState.InHelpMenu);
        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpMenuReturn_Click(object sender, RoutedEventArgs e)
        {
            this.SetState(ApplicationState.InMainMenu);
        }

        private void StartingGameMenuReturn_Click(object sender, RoutedEventArgs e)
        {
            this.SetState(ApplicationState.InMainMenu);
        }

        private void ButtonSize7x6_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size7x6);
        }

        private void ButtonSize8x7_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size8x7);
        }

        private void ButtonSize10x8_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size10x8);
        }
    }
}
