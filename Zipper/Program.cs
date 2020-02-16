﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Zipper.Terminal;

namespace Zipper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            TerminalHelp.PrintWelcome();

            Zipping zipping = new Zipping();
            zipping.Run(args);
            zipping.Dispose();

            Console.CursorVisible = true;
        }
    }
}
