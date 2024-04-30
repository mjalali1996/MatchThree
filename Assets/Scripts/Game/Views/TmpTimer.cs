using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Game.Views
{
    public class TmpTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _timerTmp;
        private CancellationTokenSource _cancellationToken;

        public void StartTimer(int seconds)
        {
            Stop();
            _cancellationToken = new CancellationTokenSource();
            CountDown(seconds, new CancellationToken()).Forget();
        }

        private async UniTask CountDown(int seconds, CancellationToken ct)
        {
            while (seconds >= 0)
            {
                SetTimerText(seconds.ToString());
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                seconds -= 1;
            }
        }

        private void SetTimerText(string seconds)
        {
            _timerTmp.text = seconds;
        }

        public void Stop()
        {
            if (_cancellationToken is null || _cancellationToken.IsCancellationRequested) return;
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
            SetTimerText("");
        }
    }
}