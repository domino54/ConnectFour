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

        private int MatchingPattern(int[] series)
        {
            if (series.Length < 4)
            {
                throw new Exception();
            }

            int currentStreakIndex = 0;
            int currentStreakCount = 0;

            for (int i = 0; i < series.Length; i++)
            {
                int index = series[i];

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
        
        private int FindWinner()
        {
            // Vertically
            foreach (BoardColumn column in this.columns)
            {
                int index = this.MatchingPattern(column.Layout);
                if (index > 0) return index;
            }

            int[,] layout = this.Layout;

            // Horizontally
            for (int row = 0; row < this.nbRows; row++)
            {
                int[] rowLayout = new int[this.nbColumns];

                for (int col = 0; col < this.nbColumns; col++)
                {
                    rowLayout[col] = layout[col, row];
                }

                int index = this.MatchingPattern(rowLayout);
                if (index > 0) return index;
            }

            // Diagonally - North East
            for (int col = 0; col <= this.nbColumns - 4; col++)
            {
                for (int row = 0; row <= this.nbRows - 4; row++)
                {
                    int[] series = new int[4];

                    for (int i = 0; i < 4; i++)
                    {
                        series[i] = layout[col + i, row + i];
                    }

                    int index = this.MatchingPattern(series);
                    if (index > 0) return index;
                }
            }

            // Diagonally - South East
            for (int col = 0; col <= this.nbColumns - 4; col++)
            {
                for (int row = this.nbRows - 1; row >= 3; row--)
                {
                    int[] series = new int[4];

                    for (int i = 0; i < 4; i++)
                    {
                        series[i] = layout[col + i, row - i];
                    }

                    int index = this.MatchingPattern(series);
                    if (index > 0) return index;
                }
            }

            return 0;
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

        public bool HasEmptyColumns
        {
            get => this.ColumnsWithEmptyRows.Length > 0;
        }

        public bool IsFinished
        {
            get => this.Winner > 0 || !this.HasEmptyColumns;
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
