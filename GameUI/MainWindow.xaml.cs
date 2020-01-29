﻿using System;
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
        private Game game = new Game();
        private ImageSource gridEmptyTexture, gridRedTexture, gridBlueTexture;
        private Image[,] gridImages;
        private List<Button> columnButtons = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();

            this.gridEmptyTexture   = (ImageSource)this.FindResource("GridTexture0");
            this.gridRedTexture     = (ImageSource)this.FindResource("GridTexture1");
            this.gridBlueTexture    = (ImageSource)this.FindResource("GridTexture2");
            
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
            this.columnButtons.Clear();

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
                columnButtons.Add(button);

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

            for (int col = 0; col < columnButtons.Count; col++)
            {
                columnButtons[col].IsEnabled = !this.game.CurrentBoard.IsFinished && unfilledColumns.Contains(col);
            }

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
                            WinnerAnnouncement.Content = "Player " + "RED" + " wins!";
                            break;
                        }
                    case 2:
                        {
                            WinnerAnnouncement.Content = "Player " + "BLUE" + " wins!";
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

            if (!columnButtons.Contains(button)) return;
            
            int col = columnButtons.IndexOf(button);

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
