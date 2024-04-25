using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [CreateAssetMenu(fileName = "LevelsContainer", menuName = "ScriptableObjects/Levels Container")]
    public class LevelsContainer : ScriptableObject
    {
        [SerializeField] private List<Level> _levels;
        public IReadOnlyList<Level> Levels => _levels;
    }
}