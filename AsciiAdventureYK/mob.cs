using System;

namespace asciiadventureYK
{
    public class Mob : MovingGameObject
    {
        public Mob(int row, int col, Screen screen) : base(row, col, "#", screen) { }
    }
}
