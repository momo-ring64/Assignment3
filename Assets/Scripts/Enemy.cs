using UnityEngine;

namespace Assignment_3.Classes
{
    public class Enemy
    {
        public string Name { get; private set; }
        public int HitPoints { get; set; }

        public Enemy(string name, int hp)
        {
            Name = name;
            HitPoints = hp;
        }

        public int RollAttack() => Random.Range(1, 9);
    }
}
