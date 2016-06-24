using System;

namespace FutoshikiSolver
{
    public class FutoshikiGrid
    {
        private int GridSize { get;}

        public int[,] Numbers { get; set; }

        public int[,] HorizontalSpacers { get; set; }

        public int[,] VerticalSpacers { get; set; }

        private bool HasValidData { get; set; }

        public FutoshikiGrid(int gridSize, string numbers, string horizontalSpacers, string verticalSpacers)
        {
            GridSize = gridSize;
            HasValidData = (SetNumbers(numbers)) && (SetHorizontalSpacers(horizontalSpacers) && (SetVerticalSpacers(verticalSpacers)));
        }

        public void Print()
        {
            if (!HasValidData)
            {
                Console.WriteLine("Can't print the grid the data is invalid.");
                return;
            }

            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    Console.Write(Numbers[r, c]);
                    if ((r < GridSize) && (c < (GridSize - 1)))
                    {
                        Console.Write(GetHorizontalSpacerCharFromValue(HorizontalSpacers[r, c]));
                    }
                }

                if (r < (GridSize - 1))
                {
                    Console.WriteLine();
                    for (int c = 0; c < GridSize; c++)
                    {
                        Console.Write(GetVerticalSpacerCharFromValue(VerticalSpacers[r, c]) + " ");
                    }
                }

                Console.WriteLine();
            }
        }

        public void Solve()
        {
            if (!HasValidData)
            {
                Console.WriteLine("Can't solve the grid the data is invalid.");
                return;
            }
        }

        #region Private Methods
        #region Methods For Setting Up The Grid
        private bool SetNumbers(string numbers)
        {
            if (numbers.Length != GridSize * GridSize)
            {                
                return false;
            }

            Numbers = new int[GridSize,GridSize];
            for (int i = 0; i < Numbers.Length; i++)
            {                
                Numbers[i / GridSize, i % GridSize] = int.Parse(numbers.Substring(i, 1)); //TODO: No checking that the numbers are valid for the grid or are even numbers.
            }
            return true;
        }

        private bool SetHorizontalSpacers(string horizontalSpacers)
        {
            if (horizontalSpacers.Length != (GridSize - 1) * GridSize)
            {
                return false;
            }

            HorizontalSpacers = new int[GridSize, GridSize - 1];
            for (int i = 0; i < horizontalSpacers.Length; i++)
            {                
                HorizontalSpacers[i / (GridSize - 1), i % (GridSize - 1)] = GetSpacerValueFromString(horizontalSpacers.Substring(i, 1));
            }
            return true;
        }

        private bool SetVerticalSpacers(string verticalSpacers)
        {
            if (verticalSpacers.Length != GridSize * (GridSize - 1))
            {
                return false;
            }

            VerticalSpacers = new int[GridSize - 1, GridSize];
            for (int i = 0; i < verticalSpacers.Length; i++)
            {
                VerticalSpacers[i % (GridSize -1), i / (GridSize - 1)] = GetSpacerValueFromString(verticalSpacers.Substring(i, 1));
            }
            return true;
        }
        #endregion       

        private int GetSpacerValueFromString(string spacerString)
        {
            int spacerValue;
            switch (spacerString)
            {
                case "<":
                    spacerValue = -1;
                    break;
                case ">":
                    spacerValue = 1;
                    break;
                default:
                    spacerValue = 0;
                    break;
            }
            return spacerValue;
        }

        private char GetHorizontalSpacerCharFromValue(int spacerValue)
        {
            char spacerChar;
            switch (spacerValue)
            {
                case -1:
                    spacerChar = '<';
                    break;
                case 1:
                    spacerChar = '>';
                    break;
                default:
                    spacerChar = '\u00B7';
                    break;
            }
            return spacerChar;
        }

        private char GetVerticalSpacerCharFromValue(int spacerValue)
        {
            char spacerChar;
            switch (spacerValue)
            {
                case -1:
                    spacerChar = '^';
                    break;
                case 1:
                    spacerChar = 'v';
                    break;
                default:
                    spacerChar = '\u00B7';
                    break;
            }
            return spacerChar;
        }
        #endregion
    }
}
