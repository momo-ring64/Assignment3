using UnityEngine;
using Assignment_2.Classes;

public class MapManager : MonoBehaviour
{
    [Header("Room Prefabs (0 = Empty, 1 = Treasure, 2 = Encounter)")]
    [SerializeField] private GameObject[] RoomPrefabs;

    [Header("Hallway Prefab")]
    [SerializeField] private GameObject hallwayPrefab;

    [Header("Layout Settings")]
    [SerializeField] private float roomSpacing = 2f;
    public float RoomSpacing => roomSpacing;

    private GameObject[,] spawnedRooms;

    public void CreateMap(Room[,] map, int rows, int columns)
    {
        spawnedRooms = new GameObject[rows, columns];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                GameObject prefabToSpawn = GetPrefabForRoom(map[x, y]);
                Vector3 spawnPos = new Vector3(x * roomSpacing, 0, y * roomSpacing);

                GameObject roomInstance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
                roomInstance.name = $"{map[x, y].GetType().Name}_{x}_{y}";
                spawnedRooms[x, y] = roomInstance;
            }
        }

        // After rooms are placed, spawn hallways
        SpawnHallways(rows, columns);
    }

    private void SpawnHallways(int rows, int columns)
    {
        if (hallwayPrefab == null)
        {
            Debug.LogWarning("Hallway prefab not assigned. Skipping hallway generation.");
            return;
        }

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                GameObject current = spawnedRooms[x, y];
                if (current == null) continue;

                // East connection
                if (y + 1 < columns)
                {
                    Vector3 eastPos = current.transform.position + new Vector3(roomSpacing / 2f, 0, 0);
                    Instantiate(hallwayPrefab, eastPos, Quaternion.identity, transform);
                }

                // South connection
                if (x + 1 < rows)
                {
                    Vector3 southPos = current.transform.position + new Vector3(0, 0, roomSpacing / 2f);
                    Instantiate(hallwayPrefab, southPos, Quaternion.Euler(0, 90, 0), transform);
                }
            }
        }
    }

    private GameObject GetPrefabForRoom(Room room)
    {
        if (RoomPrefabs == null || RoomPrefabs.Length < 3)
        {
            Debug.LogError("RoomPrefabs array not properly set! Needs 3 elements.");
            return null;
        }

        if (room is TreasureRoom)
            return RoomPrefabs[1];
        else if (room is EncounterRoom)
            return RoomPrefabs[2];
        else
            return RoomPrefabs[0];
    }
}
