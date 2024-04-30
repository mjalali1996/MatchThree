using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using UnityEngine;

namespace Game.Views
{
    public delegate void TransitionDelegate(Vector2Int start, Vector2Int end);

    public interface IBoardView
    {
        public event TransitionDelegate MoveStoneRequested;
        public void CreateBoard(Board board);
        Task SwapStone(Vector2Int start, Vector2Int end);

        Task Explode(IReadOnlyList<Vector2Int> explosionCells,
            IReadOnlyDictionary<Vector2Int, Vector2Int> fallDownStones,
            IReadOnlyDictionary<Vector2Int, StoneType> newStones);

    }
}