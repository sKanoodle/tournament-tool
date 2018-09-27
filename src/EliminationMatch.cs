using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class EliminationMatch : Match
    {
        private readonly Func<Match, IPerson> GetContestant1;
        private readonly Func<Match, IPerson> GetContestant2;
        private readonly Match PreviousMatch1;
        private readonly Match PreviousMatch2;

        public override IPerson Contestant1 => GetContestant1(PreviousMatch1);
        public override IPerson Contestant2 => GetContestant2(PreviousMatch2);

        /// <summary>
        /// Elimination match which takes its contestants from previous matches.
        /// </summary>
        /// <param name="getContestant">default is to take the winner</param>
        /// <param name="getContestant2">default is to take whatever is defined for getContestant</param>
        public EliminationMatch(Match previousMatch1, Match previousMatch2, int setCount, Func<Match, IPerson> getContestant = null, Func<Match, IPerson> getContestant2 = null)
            : base(setCount)
        {
            PreviousMatch1 = previousMatch1;
            PreviousMatch2 = previousMatch2;

            GetContestant1 = getContestant ?? (m => m.Result.Winner);
            GetContestant1 = getContestant2 ?? GetContestant1;
        }
    }
}
