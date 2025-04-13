using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;
using ChessChallenge.Application;

public class MyBot : IChessBot
{
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    //ulong fileA=;
    public Move Think(Board board, Timer timer)
    {
        Move[] legalMoves = board.GetLegalMoves();
        List<KeyValuePair<Move, float>> evaluatedMoves = new();
        float maxScore = float.MinValue;
        Move bestMove = legalMoves[0];
        foreach (var move in legalMoves)
        {
            board.MakeMove(move);
            float moveScore = GetImmediateScore(board);
            evaluatedMoves.Add(new(move, moveScore)); // O calcular otra vez la puntuacion solo al final y asi no se guarda tambien en diccionario
            if (moveScore > maxScore)
                maxScore = moveScore;
            board.UndoMove(move);
        }
        bestMove = evaluatedMoves.First(x => x.Value == maxScore).Key;
        //bestMove = evaluatedMoves.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        foreach (var move in evaluatedMoves)
        {
            Console.WriteLine(move.Key + " " + move.Value);
        }
        Console.WriteLine("Best move: " + bestMove + " with score: " + maxScore);
        return bestMove;
    }

    float GetImmediateScore(Board board)
    {
        float score = 0;
        foreach (var move in board.GetLegalMoves())
        {
            //board.MakeMove(move);
            //float materialScore = GetMaterialScore(board.IsWhiteToMove, board);
            float positionalScore = GetPositionalScore(board.IsWhiteToMove, board);
            score = positionalScore;
            if (move.MovePieceType == PieceType.Pawn) score = 1;
            //board.UndoMove(move);
        }
        return score;
    }

    private float GetPositionalScore(bool isWhiteToMove, Board board)
    {
        float score = 0;
        //derecha x>>
        //izquierda x<<
        //arriba x>>8
        //abajo x<<8


        ulong pawnsBitBoard = board.GetPieceBitboard(PieceType.Pawn, isWhiteToMove);
        //ulong pawnAttackRight = pawnsBitBoard
        // foreach (var pawn in board.GetPieceList(PieceType.Pawn, isWhiteToMove))
        // {
        //     //ulong pawnBitBoard = Convert.To((ulong)Math.Pow(2, pawn.Square.Index),2);

        //     if ((pawnBitBoard.AddDirection(-1, 1, isWhiteToMove) & board.GetPieceBitboard(PieceType.Knight, isWhiteToMove)) > 1)
        //         // si el peon esta protegiendo otra pieza
        //         score += 1;
        // }

        // board.GetPieceBitboard(PieceType.Pawn, isWhiteToMove);
        // foreach (var knight in board.GetPieceList(PieceType.Knight, isWhiteToMove))
        // {
        //     int x = knight.Square.Index % 8;
        //     int y = knight.Square.Index / 8;

        //     if (x > 2 && x < 5 && y > 2 && y < 5)
        //     {
        //         score += 10; //bonus por posicion central
        //     }
        //     else if (x > 1 && x < 6 && y > 1 && y < 6)
        //     {
        //         score += 5; //bonus por posicion semi-central
        //     }
        // }
        return score;
    }

    int GetMaterialScore(bool isWhite, Board board)
    {
        int score = 0;
        for (int i = 0; i < 64; i++)
        {
            Piece piece = board.GetPiece(new(i));
            if (piece.PieceType == PieceType.None) continue;
            if (piece.IsWhite == isWhite)
            {
                score += pieceValues[(int)piece.PieceType];
            }
            else
            {
                score -= pieceValues[(int)piece.PieceType];
            }
        }
        return score;
    }
}
public static class Extensions
{
    public static ulong AddDirection(this ulong input, int x, int y, bool isWhite)
    {
        if (!isWhite)
        {
            x = -x;
            y = -y;
        }
        return input + (ulong)Math.Pow(2, x + y * 8);
    }
}
