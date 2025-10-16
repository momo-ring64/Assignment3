using System;
using System.Linq;

namespace Assignment_2.Classes
{
    // GameManager: main flow, map, movement, and encounters
    public class GameManager
    {
        private Room[,] map;
        private Player player;
        private int currentX;
        private int currentY;
        private Random rand = new Random();

        // Start the game
        public void StartGame()
        {
            Console.WriteLine("Welcome to");
            Console.WriteLine("    _        _   _               ____                      ");
            Console.WriteLine("   / \\   ___| |_(_) ___  _ __   / ___| _   _ _ __ __ _  ___ ");
            Console.WriteLine("  / _ \\ / __| __| |/ _ \\| '_ \\  \\___ \\| | | | '__/ _` |/ _ \\");
            Console.WriteLine(" / ___ \\ (__| |_| | (_) | | | |  ___) | |_| | | | (_| |  __/");
            Console.WriteLine("/_/   \\_\\___|\\__|_|\\___/|_| |_| |____/ \\__,_|_|  \\__, |\\___|");
            Console.WriteLine("                                                 |___/      ");
            Console.WriteLine("--The Dungeon Crawling Adventure Game!--");
            Console.WriteLine("Traverse through the dungeon to fight Goblins and loot for items! When you feel accomplished, you may leave in content.");

            Console.Write("\nEnter your hero's name: ");
            string name = Console.ReadLine();
            player = new Player(name);

            Console.WriteLine($"\nWelcome, {player.Name}!");
            Console.WriteLine("You start with a sword, dagger and a health potion.\n");

            InitializeMap();

            currentX = 1;
            currentY = 1;
            GameLoop();
        }

        // Build a 4x4 map with 3 treasure, 3 encounter, rest empty
        private void InitializeMap()
        {
            int rows = 4, cols = 4;
            map = new Room[rows, cols];

            // Fill with empty rooms
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                    map[x, y] = new EmptyRoom();

            // Create list of coordinates and shuffle
            var coords = new (int x, int y)[rows * cols];
            int idx = 0;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                    coords[idx++] = (x, y);

            // Fisher-Yates shuffle
            for (int i = coords.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var tmp = coords[i];
                coords[i] = coords[j];
                coords[j] = tmp;
            }

            // Assign 3 treasure rooms
            for (int i = 0; i < 3; i++)
            {
                var c = coords[i];
                map[c.x, c.y] = new TreasureRoom();
            }

            // Assign 3 encounter rooms
            for (int i = 3; i < 6; i++)
            {
                var c = coords[i];
                map[c.x, c.y] = new EncounterRoom();
            }

            // Player always starts in the center (1,1)
        }

        // Main input loop
        private void GameLoop()
        {
            bool running = true;

            while (running)
            {
                Room current = map[currentX, currentY];
                Console.WriteLine($"\nYou are in room [{currentX},{currentY}]");
                Console.WriteLine(current.RoomDescription());
                current.OnRoomEntered(player);

                // If encounter room - start battle
                if (current is EncounterRoom)
                {
                    StartEncounter();
                    if (player.HitPoints <= 0) return; // player died
                }

                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Move (n/s/e/w)");
                Console.WriteLine("2. Search");
                Console.WriteLine("3. Inventory");
                Console.WriteLine("4. Stats");
                Console.WriteLine("5. Use Potion");
                Console.WriteLine("6. Quit");
                Console.Write("Choice: ");
                string input = Console.ReadLine().Trim().ToLower();

                switch (input)
                {
                    case "1":
                    case "move":
                        MovePlayer();
                        break;
                    case "2":
                    case "search":
                        current.OnRoomSearched(player);
                        break;
                    case "3":
                    case "inventory":
                        player.ShowInventory();
                        break;
                    case "4":
                    case "stats":
                        player.ShowStats();
                        break;
                    case "5":
                    case "use potion":
                    case "potion":
                        UsePotionMenu();
                        break;
                    case "6":
                    case "quit":
                        Console.WriteLine("Thanks for Playing!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Choice.");
                        break;
                }
            }
        }

        // Potion usage logic extracted for clarity
        private void UsePotionMenu()
        {
            var potions = player.InventoryItems.OfType<Consumable>().ToArray();
            if (potions.Length == 0)
            {
                Console.WriteLine("You have no potions.");
                return;
            }

            Console.WriteLine("Choose a potion to use:");
            for (int i = 0; i < potions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {potions[i].Name} ({potions[i].DiceCount}d{potions[i].DiceSides} heal)");
            }

            Console.Write("Potion number: ");
            string sel = Console.ReadLine().Trim();
            if (int.TryParse(sel, out int pi) && pi >= 1 && pi <= potions.Length)
            {
                var potion = potions[pi - 1];
                int healed = potion.RollHeal();
                player.ReceiveHeal(healed);
                player.DropItem(potion);
            }
            else
            {
                Console.WriteLine("Invalid Selection.");
            }
        }

        // Movement with bounds checking
        private void MovePlayer()
        {
            Console.Write("Direction (n/s/e/w): ");
            string d = Console.ReadLine().Trim().ToLower();

            int nx = currentX, ny = currentY;
            switch (d)
            {
                case "n": nx--; break;
                case "s": nx++; break;
                case "e": ny++; break;
                case "w": ny--; break;
                default:
                    Console.WriteLine("Invalid direction.");
                    return;
            }

            if (nx < 0 || ny < 0 || nx >= map.GetLength(0) || ny >= map.GetLength(1))
            {
                Console.WriteLine("You can't go that way.");
                return;
            }

            map[currentX, currentY].OnRoomExit();

            currentX = nx;
            currentY = ny;
        }

        // Encounter: turn-based combat
        private void StartEncounter()
        {
            Enemy enemy = new Enemy("Goblin", rand.Next(8, 14));
            Console.WriteLine($"\nA wild {enemy.Name} appears! ({enemy.HitPoints} HP)");

            while (player.HitPoints > 0 && enemy.HitPoints > 0)
            {
                Console.WriteLine($"\nPlayer HP: {player.HitPoints} | {enemy.Name} HP: {enemy.HitPoints}");
                Console.WriteLine("Actions: 1.Attack  2.Use consumable  3.Run");
                Console.Write("Choice: ");
                string choice = Console.ReadLine().Trim().ToLower();

                if (choice == "1" || choice == "attack")
                {
                    PlayerAttack(enemy);
                }
                else if (choice == "2" || choice.Contains("use"))
                {
                    UsePotionMenu();
                }
                else if (choice == "3" || choice == "run")
                {
                    if (rand.Next(2) == 0)
                    {
                        Console.WriteLine("You manage to run away!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You fail to escape.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid action.");
                }

                if (enemy.HitPoints > 0)
                {
                    int dmg = enemy.RollAttack();
                    Console.WriteLine($"The {enemy.Name} attacks for {dmg} damage.");
                    player.ReceiveDamage(dmg);
                }
            }

            if (player.HitPoints > 0 && enemy.HitPoints <= 0)
            {
                Console.WriteLine($"You defeated the {enemy.Name}!");
                if (rand.Next(100) < 40)
                {
                    Item loot = RandomLoot();
                    player.AddItem(loot);
                }
            }
            else if (player.HitPoints <= 0)
            {
                Console.WriteLine("You have been defeated...");
            }
        }

        private void PlayerAttack(Enemy enemy)
        {
            var weapons = player.InventoryItems.OfType<Weapon>().ToArray();
            if (weapons.Length == 0)
            {
                int dmg = RollDice(6);
                Console.WriteLine($"You punch for {dmg} damage.");
                enemy.HitPoints -= dmg;
            }
            else
            {
                Console.WriteLine("Choose a weapon:");
                for (int i = 0; i < weapons.Length; i++)
                    Console.WriteLine($"{i + 1}. {weapons[i].Name} ({weapons[i].DiceCount}d{weapons[i].DiceSides})");

                Console.Write("Weapon number: ");
                string sel = Console.ReadLine().Trim();
                if (int.TryParse(sel, out int wi) && wi >= 1 && wi <= weapons.Length)
                {
                    var weapon = weapons[wi - 1];
                    int dmg = weapon.RollDamage();
                    Console.WriteLine($"You attack with {weapon.Name} for {dmg} damage.");
                    enemy.HitPoints -= dmg;
                }
                else
                {
                    Console.WriteLine("Invalid selection, you fumble and miss.");
                }
            }
        }

        // Dice roll helper
        private int RollDice(int sides) => rand.Next(1, sides + 1);

        // Random item drops
        private Item RandomLoot()
        {
            Item[] pool = new Item[]
            {
                new Weapon("Short Dagger", 1, 4),
                new Weapon("Longsword", 1, 8),
                new Weapon("Steel Halberd", 1, 10),
                new Weapon("Steel Greatsword", 1, 10),
                new Consumable("Small Potion", 1, 4),
                new Consumable("Large Potion", 2, 6)
            };
            return pool[rand.Next(pool.Length)];
        }
    }
}
