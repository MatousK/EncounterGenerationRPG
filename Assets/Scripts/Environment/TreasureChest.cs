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
    public enum TreasureChestDrop
    {
        NotSet,
        HealthBonus,
        DamageBonus,
        HealingPotion
    }

    [Serializable]
    public class PowerupDefinition
    {
        public TreasureChestDrop DropType;
        public GameObject DropObjectTemplate;
    }

    class TreasureChest : MonoBehaviour
    {
        public List<PowerupDefinition> PowerupDefinitions; 
        public TreasureChestDrop TreasureToDrop;
        public Sprite OpenedSprite = null;
        public Sprite ClosedSprite = null;
        GameObject droppedPowerup;
        bool isOpened;
        private GameStateManager gameStateManager;

        private void Start()
        {
            GetComponent<InteractableObject>().OnInteractionTriggered += OnChestClicked;
            UpdateSprite();
        }

        private void Update()
        {
            if (gameStateManager == null)
            {
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager != null)
                {
                    gameStateManager.GameReloaded += GameStateManager_GameReloaded;
                }
            }
        }

        private void OnDestroy()
        {
            if (gameStateManager != null)
            {
                gameStateManager.GameReloaded -= GameStateManager_GameReloaded;
            }
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
            var dropDefinition =
                PowerupDefinitions.First(powerupDefinition => powerupDefinition.DropType == TreasureToDrop);
            SpawnDrop(dropDefinition.DropObjectTemplate);
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