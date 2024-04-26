using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Models
{
    public partial class Board
    {
        public List<(MatchPatternType, (Vector2Int pos, List<Vector2Int> explosionCells))> GetExplosions()
        {
            CountSameCellsAllDirectional();
            return ExtractExplosionsCells(this);
        }

        private void CountSameCellsAllDirectional()
        {
            for (var i = 0; i < RowsCount; i++)
            {
                for (var j = 0; j < ColumnsCount; j++)
                {
                    var currentCellPos = new Vector2Int(i, j);
                    SumNextCell(currentCellPos, Direction.Left);
                    SumNextCell(currentCellPos, Direction.Down);
                }
            }

            for (var i = RowsCount - 1; i >= 0; i--)
            {
                for (var j = ColumnsCount - 1; j >= 0; j--)
                {
                    var currentCellPos = new Vector2Int(i, j);
                    SumNextCell(currentCellPos, Direction.Right);
                    SumNextCell(currentCellPos, Direction.Up);
                }
            }
        }

        private void SumNextCell(Vector2Int currentCellPos, Direction counterDirection)
        {
            var currentCell = GetCell(currentCellPos);
            var nextCellPos = GetNextCellPosByDirection(currentCellPos, counterDirection);
            if (!IsCellPosValid(nextCellPos))
            {
                currentCell.Counter[counterDirection] = 0;
                return;
            }

            var nextCell = GetCell(nextCellPos);
            if (nextCell.StoneType != currentCell.StoneType)
            {
                currentCell.Counter[counterDirection] = 0;
                return;
            }

            currentCell.Counter[counterDirection] = nextCell.Counter[counterDirection] + 1;
        }

        private static List<(MatchPatternType, (Vector2Int pos, List<Vector2Int> explosionCells))>
            ExtractExplosionsCells(
                Board board)
        {
            var sortedDictionary = new SortedDictionary<MatchPatternType, List<Vector2Int>>();
            for (var i = 0; i < board.Rows.Count; i++)
            {
                var row = board.Rows[i];
                for (var j = 0; j < row.Columns.Count; j++)
                {
                    var cellPos = new Vector2Int(i, j);
                    var cell = board.GetCell(cellPos);
                    var pattern = cell.GetMatchPattern(out _);
                    if (pattern == MatchPatternType.None) continue;
                    sortedDictionary.TryAdd(pattern, new List<Vector2Int>());

                    sortedDictionary[pattern].Add(cellPos);
                }
            }

            var result = new List<(MatchPatternType, (Vector2Int pos, List<Vector2Int> explosionCells))>();
            var allExplodedCells = new List<Vector2Int>();
            foreach (var patternPair in sortedDictionary)
            {
                foreach (var cellPos in patternPair.Value)
                {
                    var cell = board.GetCell(cellPos);
                    if (allExplodedCells.Contains(cellPos))
                        continue;
                    var explosionCells = GetExplosionCellsByPattern(cell, cellPos, board.RowsCount,
                        board.ColumnsCount, allExplodedCells);

                    if (explosionCells.Count > 0)
                    {
                        result.Add((patternPair.Key, (cellPos, explosionCells)));
                        allExplodedCells.AddRange(explosionCells);
                    }
                }
            }

            return result;
        }

        private static List<Vector2Int> GetExplosionCellsByPattern(BoardCell cell, Vector2Int cellPos, int rowsCount,
            int columnsCont, IReadOnlyList<Vector2Int> explodedPoses)
        {
            var pattern = cell.GetMatchPattern(out var inRow);
            var explosionCells = new List<Vector2Int>();
            switch (pattern)
            {
                case MatchPatternType.Three:
                    for (int i = 0; i < 3; i++)
                    {
                        var pos = inRow
                            ? new Vector2Int(cellPos.x + i, cellPos.y)
                            : new Vector2Int(cellPos.x, cellPos.y + i);
                        if (explodedPoses.Contains(pos)) return new List<Vector2Int>();
                        explosionCells.Add(pos);
                    }

                    break;
                case MatchPatternType.Four:
                    for (int i = 0; i < 4; i++)
                    {
                        var pos = inRow
                            ? new Vector2Int(cellPos.x + i, cellPos.y)
                            : new Vector2Int(cellPos.x, cellPos.y + i);
                        if (explodedPoses.Contains(pos)) return new List<Vector2Int>();

                        explosionCells.Add(pos);
                    }

                    break;
                case MatchPatternType.Five:
                    for (int i = 0; i < 5; i++)
                    {
                        var pos = inRow
                            ? new Vector2Int(cellPos.x + i, cellPos.y)
                            : new Vector2Int(cellPos.x, cellPos.y + i);
                        if (explodedPoses.Contains(pos)) return new List<Vector2Int>();

                        explosionCells.Add(pos);
                    }

                    break;
                case MatchPatternType.SetSquare:
                    for (int i = cellPos.x - 2; i < cellPos.x + 2; i++)
                    {
                        for (int j = cellPos.y - 2; j < cellPos.y + 2; j++)
                        {
                            var pos = new Vector2Int(i, j);
                            if (IsCellPosValid(pos, rowsCount, columnsCont))
                            {
                                if (explodedPoses.Contains(pos)) return new List<Vector2Int>();

                                explosionCells.Add(pos);
                            }
                        }
                    }
                    
                    break;
                case MatchPatternType.Plus:

                    for (int i = 0; i < rowsCount; i++)
                    {
                        var pos = new Vector2Int(i, cellPos.y);
                        if (explodedPoses.Contains(pos)) return new List<Vector2Int>();
                        explosionCells.Add(pos);
                    }

                    for (int i = 0; i < columnsCont; i++)
                    {
                        var pos = new Vector2Int(cellPos.x, i);
                        if (explodedPoses.Contains(pos)) return new List<Vector2Int>();
                        explosionCells.Add(pos);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return explosionCells;
        }

        private static List<(MatchPatternType, (Vector2Int pos, BoardCell cell))> RemoveIntersections(
            List<(MatchPatternType, (Vector2Int pos, BoardCell cell))> patterns)
        {
            var sortedPatterns = patterns.OrderByDescending(p => (int)p.Item1).ToArray();
            var newPatterns = new List<(MatchPatternType, (Vector2Int pos, BoardCell cell))>();
            for (var i = sortedPatterns.Length - 1; i >= 0; i--)
            {
                var pair = sortedPatterns[i];
                for (var j = i - 1; j >= 0; j--)
                {
                    var upperPair = sortedPatterns[j];
                    var result = DoesTheseCellsIntersect(pair.Item2, upperPair.Item2);

                    if (!result)
                        newPatterns.Add((pair.Item1, pair.Item2));
                }
            }

            return newPatterns;
        }

        private static bool DoesTheseCellsIntersect((Vector2Int pos, BoardCell cell) firstCell,
            (Vector2Int pos, BoardCell cell) secondCell)
        {
            if (firstCell.pos.x == secondCell.pos.x)
            {
                var rightRange = firstCell.pos.x + firstCell.cell.Counter[Direction.Right];
                if (secondCell.pos.x > firstCell.pos.x && rightRange >= secondCell.pos.x)
                    return true;

                var leftRange = firstCell.pos.x - firstCell.cell.Counter[Direction.Left];
                if (secondCell.pos.x < firstCell.pos.x && leftRange <= secondCell.pos.x)
                    return true;
            }

            if (firstCell.pos.y == secondCell.pos.y)
            {
                var upRange = firstCell.pos.y + firstCell.cell.Counter[Direction.Up];
                if (secondCell.pos.y > firstCell.pos.y && upRange >= secondCell.pos.y)
                    return true;

                var downRange = firstCell.pos.y - firstCell.cell.Counter[Direction.Down];
                if (secondCell.pos.y < firstCell.pos.y && downRange >= secondCell.pos.y)
                    return true;
            }

            return false;
        }
    }
}