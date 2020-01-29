using System;

namespace GameLibrary
{
    /// <summary>
    /// Main handler of the game's logic.
    /// </summary>
    public class GameLogic
    {
        public enum AllowedBoardSizes
        {
            Size7x6,
            Size8x7,
            Size10x8
        }

        /// <summary>
        /// Board on which the game is currently played on.
        /// </summary>
        public Board CurrentBoard { get; private set; } = null;

        /// <summary>
        /// Start a new game if there isn't one running already.
        /// </summary>
        /// <param name="boardSize">Size of the Board the game will be played at.</param>
        public void StartNewGame(AllowedBoardSizes boardSize)
        {
            if (this.CurrentBoard != null)
            {
                throw new Exception("Trying to start a new game while there already is one active.");
            }

            int nbColumns = 0, nbRows = 0;

            switch (boardSize)
            {
                case AllowedBoardSizes.Size7x6:
                    {
                        nbColumns = 7;
                        nbRows = 6;
                        break;
                    }
                case AllowedBoardSizes.Size8x7:
                    {
                        nbColumns = 8;
                        nbRows = 7;
                        break;
                    }
                case AllowedBoardSizes.Size10x8:
                    {
                        nbColumns = 10;
                        nbRows = 8;
                        break;
                    }
            }

            this.CurrentBoard = new Board(nbColumns, nbRows);
        }

        /// <summary>
        /// Stop the currently running game.
        /// </summary>
        public void StopGame()
        {
            if (this.CurrentBoard == null)
            {
                throw new Exception("Trying to stop the game while there is no game running.");
            }

            this.CurrentBoard = null;
        }
    }
}
