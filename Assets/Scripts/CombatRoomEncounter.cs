using UnityEngine;

namespace Assignment_3.Classes
{
    public class CombatRoom : RoomBase
    {
        public bool cleared = false;

        public override string RoomDescription()
        {
            if (cleared)
                return "The remains of your battle lie still. The room is quiet.";

            return "A goblin ambushes you!";
        }

        public override void EnterRoom(Player player)
        {
            base.EnterRoom(player);

            if (!cleared)
                Debug.Log("Press F to fight!");
            else
                Debug.Log("This room has been cleared.");
        }
    }
}
