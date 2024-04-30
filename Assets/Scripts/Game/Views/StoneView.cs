using Common.Components;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class StoneView : MonoPoolable<StoneView>
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _selectedSpriteRenderer;
        private IMemoryPool _pool;

        private void Awake()
        {
            if (!_spriteRenderer)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
        
        public void SetSelected(bool selected)
        {
            _selectedSpriteRenderer.enabled = selected;
        }

        public override void Dispose()
        {
            base.Dispose();
            transform.localScale = Vector3.one;
        }
    }
}