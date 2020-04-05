﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class CombatantsManager : MonoBehaviour
    {
        public List<Monster> Enemies = new List<Monster>();
        public List<Hero> PlayerCharacters = new List<Hero>();

        public bool IsCombatActive => Enemies.Any();

        public IEnumerable<CombatantBase> GetAlliesFor(CombatantBase combatant, bool onlyAlive = false, bool onlySelected = false)
        {
            IEnumerable<CombatantBase> opponents = null;
            if (combatant is Hero)
            {
                return GetPlayerCharacters(onlyAlive, onlySelected);
            }
            else if (combatant is Monster)
            {
                return GetEnemies(onlyAlive);
            }
            if (opponents == null)
            {
                Debug.Assert(false, "Asked for allies for unknown combatant.");
                return null;
            }
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
                Debug.Assert(false, "Asked for opponents for unknown combatant.");
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
    }
}
