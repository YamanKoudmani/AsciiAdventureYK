using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

/*
 Cool ass stuff people could implement:
 > jumping
 > attacking
 > randomly moving monsters
 > smarter moving monsters
*/
namespace asciiadventureYK
{
    public class Game
    {
        private Random random = new Random();
        private string gameoverscreen = "╔═══╗            ╔╗               ╔╗                  ╔╗    \n║╔═╗║            ║║              ╔╝╚╗                 ║║    \n║║ ║║    ╔╗╔╗╔══╗║╚═╗    ╔══╗╔══╗╚╗╔╝    ╔╗ ╔╗╔══╗╔╗╔╗║║    \n║╚═╝║    ║╚╝║║╔╗║║╔╗║    ║╔╗║║╔╗║ ║║     ║║ ║║║╔╗║║║║║╚╝    \n║╔═╗║    ║║║║║╚╝║║╚╝║    ║╚╝║║╚╝║ ║╚╗    ║╚═╝║║╚╝║║╚╝║╔╗    \n╚╝ ╚╝    ╚╩╩╝╚══╝╚══╝    ╚═╗║╚══╝ ╚═╝    ╚═╗╔╝╚══╝╚══╝╚╝    \n                         ╔═╝║            ╔═╝║               \n                         ╚══╝            ╚══╝               \n╔═══╗╔═══╗╔═╗╔═╗╔═══╗    ╔═══╗╔╗  ╔╗╔═══╗╔═══╗\n║╔═╗║║╔═╗║║║╚╝║║║╔══╝    ║╔═╗║║╚╗╔╝║║╔══╝║╔═╗║\n║║ ╚╝║║ ║║║╔╗╔╗║║╚══╗    ║║ ║║╚╗║║╔╝║╚══╗║╚═╝║\n║║╔═╗║╚═╝║║║║║║║║╔══╝    ║║ ║║ ║╚╝║ ║╔══╝║╔╗╔╝\n║╚╩═║║╔═╗║║║║║║║║╚══╗    ║╚═╝║ ╚╗╔╝ ║╚══╗║║║╚╗\n╚═══╝╚╝ ╚╝╚╝╚╝╚╝╚═══╝    ╚═══╝  ╚╝  ╚═══╝╚╝╚═╝\n                                              \n                                              ";
        Player player;
        private static Boolean Eq(char c1, char c2)
        {
            return c1.ToString().Equals(c2.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        private static string Menu()
        {
            return "WASD to move\nIJKL to attack/interact\nCurrent cash: ";
        }

        private static void PrintScreen(Screen screen, string message, string menu)
        {
            //DisplayScreen();
            for (int x = 2; x < 15; x++)
            {
                Console.SetCursorPosition(screen.NumRows + x, screen.NumCols + x);
                ClearCurrentConsoleLine();
            }
            Console.SetCursorPosition(0, screen.NumCols + 2);
            Console.WriteLine($"\n{message}");
            Console.WriteLine($"\n{menu}");
        }
        public void Run()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Screen screen = new Screen(10, 10);
            // add a couple of walls
            for (int i = 0; i < 3; i++)
            {
                new Wall(1, 2 + i, screen);
            }
            for (int i = 0; i < 4; i++)
            {
                new Wall(3 + i, 4, screen);
            }
            // add a treasure
            Treasure treasure = new Treasure(6, 2, screen);
            Money mon = new Money(4, 1, screen, 10);
            // initially print the game board
            Console.Write(screen.BuildWorld());
            // add a player
            player = new Player(0, 0, screen, "Zelda");
            Console.ForegroundColor = ConsoleColor.Yellow;
            UpdateObjectPosition(CheckPosition(player), player.Token);
            //print welcome screen
            PrintScreen(screen, "Welcome!", "\n" + Menu());

            // add some mobs
            List<Mob> mobs = new List<Mob>();
            mobs.Add(new Mob(9, 9, screen));
            mobs.Add(new Mob(4, 7, screen));
            //draw starting position of mobs
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Mob m in mobs)
            {
                UpdateObjectPosition(CheckPosition(m), m.Token);
            }

            //MAIN GAME LOOP:
            Boolean gameOver = false;

            while (!gameOver)
            {
                char input = Console.ReadKey(true).KeyChar;
                String message = "Current cash: " + player.Money + "\n";
                Console.ForegroundColor = ConsoleColor.Yellow;
                UpdateObjectPosition(CheckPosition(player), " ");

                // OK, now move the mobs
                foreach (Mob mob in mobs)
                {
                    // TODO: Make mobs smarter, so they jump on the player, if it's possible to do so
                    List<Tuple<int, int>> moves = screen.GetLegalMoves(mob.Row, mob.Col);
                    if (moves.Count == 0)
                    {
                        continue;
                    }
                    // mobs move less randomly
                    int deltaRow = 0;
                    int deltaCol = 0;
                    int randomy = new Random().Next(2);
                    if (randomy == 0)
                    {
                        if (CheckPosition(player).Item1 < CheckPosition(mob).Item1)
                        {
                            if (moves.Contains(Tuple.Create(-1, 0)))
                            {
                                deltaRow = -1;
                            }
                            else if (moves.Contains(Tuple.Create(0, -1)))
                            {
                                deltaCol = -1;
                            }
                        }
                        if (CheckPosition(player).Item1 > CheckPosition(mob).Item1)
                        {
                            if (moves.Contains(Tuple.Create(1, 0)))
                            {
                                deltaRow = 1;
                            }
                            else if (moves.Contains(Tuple.Create(0, -1)))
                            {
                                deltaCol = -1;
                            }
                        }
                    }
                    else
                    {
                        if (CheckPosition(player).Item2 < CheckPosition(mob).Item2)
                        {
                            if (moves.Contains(Tuple.Create(0, -1)))
                            {
                                deltaCol = -1;
                            }
                            else if (moves.Contains(Tuple.Create(-1, 0)))
                            {
                                deltaRow = -1;
                            }
                        }
                        if (CheckPosition(player).Item2 > CheckPosition(mob).Item2)
                        {
                            if (moves.Contains(Tuple.Create(0, 1)))
                            {
                                deltaCol = 1;
                            }
                            else if (moves.Contains(Tuple.Create(-1, 0)))
                            {
                                deltaRow = -1;
                            }
                        }
                    }

                    if (screen[mob.Row + deltaRow, mob.Col + deltaCol] is Player)
                    {
                        // the mob got the player!
                        mob.Token = "*";
                        message += "A MOB GOT YOU! GAME OVER\n";

                        UpdateObjectPosition(CheckPosition(mob), mob.Token);
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        PrintScreen(screen, gameoverscreen, "Press T to try again, or Q to exit.");
                        Thread.Sleep(2000);
                        gameOver = true;
                        continue;
                    }

                    if (!gameOver)
                    {
                        UpdateObjectPosition(CheckPosition(mob), " ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        mob.Move(deltaCol, deltaRow);
                        UpdateObjectPosition(CheckPosition(mob), mob.Token);
                    }
                }
                message = PlayerInput(input);
                Console.ForegroundColor = ConsoleColor.Yellow;
                UpdateObjectPosition(CheckPosition(player), player.Token);
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintScreen(screen, "Player Position is: " + player.Row + ", " + player.Col + "\nMob1 Position is: " + mobs[0].Row + ", " + mobs[0].Col + "\n" + message, Menu() + player.Money);
            }
            char inp = Console.ReadKey(true).KeyChar;
            while (!Eq(inp, 'q'))
            {
                if (Eq(inp, 't'))
                {
                    Console.Clear();
                    this.Run();
                }
                if (Eq(inp, 'q'))
                {
                }
                else
                {
                    Console.WriteLine($"Unknown command: {inp}");
                    PrintScreen(screen, gameoverscreen, "Press T to try again, or Q to exit.");
                    inp = Console.ReadKey(true).KeyChar;
                }
            }
        }

        public void UpdateObjectPosition(Tuple<int, int> t, String input)
        {
            Console.SetCursorPosition(t.Item2 + 1, t.Item1 + 1);
            Console.Write(input);
        }
        public Tuple<int, int> CheckPosition(GameObject g)
        {
            var temp = Tuple.Create(g.Row, g.Col);
            return temp;
        }
        public string PlayerInput(Char input)
        {
            if (Eq(input, 'w'))
            {
                player.Move(-1, 0);
            }
            else if (Eq(input, 's'))
            {
                player.Move(1, 0);
            }
            else if (Eq(input, 'a'))
            {
                player.Move(0, -1);
            }
            else if (Eq(input, 'd'))
            {
                player.Move(0, 1);
            }
            else if (Eq(input, 'i'))
            {
                return player.Action(-1, 0) + "\n";
            }
            else if (Eq(input, 'k'))
            {
                return player.Action(1, 0) + "\n";
            }
            else if (Eq(input, 'j'))
            {
                return player.Action(0, -1) + "\n";
            }
            else if (Eq(input, 'l'))
            {
                return player.Action(0, 1) + "\n";
            }
            else if (Eq(input, 'v'))
            {
                // TODO: handle inventory
                return "You have nothing\n";
            }
            else if(Eq(input, 'q'))
            {
                Environment.Exit(0);
            }
            else
            {
                return $"Unknown command: {input}";
            }
            return "";
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }
}
