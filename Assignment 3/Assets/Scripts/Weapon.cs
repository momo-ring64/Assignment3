using System;

namespace Assignment_2.Classes
{
    // weapon item: does damage by rolling dice
    public class Weapon : Item
    {
        public int DiceCount { get; private set; }
        public int DiceSides { get; private set; }
        private Random rand = new Random();

        public Weapon(string name, int diceCount, int diceSides) : base(name)
        {
            DiceCount = diceCount;
            DiceSides = diceSides;
        }

        // roll damage (diceCount x diceSides)
        public int RollDamage()
        {
            int total = 0;
            for (int i = 0; i < DiceCount; i++)
                total += rand.Next(1, DiceSides + 1);
            return total;
        }

        public override string Use(Player user)
        {
            // using a weapon outside combat is described
            return $"{user.Name} inspects the {Name}. it looks battle-ready.";
        }
    }
}
