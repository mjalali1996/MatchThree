using System;
using Game.Models;
using Game.Views;
using UnityEngine;
using Zenject;

namespace Game.Presenters
{
    public class BoardPresenter : MonoBehaviour
    {
        private BoardView _boardView;
        private LevelsContainer _levelsContainer;

        [Inject]
        private void Constructor(BoardView boardView, LevelsContainer levelsContainer)
        {
            _boardView = boardView;
            _levelsContainer = levelsContainer;
        }

        private void Start()
        {
            LoadLevel(0);
        }

        private void LoadLevel(int levelIndex)
        {
            var level = _levelsContainer.Levels[levelIndex];
            InitializeBoard(level.Board);
        }

        private void InitializeBoard(Board board)
        {
            _boardView.CreateBoard(board);
        }
    }
}