using UnityEngine;
using System.Collections.Generic;

namespace Assignment_3.Classes
{
    public class MapManager : MonoBehaviour
    {
        [Header("Map Size")]
        public int rows = 5;
        public int columns = 5;

        [Header("Room Prefabs")]
        public GameObject normalRoomPrefab;
        public GameObject treasureRoomPrefab;
        public GameObject combatRoomPrefab;

        [Header("Spacing")]
        public float roomSpacing = 6f;

        [Header("Special Room Counts")]
        public int treasureRooms = 3;
        public int combatRooms = 3;

        private RoomBase[,] map;
        private System.Random rand = new System.Random();

        public RoomBase[,] GenerateMap()
        {

            map = new RoomBase[rows, columns];

            //lList of all coordinates
            List<(int x, int y)> coords = new List<(int x, int y)>();
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    coords.Add((x, y));

            // shuffle coordinates
            for (int i = coords.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (coords[i], coords[j]) = (coords[j], coords[i]);
            }

            int index = 0;

            // assign treasure rooms
            for (int i = 0; i < treasureRooms; i++)
            {
                var c = coords[index++];
                map[c.x, c.y] = InstantiateRoom(treasureRoomPrefab, c.x, c.y);
            }

            // assign combat rooms
            for (int i = 0; i < combatRooms; i++)
            {
                var c = coords[index++];
                map[c.x, c.y] = InstantiateRoom(combatRoomPrefab, c.x, c.y);
            }

            // fill remaining with normal rooms
            while (index < coords.Count)
            {
                var c = coords[index++];
                map[c.x, c.y] = InstantiateRoom(normalRoomPrefab, c.x, c.y);
            }

            return map;
        }

        private RoomBase InstantiateRoom(GameObject prefab, int x, int y)
        {
            Vector3 pos = GetRoomWorldPosition(x, y);
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

            RoomBase room = obj.GetComponent<RoomBase>();
            if (room == null)
                Debug.LogError(prefab.name + " is missing a RoomBase-derived script!");

            return room;
        }

        public RoomBase GetRoom(int x, int y)
        {
            if (x < 0 || y < 0 || x >= rows || y >= columns)
                return null;
            return map[x, y];
        }

        public Vector3 GetRoomWorldPosition(int x, int y)
        {
            return new Vector3(
                x * roomSpacing + roomSpacing / 2f,
                0f,
                y * roomSpacing + roomSpacing / 2f
            );
        }

    }
}
