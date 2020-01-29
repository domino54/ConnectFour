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
            Unknown,
            InMainMenu,
            InHelpMenu,
            StartingGame,
            Playing
        }

        private ApplicationState state = ApplicationState.Unknown;
        private Game game;
        private ImageSource gridEmptyTexture    = new BitmapImage(new Uri(@"/assets/GridTexture0.png", UriKind.Relative));
        private ImageSource gridBlueTexture      = new BitmapImage(new Uri(@"/assets/GridTexture1.png", UriKind.Relative));
        private ImageSource gridRedTexture   = new BitmapImage(new Uri(@"/assets/GridTexture2.png", UriKind.Relative));
        private Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();

            this.game = new Game();
            this.SetState(ApplicationState.InMainMenu);
        }

        private Visibility GetVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void SetState(ApplicationState newState)
        {
            if (this.state == newState) return;

            this.state = newState;
            
            MainMenu.Visibility         = GetVisibility(this.state == ApplicationState.InMainMenu);
            HowToPlayMenu.Visibility    = GetVisibility(this.state == ApplicationState.InHelpMenu);
            StartingGameMenu.Visibility = GetVisibility(this.state == ApplicationState.StartingGame);
            GameBoardMenu.Visibility    = GetVisibility(this.state == ApplicationState.Playing);
            ButtonResume.IsEnabled      = this.game.CurrentBoard != null;
        }

        private void StartNewGame(Game.BoardSizes size)
        {
            try
            {
                if (this.game.CurrentBoard != null)
                {
                    this.game.StopGame();
                }

                this.game.StartNewGame(size);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                this.SetState(ApplicationState.Playing);
                this.InitGameBoard();
            }
        }

        private void InitGameBoard()
        {
            int[,] layout = this.game.CurrentBoard.Layout;
            int nbColumns = layout.GetLength(0);
            int nbRows = layout.GetLength(1);

            this.gridImages = new Image[nbColumns, nbRows];

            GameBoardGrid.Children.Clear();
            GameBoardGrid.ColumnDefinitions.Clear();
            GameBoardGrid.RowDefinitions.Clear();
            GameBoardGrid.RowDefinitions.Add(new RowDefinition());

            for (int col = 0; col < nbColumns; col++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                GameBoardGrid.ColumnDefinitions.Add(gridCol);

                Button button = new Button();
                Grid.SetRow(button, 0);
                Grid.SetColumn(button, col);
                GameBoardGrid.Children.Add(button);

                button.Name = "Button_AddCoin_" + col;
                button.Click += Button_AddCoin;
                button.Style = this.FindResource("GameBoardColumn") as Style;

                Grid buttonGrid = new Grid();
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
                button.Content = buttonGrid;
                
                for (int row = 0; row <= nbRows; row++)
                {
                    RowDefinition buttonRow = new RowDefinition();
                    buttonGrid.RowDefinitions.Add(buttonRow);

                    if (row == 0)
                    {

                    }
                    else
                    {
                        Image rowImage = new Image();
                        Grid.SetRow(rowImage, row);
                        Grid.SetColumn(rowImage, 0);
                        buttonGrid.Children.Add(rowImage);
                        
                        int orderInLayout = nbRows - row;
                        this.gridImages[col, orderInLayout] = rowImage;
                    }
                }
            }

            this.DrawBoard();
        }

        private void DrawBoard()
        {
            int nbColumns = this.gridImages.GetLength(0);
            int nbRows = this.gridImages.GetLength(1);

            int[,] layout = this.game.CurrentBoard.Layout;
            int[] unfilledColumns = this.game.CurrentBoard.ColumnsWithEmptyRows;

            NextMoveContainer.Visibility = this.GetVisibility(!this.game.CurrentBoard.IsFinished);
            WinnerAnnouncement.Visibility = this.GetVisibility(this.game.CurrentBoard.IsFinished);

            if (!this.game.CurrentBoard.IsFinished)
            {
                switch (this.game.CurrentBoard.NextPlayer)
                {
                    case 1:
                        {
                            NextMoveColor.Source = gridRedTexture;
                            break;
                        }
                    case 2:
                        {
                            NextMoveColor.Source = gridBlueTexture;
                            break;
                        }
                }
            }
            else
            {
                switch (this.game.CurrentBoard.Winner)
                {
                    case 1:
                        {
                            WinnerAnnouncement.Content = "Wygrywa gracz " + "CZERWONY" + "!";
                            break;
                        }
                    case 2:
                        {
                            WinnerAnnouncement.Content = "Wygrywa gracz " + "NIEBIESKI" + "!";
                            break;
                        }
                    default:
                        {
                            WinnerAnnouncement.Content = "Gra zakończyła się remisem...";
                            break;
                        }
                }
            }

            for (int col = 0; col < nbColumns; col++)
            {
                for (int row = 0; row < nbRows; row++)
                {
                    Image cellImage = this.gridImages[col, row];

                    switch (layout[col, row])
                    {
                        case 1:
                            {
                                cellImage.Source = gridRedTexture;
                                break;
                            }
                        case 2:
                            {
                                cellImage.Source = gridBlueTexture;
                                break;
                            }
                        default:
                            {
                                cellImage.Source = gridEmptyTexture;
                                break;
                            }
                    }
                }
            }
        }
        
        /*
        private void DrawBoard()
        {
            BoardFrame.Children.Clear();

            int[,] layout = this.game.CurrentBoard.Layout;
            int[] unfilledColumns = this.game.CurrentBoard.ColumnsWithEmptyRows;
            int nbColumns = layout.GetLength(0);
            int nbRows = layout.GetLength(1);
            
            int displayedPlayer = this.game.CurrentBoard.Winner > 0 ? this.game.CurrentBoard.Winner : this.game.CurrentBoard.NextPlayer;
            string displayedPlayerName = displayedPlayer == 1 ? "CZERWONY" : "ŻÓŁTY";
            
            if (!this.game.CurrentBoard.HasEmptyColumns)
            {
                GameStatusText.Content = "Koniec gry - brak wolnych pól!";
            }
            else if (this.game.CurrentBoard.Winner > 0)
            {
                GameStatusText.Content = "Gracz " + displayedPlayerName + " wygrywa grę!";
            }
            else
            {
                GameStatusText.Content = "Następny ruch: " + displayedPlayerName;
            }
            
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
                                rowGridImage.Source = gridRedTexture;
                                break;
                            }
                        case 2:
                            {
                                rowGridImage.Source = gridBlueTexture;
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

                if (!this.game.CurrentBoard.IsFinished && unfilledColumns.Contains(col))
                {
                    Button addCoinButton = new Button();
                    columnGrid.Children.Add(addCoinButton);

                    addCoinButton.Margin = new Thickness(10, 0, 0, nbRows * GRID_SIZE + 10);
                    addCoinButton.Width = addCoinButton.Height = GRID_SIZE - 20;
                    addCoinButton.HorizontalAlignment = HorizontalAlignment.Left;
                    addCoinButton.VerticalAlignment = VerticalAlignment.Bottom;
                    addCoinButton.Name = "AddCoinButton_" + col;
                    addCoinButton.Content = "⯆";
                    addCoinButton.FontSize = 20;

                    addCoinButton.Click += ButtonAddCoin_Click;
                }
            }
        }
        */

        private void Button_ResumeGame(object sender, RoutedEventArgs e)
        {
            if (this.game.CurrentBoard == null)
            {
                return;
            }

            this.SetState(ApplicationState.Playing);
        }

        private void Button_NewGame(object sender, RoutedEventArgs e)
        {
            this.SetState(ApplicationState.StartingGame);
        }
        
        private void Button_HowToPlay(object sender, RoutedEventArgs e)
        {
            this.SetState(ApplicationState.InHelpMenu);
        }

        private void Button_CloseGame(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void Button_StartGame7x6(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size7x6);
        }

        private void Button_StartGame8x7(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size8x7);
        }

        private void Button_StartGame10x8(object sender, RoutedEventArgs e)
        {
            StartNewGame(Game.BoardSizes.Size10x8);
        }

        private void Button_AddCoin(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string[] explode = button.Name.Split('_');
            if (explode.Length != 3) return;
            int col = Int32.Parse(explode[2]);

            if (!this.game.CurrentBoard.ColumnsWithEmptyRows.Contains(col)) return;

            try
            {
                this.game.CurrentBoard.AddCoin(col, this.game.CurrentBoard.NextPlayer);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                this.DrawBoard();
            }
        }

        private void Button_ReturnToMenu(object sender, RoutedEventArgs e)
        {
            if (this.game.CurrentBoard != null && this.game.CurrentBoard.IsFinished)
            {
                this.game.StopGame();
            }

            this.SetState(ApplicationState.InMainMenu);
        }
    }
}
