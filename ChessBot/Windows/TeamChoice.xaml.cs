using ChessBot.windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class TeamChoice : Window
    {
        public TeamChoice()
        {
            InitializeComponent();
        }

        private void btnWhite_Click(object sender, RoutedEventArgs e)
        {
            BotWindow window = new BotWindow(true);
            window.Show();
            window.Focus();
            this.Close();
        }

        private void btnBlack_Click(object sender, RoutedEventArgs e)
        {
            BotWindow window = new BotWindow(false);
            window.isPlayerTurn = false;
            window.isPlayerWhite = false;
            window.Show();
            window.Focus();
            window.botMove();
            this.Close();
        }
    }
}
