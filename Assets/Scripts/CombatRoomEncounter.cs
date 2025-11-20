using UnityEngine;

namespace Assignment_3.Classes
{
    public class CombatRoom : RoomBase
    {
        private Enemy enemy;

        private void Start()
        {
            enemy = new Enemy("Goblin", 10);
        }

        public override string RoomDescription() => "A goblin attacks you!";

        public override void EnterRoom(Player player)
        {
            base.EnterRoom(player);
            Debug.Log("Press F to attack the goblin.");
        }
    }
}
