using System;

namespace Assignment_2.Classes
{
    public class EncounterRoom : Room
    {
        public override string RoomDescription()
        {
            return "a shadow moves... an enemy lurks nearby!";
        }

        public override void OnRoomEntered(Player player)
        {
            base.OnRoomEntered(player);
            Console.WriteLine("prepare for battle!");
            // actual battle is handled by GameManager when it detects this room
        }
    }
}
