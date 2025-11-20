using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assignment_3.Classes
{
    public class Player
    {
        public string Name { get; private set; }
        public int HitPoints { get; private set; }
        private const int MaxHP = 30;

        private List<Item> inventory = new List<Item>();
        public IEnumerable<Item> InventoryItems => inventory;

        public int SelectedWeaponIndex = 0;
        public int SelectedPotionIndex = 0;

        public Player(string name)
        {
            Name = name;
            HitPoints = MaxHP;

            inventory.Add(new Weapon("Starter Sword", 1, 8));
            inventory.Add(new Weapon("Starter Dagger", 1, 4));
            inventory.Add(new Consumable("Starter Potion", 1, 4));
        }

        public void ReceiveDamage(int amount)
        {
            HitPoints = Mathf.Max(HitPoints - amount, 0);
            Debug.Log($"{Name} takes {amount} dmg ({HitPoints}/{MaxHP})");
        }

        public void ReceiveHeal(int amount)
        {
            HitPoints = Mathf.Min(HitPoints + amount, MaxHP);
            Debug.Log($"{Name} heals {amount} ({HitPoints}/{MaxHP})");
        }

        public void AddItem(Item item)
        {
            inventory.Add(item);
            Debug.Log($"Added to inventory: {item.Name}");
        }

        public List<Weapon> GetWeapons() =>
            inventory.OfType<Weapon>().ToList();

        public List<Consumable> GetPotions() =>
            inventory.OfType<Consumable>().ToList();

        public Weapon GetSelectedWeapon()
        {
            var list = GetWeapons();
            if (list.Count == 0) return null;
            SelectedWeaponIndex = Mathf.Clamp(SelectedWeaponIndex, 0, list.Count - 1);
            return list[SelectedWeaponIndex];
        }

        public Consumable GetSelectedPotion()
        {
            var list = GetPotions();
            if (list.Count == 0) return null;
            SelectedPotionIndex = Mathf.Clamp(SelectedPotionIndex, 0, list.Count - 1);
            return list[SelectedPotionIndex];
        }

        public void ConsumePotion()
        {
            var potions = GetPotions();
            if (potions.Count == 0)
            {
                Debug.Log("No potions available.");
                return;
            }

            var potion = potions[SelectedPotionIndex];
            int heal = potion.RollHeal();
            ReceiveHeal(heal);

            inventory.Remove(potion);
            Debug.Log($"You drink a {potion.Name} for {heal} HP.");
        }
    }
}
