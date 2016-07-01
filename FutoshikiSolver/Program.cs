using System;

namespace FutoshikiSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var grid = new FutoshikiGrid(5, "0050000030000000010000000", "      <   ><>  < < >", "<     << <<     <   ");
            var grid = new FutoshikiGrid(5, "0004000000000000000000000", "<< >            > > ", "    >  <<    <     >");
            grid.Print();
            grid.Solve();

            Console.WriteLine("After");
            grid.Print();

            grid.PrintLog();

            Console.ReadKey();
        }
    }
}
