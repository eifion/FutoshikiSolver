using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutoshikiSolver
{
    public class FutoshikiGrid
    {
        public bool IsSolved => !HasEmptyCells;

        private int GridSize { get;}

        private int MaxIndex => GridSize - 1;

        private int[,] Numbers { get; set; }

        private bool HasEmptyCells
        {
            get
            {
                for (var row = 0; row < GridSize; row++)
                {
                    for (var col = 0; col < GridSize; col++)
                    {
                        if (CellIsEmpty(row, col))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private int[,] HorizontalInequalities { get; set; }

        private int[,] VerticalInequalities { get; set; }

        private bool HasValidData { get; }

        private int Attempts { get; set; }

        private StringBuilder Log { get; }

        public FutoshikiGrid(int gridSize, string numbers, string horizontalInequalities, string verticalInequalities)
        {
            GridSize = gridSize;
            Log = new StringBuilder();
            HasValidData = (SetNumbers(numbers)) && (SetHorizontalInequalities(horizontalInequalities) && (SetVerticalInequalities(verticalInequalities)));
            Attempts = 0;
        }

        public void Print()
        {
            if (!HasValidData)
            {
                LogMessage("Can't print the grid the data is invalid.");
                return;
            }

            for (var row = 0; row < GridSize; row++)
            {
                for (var col = 0; col < GridSize; col++)
                {
                    Console.Write(Numbers[row, col]);
                    if ((row < GridSize) && (col < (GridSize - 1)))
                    {
                        Console.Write(GetHorizontalInequalityCharFromValue(HorizontalInequalities[row, col]));
                    }
                }

                if (row < (GridSize - 1))
                {
                    Console.WriteLine();
                    for (var c = 0; c < GridSize; c++)
                    {
                        Console.Write(GetVerticalInequalityCharFromValue(VerticalInequalities[row, c]) + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void Solve()
        {
            if (!HasValidData)
            {
                LogMessage("Can't solve the grid the data is invalid.");
                return;
            }
            
            while (HasEmptyCells && Attempts++ < 100) { 
                TryAddingMaximumNumberToEachRow();
                TryAddingMinimumNumberToEachRow();
                TrySolvingForEachRow();
                TrySolvingForEachColumn();
                TrySolvingForEachCell();
            }

            LogMessage(HasEmptyCells ? "Failed to solve puzzle." :  $"Solved puzzle in {Attempts} moves.");
        }

        public void PrintLog()
        {
            Console.WriteLine(Log);
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
            for (var i = 0; i < Numbers.Length; i++)
            {                
                Numbers[i / GridSize, i % GridSize] = int.Parse(numbers.Substring(i, 1)); //TODO: No checking that the numbers are valid for the grid or are even numbers.
            }
            return true;
        }

        private bool SetHorizontalInequalities(string horizontalInequalities)
        {
            if (horizontalInequalities.Length != (GridSize - 1) * GridSize)
            {
                return false;
            }

            HorizontalInequalities = new int[GridSize, GridSize - 1];
            for (var i = 0; i < horizontalInequalities.Length; i++)
            {                
                HorizontalInequalities[i / (GridSize - 1), i % (GridSize - 1)] = GetInequalityValueFromString(horizontalInequalities.Substring(i, 1));
            }
            return true;
        }

        private bool SetVerticalInequalities(string verticalInequalities)
        {
            if (verticalInequalities.Length != GridSize * (GridSize - 1))
            {
                return false;
            }

            VerticalInequalities = new int[GridSize - 1, GridSize];
            for (var i = 0; i < verticalInequalities.Length; i++)
            {                
                VerticalInequalities[(i / GridSize), (i % GridSize)] = GetInequalityValueFromString(verticalInequalities.Substring(i, 1));
            }
            return true;
        }
        #endregion       

        private int GetInequalityValueFromString(string inequalityString)
        {
            int inequalityValue;
            switch (inequalityString)
            {
                case "<":
                    inequalityValue = -1;
                    break;
                case ">":
                    inequalityValue = 1;
                    break;
                default:
                    inequalityValue = 0;
                    break;
            }
            return inequalityValue;
        }

        private char GetHorizontalInequalityCharFromValue(int inequalityValue)
        {
            char inequalityChar;
            switch (inequalityValue)
            {
                case -1:
                    inequalityChar = '<';
                    break;
                case 1:
                    inequalityChar = '>';
                    break;
                default:
                    inequalityChar = '\u00B7';
                    break;
            }
            return inequalityChar;
        }

        private char GetVerticalInequalityCharFromValue(int inequalityValue)
        {
            char inequalityChar;
            switch (inequalityValue)
            {
                case -1:
                    inequalityChar = '^';
                    break;
                case 1:
                    inequalityChar = 'v';
                    break;
                default:
                    inequalityChar = '\u00B7';
                    break;
            }
            return inequalityChar;
        }

        #region Solving Methods
        private void TryAddingMaximumNumberToEachRow()
        {
            for (var row = 0; row < GridSize; row++)
            {
                if (!RowHasNumber(row, GridSize))
                {
                    TryAddingMaximumNumberToRow(row);
                }
            }
        }

        private void TryAddingMinimumNumberToEachRow()
        {
            for (var row = 0; row < GridSize; row++)
            {
                if (!RowHasNumber(row, 1))
                {
                    TryAddingMinimumNumberToRow(row);
                }
            }
        }

        private void TryAddingMaximumNumberToRow(int row)
        {
            var potentialMatchingColumns = new List<int>();
            for (var col = 0; col < GridSize; col++)
            {
                var cellIsEmpty = CellIsEmpty(row, col);
                var noLessThansPointToCell = NoLessThansPointToCell(row, col);
                if (cellIsEmpty && noLessThansPointToCell && (RowHasMaximumNumber(row) == false) && (ColHasMaximumNumber(col) == false))
                {
                    potentialMatchingColumns.Add(col);
                }
            }
            if (potentialMatchingColumns.Count == 1)
            {
                SetCell(row, potentialMatchingColumns[0], GridSize);                
            }            
        }

        private void TryAddingMinimumNumberToRow(int row)
        {
            var potentialMatchingColumns = new List<int>();
            for (var col = 0; col < GridSize; col++)
            {
                var cellIsEmpty = CellIsEmpty(row, col);
                var noGreaterThansPointToCell = NoGreaterThansPointToCell(row, col);
                if (cellIsEmpty && noGreaterThansPointToCell && (RowHasMinimumNumber(row) == false) && (ColHasMinimumNumber(col) == false))
                {
                    potentialMatchingColumns.Add(col);
                }
            }
            if (potentialMatchingColumns.Count == 1)
            {
                SetCell(row, potentialMatchingColumns[0], 1);                
            }
        }

        private void TrySolvingForEachRow()
        {
            for (var row = 0; row < GridSize; row++)
            {
                TrySolvingForRow(row);
            }
        }

        private void TrySolvingForEachColumn()
        {
            for (var col = 0; col < GridSize; col++)
            {
                TrySolvingForColumn(col);
            }
        }

        private void TrySolvingForEachCell()
        {
            for (var row = 0; row < GridSize; row++)
            {
                for (var col = 0; col < GridSize; col++)
                {
                    if (!CellIsEmpty(row, col))
                    {
                        continue;
                    }

                    var potentialMatchesForCell = GetPotentialMatchesForCell(row, col);
                    if (potentialMatchesForCell.Count == 1)
                    {
                       SetCell(row, col, potentialMatchesForCell[0]);
                    }
                }
            }
        }

        private void TrySolvingForRow(int row)
        {
            for (var val = 1; val <= GridSize; val++)
            {
                var potentialMatchingColumns = new List<int>();
                for (var col = 0; col < GridSize; col++)
                {
                    if (GetPotentialMatchesForCell(row, col).Contains(val))
                    {
                        potentialMatchingColumns.Add(col);
                    }
                }
                if (potentialMatchingColumns.Count == 1)
                {
                    SetCell(row, potentialMatchingColumns[0], val);
                }
            }
        }

        private void TrySolvingForColumn(int col)
        {
            for (var val = 1; val <= GridSize; val++)
            {
                var potentialMatchingRows = new List<int>();
                for (var row = 0; row < GridSize; row++)
                {
                    if (GetPotentialMatchesForCell(row, col).Contains(val))
                    {
                        potentialMatchingRows.Add(row);
                    }
                }
                if (potentialMatchingRows.Count == 1)
                {
                    SetCell(potentialMatchingRows[0], col, val);
                }
            }
        }

        private List<int> GetPotentialMatchesForCell(int row, int col)
        {
            if (!CellIsEmpty(row, col))
            {
                return new List<int>();
            }

            var potentialMatches = Enumerable.Range(1, GridSize).ToList(); // These are all the values the cell could be.
            var potentialMatchesForCell = Enumerable.Range(1, GridSize).ToList(); // We'll try whittling these down until there's only one remaining.
            foreach (var potentialMatch in potentialMatches)
            {
                // If the row or column already has a value it can be removed from the list.
                if (RowHasNumber(row, potentialMatch) || ColHasNumber(col, potentialMatch))
                {
                    potentialMatchesForCell.Remove(potentialMatch);
                }
            }

            // If there are any inequalities pointing to the cell and a number on the other side then we can remove the ineligible values.
            if ((row > 0) && InequalityAboveIsLessThan(row, col))
            {
                if (CellIsEmpty(row - 1, col))
                {
                    RemoveMatchingLargerValuesFromColumn(col, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row - 1, col];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n < valueToCompare).ToList();
                }

            }

            if ((row > 0) && InequalityAboveIsGreaterThan(row, col))
            {
                if (CellIsEmpty(row - 1, col))
                {
                    RemoveMatchingSmallerValuesFromColumn(col, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row - 1, col];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n > valueToCompare).ToList();
                }
            }

            if ((row < MaxIndex) && InequalityBelowIsLessThan(row, col))
            {
                if (CellIsEmpty(row + 1, col))
                {
                    RemoveMatchingLargerValuesFromColumn(col, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row + 1, col];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n < valueToCompare).ToList();
                }
            }

            if ((row < MaxIndex) && InequalityBelowIsGreaterThan(row, col))
            {
                if (CellIsEmpty(row + 1, col))
                {
                    RemoveMatchingSmallerValuesFromColumn(col, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row + 1, col];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n > valueToCompare).ToList();
                }
            }

            if ((col > 0) && InequalityBeforeIsLessThan(row, col))
            {
                if (CellIsEmpty(row, col - 1))
                {
                    RemoveMatchingLargerValuesFromRow(row, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row, col - 1];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n < valueToCompare).ToList();
                }
            }

            if ((col > 0) && InequalityBeforeIsGreaterThan(row, col))
            {
                if (CellIsEmpty(row, col - 1))
                {
                    RemoveMatchingSmallerValuesFromRow(row, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row, col - 1];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n > valueToCompare).ToList();
                }
            }

            if ((col < MaxIndex) && InequalityAfterIsLessThan(row, col))
            {
                if (CellIsEmpty(row, col + 1))
                {
                    RemoveMatchingLargerValuesFromRow(row, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row, col + 1];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n < valueToCompare).ToList();
                }
            }
            if ((col < MaxIndex) && InequalityAfterIsGreaterThan(row, col))
            {
                if (CellIsEmpty(row, col + 1))
                {
                    RemoveMatchingSmallerValuesFromRow(row, potentialMatchesForCell);
                }
                else
                {
                    var valueToCompare = Numbers[row, col + 1];
                    potentialMatchesForCell = potentialMatchesForCell.Where(n => n > valueToCompare).ToList();
                }
            }
            return potentialMatchesForCell;
        }

        private void SetCell(int row, int col, int val)
        {
            LogMessage($"Attempt:{Attempts}. Setting [{row},{col}] to {val}.");
            Numbers[row, col] = val;
        }
        #endregion
        #region Misc methods

        private bool RowHasMinimumNumber(int row)
        {
            return RowHasNumber(row, 1);
        }

        private bool RowHasMaximumNumber(int row)
        {
            return RowHasNumber(row, GridSize);
        }

        private bool RowHasNumber(int row, int numberToMatch)
        {
            var match = false;
            for (var col = 0; col < GridSize; col++)
            {
                if (Numbers[row, col] == numberToMatch)
                {
                    match = true;
                }
            }
            return match;
        }

        private bool ColHasMinimumNumber(int col)
        {
            return ColHasNumber(col, 1);
        }

        private bool ColHasMaximumNumber(int col)
        {
            return ColHasNumber(col, GridSize);
        }

        private bool ColHasNumber(int col, int numberToMatch)
        {
            var match = false;
            for (var row = 0; row < GridSize; row++)
            {
                if (Numbers[row, col] == numberToMatch)
                {
                    match = true;
                }
            }
            return match;
        }

        private bool CellIsEmpty(int row, int col)
        {            
            return Numbers[row, col] == 0;
        }

        private bool NoLessThansPointToCell(int row, int col)
        {
            var above = InequalityAboveIsLessThan(row, col) == false;
            var below = InequalityBelowIsLessThan(row, col) == false;
            var before = InequalityBeforeIsLessThan(row, col) == false;
            var after = InequalityAfterIsLessThan(row, col) == false;
            return above && below && before && after;
        }

        private bool NoGreaterThansPointToCell(int row, int col)
        {
            var above = InequalityAboveIsGreaterThan(row, col) == false;
            var below = InequalityBelowIsGreaterThan(row, col) == false;
            var before = InequalityBeforeIsGreaterThan(row, col) == false;
            var after = InequalityAfterIsGreaterThan(row, col) == false;
            return above && below && before && after;
        }

        private bool InequalityAboveIsLessThan(int row, int col)
        {
            if (row == 0) return false;
            return VerticalInequalities[row - 1, col] == 1;
        }

        private bool InequalityBelowIsLessThan(int row, int col)
        {
            if (row > GridSize - 3) return false;
            return VerticalInequalities[row, col] == -1;
        }

        private bool InequalityBeforeIsLessThan(int row, int col)
        {
            if (col == 0) return false;
            return HorizontalInequalities[row, col - 1] == 1;
        }

        private bool InequalityAfterIsLessThan(int row, int col)
        {
            if (col > GridSize - 3) return false;
            return HorizontalInequalities[row, col] == -1;
        }

        private bool InequalityAboveIsGreaterThan(int row, int col)
        {
            if (row == 0) return false;
            return VerticalInequalities[row - 1, col] == -1;
        }

        private bool InequalityBelowIsGreaterThan(int row, int col)
        {
            if (row > GridSize - 3) return false;
            return VerticalInequalities[row, col] == 1;
        }

        private bool InequalityBeforeIsGreaterThan(int row, int col)
        {
            if (col == 0) return false;
            return HorizontalInequalities[row, col - 1] == -1;
        }

        private bool InequalityAfterIsGreaterThan(int row, int col)
        {
            if (col > GridSize - 3) return false;
            return HorizontalInequalities[row, col] == 1;
        }

        private void RemoveMatchingSmallerValuesFromColumn(int col, ICollection<int> potentialMatchesForCell)
        {
            var valuesToRemove = new List<int>();
            foreach (var potentialMatch in potentialMatchesForCell)
            {
                var valuesToCompareAgainst = Enumerable.Range(1, potentialMatch - 1).ToList();
                var aSmallerValueIsMissing = false;
                foreach (var valueToCompareAgainst in valuesToCompareAgainst)
                {
                    if (ColHasNumber(col, valueToCompareAgainst) == false)
                    {
                        aSmallerValueIsMissing = true;
                    }
                }
                if (!aSmallerValueIsMissing)
                {
                    valuesToRemove.Add(potentialMatch);
                }
            }
            foreach (var valueToRemove in valuesToRemove)
            {
                potentialMatchesForCell.Remove(valueToRemove);
            }
        }

        private void RemoveMatchingLargerValuesFromColumn(int col, ICollection<int> potentialMatchesForCell)
        {
            var valuesToRemove = new List<int>();
            foreach (var potentialMatch in potentialMatchesForCell)
            {
                var valuesToCompareAgainst = Enumerable.Range(potentialMatch + 1, GridSize - potentialMatch).ToList();
                var biggerValueIsMissing = false;
                foreach (var valueToCompareAgainst in valuesToCompareAgainst)
                {
                    if (ColHasNumber(col, valueToCompareAgainst) == false)
                    {
                        biggerValueIsMissing = true;
                    }
                }
                if (!biggerValueIsMissing)
                {
                    valuesToRemove.Add(potentialMatch);
                }
            }
            foreach (var valueToRemove in valuesToRemove)
            {
                potentialMatchesForCell.Remove(valueToRemove);
            }
        }

        private void RemoveMatchingLargerValuesFromRow(int row, ICollection<int> potentialMatchesForCell)
        {
            var valuesToRemove = new List<int>();
            foreach (var potentialMatch in potentialMatchesForCell)
            {
                var valuesToCompareAgainst = Enumerable.Range(potentialMatch + 1, GridSize - potentialMatch).ToList();
                var biggerValueIsMissing = false;
                foreach (var valueToCompareAgainst in valuesToCompareAgainst)
                {
                    if (RowHasNumber(row, valueToCompareAgainst) == false)
                    {
                        biggerValueIsMissing = true;
                    }
                }
                if (!biggerValueIsMissing)
                {
                    valuesToRemove.Add(potentialMatch);
                }
            }
            foreach (var valueToRemove in valuesToRemove)
            {
                potentialMatchesForCell.Remove(valueToRemove);
            }
        }

        private void RemoveMatchingSmallerValuesFromRow(int row, ICollection<int> potentialMatchesForCell)
        {
            var valuesToRemove = new List<int>();
            foreach (var potentialMatch in potentialMatchesForCell)
            {
                var valuesToCompareAgainst = Enumerable.Range(1, potentialMatch - 1).ToList();
                var aSmallerValueIsMissing = false;
                foreach (var valueToCompareAgainst in valuesToCompareAgainst)
                {
                    if (RowHasNumber(row, valueToCompareAgainst) == false)
                    {
                        aSmallerValueIsMissing = true;
                    }
                }
                if (!aSmallerValueIsMissing)
                {
                    valuesToRemove.Add(potentialMatch);
                }
            }
            foreach (var valueToRemove in valuesToRemove)
            {
                potentialMatchesForCell.Remove(valueToRemove);
            }
        }

    #endregion

    private void LogMessage(string message)
        {
            Log.AppendLine(message);
        }

        #endregion
    }
}
