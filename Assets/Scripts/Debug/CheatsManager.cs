using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Debug
{
    /// <summary>
    /// The class which is used to give cheats to simplify debugging for the designer.
    /// It has two usages - in editor the developer can set some flags that will make his heroes untargetable.
    /// The other is a normal cheat - entering the text <see cref="godModeCheatCode"/> during the game will set all stats to absurdly high values.
    /// </summary>
    [ExecuteAfter(typeof(InitialPartyManager))]
    public class CheatsManager: MonoBehaviour
    {
        /// <summary>
        /// If true, the knight is untargetable by enemies and thereby invincible.
        /// </summary>
        public bool IsKnightInvincible;
        /// <summary>
        /// If true, the cleric is untargetable by enemies and thereby invincible.
        /// </summary>
        public bool IsClericInvincible;
        /// <summary>
        /// If true, the ranger is untargetable by enemies and thereby invincible.
        /// </summary>
        public bool IsRangerInvincible;
        /// <summary>
        /// The object representing the ranger player character.
        /// </summary>
        private Hero ranger;
        /// <summary>
        /// The object representing the knight player character.
        /// </summary>
        private Hero knight;
        /// <summary>
        /// The object representing the cleric player character.
        /// </summary>
        private Hero cleric;
        /// <summary>
        /// The text which, when entered, will raise the party's stats.
        /// </summary>
        private const string godModeCheatCode = "idkfa";
        /// <summary>
        /// The last few characters written on the keyboard. Once they match <see cref="godModeCheatCode"/>, the cheat will be triggered.
        /// </summary>
        private string currentCheatCode = "";
        private void Start()
        {
            InitHeroReferences();
        }
        /// <summary>
        /// Is executed every frame. Triggers the cheat code if the player entered it and updates the invincibility of the heroes to match the boolean flags on this class.
        /// </summary>
        private void Update()
        {
            if (knight == null)
            {
                InitHeroReferences();
                if (knight == null)
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(UnityEngine.Input.inputString))
            {
                currentCheatCode += UnityEngine.Input.inputString;
                if (currentCheatCode.Length > godModeCheatCode.Length)
                {
                    currentCheatCode = currentCheatCode.Substring(currentCheatCode.Length - godModeCheatCode.Length);
                }

                if (currentCheatCode.ToLower() == godModeCheatCode)
                {
                    EnableGodMode();
                }
                UnityEngine.Debug.Log(currentCheatCode);
            }
            UpdateImmortality();
        }
        /// <summary>
        /// Set attributes of all characters to 10000.
        /// </summary>
        private void EnableGodMode()
        {
            knight.Attributes.DealtDamageMultiplier = 10000;
            knight.SetTotalMaxHp(10000);
            ranger.Attributes.DealtDamageMultiplier = 10000;
            ranger.SetTotalMaxHp(10000);
            cleric.Attributes.DealtDamageMultiplier = 10000;
            cleric.SetTotalMaxHp(10000);
        }
        /// <summary>
        /// Makes sure the character invincibility flags match the flags set on this class.
        /// </summary>
        private void UpdateImmortality()
        {
            cleric.IsInvincible = IsClericInvincible;
            knight.IsInvincible = IsKnightInvincible;
            ranger.IsInvincible = IsRangerInvincible;
        }
        /// <summary>
        /// Initializes the references to all player characters in the game.
        /// If the characters are not spawned yet, this does nothing.
        /// </summary>
        private void InitHeroReferences()
        {
            var allHeroes = FindObjectsOfType<Hero>();
            foreach (var hero in allHeroes)
            {
                switch (hero.HeroProfession)
                {
                    case HeroProfession.Cleric:
                        cleric = hero;
                        break;
                    case HeroProfession.Knight:
                        knight = hero;
                        break;
                    case HeroProfession.Ranger:
                        ranger = hero;
                        break;
                }
            }
        }
    }
}
