﻿using System;

namespace TournamentTool
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer();
            server.RunServerAsync();
            Console.Read();
        }
    }
}
