using System.Collections.Generic;
using System.Threading.Tasks;
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
        private bool _isBoardInProcess = false;

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
            _board = level.GetBoard();
            InitializeBoard(_board);
        }

        private void InitializeBoard(Board board)
        {
            _boardView.CreateBoard(board);
        }

        private async void OnMoveStoneRequested(Vector2Int start, Vector2Int end)
        {
            if (_isBoardInProcess || !_board.IsCellPosValid(start) || !_board.IsCellPosValid(end)) return;
            _isBoardInProcess = true;
            
            await SwapStones(start, end);

            var explosions = _board.GetExplosions();
            if (explosions.Count == 0)
            {
                // swap back
                await SwapStones(start, end);
            }
            else
            {
                await Explode(explosions);
                await TryExplodeAutoNewBoard();
            }
            _isBoardInProcess = false;
        }

        private async Task TryExplodeAutoNewBoard()
        {
            var explosions = _board.GetExplosions();
            if (explosions.Count != 0)
            {
                await Explode(explosions);
                await TryExplodeAutoNewBoard();
            }
        }

        private async Task Explode(
            List<(MatchPatternType, (Vector2Int pos, List<Vector2Int> explosionCells))> explosions)
        {
            var explosionCells = new List<Vector2Int>();
            foreach (var pair in explosions)
            {
                explosionCells.AddRange(pair.Item2.explosionCells);
            }

            _board.Explode(explosionCells);

            var fallDownStones = _board.GetFallDownStones();
            foreach (var fallDownStone in fallDownStones)
            {
                _board.SwapStones(fallDownStone.Key, fallDownStone.Value);
            }

            var newStones = _board.GetNewStones();
            foreach (var fallDownStone in newStones)
            {
                _board.SetStone(fallDownStone.Key, fallDownStone.Value);
            }

            await _boardView.Explode(explosionCells, fallDownStones, newStones);
        }

        private async Task SwapStones(Vector2Int start, Vector2Int end)
        {
            _board.SwapStones(start, end);
            await _boardView.SwapStone(start, end);
        }
    }
}