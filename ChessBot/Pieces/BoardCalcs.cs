using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Documents;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace ChessBot.Pieces
{
    public static class BoardCalcs
    {
        //Table for piece encoding
        ////////////////////////////////////
        public const int emptyEncode = 0;
        public const int pawnEncodeW = 1;
        public const int knightEncodeW = 2;
        public const int bishopEncodeW = 3;
        public const int rookEncodeW = 4;
        public const int queenEncodeW = 5;
        public const int kingEncodeW = 6;
        public const int pawnEncodeB = 7;
        public const int knightEncodeB = 8;
        public const int bishopEncodeB = 9;
        public const int rookEncodeB = 10;
        public const int queenEncodeB = 11;
        public const int kingEncodeB = 12;
        ////////////////////////////////////

        //Table for piece values
        ////////////////////////////////////
        public const int pawnValue = 1;
        public const int knightValue = 3;
        public const int bishopValue = 3;
        public const int rookValue = 5;
        public const int queenValue = 10;
        public const int kingValue = 40;
        public const double availableMoveValueMultiplicator = 0.4;
        public const double killValueMultiplicator = 0.6;
        public const int adjecantToKingValue = 1;
        public const double protectedMultiplicator = 0.2;
        ////////////////////////////////////

        //TODO: Evaluates the given board that is a 8*8 int array using piece encodings from the tables above, and returns double value of the board
        //team boolean handles which team the funcion is evaluating for, team context shoud be handled from caller / true = white | false = black
        public static double evaluate(int[,] board, bool team) 
        {
            double totalValueW = 0;
            double totalValueB = 0;
            int[] tempMovesW = new int[] { };
            int[] tempMovesB = new int[] { };
            int tempPieceValue = 0;
            List<int[]> possibleMovesW = new List<int[]>();
            List<int[]> possibleMovesB = new List<int[]>();


            for (int i = 0, pieceCounter = 0; i < 8 && pieceCounter < 16; i++)
            {
                for (int j = 0; j < 8 && pieceCounter < 16; j++)
                {
                    if (board[i, j] >= BoardCalcs.pawnEncodeW && board[i, j] <= BoardCalcs.kingEncodeW)
                    {
                        pieceCounter++;

                        switch (board[i, j])
                        {
                            case pawnEncodeW:
                                totalValueW += pawnValue;

                                tempMovesW = getPawnMoves(board[i, j], i, j, board, j == 6 ? true : false);

                                for (int n = 0; n < tempMovesW.Length; n += 2)
                                {
                                    if (tempMovesW[n] > -1)
                                    {
                                        totalValueW += pawnValue * availableMoveValueMultiplicator;
                                        possibleMovesW.Add(new int[] { i, j, tempMovesW[n], tempMovesW[n + 1] });
                                        if (board[tempMovesW[n], tempMovesW[n + 1]] == kingEncodeB)
                                            totalValueW += kingValue;
                                    }
                                }

                                if (i - 1 >= 0 && i - 1 <= 7 && j - 1 >= 0 && j - 1 <= 7)
                                    if (board[i - 1, j - 1] == pawnEncodeW)
                                        totalValueW += pawnValue;
                                if (i - 1 >= 0 && i - 1 <= 7 && j + 1 >= 0 && j + 1 <= 7)
                                    if (board[i - 1, j + 1] == pawnEncodeW)
                                        totalValueW += pawnValue;
                                if (i + 1 >= 0 && i + 1 <= 7 && j - 1 >= 0 && j - 1 <= 7)
                                    if (board[i + 1, j - 1] == pawnEncodeW)
                                        totalValueW += pawnValue;
                                if (i + 1 >= 0 && i + 1 <= 7 && j + 1 >= 0 && j + 1 <= 7)
                                    if (board[i + 1, j + 1] == pawnEncodeW)
                                        totalValueW += pawnValue;
                                break;

                            case bishopEncodeW:
                            case knightEncodeW:
                            case rookEncodeW:
                            case queenEncodeW:
                                tempPieceValue = getPieceValue(board[i, j]);
                                totalValueW += tempPieceValue;

                                tempMovesW = getPieceMoves(board[i, j], new int[] { i, j }, board);

                                for (int n = 0; n < tempMovesW.Length; n += 2)
                                {
                                    if (tempMovesW[n] > -1)
                                    {
                                        totalValueW += tempPieceValue * availableMoveValueMultiplicator;
                                        possibleMovesW.Add(new int[] { i, j, tempMovesW[n], tempMovesW[n + 1] });

                                        if (board[tempMovesW[n], tempMovesW[n + 1]] == kingEncodeB)
                                            totalValueW += kingValue;
                                    }


                                }
                                break;

                            case kingEncodeW:
                                totalValueW += kingValue;
                                tempMovesW = getKingMoves(board[i, j], i, j, board);

                                for (int n = 0; n < tempMovesW.Length; n += 2)
                                {
                                    if (tempMovesW[n] > -1)
                                    {
                                        totalValueW += tempPieceValue * availableMoveValueMultiplicator;

                                        possibleMovesW.Add(new int[] { i, j, tempMovesW[n], tempMovesW[n + 1] });
                                    }
                                }



                                break;

                            default:
                                break;
                        }
                    }
                    else if (board[i, j] >= BoardCalcs.pawnEncodeB && board[i, j] <= BoardCalcs.kingEncodeB)
                    {
                        pieceCounter++;

                        switch (board[i, j])
                        {
                            case pawnEncodeB:
                                totalValueB += pawnValue;

                                tempMovesB = getPawnMoves(board[i, j], i, j, board, j == 1 ? true : false);

                                for (int n = 0; n < tempMovesB.Length; n += 2)
                                {
                                    if (tempMovesB[n] > -1)
                                    {
                                        totalValueB += pawnValue * availableMoveValueMultiplicator;
                                        possibleMovesB.Add(new int[] { i, j, tempMovesB[n], tempMovesB[n + 1] });

                                        if (tempMovesB[n] > -1 && board[tempMovesB[n], tempMovesB[n + 1]] == kingEncodeW)
                                            totalValueB += kingValue;
                                    }
                                }

                                if (i - 1 >= 0 && i - 1 <= 7 && j - 1 >= 0 && j - 1 <= 7)
                                    if (board[i - 1, j - 1] == pawnEncodeB)
                                        totalValueB += pawnValue;
                                if (i - 1 >= 0 && i - 1 <= 7 && j + 1 >= 0 && j + 1 <= 7)
                                    if (board[i - 1, j + 1] == pawnEncodeB)
                                        totalValueB += pawnValue;
                                if (i + 1 >= 0 && i + 1 <= 7 && j - 1 >= 0 && j - 1 <= 7)
                                    if (board[i + 1, j - 1] == pawnEncodeB)
                                        totalValueB += pawnValue;
                                if (i + 1 >= 0 && i + 1 <= 7 && j + 1 >= 0 && j + 1 <= 7)
                                    if (board[i + 1, j + 1] == pawnEncodeB)
                                        totalValueB += pawnValue;

                                break;

                            case bishopEncodeB:
                            case knightEncodeB:
                            case rookEncodeB:
                            case queenEncodeB:
                                tempPieceValue = getPieceValue(board[i, j]);
                                totalValueB += tempPieceValue;

                                tempMovesB = getPieceMoves(board[i, j], new int[] { i, j }, board);

                                for (int n = 0; n < tempMovesB.Length; n += 2)
                                {
                                    if (tempMovesB[n] > -1)
                                    {
                                        totalValueB += tempPieceValue * availableMoveValueMultiplicator;
                                        possibleMovesB.Add(new int[] { i, j, tempMovesB[n], tempMovesB[n + 1] });

                                        if (tempMovesB[n] > -1 && board[tempMovesB[n], tempMovesB[n + 1]] == kingEncodeW)
                                            totalValueB += kingValue;
                                    }
                                }
                                break;

                            case kingEncodeB:
                                totalValueB += kingValue;
                                tempMovesB = getKingMoves(board[i, j], i, j, board);

                                for (int n = 0; n < tempMovesB.Length; n += 2)
                                {
                                    if (tempMovesB[n] > -1)
                                    {
                                        totalValueB += tempPieceValue * availableMoveValueMultiplicator;
                                        possibleMovesB.Add(new int[] { i, j, tempMovesB[n], tempMovesB[n + 1] });
                                    }
                                }



                                break;

                            default:
                                break;
                        }
                    }

                }
            }

            foreach (int[] movesW in possibleMovesW)
            {
                if (board[movesW[2], movesW[3]] >= pawnEncodeB && board[movesW[2], movesW[3]] <= queenEncodeB)
                {
                    totalValueW += getPieceValue(board[movesW[2], movesW[3]]) * killValueMultiplicator;
                    totalValueB -= getPieceValue(board[movesW[2], movesW[3]]) * killValueMultiplicator;
                }
            }

            foreach (int[] movesB in possibleMovesB)
            {
                if (board[movesB[2], movesB[3]] >= pawnEncodeW && board[movesB[2], movesB[3]] <= queenEncodeW)
                {
                    totalValueW -= getPieceValue(board[movesB[2], movesB[3]]) * killValueMultiplicator;
                    totalValueB += getPieceValue(board[movesB[2], movesB[3]]) * killValueMultiplicator;
                }
            }

            if (team == true)
            {
                if (possibleMovesB.Count <= 0)
                    return totalValueW + 200;

                int enemyPieceCounter = 0;
                for(int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (board[i, j] >= pawnEncodeB && board[i, j] <= kingEncodeB)
                            enemyPieceCounter++;

                totalValueW += (16 - enemyPieceCounter) * 4;

                return totalValueW;
            }
            else
            {
                if (possibleMovesW.Count <= 0)
                    return totalValueB + 200;

                int enemyPieceCounter = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (board[i, j] >= pawnEncodeW && board[i, j] <= kingEncodeW)
                            enemyPieceCounter++;

                totalValueB += (16 - enemyPieceCounter) * 4;

                return totalValueB;
            }
        }

        public static int getPieceValue(int encode)
        {
            switch (encode)
            {
                case pawnEncodeB:
                case pawnEncodeW:
                    return pawnValue;

                case bishopEncodeB:
                case bishopEncodeW:
                    return bishopValue;

                case knightEncodeB:
                case knightEncodeW:
                    return knightValue;

                case rookEncodeB:
                case rookEncodeW:
                    return rookValue;

                case queenEncodeB:
                case queenEncodeW:
                    return queenValue;

                case kingEncodeB:
                case kingEncodeW:
                    return kingValue;

                default:
                    return 0;
            }
        }

        //Used to get moves of a piece for bot
        public static int[] getPieceMoves(int encode, int[] posXY, int[,] board, bool pawnFirstMove = false)
        {
            switch(encode)
            {
                case pawnEncodeB:
                case pawnEncodeW:
                    return getPawnMoves(encode, posXY[0], posXY[1], board, pawnFirstMove);

                case bishopEncodeB:
                case bishopEncodeW:
                    return getBishopMoves(encode, posXY[0], posXY[1], board);

                case knightEncodeB:
                case knightEncodeW:
                    return getKnightMoves(encode, posXY[0], posXY[1], board);

                case rookEncodeB:
                case rookEncodeW:
                    return getRookMoves(encode, posXY[0], posXY[1], board);

                case queenEncodeB:
                case queenEncodeW:
                    return getQueenMoves(encode, posXY[0], posXY[1], board);

                case kingEncodeB:
                case kingEncodeW:
                    return getKingMoves(encode, posXY[0], posXY[1], board);

                default:
                    return new int[] { -1, -1 };
            }
        }

        //Handles checks and pins of possible moves for selected piece, does some funny stuff with inverting arrays but its lost technology to me by now 
        public static int[] legalizeMoves(int[] moves, int[] posXY, int[,] board)
        {
            for(int orgMoveCounter = 0; orgMoveCounter < moves.Length; orgMoveCounter += 2)
            {
                if (moves[orgMoveCounter] == -1 || posXY[0] > 7 || posXY[0] < 0 || posXY[1] > 7 || posXY[1] < 0)
                    continue;

                int[,] testBoard = new int[8, 8];
                for (int m = 0; m < 8; m++)
                {
                    for (int n = 0; n < 8; n++)
                        testBoard[m, n] = board[n, m];
                }

                int pieceEncode = board[posXY[0], posXY[1]];

                testBoard[posXY[1], posXY[0]] = emptyEncode;
                testBoard[moves[orgMoveCounter + 1], moves[orgMoveCounter]] = pieceEncode;
               /* Trace.Write("Pass: " + orgMoveCounter +"-------------\n");
                for (int m = 0; m < 8; m++)
                {
                    for (int n = 0; n < 8; n++)
                        Trace.Write(testBoard[m, n] + "|");
                    Trace.Write("\n");
                }*/
                /////////
                if (pieceEncode >= pawnEncodeW && pieceEncode <= kingEncodeW)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        for(int j = 0; j < 8; j++)
                        {
                            int possibleAttacker = testBoard[i, j];
                            int[] possibleAttackerMoves = {-1, -1}; 

                            if(possibleAttacker >= pawnEncodeB && possibleAttacker <= kingEncodeB)
                            {
                                switch(possibleAttacker)
                                {
                                    case pawnEncodeB:
                                        possibleAttackerMoves = getPawnMoves(possibleAttacker, i, j, testBoard, false, true);
                                        break;
                                    case bishopEncodeB:
                                        possibleAttackerMoves = getBishopMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case knightEncodeB:
                                        possibleAttackerMoves = getKnightMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case rookEncodeB:
                                        possibleAttackerMoves = getRookMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case queenEncodeB:
                                        possibleAttackerMoves = getQueenMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case kingEncodeB:
                                        possibleAttackerMoves = getKingMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    default:
                                        break;
                                }

                                /*for (int m = 0; m < possibleAttackerMoves.Length; m += 2)
                                    Trace.Write(possibleAttacker + "-(" + possibleAttackerMoves[m] + "|" + possibleAttackerMoves[m + 1] + ")\n");
                                Trace.Write(possibleAttacker + "----end\n");*/

                                for (int n = 0; n < possibleAttackerMoves.Length; n+=2)
                                {
                                    if (possibleAttackerMoves[n] != -1)
                                    if (testBoard[possibleAttackerMoves[n], possibleAttackerMoves[n + 1]] == kingEncodeW)
                                    {
                                        moves[orgMoveCounter] = -1;
                                        moves[orgMoveCounter + 1] = -1;
                                    }
                                }
                            }
                        }
                    }
                }
                /////////
                else if (pieceEncode >= pawnEncodeB && pieceEncode <= kingEncodeB)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            int possibleAttacker = testBoard[i, j];
                            int[] possibleAttackerMoves = { -1, -1 };

                            if (possibleAttacker >= pawnEncodeW && possibleAttacker <= kingEncodeW)
                            {
                                switch (possibleAttacker)
                                {
                                    case pawnEncodeW:
                                        possibleAttackerMoves = getPawnMoves(possibleAttacker, i, j, testBoard, false, true);
                                        break;
                                    case bishopEncodeW:
                                        possibleAttackerMoves = getBishopMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case knightEncodeW:
                                        possibleAttackerMoves = getKnightMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case rookEncodeW:
                                        possibleAttackerMoves = getRookMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case queenEncodeW:
                                        possibleAttackerMoves = getQueenMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    case kingEncodeW:
                                        possibleAttackerMoves = getKingMoves(possibleAttacker, i, j, testBoard, true);
                                        break;
                                    default:
                                        break;
                                }

                                /*Trace.Write(possibleAttacker + "----start\n");
                                for (int m = 0; m < possibleAttackerMoves.Length; m += 2)
                                    Trace.Write(possibleAttacker+"-("+ possibleAttackerMoves[m] +"|"+ possibleAttackerMoves[m+1]+")\n");
                                Trace.Write(possibleAttacker + "----end\n");

                                for(int m = 0; m < 8;m++)
                                {
                                    for (int n = 0; n < 8; n++)
                                        Trace.Write(testBoard[m,n]+"|");
                                    Trace.Write("\n");
                                }*/
                                    

                                for (int n = 0; n < possibleAttackerMoves.Length; n += 2)
                                {
                                    if (possibleAttackerMoves[n] != -1)
                                    if (testBoard[possibleAttackerMoves[n], possibleAttackerMoves[n+1]] == kingEncodeB)
                                    {
                                        moves[orgMoveCounter] = -1;
                                        moves[orgMoveCounter + 1] = -1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return moves;
        }

        //Returns an array of ints that indicates available moves of the pawn, encoding is for weather its black or white, curX and curY is the curent position in bitBoard,
        //board is the layout of the pieces in 8*8 int array with above encodings and isFirstMove indicates if its pawns first move and therefore if it can move 2 sapces at once.
        //Returned array is max length 8, unavailable moves are less than 0, otherwise first 2 numbers are x,y of first available move, second 2 numbers are x,y of second available move
        //and so on.
        //TODO add en'Passant
        public static int[] getPawnMoves(int encoding, int curX, int curY, int[,] board, bool isFirstMove = false, bool isLegalization = false)
        {
            if(curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1};

            if (encoding == pawnEncodeW)
            {
                List<int> moves = new List<int>();

                if (curY > 0)
                {
                    if (board[curX, curY - 1] == 0)
                    {
                        moves.Add(curX);
                        moves.Add(curY - 1);

                        if(curY - 2 > -1)
                        if (isFirstMove && board[curX, curY - 2] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY - 2);
                        }
                    }

                    if (curX < 7)
                    {
                        if (board[curX + 1, curY - 1] >= pawnEncodeB && board[curX + 1, curY - 1] <= kingEncodeB)
                        {
                            moves.Add(curX + 1);
                            moves.Add(curY - 1);
                        }
                    }

                    if (curX > 0)
                    {
                        if (board[curX - 1, curY - 1] >= pawnEncodeB && board[curX - 1, curY - 1] <= kingEncodeB)
                        {
                            moves.Add(curX - 1);
                            moves.Add(curY - 1);
                        }
                    }
                }
                if(isLegalization)
                    return moves.ToArray();
                else
                {
                    int[] posXY = new int[] { curX, curY };
                    return legalizeMoves(moves.ToArray(), posXY, board);
                }
                
            }
            else if (encoding == pawnEncodeB)
            {
                List<int> moves = new List<int>();

                if (curY < 7)
                {
                    if (board[curX, curY + 1] == 0)
                    {
                        moves.Add(curX);
                        moves.Add(curY + 1);    

                        if(curY + 2 < 8)
                        if (isFirstMove && board[curX, curY + 2] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY + 2);
                        }
                    }

                    if (curX < 7)
                    {
                        if (board[curX + 1, curY + 1] >= pawnEncodeW && board[curX + 1, curY + 1] <= kingEncodeW)
                        {
                            moves.Add(curX + 1);
                            moves.Add(curY + 1);
                        }
                    }

                    if (curX > 0)
                    {
                        if (board[curX - 1, curY + 1] >= pawnEncodeW && board[curX - 1, curY + 1] <= kingEncodeW)
                        {
                            moves.Add(curX - 1);
                            moves.Add(curY + 1);
                        }
                    }
                }

                if (isLegalization)
                    return moves.ToArray();
                else
                {
                    int[] posXY = new int[] { curX, curY };
                    return legalizeMoves(moves.ToArray(), posXY, board);
                }
            }
            else
            {
                Trace.WriteLine("Wrong pawn encoding");
                return new int[] { -1,-1};
            }    
        }

        //Reference getPawnMoves for what this and further 'get<Piece>Moves' funcions do, this and further ones have different specifics but same principal
        public static int[] getKnightMoves(int encoding, int curX, int curY, int[,] board, bool isLegalization = false)
        {
            if (curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1 };

            int[] moves = new int[16];

            for(int i = 0; i < 16; i++)
                moves[i] = -1;

            if (curY > 1)
            {
                if (curX > 0)
                {
                    if (board[curX - 1, curY - 2] == 0)
                    {
                        moves[0] = curX - 1;
                        moves[1] = curY - 2;
                    }
                    else if((encoding == knightEncodeB) && (board[curX - 1, curY - 2] >= pawnEncodeW && board[curX - 1, curY - 2] <= kingEncodeW))
                    {
                        moves[0] = curX - 1;
                        moves[1] = curY - 2;
                    }
                    else if(encoding == knightEncodeW && (board[curX - 1, curY - 2] >= pawnEncodeB && board[curX - 1, curY - 2] <= kingEncodeB))
                    {
                        moves[0] = curX - 1;
                        moves[1] = curY - 2;
                    }

                }

                if (curX < 7)
                {
                    if (board[curX + 1, curY - 2] == 0)
                    {
                        moves[2] = curX + 1;
                        moves[3] = curY - 2;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX + 1, curY - 2] >= pawnEncodeW && board[curX + 1, curY - 2] <= kingEncodeW))
                    {
                        moves[2] = curX + 1;
                        moves[3] = curY - 2;
                    }
                    else if (encoding == knightEncodeW && (board[curX + 1, curY - 2] >= pawnEncodeB && board[curX + 1, curY - 2] <= kingEncodeB))
                    {
                        moves[2] = curX + 1;
                        moves[3] = curY - 2;
                    }
                }
            }

            if (curY < 6)
            {
                if (curX > 0)
                {
                    if (board[curX - 1, curY + 2] == 0)
                    {
                        moves[4] = curX - 1;
                        moves[5] = curY + 2;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX - 1, curY + 2] >= pawnEncodeW && board[curX - 1, curY + 2] <= kingEncodeW))
                    {
                        moves[4] = curX - 1;
                        moves[5] = curY + 2;
                    }
                    else if (encoding == knightEncodeW && (board[curX - 1, curY + 2] >= pawnEncodeB && board[curX - 1, curY + 2] <= kingEncodeB))
                    {
                        moves[4] = curX - 1;
                        moves[5] = curY + 2;
                    }
                }

                if (curX < 7)
                {
                    if (board[curX + 1, curY + 2] == 0)
                    {
                        moves[6] = curX + 1;
                        moves[7] = curY + 2;

                    }
                    else if ((encoding == knightEncodeB) && (board[curX + 1, curY + 2] >= pawnEncodeW && board[curX + 1, curY + 2] <= kingEncodeW))
                    {
                        moves[6] = curX + 1;
                        moves[7] = curY + 2;
                    }
                    else if (encoding == knightEncodeW && (board[curX + 1, curY + 2] >= pawnEncodeB && board[curX + 1, curY + 2] <= kingEncodeB))
                    {
                        moves[6] = curX + 1;
                        moves[7] = curY + 2;
                    }
                }
            }

            if (curX < 6)
            {
                if (curY > 0)
                {
                    if (board[curX + 2, curY - 1] == 0)
                    {
                        moves[8] = curX + 2;
                        moves[9] = curY - 1;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX + 2, curY - 1] >= pawnEncodeW && board[curX + 2, curY - 1] <= kingEncodeW))
                    {
                        moves[8] = curX + 2;
                        moves[9] = curY - 1;
                    }
                    else if (encoding == knightEncodeW && (board[curX + 2, curY - 1] >= pawnEncodeB && board[curX + 2, curY - 1] <= kingEncodeB))
                    {
                        moves[8] = curX + 2;
                        moves[9] = curY - 1;
                    }
                }

                if (curY < 7)
                {
                    if (board[curX + 2, curY + 1] == 0)
                    {
                        moves[10] = curX + 2;
                        moves[11] = curY + 1;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX + 2, curY + 1] >= pawnEncodeW && board[curX + 2, curY + 1] <= kingEncodeW))
                    {
                        moves[10] = curX + 2;
                        moves[11] = curY + 1;
                    }
                    else if (encoding == knightEncodeW && (board[curX + 2, curY + 1] >= pawnEncodeB && board[curX + 2, curY + 1] <= kingEncodeB))
                    {
                        moves[10] = curX + 2;
                        moves[11] = curY + 1;
                    }
                }
            }

            if (curX > 1)
            {
                if (curY > 0)
                {
                    if (board[curX - 2, curY - 1] == 0)
                    {
                        moves[12] = curX - 2;
                        moves[13] = curY - 1;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX - 2, curY - 1] >= pawnEncodeW && board[curX - 2, curY - 1] <= kingEncodeW))
                    {
                        moves[12] = curX - 2;
                        moves[13] = curY - 1;
                    }
                    else if (encoding == knightEncodeW && (board[curX - 2, curY - 1] >= pawnEncodeB && board[curX - 2, curY - 1] <= kingEncodeB))
                    {
                        moves[12] = curX - 2;
                        moves[13] = curY - 1;
                    }
                }

                if (curY < 7)
                {
                    if (board[curX - 2, curY + 1] == 0)
                    {
                        moves[14] = curX - 2;
                        moves[15] = curY + 1;
                    }
                    else if ((encoding == knightEncodeB) && (board[curX - 2, curY + 1] >= pawnEncodeW && board[curX - 2, curY + 1] <= kingEncodeW))
                    {
                        moves[14] = curX - 2;
                        moves[15] = curY + 1;
                    }
                    else if (encoding == knightEncodeW && (board[curX - 2, curY + 1] >= pawnEncodeB && board[curX - 2, curY + 1] <= kingEncodeB))
                    {
                        moves[14] = curX - 2;
                        moves[15] = curY + 1;
                    }
                }
            }

            if (isLegalization)
                return moves.ToArray();
            else
            {
                int[] posXY = new int[] { curX, curY };
                return legalizeMoves(moves.ToArray(), posXY, board);
            }
        }

        public static int[] getBishopMoves(int encoding, int curX, int curY, int[,] board, bool isLegalization = false)
        {
            //Stopwatch sw = Stopwatch.StartNew();

            if (curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1 };

            List<int> moves = new List<int>();

            bool[] checks = { true, true, true, true };

            for(int i = 1, j = 1; (checks[0] || checks[1]) || (checks[2] || checks[3]); i++, j++)
            {
                if (checks[0])
                {
                    if (curX - i > -1 && curY - j > -1)
                    {
                        if (board[curX - i, curY - j] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY - j);
                        }
                        else if (encoding == bishopEncodeB)
                        {
                            if (board[curX - i, curY - j] >= pawnEncodeW && board[curX - i, curY - j] <= kingEncodeW)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY - j);
                                checks[0] = false;
                            }
                            else
                                checks[0] = false;
                        }
                        else if (encoding == bishopEncodeW)
                        {
                            if (board[curX - i, curY - j] >= pawnEncodeB && board[curX - i, curY - j] <= kingEncodeB)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY - j);
                                checks[0] = false;
                            }
                            else
                                checks[0] = false;
                        }
                        else
                            checks[0] = false;
                    }
                    else
                        checks[0] = false;
                }

                if (checks[1])
                {
                    if (curX + i < 8 && curY - j > -1)
                    {
                        if (board[curX + i, curY - j] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY - j);
                        }
                        else if (encoding == bishopEncodeB)
                        {
                            if (board[curX + i, curY - j] >= pawnEncodeW && board[curX + i, curY - j] <= kingEncodeW)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY - j);
                                checks[1] = false;
                            }
                            else
                                checks[1] = false;
                        }
                        else if (encoding == bishopEncodeW)
                        {
                            if (board[curX + i, curY - j] >= pawnEncodeB && board[curX + i, curY - j] <= kingEncodeB)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY - j);
                                checks[1] = false;
                            }
                            else
                                checks[1] = false;
                        }
                        else
                            checks[1] = false;
                    }
                    else
                        checks[1] = false;
                }

                if (checks[2])
                {
                    if (curX - i > -1 && curY + j < 8)
                    {
                        if (board[curX - i, curY + j] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY + j);
                        }
                        else if (encoding == bishopEncodeB)
                        {
                            if (board[curX - i, curY + j] >= pawnEncodeW && board[curX - i, curY + j] <= kingEncodeW)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY + j);
                                checks[2] = false;
                            }
                            else
                                checks[2] = false;
                        }
                        else if (encoding == bishopEncodeW)
                        {
                            if (board[curX - i, curY + j] >= pawnEncodeB && board[curX - i, curY + j] <= kingEncodeB)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY + j);
                                checks[2] = false;
                            }
                            else
                                checks[2] = false;
                        }
                        else
                            checks[2] = false;
                    }
                    else
                        checks[2] = false;
                }

                if (checks[3])
                {
                    if (curX + i < 8 && curY + j < 8)
                    {
                        if (board[curX + i, curY + j] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY + j);
                        }
                        else if (encoding == bishopEncodeB)
                        {
                            if (board[curX + i, curY + j] >= pawnEncodeW && board[curX + i, curY + j] <= kingEncodeW)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY + j);
                                checks[3] = false;
                            }
                            else
                                checks[3] = false;
                        }
                        else if (encoding == bishopEncodeW)
                        {
                            if (board[curX + i, curY + j] >= pawnEncodeB && board[curX + i, curY + j] <= kingEncodeB)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY + j);
                                checks[3] = false;
                            }
                            else
                                checks[3] = false;
                        }
                        else
                            checks[3] = false;
                    }
                    else
                        checks[3] = false;
                }
            }

            //sw.Stop();
            //Trace.WriteLine((sw.ElapsedTicks*100)); //nanoseconds of execution

            if (isLegalization)
                return moves.ToArray();
            else
            {
                int[] posXY = new int[] { curX, curY };
                return legalizeMoves(moves.ToArray(), posXY, board);
            }
        }

        public static int[] getRookMoves(int encoding, int curX, int curY, int[,] board, bool isLegalization = false)
        {
            //Stopwatch sw = new Stopwatch();

            if (curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1 };

            List<int> moves = new List<int>();
            bool[] checks = { true, true , true, true};

            for(int i = 1; (checks[0] || checks[1]) || (checks[2] || checks[3]); i++)
            {
                if (checks[0])
                {
                    if(curY - i >= 0)
                    {
                        if (board[curX, curY - i] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                        }
                        else if((encoding == rookEncodeB) && (board[curX, curY - i] >= pawnEncodeW && board[curX, curY - i] <= kingEncodeW))
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                            checks[0] = false;
                        }
                        else if ((encoding == rookEncodeW) && (board[curX, curY - i] >= pawnEncodeB && board[curX, curY - i] <= kingEncodeB))
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                            checks[0] = false;
                        }
                        else
                            checks[0] = false;
                    }
                    else
                        checks[0] = false;
                }

                if (checks[1])
                {
                    if (curY + i <= 7)
                    {
                        if (board[curX, curY + i] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                        }
                        else if ((encoding == rookEncodeB) && (board[curX, curY + i] >= pawnEncodeW && board[curX, curY + i] <= kingEncodeW))
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                            checks[1] = false;
                        }
                        else if ((encoding == rookEncodeW) && (board[curX, curY + i] >= pawnEncodeB && board[curX, curY + i] <= kingEncodeB))
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                            checks[1] = false;
                        }
                        else
                            checks[1] = false;
                    }
                    else
                        checks[1] = false;
                }

                if (checks[2])
                {
                    if (curX + i <= 7)
                    {
                        if (board[curX + i, curY] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                        }
                        else if ((encoding == rookEncodeB) && (board[curX + i, curY] >= pawnEncodeW && board[curX + i, curY] <= kingEncodeW))
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                            checks[2] = false;
                        }
                        else if ((encoding == rookEncodeW) && (board[curX + i, curY] >= pawnEncodeB && board[curX + i, curY] <= kingEncodeB))
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                            checks[2] = false;
                        }
                        else
                            checks[2] = false;
                    }
                    else
                        checks[2] = false;
                }

                if (checks[3])
                {
                    if (curX - i >= 0)
                    {
                        if (board[curX - i, curY] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                        }
                        else if ((encoding == rookEncodeB) && (board[curX - i, curY] >= pawnEncodeW && board[curX - i, curY] <= kingEncodeW))
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                            checks[3] = false;
                        }
                        else if ((encoding == rookEncodeW) && (board[curX - i, curY] >= pawnEncodeB && board[curX - i, curY] <= kingEncodeB))
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                            checks[3] = false;
                        }
                        else
                            checks[3] = false;
                    }
                    else
                        checks[3] = false;
                }
            }

            //sw.Stop();
            //Trace.WriteLine((sw.ElapsedTicks * 100)); //nanoseconds of execution

            //return moves.ToArray();
            if (isLegalization)
                return moves.ToArray();
            else
            {
                int[] posXY = new int[] { curX, curY };
                return legalizeMoves(moves.ToArray(), posXY, board);
            }
        }

        public static int[] getQueenMoves(int encoding, int curX, int curY, int[,] board, bool isLegalization = false)
        {
            if (curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1 };

            int tempEncodeDiag, tempEncodeStr;

            if(encoding == queenEncodeB)
            {
                tempEncodeDiag = bishopEncodeB;
                tempEncodeStr = rookEncodeB;
            }
            else
            {
                tempEncodeDiag = bishopEncodeW;
                tempEncodeStr = rookEncodeW;
            }

            List<int> moves = new List<int>();

            bool[] checks = { true, true, true, true };
            for (int i = 1, j = 1; (checks[0] || checks[1]) || (checks[2] || checks[3]); i++, j++)
            {
                if (checks[0])
                {
                    if (curX - i > -1 && curY - j > -1)
                    {
                        if (board[curX - i, curY - j] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY - j);
                        }
                        else if (tempEncodeDiag == bishopEncodeB)
                        {
                            if (board[curX - i, curY - j] >= pawnEncodeW && board[curX - i, curY - j] <= kingEncodeW)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY - j);
                                checks[0] = false;
                            }
                            else
                                checks[0] = false;
                        }
                        else if (tempEncodeDiag == bishopEncodeW)
                        {
                            if (board[curX - i, curY - j] >= pawnEncodeB && board[curX - i, curY - j] <= kingEncodeB)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY - j);
                                checks[0] = false;
                            }
                            else
                                checks[0] = false;
                        }
                        else
                            checks[0] = false;
                    }
                    else
                        checks[0] = false;
                }

                if (checks[1])
                {
                    if (curX + i < 8 && curY - j > -1)
                    {
                        if (board[curX + i, curY - j] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY - j);
                        }
                        else if (tempEncodeDiag == bishopEncodeB)
                        {
                            if (board[curX + i, curY - j] >= pawnEncodeW && board[curX + i, curY - j] <= kingEncodeW)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY - j);
                                checks[1] = false;
                            }
                            else
                                checks[1] = false;
                        }
                        else if (tempEncodeDiag == bishopEncodeW)
                        {
                            if (board[curX + i, curY - j] >= pawnEncodeB && board[curX + i, curY - j] <= kingEncodeB)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY - j);
                                checks[1] = false;
                            }
                            else
                                checks[1] = false;
                        }
                        else
                            checks[1] = false;
                    }
                    else
                        checks[1] = false;
                }

                if (checks[2])
                {
                    if (curX - i > -1 && curY + j < 8)
                    {
                        if (board[curX - i, curY + j] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY + j);
                        }
                        else if (tempEncodeDiag == bishopEncodeB)
                        {
                            if (board[curX - i, curY + j] >= pawnEncodeW && board[curX - i, curY + j] <= kingEncodeW)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY + j);
                                checks[2] = false;
                            }
                            else
                                checks[2] = false;
                        }
                        else if (tempEncodeDiag == bishopEncodeW)
                        {
                            if (board[curX - i, curY + j] >= pawnEncodeB && board[curX - i, curY + j] <= kingEncodeB)
                            {
                                moves.Add(curX - i);
                                moves.Add(curY + j);
                                checks[2] = false;
                            }
                            else
                                checks[2] = false;
                        }
                        else
                            checks[2] = false;
                    }
                    else
                        checks[2] = false;
                }

                if (checks[3])
                {
                    if (curX + i < 8 && curY + j < 8)
                    {
                        if (board[curX + i, curY + j] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY + j);
                        }
                        else if (tempEncodeDiag == bishopEncodeB)
                        {
                            if (board[curX + i, curY + j] >= pawnEncodeW && board[curX + i, curY + j] <= kingEncodeW)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY + j);
                                checks[3] = false;
                            }
                            else
                                checks[3] = false;
                        }
                        else if (tempEncodeDiag == bishopEncodeW)
                        {
                            if (board[curX + i, curY + j] >= pawnEncodeB && board[curX + i, curY + j] <= kingEncodeB)
                            {
                                moves.Add(curX + i);
                                moves.Add(curY + j);
                                checks[3] = false;
                            }
                            else
                                checks[3] = false;
                        }
                        else
                            checks[3] = false;
                    }
                    else
                        checks[3] = false;
                }
            }

            checks = new bool[]{ true, true, true, true };
            for (int i = 1; (checks[0] || checks[1]) || (checks[2] || checks[3]); i++)
            {
                if (checks[0])
                {
                    if (curY - i >= 0)
                    {
                        if (board[curX, curY - i] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                        }
                        else if ((tempEncodeStr == rookEncodeB) && (board[curX, curY - i] >= pawnEncodeW && board[curX, curY - i] <= kingEncodeW))
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                            checks[0] = false;
                        }
                        else if ((tempEncodeStr == rookEncodeW) && (board[curX, curY - i] >= pawnEncodeB && board[curX, curY - i] <= kingEncodeB))
                        {
                            moves.Add(curX);
                            moves.Add(curY - i);
                            checks[0] = false;
                        }
                        else
                            checks[0] = false;
                    }
                    else
                        checks[0] = false;
                }

                if (checks[1])
                {
                    if (curY + i <= 7)
                    {
                        if (board[curX, curY + i] == 0)
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                        }
                        else if ((tempEncodeStr == rookEncodeB) && (board[curX, curY + i] >= pawnEncodeW && board[curX, curY + i] <= kingEncodeW))
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                            checks[1] = false;
                        }
                        else if ((tempEncodeStr == rookEncodeW) && (board[curX, curY + i] >= pawnEncodeB && board[curX, curY + i] <= kingEncodeB))
                        {
                            moves.Add(curX);
                            moves.Add(curY + i);
                            checks[1] = false;
                        }
                        else
                            checks[1] = false;
                    }
                    else
                        checks[1] = false;
                }

                if (checks[2])
                {
                    if (curX + i <= 7)
                    {
                        if (board[curX + i, curY] == 0)
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                        }
                        else if ((tempEncodeStr == rookEncodeB) && (board[curX + i, curY] >= pawnEncodeW && board[curX + i, curY] <= kingEncodeW))
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                            checks[2] = false;
                        }
                        else if ((tempEncodeStr == rookEncodeW) && (board[curX + i, curY] >= pawnEncodeB && board[curX + i, curY] <= kingEncodeB))
                        {
                            moves.Add(curX + i);
                            moves.Add(curY);
                            checks[2] = false;
                        }
                        else
                            checks[2] = false;
                    }
                    else
                        checks[2] = false;
                }

                if (checks[3])
                {
                    if (curX - i >= 0)
                    {
                        if (board[curX - i, curY] == 0)
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                        }
                        else if ((tempEncodeStr == rookEncodeB) && (board[curX - i, curY] >= pawnEncodeW && board[curX - i, curY] <= kingEncodeW))
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                            checks[3] = false;
                        }
                        else if ((tempEncodeStr == rookEncodeW) && (board[curX - i, curY] >= pawnEncodeB && board[curX - i, curY] <= kingEncodeB))
                        {
                            moves.Add(curX - i);
                            moves.Add(curY);
                            checks[3] = false;
                        }
                        else
                            checks[3] = false;
                    }
                    else
                        checks[3] = false;
                }
            }

            //moves.AddRange(movesDiagonal);
            //moves.AddRange(movesStraight);

            //sw.Stop();
            //Trace.WriteLine((sw.ElapsedTicks*100)); //nanoseconds of execution

            //return moves.ToArray();

            if (isLegalization)
                return moves.ToArray();
            else
            {
                int[] posXY = new int[] { curX, curY };
                return legalizeMoves(moves.ToArray(), posXY, board);
            }
        }

        public static int[] getKingMoves(int encoding, int curX, int curY, int[,] board, bool isLegalization = false)
        {
            if (curX > 7 || curX < 0 || curY > 7 || curY < 0)
                return new int[] { -1, -1 };

            List<int> moves = new List<int>();

            bool upperFlag = false;
            bool lowerFlag = false;
            bool leftFlag = false;
            bool rightFlag = false;

            if (curY > 0)
            {
                upperFlag = true;

                if (board[curX, curY - 1] == 0)
                {
                    moves.Add(curX);
                    moves.Add(curY - 1);
                }
                else if((encoding == kingEncodeB) && (board[curX, curY - 1] >= pawnEncodeW && board[curX, curY - 1] <= kingEncodeW))
                {
                    moves.Add(curX);
                    moves.Add(curY - 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX, curY - 1] >= pawnEncodeB && board[curX, curY - 1] <= kingEncodeB))
                {
                    moves.Add(curX);
                    moves.Add(curY - 1);
                }
            }

            if (curX > 0)
            {
                leftFlag = true;

                if (board[curX - 1, curY] == 0)
                {
                    moves.Add(curX - 1);
                    moves.Add(curY);
                }
                else if ((encoding == kingEncodeB) && (board[curX - 1, curY] >= pawnEncodeW && board[curX - 1, curY] <= kingEncodeW))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY);
                }
                else if ((encoding == kingEncodeW) && (board[curX - 1, curY] >= pawnEncodeB && board[curX - 1, curY] <= kingEncodeB))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY);
                }
            }

            if (curX < 7)
            {
                rightFlag = true;

                if (board[curX + 1, curY] == 0)
                {
                    moves.Add(curX + 1);
                    moves.Add(curY);
                }
                else if ((encoding == kingEncodeB) && (board[curX + 1, curY] >= pawnEncodeW && board[curX + 1, curY] <= kingEncodeW))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY);
                }
                else if ((encoding == kingEncodeW) && (board[curX + 1, curY] >= pawnEncodeB && board[curX + 1, curY] <= kingEncodeB))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY);
                }
            }

            if (curY < 7)
            {
                lowerFlag = true;

                if (board[curX, curY + 1] == 0)
                {
                    moves.Add(curX);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeB) && (board[curX, curY + 1] >= pawnEncodeW && board[curX, curY + 1] <= kingEncodeW))
                {
                    moves.Add(curX);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX, curY + 1] >= pawnEncodeB && board[curX, curY + 1] <= kingEncodeB))
                {
                    moves.Add(curX);
                    moves.Add(curY + 1);
                }
            }

            if(upperFlag && leftFlag)
            {
                if (board[curX - 1, curY - 1] == 0)
                {
                    moves.Add(curX - 1);
                    moves.Add(curY - 1);
                }
                else if ((encoding == kingEncodeB) && (board[curX - 1, curY - 1] >= pawnEncodeW && board[curX - 1, curY - 1] <= kingEncodeW))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY - 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX - 1, curY - 1] >= pawnEncodeB && board[curX - 1, curY - 1] <= kingEncodeB))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY - 1);
                }
            }

            if (upperFlag && rightFlag)
            {
                if (board[curX + 1, curY - 1] == 0)
                {
                    moves.Add(curX + 1);
                    moves.Add(curY - 1);
                }
                else if ((encoding == kingEncodeB) && (board[curX + 1, curY - 1] >= pawnEncodeW && board[curX + 1, curY - 1] <= kingEncodeW))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY - 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX + 1, curY - 1] >= pawnEncodeB && board[curX + 1, curY - 1] <= kingEncodeB))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY - 1);
                }
            }

            if (lowerFlag && rightFlag)
            {
                if (board[curX + 1, curY + 1] == 0)
                {
                    moves.Add(curX + 1);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeB) && (board[curX + 1, curY + 1] >= pawnEncodeW && board[curX + 1, curY + 1] <= kingEncodeW))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX + 1, curY + 1] >= pawnEncodeB && board[curX + 1, curY + 1] <= kingEncodeB))
                {
                    moves.Add(curX + 1);
                    moves.Add(curY + 1);
                }
            }

            if (lowerFlag && leftFlag)
            {
                if (board[curX - 1, curY + 1] == 0)
                {
                    moves.Add(curX - 1);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeB) && (board[curX - 1, curY + 1] >= pawnEncodeW && board[curX - 1, curY + 1] <= kingEncodeW))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY + 1);
                }
                else if ((encoding == kingEncodeW) && (board[curX - 1, curY + 1] >= pawnEncodeB && board[curX - 1, curY + 1] <= kingEncodeB))
                {
                    moves.Add(curX - 1);
                    moves.Add(curY + 1);
                }
            }



            //return moves.ToArray();

            if (isLegalization)
                return moves.ToArray();
            else
            {
                int[] posXY = new int[] { curX, curY };
                return legalizeMoves(moves.ToArray(), posXY, board);
            }
        }
    }
}
