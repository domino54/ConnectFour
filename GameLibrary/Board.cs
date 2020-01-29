using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLibrary
{
    /// <summary>
    /// Represents a board, on which the game of Connect Four is played.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Collection of columns which the board is made of.
        /// </summary>
        private List<BoardColumn> Columns = new List<BoardColumn>();

        /// <summary>
        /// Creates a new Board with given number of columns and rows.
        /// </summary>
        /// <param name="nbColumns">Number of columns in that Board.</param>
        /// <param name="nbRows">Number of rows in that Board.</param>
        public Board(int nbColumns, int nbRows)
        {
            if (nbColumns <= 0)
            {
                throw new Exception($"Specified number of columns invalid: must be greater than 0 ({nbColumns} given).");
            }
            
            if (nbRows <= 0)
            {
                throw new Exception($"Specified number of rows invalid: must be greater than 0 ({nbRows} given).");
            }

            this.NbColumns      = nbColumns;
            this.NbRows         = nbRows;
            this.NextPlayerNum  = 1;
            this.WinnerNum      = 0;

            for (int i = 0; i < this.NbColumns; i++)
            {
                this.Columns.Add(new BoardColumn(this.NbRows));
            }
        }

        /// <summary>
        /// Checks if in the given arrangement of coins there is a sequence
        /// of 4 or more coins belonging to the same player.
        /// </summary>
        /// <param name="coinsArrangement">The arrangement of coins. Must consist of at least 4 elements.</param>
        /// <returns>Identifier of the player that formed a sequence. 0 if no player formed a sequence.</returns>
        private int MatchingPattern(int[] coinsArrangement)
        {
            if (coinsArrangement.Length < 4)
            {
                throw new Exception($"Given arrangement of coins is too short: expected at least 4 elements ({coinsArrangement.Length} given).");
            }

            int currentStreakIndex = 0;
            int currentStreakCount = 0;

            for (int i = 0; i < coinsArrangement.Length; i++)
            {
                int index = coinsArrangement[i];

                if (currentStreakIndex != index)
                {
                    currentStreakIndex = index;
                    currentStreakCount = 1;
                }
                else
                {
                    currentStreakCount++;
                }

                if (currentStreakIndex > 0 && currentStreakCount >= 4) return currentStreakIndex;
            }

            return 0;
        }
        
        /// <summary>
        /// Get the identifier of player that matches the game's win conditions.
        /// </summary>
        /// <returns>Identifier of the winning player. 0 if there's no winner.</returns>
        private int FindWinner()
        {
            // Vertically
            foreach (BoardColumn column in this.Columns)
            {
                int index = this.MatchingPattern(column.Layout);
                if (index > 0) return index;
            }

            int[,] layout = this.Layout;

            // Horizontally
            for (int row = 0; row < this.NbRows; row++)
            {
                int[] rowLayout = new int[this.NbColumns];

                for (int col = 0; col < this.NbColumns; col++)
                {
                    rowLayout[col] = layout[col, row];
                }

                int index = this.MatchingPattern(rowLayout);
                if (index > 0) return index;
            }

            // Diagonally - North East
            for (int col = 0; col <= this.NbColumns - 4; col++)
            {
                for (int row = 0; row <= this.NbRows - 4; row++)
                {
                    int[] coinsArrangement = new int[4];

                    for (int i = 0; i < 4; i++)
                    {
                        coinsArrangement[i] = layout[col + i, row + i];
                    }

                    int index = this.MatchingPattern(coinsArrangement);
                    if (index > 0) return index;
                }
            }

            // Diagonally - South East
            for (int col = 0; col <= this.NbColumns - 4; col++)
            {
                for (int row = this.NbRows - 1; row >= 3; row--)
                {
                    int[] coinsArrangement = new int[4];

                    for (int i = 0; i < 4; i++)
                    {
                        coinsArrangement[i] = layout[col + i, row - i];
                    }

                    int index = this.MatchingPattern(coinsArrangement);
                    if (index > 0) return index;
                }
            }

            return 0;
        }

        /// <summary>
        /// Add a coin on top of the stack in a column in the Board.
        /// </summary>
        /// <param name="columnNum">Number of column to add a coin to.</param>
        /// <param name="playerNum">Player who added a coin.</param>
        public void AddCoinToColumn(int columnNum, int playerNum)
        {
            if (Columns.ElementAt(columnNum) == null)
            {
                throw new Exception($"Column at given index ({columnNum}) does not exist.");
            }

            BoardColumn column = Columns[columnNum];
            
            column.AddCoin(playerNum);

            // Update game status.
            this.WinnerNum = this.FindWinner();
            this.NextPlayerNum = this.WinnerNum > 0 ? 0 : 3 - this.NextPlayerNum;
        }

        /// <summary>
        /// Number of columns in the Board.
        /// </summary>
        public int NbColumns { get; }

        /// <summary>
        /// Number of rows in the Board.
        /// </summary>
        public int NbRows { get; }

        /// <summary>
        /// Identifier of the player who should be next to add a new coin.
        /// </summary>
        public int NextPlayerNum { get; private set; }

        /// <summary>
        /// Identifier of the player who won the game. 0 if there is no winner.
        /// </summary>
        public int WinnerNum { get; private set; }

        /// <summary>
        /// Array of indexes of columns which have not been filled with coins yet.
        /// </summary>
        public int[] ColumnsWithUnfilledRows
        {
            get
            {
                List<int> columnNums = new List<int>();

                for (int col = 0; col < Columns.Count; col++)
                {
                    if (Columns[col].HasEmptyRows)
                    {
                        columnNums.Add(col);
                    }
                }

                return columnNums.ToArray();
            }
        }

        /// <summary>
        /// Whether there still are any columns with empty rows or not.
        /// </summary>
        public bool HasUnfilledColumns
        {
            get => this.ColumnsWithUnfilledRows.Length > 0;
        }

        /// <summary>
        /// Whether the game is finished or not.
        /// </summary>
        public bool IsFinished
        {
            get => this.WinnerNum > 0 || !this.HasUnfilledColumns;
        }

        /// <summary>
        /// Two dimensional array representing the Board layout, where values are
        /// identifiers of players, who placed their coin in the given cell.
        /// </summary>
        public int[,] Layout
        {
            get
            {
                int[,] layout = new int[Columns.Count, this.NbRows];

                for (int column = 0; column < Columns.Count; column++)
                {
                    int[] columnLayout = Columns[column].Layout;

                    for (int row = 0; row < columnLayout.Length; row++)
                    {
                        layout[column, row] = columnLayout[row];
                    }
                }

                return layout;
            }
        }
    }
}
