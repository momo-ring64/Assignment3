using System;

namespace Assignment_2.Classes
{
    // enemy: simple hp and attack roll
    public class Enemy
    {
        public string Name { get; private set; }
        public int HitPoints { get; set; }
        private Random rand = new Random();

        public Enemy(string name, int hp)
        {
            Name = name;
            HitPoints = hp;
        }

        public int RollAttack()
        {
            // roll 1d8 for enemy attack
            return rand.Next(1, 9);
        }
    }
}
