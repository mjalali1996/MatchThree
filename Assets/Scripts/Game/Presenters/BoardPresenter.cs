using System.Collections.Generic;
using Game.Models;
using Game.Views;
using UnityEngine;
using Zenject;

namespace Game.Presenters
{
    public class BoardPresenter : MonoBehaviour
    {
        private IBoardView _boardView;
        private LevelsContainer _levelsContainer;
        private Board _board;

        [Inject]
        private void Constructor(IBoardView boardView, LevelsContainer levelsContainer)
        {
            _boardView = boardView;
            _levelsContainer = levelsContainer;
        }

        private void Awake()
        {
            _boardView.MoveStoneRequested += OnMoveStoneRequested;
        }

        private void Start()
        {
            LoadLevel(0);
        }

        private void LoadLevel(int levelIndex)
        {
            var level = _levelsContainer.Levels[levelIndex];
            _board = new Board(level.Board);
            InitializeBoard(_board);
        }

        private void InitializeBoard(Board board)
        {
            _boardView.CreateBoard(board);
        }

        private async void OnMoveStoneRequested(Vector2Int start, Vector2Int end)
        {
            if (!_board.IsCellPosValid(start) || !_board.IsCellPosValid(end)) return;

            _board.SwapStones(start, end);
            await _boardView.SwapStone(start, end);

            var explosions = _board.GetExplosions();
            if (explosions.Count == 0)
            {
                _board.SwapStones(start, end);
                await _boardView.SwapStone(start, end);
            }
            else
            {
                var explosionCells = new List<Vector2Int>();
                foreach (var pair in explosions)
                {
                    explosionCells.AddRange(pair.Item2.explosionCells);
                }
            
                _board.Explode(explosionCells);
                await _boardView.Explode(explosionCells);
            }
        }
    }
}