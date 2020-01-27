using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    class BoardColumn
    {
        private List<BoardCoin> filledRows = new List<BoardCoin>();
        public int NbRows { get; }

        public BoardColumn(int rows)
        {
            if (rows <= 0)
            {
                throw new Exception();
            }
            
            this.NbRows = rows;
        }
        
        public bool HasEmptyRows
        {
            get => this.filledRows.Count < this.NbRows;
        }

        public void AddCoin(int player)
        {
            if (!this.HasEmptyRows)
            {
                throw new Exception();
            }

            if (player <= 0)
            {
                throw new Exception();
            }

            BoardCoin coin = new BoardCoin(player);

            this.filledRows.Add(coin);
        }
        
        public int[] Layout
        {
            get {
                int[] rowsPlayers = new int[this.NbRows];

                for (int i = 0; i < this.NbRows; i++)
                {
                    if (i < this.filledRows.Count) {
                        rowsPlayers[i] = this.filledRows[i].Player;
                    }
                    else
                    {
                        rowsPlayers[i] = 0;
                    }
                }

                return rowsPlayers;
            }
        }
    }
}
