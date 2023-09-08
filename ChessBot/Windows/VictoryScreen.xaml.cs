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

namespace ChessBot.Windows
{
    public partial class VictoryScreen : Window
    {
        public VictoryScreen(bool whiteWon)
        {
            InitializeComponent();
            if (!whiteWon)
                tbWinnerDisplay.Text = "Black has won";
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            TeamChoice window = new TeamChoice();
            window.Show();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            foreach(Window w in Application.Current.Windows)
            {
                if(w != this)
                    w.Close();
            }

            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}
