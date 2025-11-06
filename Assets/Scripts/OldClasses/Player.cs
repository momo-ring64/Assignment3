using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment_2.Classes
{
    // player: stores hp, inventory, and item helpers
    public class Player
    {
        public string Name { get; private set; }
        public int HitPoints { get; private set; }
        private int maxHP = 30;
        private int potions = 1;

        // inventory stores items (weapons, consumables, etc.)
        private List<Item> inventory = new List<Item>();

        public IEnumerable<Item> InventoryItems => inventory.AsEnumerable();

        private Random rand = new Random();

        public Player(string name)
        {
            Name = name;
            HitPoints = maxHP;

            // starter items
            inventory.Add(new Weapon("starter sword", 1, 8)); // 1d8
            inventory.Add(new Weapon("starter dagger", 1, 4)); // 1d4
            inventory.Add(new Consumable("starter potion", 1, 4)); // 1d4 heal
        }

        // apply damage
        public void ReceiveDamage(int amount)
        {
            HitPoints -= amount;
            if (HitPoints < 0) HitPoints = 0;
            Console.WriteLine($"{Name} takes {amount} damage (hp: {HitPoints}/{maxHP}).");
        }

        // apply heal
        public void ReceiveHeal(int amount)
        {
            HitPoints += amount;
            if (HitPoints > maxHP) HitPoints = maxHP;
            Console.WriteLine($"{Name} gains {amount} hp (hp: {HitPoints}/{maxHP}).");
        }

        // add an item to inventory
        public void AddItem(Item item)
        {
            inventory.Add(item);
            Console.WriteLine($"{item.Name} added to inventory.");
        }

        // drop/remove an item (used for consumables)
        public void DropItem(Item item)
        {
            if (inventory.Contains(item))
            {
                inventory.Remove(item);
                Console.WriteLine($"{item.Name} was consumed/removed.");
            }
        }

        // show inventory
        public void ShowInventory()
        {
            Console.WriteLine("\ninventory:");
            if (!inventory.Any())
            {
                Console.WriteLine(" (empty)");
                return;
            }

            int i = 1;
            foreach (var it in inventory)
            {
                if (it is Weapon w)
                    Console.WriteLine($"{i++}. {w.Name} (weapon {w.DiceCount}d{w.DiceSides})");
                else if (it is Consumable c)
                    Console.WriteLine($"{i++}. {c.Name} (consumable {c.DiceCount}d{c.DiceSides})");
                else
                    Console.WriteLine($"{i++}. {it.Name}");
            }
        }

        public void ShowStats()
        {
            Console.WriteLine($"\n{nameof(Name)}: {Name}");
            Console.WriteLine($"hp: {HitPoints}/{maxHP}");
            Console.WriteLine($"items: {inventory.Count}");
            Console.WriteLine();
        }
    }
}
