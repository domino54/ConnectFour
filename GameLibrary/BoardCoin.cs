using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    class BoardCoin
    {
        private int playerNum;
        private bool isVictory;

        public BoardCoin(int player)
        {
            if (player <= 0)
            {
                throw new Exception();
            }

            this.playerNum = player;
        }

        public int Player
        {
            get => this.playerNum;
        }
        
        public void MarkAsVictory()
        {
            this.isVictory = true;
        }
    }
}
