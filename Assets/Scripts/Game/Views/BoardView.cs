using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private float _rowSpace;
        [SerializeField] private float _columnSpace;
        [SerializeField] private Transform _cellsContainer;
        private CellView.Factory _cellViewFactory;
        private Stone.Factory _stoneFactory;

        private readonly List<List<CellView>> _cellViews = new();

        [Inject]
        private void Constructor(CellView.Factory cellFactory, Stone.Factory stoneFactory)
        {
            _cellViewFactory = cellFactory;
            _stoneFactory = stoneFactory;
        }


        public void CreateBoard(Board board)
        {
            DisposeAllCells();

            _cellViews.Clear();
            for (var i = 0; i < board.Rows.Count; i++)
            {
                var row = board.Rows[i];
                _cellViews.Add(new List<CellView>());
                for (var j = 0; j < row.Columns.Count; j++)
                {
                    var cell = row.Columns[j];

                    CreateCell(i, j);
                    CreateStone(_cellViews[i][j], cell.StoneType);
                }
            }
        }

        private void CreateCell(int row, int column)
        {
            var cellView = _cellViewFactory.Create();
            cellView.transform.SetParent(_cellsContainer);
            cellView.transform.localPosition = new Vector2(row * _rowSpace, column * _columnSpace);
            cellView.SetSprite(null);
            _cellViews[row].Add(cellView);
        }

        private void CreateStone(CellView row, StoneType stone)
        {
            var stoneView = _stoneFactory.Create();
            stoneView.transform.SetParent(_cellsContainer);
            stoneView.transform.localPosition = Vector3.zero;
            stoneView.SetSprite(null);
        }

        private void DisposeAllCells()
        {
            foreach (var rows in _cellViews)
            {
                foreach (var cellView in rows)
                {
                    cellView.Dispose();
                }
            }
        }
    }
}