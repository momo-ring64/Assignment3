using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assignment_3.Classes
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        public MapManager mapManager;
        public GameObject playerPrefab;

        public Player player;
        public RoomBase[,] map;
        private bool gameOver = false;

        //cameras
        private Camera aliveCamera;
        private Camera deadCamera;
        private Camera winCamera;




        private GameObject playerObj;
        private int xPos = 0;
        private int yPos = 0;

        private Enemy activeEnemy = null;
        private bool inCombat = false;

        [Header("Player Positioning")]
        public Vector3 playerOffset = Vector3.up;

        void Start()
        {
            Debug.Log("=== GAME START ===");

            player = new Player("Hero");
            map = mapManager.GenerateMap();

            SpawnPlayer();
            EnterCurrentRoom();
        }

        private void SpawnPlayer()
        {
            xPos = 0;
            yPos = 0;

            playerObj = Instantiate(
                playerPrefab,
                mapManager.GetRoomWorldPosition(xPos, yPos) + playerOffset,
                Quaternion.identity
            );

            // Correct camera assignment
            aliveCamera = playerObj.transform.Find("AliveCamera")?.GetComponent<Camera>();
            deadCamera = playerObj.transform.Find("DeadCamera")?.GetComponent<Camera>();
            winCamera = playerObj.transform.Find("WinCamera")?.GetComponent<Camera>();

            if (aliveCamera == null || deadCamera == null || winCamera == null)
            {
                Debug.LogError("One or more cameras missing! Player prefab must include AliveCamera, DeadCamera, WinCamera.");
                return;
            }

            // Initial camera states
            aliveCamera.enabled = true;
            deadCamera.enabled = false;
            winCamera.enabled = false;

        }



        void Update()
        {
            if (gameOver)
                return; // NO INPUT WHEN DEAD

            if (inCombat)
            {
                CombatInput();
                return;
            }

            RoomBase room = mapManager.GetRoom(xPos, yPos);

            // treasure search
            if (room is TreasureRoom tr && Input.GetKeyDown(KeyCode.E))
                tr.Search(player);

            // always available
            if (Input.GetKeyDown(KeyCode.I)) PrintInventory();
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
            if (Input.GetKeyDown(KeyCode.P)) player.ConsumePotion();

            MovementUpdate();
        }

        private void PlayerDied()
        {
            inCombat = false;
            gameOver = true;

            // SWITCH CAMERAS PROPERLY
            if (aliveCamera != null)
            {
                aliveCamera.enabled = false;
                aliveCamera.gameObject.SetActive(false);
            }

            if (deadCamera != null)
            {
                deadCamera.gameObject.SetActive(true);
                deadCamera.enabled = true;
            }

            Debug.Log("YOU DIED.");
        }




        private void PrintInventory()
        {
            Debug.Log("=== WEAPONS ===");
            var w = player.GetWeapons();
            for (int i = 0; i < w.Count; i++)
                Debug.Log($"{i + 1}. {w[i].Name} ({w[i].DiceCount}d{w[i].DiceSides})");

            Debug.Log("=== POTIONS ===");
            var p = player.GetPotions();
            for (int i = 0; i < p.Count; i++)
                Debug.Log($"{i + 1}. {p[i].Name} ({p[i].DiceCount}d{p[i].DiceSides})");
        }

        private void SelectWeapon(int index)
        {
            if (player.GetWeapons().Count > index)
            {
                player.SelectedWeaponIndex = index;
                Debug.Log($"Selected weapon: {player.GetSelectedWeapon().Name}");
            }
        }

        // Movement
        private void MovementUpdate()
        {
            if (Input.GetKeyDown(KeyCode.W)) TryMove(-1, 0);
            if (Input.GetKeyDown(KeyCode.S)) TryMove(1, 0);
            if (Input.GetKeyDown(KeyCode.A)) TryMove(0, -1);
            if (Input.GetKeyDown(KeyCode.D)) TryMove(0, 1);
        }

        private void TryMove(int dx, int dy)
        {
            int nx = xPos + dx;
            int ny = yPos + dy;

            if (mapManager.GetRoom(nx, ny) == null)
            {
                Debug.Log("You can't move that way!");
                return;
            }

            xPos = nx;
            yPos = ny;

            playerObj.transform.position =
                mapManager.GetRoomWorldPosition(xPos, yPos) + playerOffset;

            EnterCurrentRoom();
        }

        private void TriggerWinState()
        {
            Debug.Log("YOU WIN!");

            inCombat = false;
            gameOver = true;

            // disable alive camera
            if (aliveCamera != null)
            {
                aliveCamera.enabled = false;
                aliveCamera.gameObject.SetActive(false);
            }

            // disable dead cam just in case
            if (deadCamera != null)
            {
                deadCamera.enabled = false;
                deadCamera.gameObject.SetActive(false);
            }

            // enable win cam
            if (winCamera != null)
            {
                winCamera.gameObject.SetActive(true);
                winCamera.enabled = true;
            }

            // disable input/dead camera UI etc.
        }

        private void CheckWinCondition()
        {
            // If you're already dead, never win
            if (gameOver)
                return;

            int totalRooms = map.GetLength(0) * map.GetLength(1);
            int visited = 0;

            foreach (var room in map)
            {
                if (room != null && room.visited)
                    visited++;
            }

            if (visited >= totalRooms)
            {
                TriggerWinState();
            }
        }


        private void EnterCurrentRoom()
        {
            RoomBase room = mapManager.GetRoom(xPos, yPos);
            room.EnterRoom(player);

            if (room is CombatRoom combatRoom)
            {
                if (!combatRoom.cleared)
                    StartCombat(combatRoom);
            }
            CheckWinCondition();

        }



        // combat System

        private CombatRoom currentCombatRoom;
        private void StartCombat(CombatRoom room)
        {
            currentCombatRoom = room;
            activeEnemy = new Enemy("Goblin", 12);
            inCombat = true;

            Debug.Log($"A {activeEnemy.Name} appears! HP: {activeEnemy.HitPoints}");
            PrintInventory();
            Debug.Log("Select a weapon (1–3), then press F to attack.");
        }


        private void CombatInput()
        {
            // attack
            if (Input.GetKeyDown(KeyCode.F))
            {
                Weapon weapon = player.GetSelectedWeapon();

                int dmg = (weapon != null)
                    ? weapon.RollDamage()
                    : Random.Range(1, 4);   // bare-handed attack

                activeEnemy.HitPoints -= dmg;
                Debug.Log($"You hit for {dmg}! Enemy HP: {activeEnemy.HitPoints}");

                if (activeEnemy.HitPoints <= 0)
                {
                    Debug.Log("You defeated the enemy!");
                    currentCombatRoom.cleared = true;   // <--- prevent respawn
                    inCombat = false;
                    activeEnemy = null;
                    return;
                }


                // enemy counterattack
                int eDmg = activeEnemy.RollAttack();
                player.ReceiveDamage(eDmg);

                if (player.HitPoints <= 0)
                {
                    PlayerDied();
                    return;
                }

            }

            // use potion
            if (Input.GetKeyDown(KeyCode.Q))
                player.ConsumePotion();

            // weapon selection during combat
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
        }

        //ui functions
        public void TryAgain()
        {
            Debug.Log("Restarting game...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }




        public void QuitGame()
        {
            Debug.Log("Quit pressed.");

            // Works only in build
            Application.Quit();

#if UNITY_EDITOR
            // In editor — just log
            Debug.Log("QuitGame() called – Application.Quit() only works in a build.");
#endif
        }


    }
}
