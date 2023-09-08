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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessBot.Pieces
{
    public partial class Cell : UserControl
    {
        public BasePiece piece = null;

        public Cell()
        {
            InitializeComponent();
        }

        //Clears the current piece from this cell
        public void clearPiece()
        {
            cellContainer.Children.Remove((UserControl)piece);
            piece = null;
        }

        //Clears the previus piece and adds the new one
        public void addPiece(BasePiece newPiece)
        {
            if (piece != null)
            {
                BotWindow win = (BotWindow)Window.GetWindow(this);
                if (piece.isWhite)
                    win.whitePieces.Remove(piece);
                else
                    win.blackPieces.Remove(piece);
            }

            cellContainer.Children.Remove((UserControl)piece);
            piece = newPiece;
            cellContainer.Children.Add((UserControl)piece);
        }

        //Checks if this cell is ocupied by a piece and returns its encoding or 0 if its empty
        public int IsOcupied()
        {
            if (piece != null)
                return piece.pieceEncode;
            else return 0;
        }

        //Function thats triggered when user clicks on this cell and its empty, used for selecting players landing cell for currently selected piece
        private void cellContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(IsOcupied() == 0)
            {
                BotWindow win = (BotWindow)Window.GetWindow(this);

                if (win.playerSelection != null && moveIndicator.Visibility == Visibility.Visible)
                    win.movePiecePlayer(this);

                if (win.playerSelection != null && moveIndicator.Visibility != Visibility.Visible)
                {
                    win.playerSelection = null;
                    win.removeAvailableMoves();
                }
            }
        }
    }
}
