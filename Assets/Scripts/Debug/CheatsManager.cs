using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Debug
{
    [ExecuteAfter(typeof(InitialPartyManager))]
    public class CheatsManager: MonoBehaviour
    {
        public bool IsKnightInvincible;
        public bool IsClericInvincible;
        public bool IsRangerInvincible;
        private Hero ranger;
        private Hero knight;
        private Hero cleric;
        private const string godModeCheatCode = "idkfa";
        private string currentCheatCode = "";
        public void Start()
        {
            InitHeroReferences();
        }

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
            if (!String.IsNullOrEmpty(UnityEngine.Input.inputString))
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

        private void EnableGodMode()
        {
            // Reenabled before thesis submission.
            knight.Attributes.DealtDamageMultiplier = 10000;
            knight.SetTotalMaxHp(10000);
            ranger.Attributes.DealtDamageMultiplier = 10000;
            ranger.SetTotalMaxHp(10000);
            cleric.Attributes.DealtDamageMultiplier = 10000;
            cleric.SetTotalMaxHp(10000);
        }

        private void UpdateImmortality()
        {
            cleric.IsInvincible = IsClericInvincible;
            knight.IsInvincible = IsKnightInvincible;
            ranger.IsInvincible = IsRangerInvincible;
        }

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
