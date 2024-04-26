using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public partial class Board
    {
        public int RowsCount => _rows.Count;
        public int ColumnsCount => _rows.Count == 0 ? 0 : _rows[0].Columns.Count;

        [SerializeField] private List<BoardRow> _rows = new();
        public IReadOnlyList<BoardRow> Rows => _rows;

        public Board()
        {
        }

        public Board(Board board)
        {
            _rows = new List<BoardRow>(board.Rows);
        }

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

        private void SetStone(Vector2Int pos, StoneType stone)
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

        public void Explode(List<Vector2Int> explosionCells)
        {
            foreach (var explosionCellPos in explosionCells)
            {
                SetStone(explosionCellPos, StoneType.None);
            }
        }
    }
}