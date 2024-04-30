using UnityEngine;

namespace Game.Views
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private TmpTimer _tmpTimer;

        public void StartTimer(int seconds)
        {
            _tmpTimer.StartTimer(seconds);
        }
        
        public void Stop()
        {
            _tmpTimer.Stop();
        }
    }
}