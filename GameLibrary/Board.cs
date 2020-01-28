using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    public class Board
    {
        private int nbColumns, nbRows, nextPlayerMove, winnerNum;
        private List<BoardColumn> columns = new List<BoardColumn>();

        public Board(int columns, int rows)
        {
            if (columns <= 0)
            {
                throw new Exception();
            }
            
            if (rows <= 0)
            {
                throw new Exception();
            }

            this.nbColumns          = columns;
            this.nbRows             = rows;
            this.nextPlayerMove     = 1;
            this.winnerNum          = 0;

            for (int i = 0; i < this.nbColumns; i++)
            {
                BoardColumn column = new BoardColumn(this.nbRows);

                this.columns.Add(column);
            }
        }
        
        private int FindWinner()
        {
            return 0; // todo
        }

        public void AddCoin(int col, int player)
        {
            if (player <= 0 || player > 2)
            {
                throw new Exception();
            }

            if (columns.ElementAt(col) == null)
            {
                throw new Exception();
            }

            BoardColumn column = columns[col];

            if (!column.HasEmptyRows)
            {
                throw new Exception();
            }
            
            column.AddCoin(player);
            this.winnerNum = this.FindWinner();
            this.nextPlayerMove = this.winnerNum > 0 ? 0 : 3 - this.nextPlayerMove;
        }

        public int NbColumns
        {
            get => this.nbColumns;
        }

        public int NbRows
        {
            get => this.nbRows;
        }

        public int NextPlayer
        {
            get => this.nextPlayerMove;
        }

        public int Winner
        {
            get => this.winnerNum;
        }

        public int[] ColumnsWithEmptyRows
        {
            get
            {
                List<int> columnNums = new List<int>();

                for (int col = 0; col < columns.Count; col++)
                {
                    if (columns[col].HasEmptyRows)
                    {
                        columnNums.Add(col);
                    }
                }

                return columnNums.ToArray();
            }
        }

        public bool HasEmptyVolumns
        {
            get => this.ColumnsWithEmptyRows.Length > 0;
        }

        public bool IsFinished
        {
            get => this.Winner > 0 || !this.HasEmptyVolumns;
        }

        public int[,] Layout
        {
            get
            {
                int[,] columnsAndRowsPlayers = new int[columns.Count, this.nbRows];

                for (int col = 0; col < columns.Count; col++)
                {
                    int[] columnLayout = columns[col].Layout;

                    for (int row = 0; row < columnLayout.Length; row++)
                    {
                        columnsAndRowsPlayers[col, row] = columnLayout[row];
                    }
                }

                return columnsAndRowsPlayers;
            }
        }
    }
}
