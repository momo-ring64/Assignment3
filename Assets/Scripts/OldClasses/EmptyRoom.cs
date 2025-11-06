using System;

namespace Assignment_2.Classes
{
    public class EmptyRoom : Room
    {
        public override string RoomDescription()
        {
            return "an empty, dusty room. nothing of interest here.";
        }

        public override void OnRoomEntered(Player player)
        {
            Console.WriteLine("you enter an empty room. it's eerily quiet...");
            visited = true;
        }

        public override void OnRoomSearched(Player player)
        {
            Console.WriteLine("you search the room, but find nothing.");
        }

        public override void OnRoomExit()
        {
            Console.WriteLine("you leave the empty room behind.");
        }
    }
}
