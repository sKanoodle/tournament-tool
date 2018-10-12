using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class OptionalSecondFinalMatch : Match
    {
        private readonly Match PreviousMatch;

        public override IPerson Contestant1
        {
            get
            {
                if (PreviousMatch.Result.Loser == PreviousMatch.Contestant1)
                    return PreviousMatch.Contestant1;
                return null;
            }
        }

        public override IPerson Contestant2
        {
            get
            {
                if (Contestant1 is null)
                    return null;
                return PreviousMatch.Contestant2;
            }
        }

        /// <summary>
        /// Elimination match which takes its contestants from previous matches.
        /// </summary>
        /// <param name="getContestant">default is to take the winner</param>
        /// <param name="getContestant2">default is to take whatever is defined for getContestant</param>
        public OptionalSecondFinalMatch(Match previousMatch, int setCount, Func<Match, IPerson> getContestant = null, Func<Match, IPerson> getContestant2 = null)
            : base(setCount)
        {
            PreviousMatch = previousMatch;
        }

        public override string RenderElimination()
        {
            if (Contestant1 is null)
                return null;
            return base.RenderElimination();
        }
    }
}
