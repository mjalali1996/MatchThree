using System;
using System.Collections.Generic;

namespace Game.Models
{
    [Serializable]
    public class BoardCell
    {
        public StoneType StoneType;

        public Dictionary<Direction, int> Counter = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 0 },
            { Direction.Down, 0 },
            { Direction.Left, 0 },
        };

        public MatchPatternType GetMatchPattern(out bool inRow)
        {
            var sameStoneAtRight = Counter[Direction.Right];
            var sameStoneAtUp = Counter[Direction.Up];
            var sameStoneAtDown = Counter[Direction.Down];
            var sameStoneAtLeft = Counter[Direction.Left];

            // Plus pattern detection
            if (sameStoneAtRight >= 1 && sameStoneAtLeft >= 1 && sameStoneAtUp >= 1 && sameStoneAtDown >= 1)
            {
                inRow = true;
                return MatchPatternType.Plus;
            }

            var inRowCount = sameStoneAtRight + 1;
            var inColumnCount = sameStoneAtUp + 1;
            // Five pattern detection
            if (inRowCount >= 5)
            {
                inRow = true;
                return MatchPatternType.Five;
            }

            if (inColumnCount >= 5)
            {
                inRow = false;
                return MatchPatternType.Five;
            }

            #region SetSquarePatterns

            //Right-Up
            if (sameStoneAtRight >= 2 && sameStoneAtUp >= 2)
            {
                inRow = true;
                return MatchPatternType.SetSquare;
            }

            //Right-Down
            if (sameStoneAtRight >= 2 && sameStoneAtDown >= 2)
            {
                inRow = true;
                return MatchPatternType.SetSquare;
            }

            //Left-Up
            if (sameStoneAtLeft >= 2 && sameStoneAtUp >= 2)
            {
                inRow = true;
                return MatchPatternType.SetSquare;
            }

            //Left-Down
            if (sameStoneAtLeft >= 2 && sameStoneAtDown >= 2)
            {
                inRow = true;
                return MatchPatternType.SetSquare;
            }

            #endregion

            // Other pattern detection
            if (inRowCount >= 4)
            {
                inRow = true;
                return MatchPatternType.Four;
            }

            if (inColumnCount >= 4)
            {
                inRow = false;
                return MatchPatternType.Four;
            }

            if (inRowCount >= 3)
            {
                inRow = true;
                return MatchPatternType.Three;
            }

            if (inColumnCount >= 3)
            {
                inRow = false;
                return MatchPatternType.Three;
            }

            inRow = true;
            return MatchPatternType.None;
        }
    }
}