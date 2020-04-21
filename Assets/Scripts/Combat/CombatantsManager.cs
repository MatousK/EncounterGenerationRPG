using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class CombatantsManager : MonoBehaviour
    {
        public List<Monster> Enemies = new List<Monster>();
        public List<Hero> PlayerCharacters = new List<Hero>();
        public event EventHandler CombatStarted;
        public event EventHandler CombatOver;

        private bool lastFrameCombatActive = false;
        private GameStateManager gameStateManager;
        public bool IsCombatActive => Enemies.Any();

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
                CombatStarted?.Invoke(this, new EventArgs());
            }
            if (lastFrameCombatActive && !IsCombatActive)
            {
                CombatOver?.Invoke(this, new EventArgs());
            }
            lastFrameCombatActive = IsCombatActive;
        }

        public void DestroyPlayerCharacters()
        {
            foreach (var playerCharacter in PlayerCharacters)
            {
                Destroy(playerCharacter.gameObject);
            }
            PlayerCharacters.Clear();
        }

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

        public IEnumerable<CombatantBase> GetAllCombatants()
        {
            IEnumerable<CombatantBase> enemiesCombatantEnumerable = Enemies;
            IEnumerable<CombatantBase> playerCharactersCombatantEnumerable = PlayerCharacters;
            return enemiesCombatantEnumerable.Concat(playerCharactersCombatantEnumerable);
        }

        public IEnumerable<Hero> GetPlayerCharacters(bool onlyAlive = false, bool onlySelected = false)
        {
            return PlayerCharacters.Where(opponent => (!onlyAlive || !opponent.IsDown) && (!onlySelected || opponent.GetComponent<SelectableObject>().IsSelected == true));
        }

        public IEnumerable<Monster> GetEnemies(bool onlyAlive = false)
        {
            return Enemies.Where(opponent => (!onlyAlive || !opponent.IsDown));
        }

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
}
