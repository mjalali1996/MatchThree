using System.Collections.Generic;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/Sprite Container")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private List<Sprite> _sprites;

        public Sprite GetSprite(int id)
        {
            if (_sprites.Count < id) return null;
            return _sprites[id];
        }
    }
}