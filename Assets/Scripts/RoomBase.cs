using UnityEngine;

namespace Assignment_3.Classes
{
    public abstract class RoomBase : MonoBehaviour
    {
        public abstract string RoomDescription();

        public virtual void EnterRoom(Player player)
        {
            Debug.Log(RoomDescription());
        }
    }
}
    