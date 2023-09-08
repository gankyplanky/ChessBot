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
    public partial class Bishop : UserControl, BasePiece
    {
        public int pieceValue { get; set; }
        public bool isWhite { get; set; } 
        public int pieceEncode { get; set; }

        public Bishop(bool isWhite)
        {
            InitializeComponent();

            //Sets appropriate encoding, value and image of the piece
            pieceEncode = BoardCalcs.bishopEncodeB;
            if (isWhite)
            {
                imgPawnIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Resources/Images/BishopW.png"));
                pieceEncode = BoardCalcs.bishopEncodeW;
            }

            this.isWhite = isWhite;
            pieceValue = BoardCalcs.bishopValue;
        }

        //Triggered when user clicks on this piece and calls 'pieceSelected' from parent window for further logic
        private void imgPawnIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BotWindow win = (BotWindow)Window.GetWindow(this);
            if (win != null)
            {
                if (win.playerSelection == null)
                {
                    win.pieceSelected(this);
                }
                else
                {
                    if (pieceEncode == BoardCalcs.bishopEncodeB && (win.playerSelection.pieceEncode >= BoardCalcs.pawnEncodeW && win.playerSelection.pieceEncode <= BoardCalcs.kingEncodeW))
                    {
                        if (((Cell)((Grid)this.Parent).Parent).moveIndicator.Visibility == Visibility.Visible)
                            win.movePiecePlayer((Cell)((Grid)this.Parent).Parent);
                    }
                    else if (pieceEncode == BoardCalcs.bishopEncodeW && (win.playerSelection.pieceEncode >= BoardCalcs.pawnEncodeB && win.playerSelection.pieceEncode <= BoardCalcs.kingEncodeB))
                    {
                        if (((Cell)((Grid)this.Parent).Parent).moveIndicator.Visibility == Visibility.Visible)
                            win.movePiecePlayer((Cell)((Grid)this.Parent).Parent);
                    }
                    else
                    {
                        win.pieceSelected(this);
                    }
                }
            }
        }
    }
}
