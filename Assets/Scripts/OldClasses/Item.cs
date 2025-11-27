using UnityEngine;

namespace Assignment_3.Classes
{
    public abstract class Item
    {
        public string Name { get; private set; }

        protected Item(string name)
        {
            Name = name;
        }
    }
}
