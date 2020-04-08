using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    [Serializable]
    class DropProbability
    {
        public GameObject ObjectToDrop = null;
        /// <summary>
        /// Weight influences the probability of the drop. If one object has weight 6 and second weight 2, the first one will be 3x as likely to drop.
        /// </summary>
        public int Weight = 0;
    }

    class TreasureChest : MonoBehaviour
    {
        public DropProbability[] DropTable = null;
        public Sprite OpenedSprite = null;
        public Sprite ClosedSprite = null;
        GameObject droppedPowerup;
        bool isOpened;
        private GameStateManager gameStateManager;

        private void Awake()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameReloaded += GameStateManager_GameReloaded;
            GetComponent<InteractableObject>().OnInteractionTriggered += OnChestClicked;
            UpdateSprite();
        }

        private void OnChestClicked(object chest, Hero openingHero)
        {
            if (!isOpened)
            {
                isOpened = true;
                UpdateSprite();
                SpawnDrop();
            }
            else if (droppedPowerup != null)
            {
                // The chest is opened, and powerup not applied - apply it.
                droppedPowerup.GetComponent<PowerUp>().ApplyPowerup(openingHero);
                Destroy(droppedPowerup);
            }
        }

        void SpawnDrop()
        {
            var random = new System.Random();
            var totalProbability = DropTable.Sum(dropDefinition => dropDefinition.Weight);
            var itemToDrop = random.Next(totalProbability);
            // We simulate each element being in the array Weight times.
            var itemToDropIndex = 0;
            while (itemToDrop >= DropTable[itemToDropIndex].Weight)
            {
                itemToDrop -= DropTable[itemToDropIndex].Weight;
                itemToDropIndex++;
            }
            SpawnDrop(DropTable[itemToDropIndex].ObjectToDrop);
        }

        void SpawnDrop(GameObject toSpawn)
        {
            droppedPowerup = Instantiate(toSpawn, transform, false);
        }

        void UpdateSprite()
        {
            GetComponent<SpriteRenderer>().sprite = isOpened ? OpenedSprite : ClosedSprite;
        }

        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            isOpened = false;
            UpdateSprite();
        }
    }
}