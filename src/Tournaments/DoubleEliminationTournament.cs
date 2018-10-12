using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool.Tournaments
{
    class DoubleEliminationTournament : SingleEliminationTournament
    {
        protected readonly Match[][] LosersBracket;

        public DoubleEliminationTournament(string[] names, int setCount)
            : base(names, setCount)
        {
            LosersBracket = new Match[Matches.Length + 1][];
            LosersBracket[0] = new Match[Matches[0].Length / 2];
            for (int i = 0; i < LosersBracket[0].Length; i++)
            {
                LosersBracket[0][i] = new EliminationMatch(Matches[0][i * 2], Matches[0][i * 2 + 1], setCount, m => m.Result.Loser);
            }

            for (int round = 1; round < LosersBracket.Length; round++)
            {
                LosersBracket[round] = new Match[Matches[round].Length];
            }
        }

        public override string Render()
        {
            return base.Render() + RenderMatchTree(LosersBracket);
        }
    }
}
