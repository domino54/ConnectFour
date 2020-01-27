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
        private const int GRID_SIZE = 60;

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
        private ImageSource gridRedTexture = new BitmapImage(new Uri(@"/assets/GridRed.png", UriKind.Relative));
        private ImageSource gridYellowTexture = new BitmapImage(new Uri(@"/assets/GridYellow.png", UriKind.Relative));

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
            int[] unfilledColumns = this.game.CurrentBoard.ColumnsWithEmptyRows;
            int nbColumns = layout.GetLength(0);
            int nbRows = layout.GetLength(1);

            Thickness newMargin = BoardFrame.Margin;
            newMargin.Left = (1280 - nbColumns * GRID_SIZE) / 2;
            BoardFrame.Margin = newMargin;

            for (int col = 0; col < nbColumns; col++)
            {
                Grid columnGrid = new Grid();
                BoardFrame.Children.Add(columnGrid);

                columnGrid.Margin = new Thickness(col * GRID_SIZE, 0, 0, 0);
                
                for (int row = 0; row < nbRows; row++)
                {
                    Image rowGridImage = new Image();
                    columnGrid.Children.Add(rowGridImage);
                    
                    switch (layout[col, row])
                    {
                        case 1:
                            {
                                rowGridImage.Source = gridYellowTexture;
                                break;
                            }
                        case 2:
                            {
                                rowGridImage.Source = gridRedTexture;
                                break;
                            }
                        default:
                            {
                                rowGridImage.Source = gridEmptyTexture;
                                break;
                            }
                    }

                    rowGridImage.Margin = new Thickness(0, 0, 0, row * GRID_SIZE);
                    rowGridImage.Width = rowGridImage.Height = GRID_SIZE;
                    rowGridImage.HorizontalAlignment = HorizontalAlignment.Left;
                    rowGridImage.VerticalAlignment = VerticalAlignment.Bottom;
                }

                if (unfilledColumns.Contains(col))
                {
                    Button addCoinButton = new Button();
                    columnGrid.Children.Add(addCoinButton);

                    addCoinButton.Margin = new Thickness(10, 0, 0, (nbColumns - 1) * GRID_SIZE + 10);
                    addCoinButton.Width = addCoinButton.Height = GRID_SIZE - 20;
                    addCoinButton.HorizontalAlignment = HorizontalAlignment.Left;
                    addCoinButton.VerticalAlignment = VerticalAlignment.Bottom;
                    addCoinButton.Name = "AddCoinButton_" + col;

                    addCoinButton.Click += ButtonAddCoin_Click;
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

        private void ButtonAddCoin_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string[] explode = button.Name.Split('_');

            if (explode.Length != 2) return;

            int col = Int32.Parse(explode[1]);

            this.game.CurrentBoard.AddCoin(col, this.game.CurrentBoard.NextPlayer);
        }
    }
}
