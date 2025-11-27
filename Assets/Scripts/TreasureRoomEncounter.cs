using UnityEngine;
using Assignment_3.Classes;

public class TreasureRoom : RoomBase
{
    private bool searched = false;

    public override string RoomDescription() => "You found a treasure room!";

    public override void EnterRoom(Player player)
    {
        base.EnterRoom(player);
        Debug.Log("Press E to search for treasure.");
    }

    public void Search(Player player)
    {
        if (searched)
        {
            Debug.Log("You already searched this room.");
            return;
        }

        searched = true;

        Item[] loot =
        {
            new Weapon("Iron Dagger", 1, 4),
            new Weapon("Steel Longsword", 1, 8),
            new Consumable("Small Potion", 1, 4),
            new Consumable("Large Potion", 2, 6)
        };

        System.Random rand = new System.Random();
        Item found = loot[rand.Next(loot.Length)];

        player.AddItem(found);
        Debug.Log($"You found a {found.Name}!");
    }
}
