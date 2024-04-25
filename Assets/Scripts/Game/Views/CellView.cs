using Common.Components;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CellView : MonoPoolable<CellView>
    {
        private SpriteRenderer _spriteRenderer;
        private IMemoryPool _pool;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            if (!sprite)
                _spriteRenderer.enabled = false;
            
            _spriteRenderer.sprite = sprite;
        }
    }
}