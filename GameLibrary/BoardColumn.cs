using System;
using System.Collections.Generic;

namespace GameLibrary
{
    /// <summary>
    /// Represents a single column of coins in the Board.
    /// </summary>
    public class BoardColumn
    {
        /// <summary>
        /// Coins added to the column.
        /// </summary>
        private List<BoardCoin> FilledRows = new List<BoardCoin>();

        /// <summary>
        /// Number of rows in the column.
        /// </summary>
        public int NbRows { get; }

        /// <summary>
        /// Creates a new column.
        /// </summary>
        /// <param name="nbRows">Number of rows in that column.</param>
        public BoardColumn(int nbRows)
        {
            if (nbRows <= 0)
            {
                throw new Exception($"Specified number of rows invalid: must be greater than 0 ({nbRows} given).");
            }
            
            this.NbRows = nbRows;
        }

        /// <summary>
        /// Whether the column still has some empty rows or not.
        /// </summary>
        public bool HasEmptyRows
        {
            get => this.FilledRows.Count < this.NbRows;
        }

        /// <summary>
        /// Add a coin to the column.
        /// </summary>
        /// <param name="playerNum">Identifier of the player who added the coin.</param>
        public void AddCoin(int playerNum)
        {
            if (!this.HasEmptyRows)
            {
                throw new Exception("Cannot add more coins to the column - it is full.");
            }
            
            this.FilledRows.Add(new BoardCoin(playerNum));
        }

        /// <summary>
        /// Array representing the Board layout, where values are identifiers
        /// of players, who placed their coin in the given row.
        /// </summary>
        public int[] Layout
        {
            get {
                int[] layout = new int[this.NbRows];

                for (int i = 0; i < this.NbRows; i++)
                {
                    if (i < this.FilledRows.Count) {
                        layout[i] = this.FilledRows[i].PlayerNum;
                    }
                    else
                    {
                        layout[i] = 0;
                    }
                }

                return layout;
            }
        }
    }
}
