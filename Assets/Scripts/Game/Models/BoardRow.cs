using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class BoardRow
    {
        [SerializeField] private List<BoardCell> _columns = new();
        public List<BoardCell> Columns => _columns;
    }
}