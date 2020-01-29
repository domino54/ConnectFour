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
            Unknown,
            InMainMenu,
            InHelpMenu,
            StartingGame,
            Playing
        }

        private ApplicationState State = ApplicationState.Unknown;
        private GameLogic Game = new GameLogic();
        private ImageSource GridEmptyTexture, GridBlueTexture, GridRedTexture;
        private Image[,] GridImages;
        private List<Button> ColumnButtons = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();

            this.GridEmptyTexture   = (ImageSource)this.FindResource("GridTextureEmpty");
            this.GridBlueTexture    = (ImageSource)this.FindResource("GridTextureBlue");
            this.GridRedTexture     = (ImageSource)this.FindResource("GridTextureRed");
            
            this.SetState(ApplicationState.InMainMenu);
        }

        private Visibility GetVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void SetState(ApplicationState newState)
        {
            if (this.State == newState) return;

            this.State = newState;
            
            MainMenu.Visibility         = GetVisibility(this.State == ApplicationState.InMainMenu);
            HowToPlayMenu.Visibility    = GetVisibility(this.State == ApplicationState.InHelpMenu);
            StartingGameMenu.Visibility = GetVisibility(this.State == ApplicationState.StartingGame);
            GameBoardMenu.Visibility    = GetVisibility(this.State == ApplicationState.Playing);
            ButtonResume.IsEnabled      = this.Game.CurrentBoard != null;
        }

        private void ShowException(Exception exception)
        {
            if (exception == null) return;

            ExceptionViewer.Visibility = Visibility.Visible;
            ExceptionBody.Text = $"An exception has been caught:\n{exception.Message}\nPlease contact the authors if this problem persists.";
        }

        private void StartNewGame(GameLogic.AllowedBoardSizes size)
        {
            try
            {
                if (this.Game.CurrentBoard != null)
                {
                    this.Game.StopGame();
                }

                this.Game.StartNewGame(size);
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
            int[,] layout = this.Game.CurrentBoard.Layout;
            int nbColumns = layout.GetLength(0);
            int nbRows = layout.GetLength(1);

            this.GridImages = new Image[nbColumns, nbRows];
            this.ColumnButtons.Clear();

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
                ColumnButtons.Add(button);

                button.Name = "Button_AddCoin_" + col;
                button.Click += Button_AddCoin;
                button.Style = this.FindResource("GameBoardColumn") as Style;

                Grid buttonGrid = new Grid();
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
                button.Content = buttonGrid;
                
                for (int row = 0; row < nbRows; row++)
                {
                    RowDefinition buttonRow = new RowDefinition();
                    buttonGrid.RowDefinitions.Add(buttonRow);
                    
                    Image rowImage = new Image();
                    Grid.SetRow(rowImage, row);
                    Grid.SetColumn(rowImage, 0);
                    buttonGrid.Children.Add(rowImage);
                        
                    int orderInLayout = nbRows - row - 1;
                    this.GridImages[col, orderInLayout] = rowImage;
                }
            }

            this.DrawBoard();
        }

        private void DrawBoard()
        {
            int nbColumns = this.GridImages.GetLength(0);
            int nbRows = this.GridImages.GetLength(1);

            int[,] layout = this.Game.CurrentBoard.Layout;
            int[] unfilledColumns = this.Game.CurrentBoard.ColumnsWithUnfilledRows;

            NextMoveContainer.Visibility = this.GetVisibility(!this.Game.CurrentBoard.IsFinished);
            WinnerAnnouncement.Visibility = this.GetVisibility(this.Game.CurrentBoard.IsFinished);

            for (int col = 0; col < ColumnButtons.Count; col++)
            {
                ColumnButtons[col].IsEnabled = !this.Game.CurrentBoard.IsFinished && unfilledColumns.Contains(col);
            }

            if (!this.Game.CurrentBoard.IsFinished)
            {
                switch (this.Game.CurrentBoard.NextPlayerNum)
                {
                    case 1:
                        {
                            NextMoveColor.Source = GridBlueTexture;
                            break;
                        }
                    case 2:
                        {
                            NextMoveColor.Source = GridRedTexture;
                            break;
                        }
                }
            }
            else
            {
                switch (this.Game.CurrentBoard.WinnerNum)
                {
                    case 1:
                        {
                            WinnerAnnouncement.Content = "Player BLUE wins!";
                            break;
                        }
                    case 2:
                        {
                            WinnerAnnouncement.Content = "Player RED wins!";
                            break;
                        }
                    default:
                        {
                            WinnerAnnouncement.Content = "Game resulted in a draw...";
                            break;
                        }
                }
            }

            for (int col = 0; col < nbColumns; col++)
            {
                for (int row = 0; row < nbRows; row++)
                {
                    Image cellImage = this.GridImages[col, row];

                    switch (layout[col, row])
                    {
                        case 1:
                            {
                                cellImage.Source = GridBlueTexture;
                                break;
                            }
                        case 2:
                            {
                                cellImage.Source = GridRedTexture;
                                break;
                            }
                        default:
                            {
                                cellImage.Source = GridEmptyTexture;
                                break;
                            }
                    }
                }
            }
        }
        
        private void Button_ResumeGame(object sender, RoutedEventArgs e)
        {
            if (this.Game.CurrentBoard == null)
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
            StartNewGame(GameLogic.AllowedBoardSizes.Size7x6);
        }

        private void Button_StartGame8x7(object sender, RoutedEventArgs e)
        {
            StartNewGame(GameLogic.AllowedBoardSizes.Size8x7);
        }

        private void Button_StartGame10x8(object sender, RoutedEventArgs e)
        {
            StartNewGame(GameLogic.AllowedBoardSizes.Size10x8);
        }

        private void Button_AddCoin(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (!ColumnButtons.Contains(button)) return;
            
            int col = ColumnButtons.IndexOf(button);

            if (!this.Game.CurrentBoard.ColumnsWithUnfilledRows.Contains(col)) return;

            try
            {
                this.Game.CurrentBoard.AddCoinToColumn(col, this.Game.CurrentBoard.NextPlayerNum);
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
            if (this.Game.CurrentBoard != null && this.Game.CurrentBoard.IsFinished)
            {
                this.Game.StopGame();
            }

            this.SetState(ApplicationState.InMainMenu);
        }
        
        private void Button_CloseException(object sender, RoutedEventArgs e)
        {
            ExceptionViewer.Visibility = Visibility.Visible;
        }
    }
}
