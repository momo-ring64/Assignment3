using System;

namespace Assignment_2.Classes
{
    public class NormalRoom : Room
    {
        private static string[] descs = new[]
        {
            "a quiet chamber with stone walls.",
            "dust covers the floor here.",
            "old markings are carved into the wall.",
            "it's eerily silent in this room."
        };

        public override string RoomDescription()
        {
            var r = new Random();
            return descs[r.Next(descs.Length)];
        }

        public override void OnRoomEntered(Player player)
        {
            base.OnRoomEntered(player);
            Console.WriteLine("this room feels ordinary.");
        }
    }
}
