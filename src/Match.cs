using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool
{
    class Match
    {
        public IPerson Contestant1;
        public IPerson Contestant2;
        public Set[] Sets;

        public Match(IPerson person1, IPerson person2, int setCount)
        {
            Contestant1 = person1;
            Contestant2 = person2;
            Sets = Enumerable.Range(0, setCount).Select(_ => new Set(this)).ToArray();
        }

        public bool Contains(IPerson person) => person == Contestant1 || person == Contestant2;

        public int GetPointSum(IPerson person) => GetPointSum(person, false);
        public int GetOpponentPointSum(IPerson person) => GetPointSum(person, true);
        private int GetPointSum(IPerson person, bool getOtherPerson)
        {
            var pers1match = person == Contestant1;
            var pers2match = person == Contestant2;
            if (pers1match || getOtherPerson && pers2match)
                return Sets.Sum(s => s.Points1.Value);
            if (pers2match || getOtherPerson && pers1match)
                return Sets.Sum(s => s.Points2.Value);
            return 0;
        }

        public ClashResult Result
        {
            get
            {
                ClashResult result = new ClashResult();
                int wins1 = 0;
                int wins2 = 0;

                foreach (Set set in Sets)
                {
                    ClashResult cachedResult = set.Result;
                    if (cachedResult.IsTied)
                    {
                        wins1 += 1;
                        wins2 += 1;
                    }
                    else if (cachedResult.Winner == Contestant1)
                        wins1 += 1;
                    else if (cachedResult.Winner == Contestant2)
                        wins2 += 1;
                }

                return ClashResult.FromPoints(Contestant1, Contestant2, wins1, wins2);
            }
        }

        public int GetSetWinSum(IPerson person) => Sets.Count(s => s.Result.Winner == person);
        public int GetSetTieSum(IPerson person) => Sets.Count(s => s.Result.IsTied);
        public int GetSetLossSum(IPerson person) => Sets.Count(s => s.Result.Loser == person);

        public string RenderResult(IPerson first) => throw new NotImplementedException();

        public virtual string RenderNeutral()
        {
            return $@"
<div class=""table-cell"">
    <div class=""inline cell-name-left"">{Contestant1.Name}</div>
    <div class=""inline"">{String.Join("", Sets.Select(s => $"<div>{s.Points1.Render()} : {s.Points2.Render()}</div>"))}</div>
    <div class=""inline cell-name"">{Contestant2.Name}</div>
</div>";
        }
    }
}
