using System;

namespace asciiadventureYK
{
    class Player : MovingGameObject
    {
        public Player(int row, int col, Screen screen, string name) : base(row, col, "@", screen)
        {
            Name = name;
        }
        public string Name
        {
            get;
            protected set;
        }
        public int Money
        {
            get;
            protected set;
        }
        public override Boolean IsPassable()
        {
            return true;
        }

        public String Action(int deltaRow, int deltaCol)
        {
            int newRow = Row + deltaRow;
            int newCol = Col + deltaCol;
            if (!Screen.IsInBounds(newRow, newCol))
            {
                return "nope";
            }
            GameObject other = Screen[newRow, newCol];
            if (other == null)
            {
                return "negative";
            }
            // TODO: Interact with the object
            if (other is Treasure)
            {
                other.Delete();
                Console.SetCursorPosition(other.Col + 1, other.Row + 1);
                Console.Write(" ");
                return "Yay, we got the treasure!";
            }
            if (other is Money)
            {
                other.Delete();
                Money m = (Money) other;
                Money += m.CashBlock;
                Console.SetCursorPosition(other.Col+1, other.Row+1);
                Console.Write(" ");
                return "Yay, we got " + m.CashBlock +" dollars!";
            }
            return "yikes";
        }
    }
}
