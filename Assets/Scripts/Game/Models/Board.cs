using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class Board
    {
        [ReadOnly] private int _rowsCount;
        public int RowsCount => _rowsCount;

        [ReadOnly] private int _columnsCount;
        public int ColumnsCount => _columnsCount;

        [SerializeField] private List<BoardRow> _rows = new();
        public IReadOnlyList<BoardRow> Rows => _rows;

        public void CreateBoard(int rows, int columns)
        {
            _rowsCount = rows;
            _columnsCount = columns;

            _rows.Clear();
            for (var i = 0; i < _rowsCount; i++)
            {
                var boardRow = new BoardRow();
                for (int j = 0; j < _columnsCount; j++)
                {
                    boardRow.Columns.Add(new BoardCell());
                }

                _rows.Add(boardRow);
            }
        }
    }
}