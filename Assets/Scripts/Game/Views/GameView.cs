using System;
using UnityEngine;

namespace Game.Views
{
    public class GameView : MonoBehaviour
    {
        public event Action PlayButtonClicked;
        public event Action TimedOut;

        [SerializeField] private TmpTimer _tmpTimer;
        [SerializeField] private GUIStyle _menGUIStyle;
        private bool _showingMenu;

        public void ShowMenu(bool status)
        {
            _showingMenu = status;
        }

        public void StartTimer(int seconds)
        {
            _tmpTimer.StartTimer(seconds, OnTimerTimedOut);
        }

        private void OnTimerTimedOut()
        {
            TimedOut?.Invoke();
        }

        public void Stop()
        {
            _tmpTimer.Stop();
        }

        private void OnGUI()
        {
            if (!_showingMenu) return;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", _menGUIStyle);

            var buttonWidth = Screen.width / 10;
            var buttonHeight = Screen.height / 10;
            if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, Screen.height / 2 - buttonHeight / 2,
                    buttonWidth, buttonHeight), "Play"))
            {
                PlayButtonClicked?.Invoke();
            }
        }
    }
}