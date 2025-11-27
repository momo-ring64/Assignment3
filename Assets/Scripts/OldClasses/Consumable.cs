using UnityEngine;

namespace Assignment_3.Classes
{
    public class Consumable : Item
    {
        public int DiceCount { get; private set; }
        public int DiceSides { get; private set; }

        private System.Random rand = new System.Random();

        public Consumable(string name, int diceCount, int diceSides)
            : base(name)
        {
            DiceCount = diceCount;
            DiceSides = diceSides;
        }

        public int RollHeal()
        {
            int total = 0;
            for (int i = 0; i < DiceCount; i++)
                total += rand.Next(1, DiceSides + 1);
            return total;
        }
    }
}
