using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Xml.Schema;
using ChessBot.Pieces;
using ChessBot.Windows;

namespace ChessBot.windows
{
    public partial class BotWindow : Window
    {
        public Cell[,] Cells = new Cell[8, 8]; //Array of 'Cell' user controls that hold and display pieces on screen, only visual with some logic
        public int[,] bitBoard = new int[8, 8]; //8*8 array of bytes representing the current board as displayed, used for calculations 
        public List<BasePiece> whitePieces = new List<BasePiece>(); //List of white pieces(user control) for visual dislay with some logic 
        public List<BasePiece> blackPieces= new List<BasePiece>(); //List of black pieces(user control) for visual dislay with some logic 
        public bool isPlayerWhite = true; //Determens the players team / true = white | false = black
        public bool isPlayerTurn = true; //Determens if its players turn if its false the player cant select or move pieces 
        public BasePiece playerSelection = null; //Holds the players current selection, used for making moves
        public int searchDepth = 5; // Depth of turns that the bot is gonna search
        public bool gameOver = false;
        public int iterations = 0;
        public int botOpenerCounter = 0;
        public List<int[]> botOpenerMoves = new List<int[]>();

        public BotWindow(bool isPlayerWhite)
        {
            InitializeComponent();
            setUpBoard();

            this.isPlayerWhite = isPlayerWhite;

            if(isPlayerWhite == true)
            {
                botOpenerMoves.Add(new int[] { 3, 1, 3, 3 });
                botOpenerMoves.Add(new int[] { 2, 1, 2, 2 });
                botOpenerMoves.Add(new int[] { 6, 0, 5, 2 });
                botOpenerMoves.Add(new int[] { 2, 0, 5, 3 });
            }
            else
            {
                botOpenerMoves.Add(new int[] { 2, 6, 2, 4 });
                botOpenerMoves.Add(new int[] { 3, 6, 3, 4 });
                botOpenerMoves.Add(new int[] { 5, 6, 5, 5 });
                botOpenerMoves.Add(new int[] { 1, 7, 2, 5 });
            }
        }

        //Setup the board for start of new game, also clears existing pieces and resets board if it already exists, DOES NOT reset scores, player team indication and similar
        public void setUpBoard()
        {
            whitePieces.Clear();
            blackPieces.Clear();

            //Setup 8*8 array of cells to hold piecesi in display
           for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    Cells[i, j] = new Cell();

            //paint each cell in Cells with apropriate background and place it in apropriate place in grdBoard 
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Resources/Images/BoardTileWhite.png"));

                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = img.Source;

                            Cells[i,j].Background = imgBrush;
                        }
                        else
                        {
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Resources/Images/BoardTileBrown.png"));

                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = img.Source;

                            Cells[i, j].Background = imgBrush;
                        }
                    }
                    else 
                    {
                        if (j % 2 != 0)
                        {
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Resources/Images/BoardTileWhite.png"));

                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = img.Source;

                            Cells[i, j].Background = imgBrush;
                        }
                        else
                        {
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../../Resources/Images/BoardTileBrown.png"));

                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = img.Source;

                            Cells[i, j].Background = imgBrush;
                        }
                    }

                    Grid.SetColumn(Cells[i, j], i);
                    Grid.SetRow(Cells[i, j], j);
                    grdBoard.Children.Add(Cells[i, j]);                   
                }

            //add and setup white pieces
            ////////////////////////////

            //Pawns
            for(int i = 0; i < 8; i++)
            {
                whitePieces.Add(new Pawn(true));
                Cells[i, 6].addPiece(whitePieces[i]);
            }

            //Knights
            whitePieces.Add(new Knight(true));
            Cells[1, 7].addPiece(whitePieces[8]);

            whitePieces.Add(new Knight(true));
            Cells[6, 7].addPiece(whitePieces[9]);

            //Bishops
            whitePieces.Add(new Bishop(true));
            Cells[2, 7].addPiece(whitePieces[10]);

            whitePieces.Add(new Bishop(true));
            Cells[5, 7].addPiece(whitePieces[11]);

            //Rooks
            whitePieces.Add(new Rook(true));
            Cells[0, 7].addPiece(whitePieces[12]);

            whitePieces.Add(new Rook(true));
            Cells[7, 7].addPiece(whitePieces[13]);

            //Queen
            whitePieces.Add(new Queen(true));
            Cells[3, 7].addPiece(whitePieces[14]);

            //King
            whitePieces.Add(new King(true));
            Cells[4, 7].addPiece(whitePieces[15]);

            ////////////////////////////
            
            //add and setup black pieces
            ////////////////////////////

            //Pawns
            for (int i = 0; i < 8; i++)
            {
                blackPieces.Add(new Pawn(false));
                Cells[i, 1].addPiece(blackPieces[i]);  
            }

            //Knights
            blackPieces.Add(new Knight(false));
            Cells[1, 0].addPiece(blackPieces[8]);

            blackPieces.Add(new Knight(false));
            Cells[6, 0].addPiece(blackPieces[9]);

            //Bishops
            blackPieces.Add(new Bishop(false));
            Cells[2, 0].addPiece(blackPieces[10]);

            blackPieces.Add(new Bishop(false));
            Cells[5, 0].addPiece(blackPieces[11]);

            //Rooks
            blackPieces.Add(new Rook(false));
            Cells[0, 0].addPiece(blackPieces[12]);

            blackPieces.Add(new Rook(false));
            Cells[7, 0].addPiece(blackPieces[13]);

            //Queen
            blackPieces.Add(new Queen(false));
            Cells[3, 0].addPiece(blackPieces[14]);

            //King
            blackPieces.Add(new King(false));
            Cells[4, 0].addPiece(blackPieces[15]);

            ////////////////////////////
            ///

            //Setup board representation in 8*8 byte array
            ////////////////////////////

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    bitBoard[i, j] = Cells[i, j].IsOcupied();

            ////////////////////////////
        }

        //Highlits the current selected piece of the player and its possible moves
        public void pieceSelected(BasePiece sender)
        {
            if (isPlayerTurn)
            {
                if (sender.isWhite == isPlayerWhite)
                {
                    int[] pieceCoords = getXYOfPiece(sender);
                    int[] moves;

                    switch (sender.pieceEncode)
                    {
                        case BoardCalcs.pawnEncodeB:
                        case BoardCalcs.pawnEncodeW:

                            Pawn tempPawn = (Pawn)sender;

                            moves = BoardCalcs.getPawnMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard, tempPawn.firstMove);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);

                            playerSelection = sender;
                            break;

                        case BoardCalcs.knightEncodeB:
                        case BoardCalcs.knightEncodeW:

                            moves = BoardCalcs.getKnightMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.bishopEncodeB:
                        case BoardCalcs.bishopEncodeW:

                            moves = BoardCalcs.getBishopMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.rookEncodeB:
                        case BoardCalcs.rookEncodeW:

                            moves = BoardCalcs.getRookMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.queenEncodeB:
                        case BoardCalcs.queenEncodeW:

                            moves = BoardCalcs.getQueenMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.kingEncodeB:
                        case BoardCalcs.kingEncodeW:

                            moves = BoardCalcs.getKingMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        default:
                            Trace.WriteLine("Error in pieceSelected()");
                            playerSelection = null;
                            break;
                    }

                    
                }
            }
            else
            {
                if (sender.isWhite != isPlayerWhite)
                {
                    int[] pieceCoords = getXYOfPiece(sender);
                    int[] moves;

                    switch (sender.pieceEncode)
                    {
                        case BoardCalcs.pawnEncodeB:
                        case BoardCalcs.pawnEncodeW:

                            Pawn tempPawn = (Pawn)sender;

                            moves = BoardCalcs.getPawnMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard, tempPawn.firstMove);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);

                            playerSelection = sender;
                            break;

                        case BoardCalcs.knightEncodeB:
                        case BoardCalcs.knightEncodeW:

                            moves = BoardCalcs.getKnightMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.bishopEncodeB:
                        case BoardCalcs.bishopEncodeW:

                            moves = BoardCalcs.getBishopMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.rookEncodeB:
                        case BoardCalcs.rookEncodeW:

                            moves = BoardCalcs.getRookMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.queenEncodeB:
                        case BoardCalcs.queenEncodeW:

                            moves = BoardCalcs.getQueenMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        case BoardCalcs.kingEncodeB:
                        case BoardCalcs.kingEncodeW:

                            moves = BoardCalcs.getKingMoves(sender.pieceEncode, pieceCoords[0], pieceCoords[1], bitBoard);
                            removeAvailableMoves();
                            highlightAvailableMoves(moves);
                            playerSelection = sender;
                            break;

                        default:
                            Trace.WriteLine("Error in pieceSelected()");
                            playerSelection = null;
                            break;
                    }



                }
            }
        }

        //After every move checks for available moves of oposite team, if there are none displays victory / teamMoved: true = white | false = black 
        public void victoryCheck(bool teamMoved)
        {
            List<int> totalMoves = new List<int>();

            if (teamMoved)
            {
                foreach(BasePiece p in blackPieces)
                {
                    if(p.pieceEncode == BoardCalcs.kingEncodeB)
                    {
                        totalMoves.AddRange(BoardCalcs.getKingMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                        break;
                    }
                }

                for (int i = 0; i < totalMoves.Count; i += 2)
                    if (totalMoves[i] > -1)
                        return;

                totalMoves.Clear();

                foreach(BasePiece p in blackPieces)
                {
                    switch (p.pieceEncode)
                    {
                        case BoardCalcs.pawnEncodeB:
                            Pawn tempPawn = (Pawn)p;
                            totalMoves.AddRange(BoardCalcs.getPawnMoves(tempPawn.pieceEncode, getXYOfPiece(tempPawn)[0], getXYOfPiece(tempPawn)[1], bitBoard, tempPawn.firstMove));
                            break;

                        case BoardCalcs.knightEncodeB:
                            totalMoves.AddRange(BoardCalcs.getKnightMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.bishopEncodeB:
                            totalMoves.AddRange(BoardCalcs.getBishopMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.rookEncodeB:
                            totalMoves.AddRange(BoardCalcs.getRookMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.queenEncodeB:
                            totalMoves.AddRange(BoardCalcs.getQueenMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        default:
                            break;
                    }
                }

                for (int i = 0; i < totalMoves.Count; i += 2)
                    if (totalMoves[i] > -1)
                        return;

                gameOver = true;
                VictoryScreen victoryScreen = new VictoryScreen(true);
                victoryScreen.Show();
            }
            else
            {
                foreach (BasePiece p in whitePieces)
                {
                    if (p.pieceEncode == BoardCalcs.kingEncodeW)
                    {
                        totalMoves.AddRange(BoardCalcs.getKingMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                        break;
                    }
                }

                for (int i = 0; i < totalMoves.Count; i += 2)
                    if (totalMoves[i] > -1)
                        return;

                totalMoves.Clear();

                foreach (BasePiece p in whitePieces)
                {
                    switch (p.pieceEncode)
                    {
                        case BoardCalcs.pawnEncodeW:
                            Pawn tempPawn = (Pawn)p;
                            totalMoves.AddRange(BoardCalcs.getPawnMoves(tempPawn.pieceEncode, getXYOfPiece(tempPawn)[0], getXYOfPiece(tempPawn)[1], bitBoard, tempPawn.firstMove));
                            break;

                        case BoardCalcs.knightEncodeW:
                            totalMoves.AddRange(BoardCalcs.getKnightMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.bishopEncodeW:
                            totalMoves.AddRange(BoardCalcs.getBishopMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.rookEncodeW:
                            totalMoves.AddRange(BoardCalcs.getRookMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        case BoardCalcs.queenEncodeW:
                            totalMoves.AddRange(BoardCalcs.getQueenMoves(p.pieceEncode, getXYOfPiece(p)[0], getXYOfPiece(p)[1], bitBoard));
                            break;

                        default:
                            break;
                    }
                }

                for (int i = 0; i < totalMoves.Count; i += 2)
                    if (totalMoves[i] > -1)
                        return;

                gameOver = true;
                VictoryScreen victoryScreen = new VictoryScreen(false);
                victoryScreen.Show();
            }
        }

        //Moves 'playerSeleciton' to destination, updates bit board, clears obsolite UI and turns pawns into queens when edge of board is reached
        public void movePiecePlayer(Cell destiantion)
        {
            if (destiantion != null && gameOver == false)
            {
                Cells[getXYOfPiece(playerSelection)[0], getXYOfPiece(playerSelection)[1]].clearPiece();
                destiantion.addPiece(playerSelection);

                if (playerSelection.pieceEncode == BoardCalcs.pawnEncodeB || playerSelection.pieceEncode == BoardCalcs.pawnEncodeW)
                {
                    ((Pawn)playerSelection).firstMove = false;

                    int[] pawnPos = getXYOfPiece(playerSelection);

                    if (pawnPos[1] == 0 && playerSelection.pieceEncode == BoardCalcs.pawnEncodeW)
                    {
                        Queen tempQ = new Queen(true);
                        whitePieces.Add(tempQ);
                        Cells[getXYOfPiece(playerSelection)[0], getXYOfPiece(playerSelection)[1]].clearPiece();
                        destiantion.addPiece(tempQ);
                    }
                    else if (pawnPos[1] == 7 && playerSelection.pieceEncode == BoardCalcs.pawnEncodeB)
                    {
                        Queen tempQ = new Queen(false);
                        blackPieces.Add(tempQ);
                        Cells[getXYOfPiece(playerSelection)[0], getXYOfPiece(playerSelection)[1]].clearPiece();
                        destiantion.addPiece(tempQ);
                    }
                }

                playerSelection = null;
                removeAvailableMoves();
                updateBitBoard();

                if (isPlayerTurn && isPlayerWhite)
                    victoryCheck(true);
                else if (isPlayerTurn && !isPlayerWhite)
                    victoryCheck(false);
                else if (!isPlayerTurn && isPlayerWhite)
                    victoryCheck(false);
                else if (!isPlayerTurn && !isPlayerWhite)
                    victoryCheck(true);

                isPlayerTurn = !isPlayerTurn;

                setTurnDisplay();

                if (gameOver == false && isPlayerTurn == false)
                    botMove();
            }
        }

        //Calculation of bots moves
        //////////////////////////////////////////////////////////////////////////////////////////
        public void botMove()
        {
            if(botOpenerCounter >= 0)
            {
                if (Cells[botOpenerMoves[botOpenerCounter][0], botOpenerMoves[botOpenerCounter][1]] != null && Cells[botOpenerMoves[botOpenerCounter][2], botOpenerMoves[botOpenerCounter][3]] != null)
                {
                    if(Cells[botOpenerMoves[botOpenerCounter][2], botOpenerMoves[botOpenerCounter][3]].IsOcupied() != 0)
                    {
                        botOpenerCounter = -1;
                        goto doMove;
                    }

                    if(isPlayerWhite == true)
                    {
                        if(blackPieces.Count < 16)
                        {
                            botOpenerCounter = -1;
                            goto doMove;
                        }
                    }
                    else
                    {
                        if (whitePieces.Count < 16)
                        {
                            botOpenerCounter = -1;
                            goto doMove;
                        }
                    }

                    playerSelection = Cells[botOpenerMoves[botOpenerCounter][0], botOpenerMoves[botOpenerCounter][1]].piece;
                    movePiecePlayer(Cells[botOpenerMoves[botOpenerCounter][2], botOpenerMoves[botOpenerCounter][3]]);
                    botOpenerCounter++;

                    if (botOpenerCounter == botOpenerMoves.Count)
                        botOpenerCounter = -1; 
                }

                return;
            }

            doMove:

            Stopwatch sw = Stopwatch.StartNew();
            List<int> movesAndEncodes = new List<int>();
            int movesAndEncodesCounter = 0;
            int[] tempMoves = new int[] { };
            bool tempPawnFirstMove = false;

            int[,] invertedBoard = new int[8, 8];
            for (int m = 0; m < 8; m++)
            {
                for (int n = 0; n < 8; n++)
                    invertedBoard[m, n] = bitBoard[n, m];
            }

            List<double> scores = new List<double>();
            double scoresCounter = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tempPawnFirstMove = false;
                    int pieceEncode = 0;
                    int[] tempCoords = new int[] { -1, -1 };
                    if (Cells[i, j].IsOcupied() != 0)
                    {
                        pieceEncode = Cells[i, j].piece.pieceEncode;
                        tempCoords = getXYOfPiece(Cells[i, j].piece);
                    }
                    else
                        continue;

                    if (isPlayerWhite == true && (pieceEncode >= BoardCalcs.pawnEncodeB && pieceEncode <= BoardCalcs.kingEncodeB))
                    {

                        if (j == 1)
                            tempPawnFirstMove = true;

                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, bitBoard, tempPawnFirstMove);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() >= BoardCalcs.pawnEncodeW && Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() <= BoardCalcs.queenEncodeW) || Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };
                                int[] tempLegalMoves = BoardCalcs.legalizeMoves(new int[] { tempMoves[n], tempMoves[n + 1] }, tempCoords, bitBoard);

                                scores.Add(alphaBetaMax(-1000, 1000, searchDepth, simulateMove(invertedBoard, tempCoords, tempLegalMoves), false));
                                movesAndEncodes.Add(Convert.ToInt32(scoresCounter));
                                scoresCounter++;
                                movesAndEncodes.Add(pieceEncode);

                                movesAndEncodes.Add(tempCoords[0]);
                                movesAndEncodes.Add(tempCoords[1]);

                                movesAndEncodes.Add(tempLegalMoves[0]);
                                movesAndEncodes.Add(tempLegalMoves[1]);

                                movesAndEncodesCounter += 6;
                            }
                        }

                    }
                    else if (isPlayerWhite == false && (pieceEncode >= BoardCalcs.pawnEncodeW && pieceEncode <= BoardCalcs.kingEncodeW))
                    {

                        if (j == 6)
                            tempPawnFirstMove = true;

                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, bitBoard, tempPawnFirstMove);
                        BoardCalcs.legalizeMoves(tempMoves, tempCoords, bitBoard);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() >= BoardCalcs.pawnEncodeB && Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() <= BoardCalcs.queenEncodeB) || Cells[tempMoves[n], tempMoves[n + 1]].IsOcupied() == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };
                                int[] tempLegalMoves = BoardCalcs.legalizeMoves(new int[] { tempMoves[n], tempMoves[n + 1] }, tempCoords, bitBoard);


                                scores.Add(alphaBetaMax(-1000, 1000, searchDepth, simulateMove(invertedBoard, tempCoords, tempLegalMoves), false));
                                movesAndEncodes.Add(Convert.ToInt32(scoresCounter));
                                scoresCounter++;
                                movesAndEncodes.Add(pieceEncode);

                                movesAndEncodes.Add(tempCoords[0]);
                                movesAndEncodes.Add(tempCoords[1]);

                                movesAndEncodes.Add(tempLegalMoves[0]);
                                movesAndEncodes.Add(tempLegalMoves[1]);

                                movesAndEncodesCounter += 6;

                            }
                        }
                    }
                }
            }

            for (int i = 0; i < movesAndEncodesCounter; i += 6)
            {
                Trace.Write(scores[movesAndEncodes[i]]+"|"+ movesAndEncodes[i+1] + "|"+ movesAndEncodes[i+2] + "|"+ movesAndEncodes[i+3] + "|"+ movesAndEncodes[i+4] + "|"+ movesAndEncodes[i+5] + "|\n");
            }

            List<int> maxMovesAndEncodes = new List<int>();
            maxMovesAndEncodes.Add(movesAndEncodes[0]);
            maxMovesAndEncodes.Add(movesAndEncodes[1]);
            maxMovesAndEncodes.Add(movesAndEncodes[2]);
            maxMovesAndEncodes.Add(movesAndEncodes[3]);
            maxMovesAndEncodes.Add(movesAndEncodes[4]);
            maxMovesAndEncodes.Add(movesAndEncodes[5]);

            for (int i = 6; i < movesAndEncodesCounter; i += 6)
            {
                if (scores[movesAndEncodes[i]] >= scores[maxMovesAndEncodes[0]] && movesAndEncodes[i + 4] > -1)
                {
                    maxMovesAndEncodes[0] = movesAndEncodes[i];
                    maxMovesAndEncodes[1] = movesAndEncodes[i + 1];
                    maxMovesAndEncodes[2] = movesAndEncodes[i + 2];
                    maxMovesAndEncodes[3] = movesAndEncodes[i + 3];
                    maxMovesAndEncodes[4] = movesAndEncodes[i + 4];
                    maxMovesAndEncodes[5] = movesAndEncodes[i + 5];
                }
            }

            sw.Stop();
            Trace.WriteLine(sw.Elapsed);
            Trace.WriteLine(iterations);
            iterations = 0;
            if (Cells[maxMovesAndEncodes[2], maxMovesAndEncodes[3]] != null && Cells[maxMovesAndEncodes[4], maxMovesAndEncodes[5]] != null)
            {             
                playerSelection = Cells[maxMovesAndEncodes[2], maxMovesAndEncodes[3]].piece;
                movePiecePlayer(Cells[maxMovesAndEncodes[4], maxMovesAndEncodes[5]]); 
            }
        }

        public int[,] simulateMove(int[,] board, int[] posXY, int[] destXY)
        {
            if (destXY[0] > 7 || destXY[1] < 0 || posXY[0] > 7 || posXY[1] < 0)
                return board;

            board[destXY[0], destXY[1]] = board[posXY[0], posXY[1]];
            board[posXY[0], posXY[1]] = 0;
            return board;
        }

        public double alphaBetaMax(double alpha, double beta, int depthLeft, int[,] board, bool team)
        {
            iterations++;
            if (depthLeft == 0)
            {
                if(team == false)
                    return -BoardCalcs.evaluate(board, team);
                return BoardCalcs.evaluate(board, team);
            }


            List<int> movesAndEncodes = new List<int>();
            int movesAndEncodesCounter = 0;
            int[] tempMoves = new int[] { };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int pieceEncode = 0;
                    int[] tempCoords = new int[] { -1, -1 };
                    if (board[i, j] != 0)
                    {
                        pieceEncode = board[i, j];
                        tempCoords = new int[] { i, j };
                    }
                    else
                        continue;

                    if (team == false && (pieceEncode >= BoardCalcs.pawnEncodeB && pieceEncode <= BoardCalcs.kingEncodeB))
                    {
                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, board, j == 1 ? true : false);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((board[tempMoves[n], tempMoves[n + 1]] >= BoardCalcs.pawnEncodeW && board[tempMoves[n], tempMoves[n + 1]] <= BoardCalcs.queenEncodeW) || board[tempMoves[n], tempMoves[n + 1]] == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };

                                double score = alphaBetaMin(alpha, beta, depthLeft - 1, simulateMove(board, tempCoords, tempMove), true);
                                //movesAndEncodesCounter++;

                               /* if (movesAndEncodes[movesAndEncodesCounter - 1] >= beta)
                                    return beta;
                                if (movesAndEncodes[movesAndEncodesCounter - 1] > alpha)
                                    alpha = movesAndEncodes[movesAndEncodesCounter - 1];*/

                                if (score >= beta)
                                    return beta;
                                if (score > alpha)
                                    alpha = score;
                            }
                        }

                    }
                    else if (team == true && (pieceEncode >= BoardCalcs.pawnEncodeW && pieceEncode <= BoardCalcs.kingEncodeW))
                    {
                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, bitBoard, j == 6 ? true : false);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((board[tempMoves[n], tempMoves[n + 1]] >= BoardCalcs.pawnEncodeB && board[tempMoves[n], tempMoves[n + 1]] <= BoardCalcs.queenEncodeB) || board[tempMoves[n], tempMoves[n + 1]] == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };

                                double score = alphaBetaMin(alpha, beta, depthLeft - 1, simulateMove(board, tempCoords, tempMove), true);
                                //movesAndEncodesCounter++;

                                /* if (movesAndEncodes[movesAndEncodesCounter - 1] >= beta)
                                     return beta;
                                 if (movesAndEncodes[movesAndEncodesCounter - 1] > alpha)
                                     alpha = movesAndEncodes[movesAndEncodesCounter - 1];*/
                   
                                if (score >= beta)
                                    return beta;
                                if (score > alpha)
                                    alpha = score;
                            }
                        }
                    }
                }
            }

            return alpha;
        }

        public double alphaBetaMin(double alpha, double beta, int depthLeft, int[,] board, bool team)
        {
            iterations++;
            if (depthLeft == 0)
            {
                if (team == false)
                    return -BoardCalcs.evaluate(board, team);
                return BoardCalcs.evaluate(board, team);
            }

            List<int> movesAndEncodes = new List<int>();
            int movesAndEncodesCounter = 0;
            int[] tempMoves = new int[] { };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int pieceEncode = 0;
                    int[] tempCoords = new int[] { -1, -1 };
                    if (board[i, j] != 0)
                    {
                        pieceEncode = board[i, j];
                        tempCoords = new int[] { i, j};
                    }
                    else
                        continue;

                    if (team == false && (pieceEncode >= BoardCalcs.pawnEncodeB && pieceEncode <= BoardCalcs.kingEncodeB))
                    {
                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, board, j == 1 ? true : false);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((board[tempMoves[n], tempMoves[n + 1]] >= BoardCalcs.pawnEncodeW && board[tempMoves[n], tempMoves[n + 1]] <= BoardCalcs.queenEncodeW) || board[tempMoves[n], tempMoves[n + 1]] == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };

                                double score = alphaBetaMax(alpha, beta, depthLeft - 1, simulateMove(board, tempCoords, tempMove), true);
                                //movesAndEncodesCounter++;

                                /*if (movesAndEncodes[movesAndEncodesCounter-1] <= alpha)
                                    return alpha;
                                if (movesAndEncodes[movesAndEncodesCounter-1] < beta)
                                    beta = movesAndEncodes[movesAndEncodesCounter - 1];*/

                                if (score <= alpha)
                                    return alpha;
                                if (score < beta)
                                    beta = score;
                            }
                        }

                    }
                    else if (team == true && (pieceEncode >= BoardCalcs.pawnEncodeW && pieceEncode <= BoardCalcs.kingEncodeW))
                    {
                        tempMoves = BoardCalcs.getPieceMoves(pieceEncode, tempCoords, bitBoard, j == 6 ? true : false);

                        for (int n = 0; n < tempMoves.Length; n += 2)
                        {
                            if (tempMoves[n] > 7 || tempMoves[n] < 0 || tempMoves[n + 1] > 7 || tempMoves[n + 1] < 0)
                                continue;

                            if ((board[tempMoves[n], tempMoves[n + 1]] >= BoardCalcs.pawnEncodeB && board[tempMoves[n], tempMoves[n + 1]] <= BoardCalcs.queenEncodeB) || board[tempMoves[n], tempMoves[n + 1]] == 0)
                            {
                                int[] tempMove = new int[] { tempMoves[n], tempMoves[n + 1] };

                                double score = alphaBetaMax(alpha, beta, depthLeft - 1, simulateMove(board, tempCoords, tempMove), true);
                                //movesAndEncodesCounter++;

                                /*if (movesAndEncodes[movesAndEncodesCounter-1] <= alpha)
                                    return alpha;
                                if (movesAndEncodes[movesAndEncodesCounter-1] < beta)
                                    beta = movesAndEncodes[movesAndEncodesCounter - 1];*/

                                if (score <= alpha)
                                    return alpha;
                                if (score < beta)
                                    beta = score;
                            }
                        }
                    }
                }
            }

            return beta;
        }
        //////////////////////////////////////////////////////////////////////////////////////////

        //Controls what is writen in tbTurnDisplay, should be called after every piece movement
        public void setTurnDisplay()
        {
            if(isPlayerTurn && isPlayerWhite)
            {
                tbTurnDisplay.Text = "White"; 
            }
            if (isPlayerTurn && !isPlayerWhite)
            {
                tbTurnDisplay.Text = "Black";
            }
            if (!isPlayerTurn && isPlayerWhite)
            {
                tbTurnDisplay.Text = "Black";
            }
            if (!isPlayerTurn && !isPlayerWhite)
            {
                tbTurnDisplay.Text = "White";
            }

            tbBlackScore.Text = "Score: " + Convert.ToInt32(BoardCalcs.evaluate(bitBoard, false));
            tbWhiteScore.Text = "Score: " + Convert.ToInt32(BoardCalcs.evaluate(bitBoard, true));
        }

        //Return X and Y of a piece in bitBoard in format of int array (first number is X, second is Y), if piece doesn't exist returns negative numbers
        public int[] getXYOfPiece(BasePiece piece)
        {
            int[] coords = new int[2];
            coords[0] = - 1;
            coords[1] = - 1;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Cells[i, j].piece == piece)
                    {
                        coords[0] = i;
                        coords[1] = j;
                        break;
                    }
                }
                if (coords[0] > -1)
                    break;
            }

            //Trace.WriteLine(coords[0] + " " + coords[1]);
            return coords;
        }

        //Highlights the cells indicated by moves array, the array is in format of [x1,y1,x2,y2...] 
        public void highlightAvailableMoves(int[] moves)
        {
            for (int i = 0; i < moves.Length; i += 2)
                if (moves[i] > -1)
                    Cells[moves[i], moves[i + 1]].moveIndicator.Visibility = Visibility.Visible;
        }

        //Removes the highlight from all cells
        public void removeAvailableMoves()
        {
            foreach(Cell c in Cells)
                c.moveIndicator.Visibility = Visibility.Collapsed;
        }

        //Updates the whole bitBoard with the current state of UI board using 'Cell' as reference
        public void updateBitBoard()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    bitBoard[i, j] = Cells[i, j].IsOcupied();
        }
    }
}
