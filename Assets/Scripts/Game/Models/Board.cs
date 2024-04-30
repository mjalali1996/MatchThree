using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    [Serializable]
    public partial class Board
    {
        public int RowsCount => _rows.Count;
        public int ColumnsCount => _rows.Count == 0 ? 0 : _rows[0].Columns.Count;

        [SerializeField] private List<BoardRow> _rows = new();
        public IReadOnlyList<BoardRow> Rows => _rows;

        public void CreateBoard(int rows, int columns)
        {
            _rows.Clear();
            for (var i = 0; i < rows; i++)
            {
                var boardRow = new BoardRow();
                for (int j = 0; j < columns; j++)
                {
                    var cell = new BoardCell();
                    boardRow.Columns.Add(cell);
                }

                _rows.Add(boardRow);
            }
        }

        public void SwapStones(Vector2Int firstStonePos, Vector2Int secondStonePos)
        {
            var firstStone = GetStone(firstStonePos);
            var secondStone = GetStone(secondStonePos);

            SetStone(firstStonePos, secondStone);
            SetStone(secondStonePos, firstStone);
        }


        private BoardCell GetCell(Vector2Int pos)
        {
            return _rows[pos.x].Columns[pos.y];
        }

        private StoneType GetStone(Vector2Int pos)
        {
            return GetCell(pos).StoneType;
        }

        public void SetStone(Vector2Int pos, StoneType stone)
        {
            _rows[pos.x].Columns[pos.y].StoneType = stone;
        }

        public bool IsThreeACell(Vector2Int startPos, Direction dir)
        {
            var nextCell = GetNextCellPosByDirection(startPos, dir);

            return IsCellPosValid(nextCell);
        }

        public bool IsCellPosValid(Vector2Int pos)
        {
            return IsCellPosValid(pos, RowsCount, ColumnsCount);
        }

        public void Explode(List<Vector2Int> explosionStones)
        {
            foreach (var explosionCellPos in explosionStones)
            {
                SetStone(explosionCellPos, StoneType.None);
            }
        }

        public Dictionary<Vector2Int, Vector2Int> GetFallDownStones()
        {
            var fallDownStones = new Dictionary<Vector2Int, Vector2Int>();
            for (var i = RowsCount - 1; i >= 0; i--)
            {
                var verticalEmptyCellsCounter = 0;
                for (var j = ColumnsCount - 1; j >= 0; j--)
                {
                    var pos = new Vector2Int(i, j);
                    var cell = GetCell(pos);
                    if (cell.StoneType == StoneType.None)
                    {
                        verticalEmptyCellsCounter++;
                        continue;
                    }

                    if (verticalEmptyCellsCounter != 0)
                        fallDownStones.Add(pos, new Vector2Int(i, j + verticalEmptyCellsCounter));
                }
            }

            return fallDownStones;
        }

        public Dictionary<Vector2Int, StoneType> GetNewStones()
        {
            var newStones = new Dictionary<Vector2Int, StoneType>();
            var stoneTypesCount = Enum.GetValues(typeof(StoneType)).Length;
            for (var j = ColumnsCount - 1; j >= 0; j--)
            {
                for (var i = RowsCount - 1; i >= 0; i--)
                {
                    var pos = new Vector2Int(i, j);
                    var cell = GetCell(pos);
                    if (cell.StoneType == StoneType.None)
                    {
                        newStones.Add(pos, (StoneType)Random.Range(1, stoneTypesCount));
                    }
                }
            }

            return newStones;
        }
    }
}