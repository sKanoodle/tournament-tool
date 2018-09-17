using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class MatchPairing
    {
        public PairingRow Contestant1;
        public PairingRow Contestant2;
    }

    class PairingRow
    {
        public string Contestant;
        public int ContestantPoints;
        public MatchPairing Parent;
    }
}
