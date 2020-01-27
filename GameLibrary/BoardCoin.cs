using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    class BoardCoin
    {
        public int Player { get; }

        public BoardCoin(int player)
        {
            if (player <= 0)
            {
                throw new Exception();
            }

            this.Player = player;
        }
    }
}
