using System;

namespace Assignment_2.Classes
{
    public class TreasureRoom : Room
    {
        private bool searched = false;

        public override string RoomDescription()
        {
            return "you step into a glittering treasure room filled with shining objects.";
        }

        public override void OnRoomSearched(Player player)
        {
            if (!searched)
            {
                // simple loot set (we create new instances so player owns them)
                Item[] loot = new Item[]
                {
                    new Weapon("iron dagger", 1, 4),
                    new Weapon("steel longsword", 1, 8),
                    new Weapon("steel halberd", 1, 10),
                    new Weapon("steel greatsword", 1, 10),
                    new Consumable("small healing potion", 1, 4),
                    new Consumable("large healing potion", 2, 6)
                };

                var rand = new Random();
                Item found = loot[rand.Next(loot.Length)];
                player.AddItem(found);
                Console.WriteLine($"you found a {found.Name}!");
                searched = true;
            }
            else
            {
                Console.WriteLine("you already searched here.");
            }
        }
    }
}
