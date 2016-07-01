using Microsoft.VisualStudio.TestTools.UnitTesting;
using FutoshikiSolver;

namespace FutoshikiSolverUnitTests
{
    [TestClass]
    public class SolvableGrid
    {
        [TestMethod]
        public void TestSolvableGrid1()
        {
            var grid = new FutoshikiGrid(5, "0050000030000000010000000", "      <   ><>  < < >", "<     << <<     <   ");            
            grid.Solve();
            Assert.IsTrue(grid.IsSolved);
        }

        [TestMethod]
        public void TestSolvableGrid2()
        {
            var grid = new FutoshikiGrid(5, "0004000000000000000000000", "<< >            > > ", "    >  <<    <     >");
            grid.Solve();
            Assert.IsTrue(grid.IsSolved);
        }

        [TestMethod]
        public void TestUnsolvableGrid1()
        {
            var grid = new FutoshikiGrid(5, "0000000000000000000000000", "      <   ><>  < < >", "<     << <<     <   ");
            grid.Solve();
            Assert.IsFalse(grid.IsSolved);
        }

    }
}
