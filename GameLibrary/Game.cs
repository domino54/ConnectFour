using System;

namespace GameLibrary
{
    public class Game
    {
        public enum BoardSizes
        {
            Size7x6,
            Size8x7,
            Size10x8
        }

        private Board currentBoard = null;

        public Game()
        {
            
        }

        public Board CurrentBoard
        {
            get => this.currentBoard;
        }
        
        public void StartNewGame(BoardSizes boardSize)
        {
            if (this.currentBoard != null)
            {
                throw new Exception();
            }

            int columns = 0, rows = 0;

            switch (boardSize)
            {
                case BoardSizes.Size7x6:
                    {
                        columns = 7;
                        rows = 6;
                        break;
                    }
                case BoardSizes.Size8x7:
                    {
                        columns = 8;
                        rows = 7;
                        break;
                    }
                case BoardSizes.Size10x8:
                    {
                        columns = 10;
                        rows = 8;
                        break;
                    }
            }

            this.currentBoard = new Board(columns, rows);
        }

        public void AddCoin(int col, int player)
        {
            if (this.currentBoard == null)
            {
                throw new Exception();
            }

            this.currentBoard.AddCoin(col, player);
        }

        public void StopGame()
        {
            if (this.currentBoard == null)
            {
                throw new Exception();
            }

            this.currentBoard = null;
        }
    }
}
