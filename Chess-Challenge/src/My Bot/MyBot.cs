using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        SortedList<Move, float> evaluatedMoves = new();
        float maxScore = float.MinValue;
        Move bestMove = moves[0];
        foreach (var move in moves)
        {
            float moveScore = GetImmediateScore(board);
            evaluatedMoves.Add(move, moveScore); // O calcular otra vez la puntuacion solo al final y asi no se guarda tambien en diccionario
            if (moveScore > maxScore)
                maxScore = moveScore;

            bestMove = evaluatedMoves.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        }
        return bestMove;
    }
    float score = 0;
    float GetImmediateScore(Board board)
    {
        score = 0;
        foreach (var move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            float materialScore = GetMaterialScore(board.IsWhiteToMove, board);
            float positionalScore = GetPositionalScore(board.IsWhiteToMove, board);
            score = positionalScore;
            board.UndoMove(move);
        }
        return score;
    }

    private float GetPositionalScore(bool isWhiteToMove, Board board)
    {
        foreach (var pawn in board.GetPieceList(PieceType.Pawn, isWhiteToMove)) //GetPieceBitboard quizas mas eficiente
        {
            // si el peon esta protegiendo otra pieza
            if (board.GetPiece(new(pawn.Square.Index)).IsWhite)
            {
                score += 10; //bonus por proteger
            }
        }
        foreach (var knight in board.GetPieceList(PieceType.Knight, isWhiteToMove))
        {
            int x = knight.Square.Index % 8;
            int y = knight.Square.Index / 8;

            if (x > 2 && x < 5 && y > 2 && y < 5)
            {
                score += 10; //bonus por posicion central
            }
            else if (x > 1 && x < 6 && y > 1 && y < 6)
            {
                score += 5; //bonus por posicion semi-central
            }
        }
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