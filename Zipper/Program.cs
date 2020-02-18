using System;
using System.Threading;
using System.Threading.Tasks;
using Zipper.Terminal;

namespace Zipper
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.CursorVisible = false;
            int result = 1;

            TerminalHelp.PrintWelcome();

            Zipping zipping = new Zipping();
            result = zipping.Run(args);
            zipping.Dispose();

            Console.CursorVisible = true;

            return result;
        }
    }
}
