using Assignment_2.Classes;
using Lab_6.Classes;
using System;
using UnityEngine;

namespace Lab_6.Classes
{
    public class GameManager : MonoBehaviour
    {
        private Room[,] map;
        private Player player;
        private int currentX = 1;
        private int currentY = 1;
        private System.Random rand = new System.Random();

        // Unity object representations
        private GameObject[,] roomVisuals;
        private GameObject playerVisual;

        private readonly int rows = 5;
        private readonly int cols = 5;

        void Start()
        {
            Debug.Log("=== DUNGEON CRAWLER START ===");
            StartGame();
        }

        private void StartGame()
        {
            Console.WriteLine("Enter your hero's name: ");
            string name = "Hero"; // placeholder since Unity console can't take input
            player = new Player(name);
            InitializeMap();
            VisualizeMap();
        }

        //Initialize the logical map
        private void InitializeMap()
        {
            map = new Room[rows, cols];

            // fill all with empty
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                    map[x, y] = new EmptyRoom();

            // random positions
            var coords = new (int x, int y)[rows * cols];
            int idx = 0;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < cols; y++)
                    coords[idx++] = (x, y);

            // shuffle coords
            for (int i = coords.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var temp = coords[i];
                coords[i] = coords[j];
                coords[j] = temp;
            }

            // place one treasure and one encounter
            map[coords[0].x, coords[0].y] = new TreasureRoom();
            map[coords[1].x, coords[1].y] = new EncounterRoom();
        }

        // --- Build the visual grid of cubes and a player ---
        private void VisualizeMap()
        {
            roomVisuals = new GameObject[rows, cols];
            float spacing = 2f;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    // create cube for room
                    GameObject roomObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    roomObj.transform.position = new Vector3(x * spacing, 0, y * spacing);

                    // color by type
                    Renderer r = roomObj.GetComponent<Renderer>();
                    if (map[x, y] is TreasureRoom)
                        r.material.color = Color.yellow;
                    else if (map[x, y] is EncounterRoom)
                        r.material.color = Color.red;
                    else
                        r.material.color = Color.gray;

                    roomObj.name = $"Room_{x}_{y}";
                    roomVisuals[x, y] = roomObj;
                }
            }

            // create player visual with a sphere
            playerVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerVisual.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            playerVisual.GetComponent<Renderer>().material.color = Color.cyan;
            playerVisual.name = "Player";

            // offset Y-axis
            playerVisual.transform.position = new Vector3(currentX * spacing, 1f, currentY * spacing);
        }

        //Simple input controls using number keys 1-4
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W)) MovePlayer(-1, 0); // North
            if (Input.GetKeyDown(KeyCode.S)) MovePlayer(1, 0);  // South
            if (Input.GetKeyDown(KeyCode.D)) MovePlayer(0, 1);  // East
            if (Input.GetKeyDown(KeyCode.A)) MovePlayer(0, -1); // West
        }

        //Move player and update their visual position
        private void MovePlayer(int dx, int dy)
        {
            int nx = currentX + dx;
            int ny = currentY + dy;

            if (nx < 0 || ny < 0 || nx >= rows || ny >= cols)
            {
                Debug.Log("You can’t move that way!");
                return;
            }

            currentX = nx;
            currentY = ny;

            float spacing = 2f;
            playerVisual.transform.position = new Vector3(currentX * spacing, 1f, currentY * spacing);
            Debug.Log($"Moved to room [{currentX},{currentY}] - {map[currentX, currentY].RoomDescription()}");
        }
    }
}
