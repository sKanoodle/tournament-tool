using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTool
{
    class Person : IPerson
    {
        public string Name { get; private set; }

        public Person(string name)
        {
            Name = name;
        }
    }
}
