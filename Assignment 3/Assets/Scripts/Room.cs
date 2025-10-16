using System;

namespace Assignment_2.Classes
{
    // simple room base with visit/search hooks
    public class Room
    {
        protected bool visited = false;
        protected bool searched = false;

        public virtual string RoomDescription()
        {
            if (!visited)
                return "dust covers the floor here.";
            else
                return "you've been here before.";
        }

        public virtual void OnRoomEntered(Player player)
        {
            visited = true;
        }

        public virtual void OnRoomSearched(Player player)
        {
            if (!searched)
            {
                Console.WriteLine("you look around but find nothing special.");
                searched = true;
            }
            else
            {
                Console.WriteLine("you already searched this room.");
            }
        }

        public virtual void OnRoomExit()
        {
            // default leave message
            Console.WriteLine("you leave the room.");
        }
    }
}
