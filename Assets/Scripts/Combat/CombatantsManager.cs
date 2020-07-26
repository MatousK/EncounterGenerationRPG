using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// A class which has references to all combatants in the game.
    /// </summary>
    public class CombatantsManager : MonoBehaviour
    {
        /// <summary>
        /// All enemies currently in the game.
        /// </summary>
        public List<Monster> Enemies = new List<Monster>();
        /// <summary>
        /// All player characters currently in the game.
        /// </summary>
        public List<Hero> PlayerCharacters = new List<Hero>();
        /// <summary>
        /// An event raised when a combat is started.
        /// </summary>
        public event EventHandler<CombatStartedEventArgs> CombatStarted;
        /// <summary>
        /// An event raised when a combat is over.
        /// </summary>
        public event EventHandler CombatOver;
        /// <summary>
        /// If true, in the last frame a combat was active.
        /// Used to detect when we switch to and from combat.
        /// </summary>
        private bool lastFrameCombatActive = false;
        /// <summary>
        /// An object with events which are fired on game over and reload.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// If true, we are in combat right now.
        /// </summary>
        public bool IsCombatActive => Enemies.Any();
        /// <summary>
        /// If true, the current combat is a boss fight.
        /// </summary>
        public bool IsBossFight;

        private void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.GameReloaded += GameStateManager_GameReloaded;
            }
        }

        private void Update()
        {
            if (!lastFrameCombatActive && IsCombatActive)
            {
                CombatStarted?.Invoke(this, new CombatStartedEventArgs {IsBossFight = IsBossFight});
            }
            if (lastFrameCombatActive && !IsCombatActive)
            {
                CombatOver?.Invoke(this, new EventArgs());
            }
            lastFrameCombatActive = IsCombatActive;
        }
        /// <summary>
        /// Destroys the objects of all player characters and clears <see cref="PlayerCharacters"/>.
        /// </summary>
        public void DestroyPlayerCharacters()
        {
            foreach (var playerCharacter in PlayerCharacters)
            {
                Destroy(playerCharacter.gameObject);
            }
            PlayerCharacters.Clear();
        }
        /// <summary>
        /// Retrieves all allies of the specified combatants with specified filters.
        /// </summary>
        /// <param name="combatant">The combatant whose allies are requested.</param>
        /// <param name="onlyAlive">If true, we only want the combatants who are not down.</param>
        /// <param name="onlySelected">If true, we only want the combatants who are selected by the player.</param>
        /// <returns>The requested list of allied combatants.</returns>
        public IEnumerable<CombatantBase> GetAlliesFor(CombatantBase combatant, bool onlyAlive = false, bool onlySelected = false)
        {
            if (combatant is Hero)
            {
                return GetPlayerCharacters(onlyAlive, onlySelected);
            }
            if (combatant is Monster)
            {
                return GetEnemies(onlyAlive);
            }
            UnityEngine.Debug.Assert(false, "Asked for allies for unknown combatant.");
            return null;
        }
        /// <summary>
        /// Get opponents the specified combatant.
        /// </summary>
        /// <param name="combatant">The combatant whose opponents are requested.</param>
        /// <param name="onlyAlive">If true, we only want opponents who are alive.</param>
        /// <param name="onlySelected">If true, we want only opponents who are selected by the player.</param>
        /// <returns>The requested list of opponent combatants.</returns>
        public IEnumerable<CombatantBase> GetOpponentsFor(CombatantBase combatant, bool onlyAlive = false, bool onlySelected = false)
        {
            IEnumerable<CombatantBase> opponents = null;
            if (combatant is Hero)
            {
                return GetEnemies(onlyAlive);
            }
            else if (combatant is Monster)
            {
                return GetPlayerCharacters(onlyAlive, onlySelected);
            }
            if (opponents == null)
            {
                UnityEngine.Debug.Assert(false, "Asked for opponents for unknown combatant.");
                return null;
            }
            return null;
        }
        /// <summary>
        /// Retrieves all combatants in the game.
        /// </summary>
        /// <returns>List of all combatants.</returns>
        public IEnumerable<CombatantBase> GetAllCombatants()
        {
            IEnumerable<CombatantBase> enemiesCombatantEnumerable = Enemies;
            IEnumerable<CombatantBase> playerCharactersCombatantEnumerable = PlayerCharacters;
            return enemiesCombatantEnumerable.Concat(playerCharactersCombatantEnumerable);
        }
        /// <summary>
        /// Retrieve player characters fitting the specified criteria.
        /// </summary>
        /// <param name="onlyAlive">If true, returns only player characters who are alive.</param>
        /// <param name="onlySelected">If true, returns only player characters who are selected.</param>
        /// <returns>The filtered list of player characters.</returns>
        public IEnumerable<Hero> GetPlayerCharacters(bool onlyAlive = false, bool onlySelected = false)
        {
            return PlayerCharacters.Where(opponent => (!onlyAlive || !opponent.IsDown) && (!onlySelected || opponent.GetComponent<SelectableObject>().IsSelected == true));
        }
        /// <summary>
        /// Retrieves the list of all enemies in the game.
        /// </summary>
        /// <param name="onlyAlive">If true, return only enemies who are alive.</param>
        /// <returns>The filtered list of enemies.</returns>
        public IEnumerable<Monster> GetEnemies(bool onlyAlive = false)
        {
            return Enemies.Where(opponent => (!onlyAlive || !opponent.IsDown));
        }
        /// <summary>
        /// Called when the game is reloaded. Destroys all monsters.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            // Game is being reloaded - kill all monsters, restore players.
            foreach (var monster in Enemies)
            {
                Destroy(monster.gameObject);
            }
            Enemies.Clear();
        }
    }
    /// <summary>
    /// Arguments for the event raised when a combat is started.
    /// </summary>
    public class CombatStartedEventArgs : EventArgs
    {
        /// <summary>
        /// If true, this combat is a boss fight.
        /// </summary>
        public bool IsBossFight;
    }
}
