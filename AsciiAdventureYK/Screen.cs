
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace asciiadventureYK
{
    public class Screen
    {
        private GameObject[,] grid;

        public int NumRows
        {
            get;
            private set;
        }
        public int NumCols
        {
            get;
            private set;
        }

        public Screen(int numRows, int numCols)
        {
            NumRows = numRows;
            NumCols = numCols;
            this.grid = new GameObject[NumRows, NumCols];
        }

        private Boolean IsLegalMove(int row, int col, int deltaRow, int deltaCol)
        {
            int newRow = row + deltaRow;
            int newCol = col + deltaCol;
            if (!IsInBounds(newRow, newCol))
            {
                return false;
            }
            GameObject other = this[newRow, newCol];
            if (other == null)
            {
                return true;
            }
            return other.IsPassable() || other is Player;
        }

        public List<Tuple<int, int>> GetLegalMoves(int row, int col)
        {
            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            if (IsLegalMove(row, col, -1, 0))
            {
                moves.Add(Tuple.Create(-1, 0));
            }
            if (IsLegalMove(row, col, 1, 0))
            {
                moves.Add(Tuple.Create(1, 0));
            }
            if (IsLegalMove(row, col, 0, -1))
            {
                moves.Add(Tuple.Create(0, -1));
            }
            if (IsLegalMove(row, col, 0, 1))
            {
                moves.Add(Tuple.Create(0, 1));
            }
            return moves;
        }

        public GameObject this[int row, int col]
        {
            get
            {
                UseRowAndCol(row, col);
                return grid[row, col];
            }
            set
            {
                UseRowAndCol(row, col);
                grid[row, col] = value;
            }
        }

        protected Boolean CheckRow(int row)
        {
            return row >= 0 && row < NumRows;
        }

        protected Boolean CheckCol(int col)
        {
            return col >= 0 && col < NumCols;
        }

        internal Boolean IsInBounds(int row, int col)
        {
            // TODO: Check for obstacles
            return CheckRow(row) && CheckCol(col);
        }

        protected void UseRowAndCol(int row, int col)
        {
            if (!CheckRow(row))
            {
                throw new ArgumentOutOfRangeException("row", $"{row} is out of range");
            }
            if (!CheckCol(col))
            {
                throw new ArgumentOutOfRangeException("col", $"{col} is out of range");
            }
        }

        public Boolean IsOtherObject(int row, int col)
        {
            return grid[row, col] != null;
        }
        public String BuildWorld()
        {
            // create walls if needed
            StringBuilder result = new StringBuilder();
            result.Append("+");
            result.Append(String.Concat(Enumerable.Repeat("-", NumCols)));
            result.Append("+\n");
            for (int r = 0; r < NumRows; r++)
            {
                result.Append('|');
                for (int c = 0; c < NumCols; c++)
                {
                GameObject gameObject = this[r, c];
                    if (gameObject == null || gameObject is Player || gameObject is Mob)
                    {
                        result.Append(' ');
                    }
                    else
                    {
                        result.Append(gameObject.Token);
                    }
                }
                //Console.WriteLine($"newline for {r}");
                result.Append("|\n");
            }
            result.Append('+');
            result.Append(String.Concat(Enumerable.Repeat("-", NumRows)));
            result.Append('+');
            return result.ToString();
        }

    }
}