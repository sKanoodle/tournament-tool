using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class Set
    {
        public Match Match { get; private set; }
        public Point Points1 { get; } = new Point();
        public Point Points2 { get; } = new Point();
        public ClashResult Result => ClashResult.FromPoints(Match.Contestant1, Match.Contestant2, Points1.Value, Points2.Value);

        public Set(Match match)
        {
            Match = match;
        }
    }

    class Point : InputObject
    {
        public int Value
        {
            get
            {
                if (int.TryParse(RawValue, out int result))
                    return result;
                return 0;
            }
        }

        public string Render()
        {
            return $@"<input class=""points"" {DefaultAttributesAutoSubmit} type=""number"" step=""1"" min=""0"" />";
        }
    }
}
