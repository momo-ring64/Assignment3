using Assignment_2.Classes;
using UnityEngine;

namespace Assignment_2.Classes
{
    public class GameManager : MonoBehaviour
    {
        private Room[,] map;
        private Player player;
        private int currentX = 1;
        private int currentY = 1;
        private System.Random rand = new System.Random();

        [Header("Map Size")]
        [SerializeField] private int rows = 5;
        [SerializeField] private int columns = 5;

        [Header("Room Distribution")]
        [SerializeField] private int treasureRoomCount = 3;
        [SerializeField] private int encounterRoomCount = 3;

        [Header("References")]
        [SerializeField] private MapManager mapManager; // Drag the MapManager GameObject in Inspector

        private GameObject playerVisual;

        void Start()
        {
            Debug.Log("=== DUNGEON CRAWLER START ===");
            StartGame();
        }

        private void StartGame()
        {
            string name = "Hero";
            player = new Player(name);

            InitializeMap();

            // Delegate visualization to MapManager
            mapManager.CreateMap(map, rows, columns);

            // Spawn player
            SpawnPlayer();
        }

        private void InitializeMap()
        {
            map = new Room[rows, columns];

            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    map[x, y] = new EmptyRoom();

            // randomize room positions
            var coords = new (int x, int y)[rows * columns];
            int idx = 0;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    coords[idx++] = (x, y);

            for (int i = coords.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (coords[i], coords[j]) = (coords[j], coords[i]);
            }

            int totalRooms = rows * columns;
            treasureRoomCount = Mathf.Clamp(treasureRoomCount, 0, totalRooms);
            encounterRoomCount = Mathf.Clamp(encounterRoomCount, 0, totalRooms - treasureRoomCount);

            int index = 0;

            // place treasure rooms
            for (int i = 0; i < treasureRoomCount; i++)
            {
                var c = coords[index++];
                map[c.x, c.y] = new TreasureRoom();
            }

            // place encounter rooms
            for (int i = 0; i < encounterRoomCount; i++)
            {
                var c = coords[index++];
                map[c.x, c.y] = new EncounterRoom();
            }
        }

        private void SpawnPlayer()
        {
            playerVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerVisual.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            playerVisual.GetComponent<Renderer>().material.color = Color.cyan;
            playerVisual.name = "Player";

            playerVisual.transform.position = new Vector3(currentX * 2f, 1f, currentY * 2f);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W)) MovePlayer(-1, 0); // North
            if (Input.GetKeyDown(KeyCode.S)) MovePlayer(1, 0);  // South
            if (Input.GetKeyDown(KeyCode.D)) MovePlayer(0, 1);  // East
            if (Input.GetKeyDown(KeyCode.A)) MovePlayer(0, -1); // West
        }

        private void MovePlayer(int dx, int dy)
        {
            int nx = currentX + dx;
            int ny = currentY + dy;

            if (nx < 0 || ny < 0 || nx >= rows || ny >= columns)
            {
                Debug.Log("You can’t move that way!");
                return;
            }

            currentX = nx;
            currentY = ny;

            playerVisual.transform.position = new Vector3(currentX * 2f, 1f, currentY * 2f);
            Debug.Log($"Moved to room [{currentX},{currentY}] - {map[currentX, currentY].RoomDescription()}");
        }
    }
}
