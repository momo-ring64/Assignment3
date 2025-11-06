using System;

namespace Assignment_2.Classes
{
    // consumable item: heals by rolling dice
    public class Consumable : Item
    {
        public int DiceCount { get; private set; }
        public int DiceSides { get; private set; }
        private Random rand = new Random();

        public Consumable(string name, int diceCount, int diceSides) : base(name)
        {
            DiceCount = diceCount;
            DiceSides = diceSides;
        }

        // roll heal total
        public int RollHeal()
        {
            int total = 0;
            for (int i = 0; i < DiceCount; i++)
                total += rand.Next(1, DiceSides + 1);
            return total;
        }

        public override string Use(Player user)
        {
            int healed = RollHeal();
            user.ReceiveHeal(healed);
            return $"{user.Name} uses {Name} and heals {healed} hp.";
        }
    }
}
