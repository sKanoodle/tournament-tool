using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool.Tournaments
{
    class RoundRobinPerson : IPerson
    {
        public string Name { get; private set; }
        public int Points => _Matches.Sum(m => m.GetPointSum(this));
        public int PointsAgainst;

        public int SetWins => _Matches.Sum(m => m.GetSetWinSum(this));
        public int SetTies => _Matches.Sum(m => m.GetSetTieSum(this));
        public int SetLosses => _Matches.Sum(m => m.GetSetLossSum(this));

        public int MatchCount => MatchWins + MatchTies + MatchLosses;
        public int MatchWins => _Matches.Count(m => m.Result.Winner == this);
        public int MatchTies => _Matches.Count(m => m.Result.IsTied);
        public int MatchLosses => _Matches.Count(m => m.Result.Loser == this);

        public int RankingPoints;
        public int Position;

        private RoundRobinTournament Tournament;
        private IEnumerable<Match> _Matches => Tournament.Matches.Where(m => m.Contains(this));

        public RoundRobinPerson(string name, RoundRobinTournament tournament)
        {
            Name = name;
            Tournament = tournament;
        }
    }
}
