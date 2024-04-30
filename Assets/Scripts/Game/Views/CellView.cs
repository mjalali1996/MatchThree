using System;
using Common.Components;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CellView : MonoPoolable<CellView>
    {
        public event EventHandler Clicked;
        public event EventHandler<Vector2> Dragged;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private IMemoryPool _pool;
        private Vector2 _pressingPosition;
        private bool _isPressed;
        [SerializeField] private float _draggingDetectionRange = 0.5f;

        private void Awake()
        {
            if (!_spriteRenderer)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            if (!sprite)
                _spriteRenderer.enabled = false;

            _spriteRenderer.sprite = sprite;
        }

        private void OnMouseDown()
        {
            _isPressed = true;
            _pressingPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            if (!_isPressed) return;
            _isPressed = false;
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distanceRange = Vector2.Distance(currentPosition, _pressingPosition);

            if (distanceRange < _draggingDetectionRange)
            {
                Clicked?.Invoke(this, EventArgs.Empty);
                return;
            }

            var dir = currentPosition - _pressingPosition;
            Dragged?.Invoke(this, dir.normalized);
        }

        public override void Dispose()
        {
            Clicked = null;
            Dragged = null;
            base.Dispose();
        }
    }
}