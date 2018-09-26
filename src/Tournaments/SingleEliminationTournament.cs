using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool.Tournaments
{
    class SingleEliminationTournament : ITournament
    {
        private Person[] Persons;

        public SingleEliminationTournament(string[] names)
        {
            Persons = names.Select(s => new Person(s)).ToArray();
        }

        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}
