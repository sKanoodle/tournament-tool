using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class ClashResult
    {
        public bool IsTied { get; private set; }
        public IPerson Winner { get; private set; }
        public IPerson Loser { get; private set; }

        public static ClashResult FromPoints(IPerson person1, IPerson person2, int points1, int points2)
        {
            if (points1 < 0 || points2 < 0)
                return null;
            ClashResult result = new ClashResult();
            if (points1 == points2)
                result.IsTied = true;
            else if (points1 > points2)
            {
                result.Winner = person1;
                result.Loser = person2;
            }
            else if (points2 > points1)
            {
                result.Winner = person2;
                result.Loser = person1;
            }
            else
                throw new ArgumentException();
            return result;
        }
    }
}
