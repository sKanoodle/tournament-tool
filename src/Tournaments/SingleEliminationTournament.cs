using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool.Tournaments
{
    class SingleEliminationTournament : ITournament
    {
        protected readonly Person[] Persons;
        protected readonly Match[][] Matches;

        public SingleEliminationTournament(string[] names)
        {
            Persons = names.Select(s => new Person(s)).ToArray();
            Math.Log(1023, 2);
            Matches = CreateBrackets();
        }

        /// <summary>
        /// Create matches following the first seeding match in an elimination tournament. Count of seeding matches has to be 2^n.
        /// </summary>
        protected Match[][] CreateBrackets(Match[] seedings)
        {
            Match[][] result;
            List<int> roundMatchCounts = new List<int>();
            roundMatchCounts.Add(1);

            int maxMatchCount = 1;
            while (seedings.Length > maxMatchCount)
            {
                maxMatchCount *= 2;
                roundMatchCounts.Add(maxMatchCount);
            }
            roundMatchCounts.Reverse();

            if (seedings.Length != maxMatchCount)
                throw new ArgumentException("count of seeding matches passed in is not 2^n.");

            result = new Match[roundMatchCounts.Count + 1][];
            result[0] = seedings;

            for (int round = 1; round < result.Length; round++)
            {
                result[round] = new Match[roundMatchCounts[round]];
                for (int i = 0; i < roundMatchCounts[round]; i++)
                    result[round][i] = new EliminationMatch(result[round - 1][i * 2], result[round - 1][i * 2 + 1], 3);
            }

            return result;
        }

        public virtual string Render()
        {
            StringBuilder sb = new StringBuilder("<div>");

            foreach (Match[] round in Matches)
            {
                sb.Append($@"<div class=""inline"">");
                foreach (Match match in round)
                {
                    RenderMatch(match);
                }
                sb.Append("</div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        private string RenderMatch(Match match)
        {
            return $@"<div>{RenderMatchPart(match, m => m.Contestant1, s => s.Points1)}{RenderMatchPart(match, m => m.Contestant2, s => s.Points2)}</div>";
        }

        private string RenderMatchPart(Match match, Func<Match, IPerson> getPerson, Func<Set, Point> getPoint)
        {
            return $@"<div class=""{(getPerson(match) is null ? "hidden" : String.Empty)}"">
    <div class=""cell-name-left inline"">{getPerson(match)?.Name}</div>
    {String.Join("", match.Sets.Select(s => getPoint(s).Render()))}
</div>";
        }
    }
}
