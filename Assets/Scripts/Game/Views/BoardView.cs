using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Containers;
using DG.Tweening;
using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        public event TransitionDelegate MoveStoneRequested;

        private readonly Dictionary<Vector2Int, CellView> _cellViewsForward = new();
        private readonly Dictionary<CellView, Vector2Int> _cellViewsBackward = new();
        private readonly Dictionary<Vector2Int, StoneView> _stoneViews = new();
        private readonly List<Vector2Int> _selectedStones = new();

        [SerializeField] private float _rowSpace;
        [SerializeField] private float _columnSpace;
        [SerializeField] private Transform _cellsContainer;
        [SerializeField] private Transform _stonesContainer;
        private Board _board;
        private CellView.Factory _cellViewFactory;
        private StoneView.Factory _stoneFactory;
        private SpriteContainer _stoneSpriteContainer;


        [Inject]
        private void Constructor(CellView.Factory cellFactory, StoneView.Factory stoneFactory,
            SpriteContainer stoneSpriteContainer)
        {
            _cellViewFactory = cellFactory;
            _stoneFactory = stoneFactory;
            _stoneSpriteContainer = stoneSpriteContainer;
        }

        public void CreateBoard(Board board)
        {
            _board = board;
            ClearAllViews();
            for (var i = 0; i < board.Rows.Count; i++)
            {
                var row = board.Rows[i];
                for (var j = 0; j < row.Columns.Count; j++)
                {
                    var cell = row.Columns[j];
                    var pos = new Vector2Int(i, j);

                    var cellView = CreateCell(pos);
                    _cellViewsForward.Add(pos, cellView);
                    _cellViewsBackward.Add(cellView, pos);
                    ListenToCellView(cellView);

                    var stoneView = CreateStoneView(pos, cell.StoneType);
                    _stoneViews.Add(pos, stoneView);
                }
            }
        }

        private CellView CreateCell(Vector2Int pos)
        {
            var cellView = _cellViewFactory.Create();
            cellView.SetSprite(null);

            cellView.transform.SetParent(_cellsContainer);
            SetLocalPositionByIndex(cellView.transform, pos);
            return cellView;
        }

        private void ListenToCellView(CellView cellView)
        {
            cellView.Clicked += CellViewOnClicked;
            cellView.Dragged += CellViewOnDragged;
        }


        private StoneView CreateStoneView(Vector2Int pos, StoneType stone)
        {
            var stoneView = _stoneFactory.Create();
            var stoneSprite = _stoneSpriteContainer.GetSprite((int)stone);
            stoneView.SetSprite(stoneSprite);

            stoneView.transform.SetParent(_stonesContainer);
            SetLocalPositionByIndex(stoneView.transform, pos);
            return stoneView;
        }

        private void SetLocalPositionByIndex(Transform trans, Vector2Int pos)
        {
            trans.localPosition = ConvertIndexesToPosition(pos);
        }

        private Vector2 ConvertIndexesToPosition(Vector2Int pos)
        {
            return new Vector2(pos.x * _rowSpace, pos.y * _columnSpace);
        }

        public async Task SwapStone(Vector2Int start, Vector2Int end)
        {
            var stone1 = GetStoneView(start);
            var stone2 = GetStoneView(end);

            SetStoneView(end, stone1);
            SetStoneView(start, stone2);

            var seq = DOTween.Sequence();
            seq.Join(MoveTo(stone1.transform, end));
            seq.Join(MoveTo(stone2.transform, start));
            await seq.AsyncWaitForCompletion();
        }

        public async Task Explode(IReadOnlyList<Vector2Int> explosionCells,
            IReadOnlyDictionary<Vector2Int, Vector2Int> fallDownStones,
            IReadOnlyDictionary<Vector2Int, StoneType> newStones)
        {
            await Explode(explosionCells);
            await FillBoard(fallDownStones, newStones);
        }

        private async Task Explode(IReadOnlyList<Vector2Int> explosionCells)
        {
            var seq = DOTween.Sequence();
            foreach (var explosionCellPos in explosionCells)
            {
                var stoneView = GetStoneView(explosionCellPos);
                seq.Join(stoneView.transform.DOScale(0, 0.3f).OnComplete(stoneView.Dispose));
            }

            await seq.AsyncWaitForCompletion();
        }

        public async Task FillBoard(IReadOnlyDictionary<Vector2Int, Vector2Int> fallDownStones,
            IReadOnlyDictionary<Vector2Int, StoneType> newStones)
        {
            var seq = DOTween.Sequence();
            foreach (var fallDownStone in fallDownStones)
            {
                var stone = GetStoneView(fallDownStone.Key);

                SetStoneView(fallDownStone.Value, stone);
                seq.Join(MoveTo(stone.transform, fallDownStone.Value));
            }

            var newStonesInPerColumn = new Dictionary<int, int>();
            foreach (var newStone in newStones)
            {
                newStonesInPerColumn.TryAdd(newStone.Key.x, 0);
                newStonesInPerColumn[newStone.Key.x] += 1;
            }

            foreach (var newStone in newStones)
            {
                var yStonePos = newStone.Key.y - newStonesInPerColumn[newStone.Key.x] - 1;
                var stone = CreateStoneView(new Vector2Int(newStone.Key.x, yStonePos), newStone.Value);

                SetStoneView(newStone.Key, stone);
                seq.Join(MoveTo(stone.transform, newStone.Key));
            }

            await seq.AsyncWaitForCompletion();
        }

        private Tween MoveTo(Transform trans, Vector2Int pos)
        {
            return trans.DOLocalMove(ConvertIndexesToPosition(pos), 0.3f);
        }

        private StoneView GetStoneView(Vector2Int pos)
        {
            return _stoneViews[pos];
        }

        private void SetStoneView(Vector2Int pos, StoneView stoneView)
        {
            _stoneViews[pos] = stoneView;
        }

        private void ClearAllViews()
        {
            foreach (var cellView in _cellViewsForward.Values)
            {
                cellView.Dispose();
            }

            foreach (var stoneView in _stoneViews.Values)
            {
                stoneView.Dispose();
            }

            _cellViewsForward.Clear();
            _cellViewsBackward.Clear();
            _stoneViews.Clear();
        }

        private static Vector2Int FixViewDirection(Vector2Int dir, Vector2Int boardAnchor)
        {
            return dir * boardAnchor;
        }

        #region CellEvents

        private void CellViewOnClicked(object sender, EventArgs e)
        {
            if (sender is not CellView cellView) return;

            var cellPos = _cellViewsBackward[cellView];
            if (_selectedStones.Contains(cellPos))
            {
                _selectedStones.Remove(cellPos);
                _stoneViews[cellPos].SetSelected(false);
                return;
            }

            _selectedStones.Add(cellPos);
            _stoneViews[cellPos].SetSelected(true);
            CheckForSwapSelectedCells();
        }

        private void CheckForSwapSelectedCells()
        {
            if (_selectedStones.Count <= 1) return;
            var stone1Pos = _selectedStones[0];
            var stone2Pos = _selectedStones[1];
            DeselectAllCells();

            if (stone1Pos.x == stone2Pos.x && Mathf.Abs(stone1Pos.y - stone2Pos.y) == 1)
            {
                MoveStoneRequested?.Invoke(stone1Pos, stone2Pos);
            }
            
            if (stone1Pos.y == stone2Pos.y && Mathf.Abs(stone1Pos.x - stone2Pos.x) == 1)
            {
                MoveStoneRequested?.Invoke(stone1Pos, stone2Pos);
            }
        }

        private void DeselectAllCells()
        {
            foreach (var selectedStone in _selectedStones)
            {
                _stoneViews[selectedStone].SetSelected(false);
            }

            _selectedStones.Clear();
        }

        private void CellViewOnDragged(object sender, Vector2 dir)
        {
            if (sender is not CellView cellView) return;
            DeselectAllCells();
            
            var cellViewPos = _cellViewsBackward[cellView];
            var direction = Board.GetDirection(dir);
            var fixViewDirection = FixViewDirection(Board.DirectionToVector[direction], new Vector2Int(1, -1));
            var nextCellPos = Board.GetNextCellByDirection(cellViewPos, fixViewDirection);

            MoveStoneRequested?.Invoke(cellViewPos, nextCellPos);
        }

        #endregion
    }
}