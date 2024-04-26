using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public partial class Board
    {
        private static readonly Dictionary<Vector2Int, Direction> _vectorToDirection = new()
        {
            { new Vector2Int(0, 1), Direction.Up },
            { new Vector2Int(1, 0), Direction.Right },
            { new Vector2Int(0, -1), Direction.Down },
            { new Vector2Int(-1, 0), Direction.Left },
        };

        public static IReadOnlyDictionary<Vector2Int, Direction> VectorToDirection => _vectorToDirection;

        private static readonly Dictionary<Direction, Vector2Int> _directionToVector = new()
        {
            { Direction.Up, new Vector2Int(0, 1) },
            { Direction.Right, new Vector2Int(1, 0) },
            { Direction.Down, new Vector2Int(0, -1) },
            { Direction.Left, new Vector2Int(-1, 0) },
        };

        public static IReadOnlyDictionary<Direction, Vector2Int> DirectionToVector => _directionToVector;

        public static Direction GetDirection(Vector2 dir)
        {
            var dirSign = new Vector2Int(dir.x > 0 ? 1 : -1, dir.y > 0 ? 1 : -1);

            var absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            Vector2Int vectorDir;
            if (absDir.x > absDir.y)
            {
                absDir.x = 1;
                absDir.y = 0;
                vectorDir = new Vector2Int(1, 0) * dirSign;
            }
            else
            {
                absDir.x = 0;
                absDir.y = 1;
                vectorDir = new Vector2Int(0, 1) * dirSign;
            }

            return VectorToDirection[vectorDir];
        }

        public static Vector2Int GetNextCellPosByDirection(Vector2Int startPos, Direction dir)
        {
            return startPos + DirectionToVector[dir];
        }

        public static Vector2Int GetNextCellByDirection(Vector2Int startPos, Vector2Int dir)
        {
            return startPos + dir;
        }
        
        public static bool IsCellPosValid(Vector2Int pos, int rowsCount, int columnsCount)
        {
            if (pos.x < 0 || pos.x >= rowsCount) return false;
            if (pos.y < 0 || pos.y >= columnsCount) return false;
            return true;
        }
    }
}