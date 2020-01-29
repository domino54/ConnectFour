using System;

namespace GameLibrary
{
    /// <summary>
    /// Represents a coin filling an empty cell in the Board grid.
    /// </summary>
    class BoardCoin
    {
        /// <summary>
        /// Identifier of the player who the coin belongs to.
        /// </summary>
        public int PlayerNum { get; }

        /// <summary>
        /// Creates a new BoardCoin object.
        /// </summary>
        /// <param name="playerNum">Identifier of the player who the coin belongs to.</param>
        public BoardCoin(int playerNum)
        {
            if (playerNum <= 0 || playerNum > 2)
            {
                throw new Exception();
            }

            this.PlayerNum = playerNum;
        }
    }
}
