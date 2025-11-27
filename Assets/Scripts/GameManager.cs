using UnityEngine;

namespace Assignment_3.Classes
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        public MapManager mapManager;
        public GameObject playerPrefab;

        public Player player;
        public RoomBase[,] map;

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
        }

        void Update()
        {
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
            if (Input.GetKeyDown(KeyCode.E)) PrintInventory();
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);

            if (Input.GetKeyDown(KeyCode.C)) player.ConsumePotion();

            MovementUpdate();
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

        private void EnterCurrentRoom()
        {
            RoomBase room = mapManager.GetRoom(xPos, yPos);
            room.EnterRoom(player);

            if (room is CombatRoom)
                StartCombat();
        }

       
        // combat System
    
        private void StartCombat()
        {
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
                    inCombat = false;
                    return;
                }

                // enemy counterattack
                int eDmg = activeEnemy.RollAttack();
                player.ReceiveDamage(eDmg);

                if (player.HitPoints <= 0)
                {
                    Debug.Log("YOU DIED.");
                    inCombat = false;
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

    }
}
