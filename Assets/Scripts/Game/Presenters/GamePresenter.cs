using Game.Models;
using Game.Views;
using UnityEngine;
using Zenject;

namespace Game.Presenters
{
    public class GamePresenter : MonoBehaviour
    {
        private LevelsContainer _levelsContainer;
        private BoardPresenter _boardPresenter;
        private Level _level;
        private GameView _gameView;

        [Inject]
        private void Constructor(LevelsContainer levelsContainer, BoardPresenter boardPresenter, GameView gameView)
        {
            _levelsContainer = levelsContainer;
            _boardPresenter = boardPresenter;
            _gameView = gameView;
        }

        private void Start()
        {
            LoadLevel(0);
        }

        private void LoadLevel(int levelIndex)
        {
            _level = _levelsContainer.Levels[levelIndex];
            
            _boardPresenter.LoadBoard(_level.GetBoard());
            _gameView.StartTimer(_level.Time);
        }
    }
}