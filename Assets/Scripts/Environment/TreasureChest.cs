using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Effects;
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
        /// <summary>
        /// True if the chest is opened, false if not.
        /// </summary>
        public bool IsOpened;
        public bool AllowPowerupPickup;
        public List<PowerupDefinition> PowerupDefinitions; 
        public TreasureChestDrop TreasureToDrop;
        public Sprite OpenedSprite = null;
        public Sprite ClosedSprite = null;
        GameObject droppedPowerup;
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
            if (!IsOpened)
            {
                IsOpened = true;
                UpdateSprite();
                SpawnDrop();
                UpdateShimmer();
            }
            else if (droppedPowerup != null && AllowPowerupPickup)
            {
                // The chest is opened, and powerup not applied - apply it.
                droppedPowerup.GetComponent<PowerUp>().ApplyPowerup(openingHero);
                Destroy(droppedPowerup);
                droppedPowerup = null;
                UpdateShimmer();
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
            GetComponent<SpriteRenderer>().sprite = IsOpened ? OpenedSprite : ClosedSprite;
        }

        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            IsOpened = false;
            UpdateSprite();
            UpdateShimmer();
        }

        private void UpdateShimmer()
        {
            var shimmerEffect = GetComponentInChildren<InteractableObjectShimmer>();
            if (IsOpened && droppedPowerup == null)
            {
                // Not usable ever again.
                shimmerEffect.ObjectAlreadyUsed = true;
            }
            else if (IsOpened)
            {
                shimmerEffect.transform.localPosition = Vector3.zero;
                shimmerEffect.transform.localScale = Vector3.one;
            }
            else
            {
                shimmerEffect.ObjectAlreadyUsed = false;
                // Ugly, basically forcing the shimmer to be valid for a closed chest.
                shimmerEffect.transform.localPosition = new Vector3(0,-0.1f,0);
                shimmerEffect.transform.localScale = new Vector3(1, 0.8f, 1);
            }
        }
    }
}