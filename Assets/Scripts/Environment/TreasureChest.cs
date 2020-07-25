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
    /// <summary>
    /// All the things that can drop from a chest.
    /// </summary>
    public enum TreasureChestDrop
    {
        /// <summary>
        /// Nothing set, this should never happen during the game.
        /// </summary>
        NotSet,
        /// <summary>
        /// Max HP bonus should drop from the chest.
        /// </summary>
        HealthBonus,
        /// <summary>
        /// Damage bonus should drop from the chest.
        /// </summary>
        DamageBonus,
        /// <summary>
        /// A healing potion should drop from the chest.
        /// </summary>
        HealingPotion
    }
    /// <summary>
    /// For some <see cref="TreasureChestDrop"/>, define what is the game object that should actually spawn when the chest is opened.
    /// </summary>
    [Serializable]
    public class PowerupDefinition
    {
        /// <summary>
        /// The relevant type of drop.
        /// </summary>
        public TreasureChestDrop DropType;
        /// <summary>
        /// The object that should spawn for this drop.
        /// </summary>
        public GameObject DropObjectTemplate;
    }

    class TreasureChest : MonoBehaviour
    {
        /// <summary>
        /// True if the chest is opened, false if not.
        /// </summary>
        public bool IsOpened;
        /// <summary>
        /// If true, the hero can pick up what is in the chest.
        /// This is a workaround, starts false and gets set to true only when clicking on an open chest.
        /// Ensures that the player cannot give a command to pick up a power up while the chest is still closed.
        /// </summary>
        public bool AllowPowerupPickup;
        /// <summary>
        /// For each possible drop type defines what object should spawn to represent it.
        /// We cannot use a dictionary, as those cannot be exposed to editor.
        /// </summary>
        public List<PowerupDefinition> PowerupDefinitions; 
        /// <summary>
        /// What should this chest drop when opened.
        /// </summary>
        public TreasureChestDrop TreasureToDrop;
        /// <summary>
        /// How should this chest look when opened.
        /// </summary>
        public Sprite OpenedSprite = null;
        /// <summary>
        /// How should this chest look when closed.
        /// </summary>
        public Sprite ClosedSprite = null;
        /// <summary>
        /// The spawned powerup. Null before a power up is spawned or if the player picked it up.
        /// </summary>
        GameObject droppedPowerup;
        /// <summary>
        /// The game state manager notifies this object about a game reload to close the chest and reset the drop.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// Called before the first update, binds to events and initializes the initial chest sprite.
        /// </summary>
        private void Start()
        {
            GetComponent<InteractableObject>().OnInteractionTriggered += OnChestUsed;
            UpdateSprite();
        }
        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            // Hacky, because of order of initialization hell, GameStateManager does not exist on start, so we have to keep trying until it appears.
            if (gameStateManager == null)
            {
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager != null)
                {
                    gameStateManager.GameReloaded += GameStateManager_GameReloaded;
                }
            }
        }
        /// <summary>
        /// When destroyed, unbind from events.
        /// </summary>
        private void OnDestroy()
        {
            if (gameStateManager != null)
            {
                gameStateManager.GameReloaded -= GameStateManager_GameReloaded;
            }
        }
        /// <summary>
        /// When a closed chest is used, open it.
        /// If an opened chest is used, apply the power up if it was not applied.
        /// Otherwise do nothing.
        /// </summary>
        /// <param name="chest">The sender of the event.</param>
        /// <param name="openingHero">The hero who opened the chest.</param>
        private void OnChestUsed(object chest, Hero openingHero)
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
        /// <summary>
        /// Creates the object representing the power up and spawns it over the chest.
        /// </summary>
        void SpawnDrop()
        {
            var dropDefinition =
                PowerupDefinitions.First(powerupDefinition => powerupDefinition.DropType == TreasureToDrop);
            SpawnDrop(dropDefinition.DropObjectTemplate);
        }
        /// <summary>
        /// Creates specified object and spawns it over the chest.
        /// </summary>
        /// <param name="toSpawn">The power up object to spawn.</param>
        void SpawnDrop(GameObject toSpawn)
        {
            droppedPowerup = Instantiate(toSpawn, transform, false);
        }
        /// <summary>
        /// Update the sprite of this object, based on whether it is closed or opened.
        /// </summary>
        void UpdateSprite()
        {
            GetComponent<SpriteRenderer>().sprite = IsOpened ? OpenedSprite : ClosedSprite;
        }
        /// <summary>
        /// When the game is reloaded, close the chest.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            IsOpened = false;
            UpdateSprite();
            UpdateShimmer();
        }
        /// <summary>
        /// Update the effect around the chest, indicating that it is usable.
        /// It is visible either if chest is opened or if a powerup can be picked up.
        /// </summary>
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
                // Ugly, scaling the shimmer to be valid for a closed chest.
                shimmerEffect.transform.localPosition = new Vector3(0,-0.1f,0);
                shimmerEffect.transform.localScale = new Vector3(1, 0.8f, 1);
            }
        }
    }
}