using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool.Tournaments
{
    class SingleEliminationTournament : ITournament
    {
        protected readonly Person[] People;
        protected readonly Match[][] Matches;

        public SingleEliminationTournament(string[] names, int setCount)
        {
            People = names.Select(s => new Person(s)).ToArray();
            int maxCount = (int)Math.Pow(2, Math.Ceiling(Math.Log(names.Length, 2)));
            Match[] seeding = new Match[maxCount / 2];
            for (int i = 0; i < seeding.Length; i++)
            {
                seeding[i] = new Match(tryGetValue(People, i * 2), tryGetValue(People, i * 2 + 1), setCount);
            }

            IPerson tryGetValue(IPerson[] people, int index)
            {
                if (index >= 0 && index < people.Length)
                    return people[index];
                return new EmptyPerson();
            }

            Matches = CreateBrackets(seeding);
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

            result = new Match[roundMatchCounts.Count][];
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
                    sb.Append(match.RenderElimination());
                }
                sb.Append("</div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
