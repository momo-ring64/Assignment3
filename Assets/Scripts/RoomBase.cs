using UnityEngine;

namespace Assignment_3.Classes
{
    public abstract class RoomBase : MonoBehaviour
    {
        // Tracks whether the player has already entered this room
        public bool visited = false;

        public abstract string RoomDescription();

        public virtual void EnterRoom(Player player)
        {
            // Mark as visited
            if (!visited)
                visited = true;

            Debug.Log(RoomDescription());
        }
    }
}
