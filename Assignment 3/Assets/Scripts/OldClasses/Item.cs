using System;

namespace Assignment_2.Classes
{
    // item base class: abstraction for all item types
    public abstract class Item
    {
        public string Name { get; protected set; }

        protected Item(string name)
        {
            Name = name;
        }

        // use the item in a context; returns a brief result message
        public abstract string Use(Player user);
    }
}
