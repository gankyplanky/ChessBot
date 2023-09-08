using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessBot.Pieces
{
    public interface BasePiece
    {
        int pieceValue { get; set; }
        bool isWhite { get; set; }
        int pieceEncode { get; set; }
    }
}
