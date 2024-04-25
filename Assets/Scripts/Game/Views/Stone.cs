using Common.Components;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Stone : MonoPoolable<Stone>
    {
        private SpriteRenderer _spriteRenderer;
        private IMemoryPool _pool;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}