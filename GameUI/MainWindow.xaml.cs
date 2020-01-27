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
        
        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            if (game.CurrentBoard != null) return;

            this.SetState(ApplicationState.StartingGame);
        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
