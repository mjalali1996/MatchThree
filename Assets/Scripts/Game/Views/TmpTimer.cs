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

        public async void StartTimer(int seconds, Action timedOut)
        {
            Stop();
            _cancellationToken = new CancellationTokenSource();
            await CountDown(seconds, timedOut, _cancellationToken.Token);
        }

        private async UniTask CountDown(int seconds, Action timedOut, CancellationToken ct)
        {
            while (seconds >= 0)
            {
                SetTimerText(seconds.ToString());
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                seconds -= 1;
            }

            timedOut?.Invoke();
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