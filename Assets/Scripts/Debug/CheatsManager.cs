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
        public void Start()
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

            UpdateImmortality();
        }

        private void Update()
        {
            UpdateImmortality();
        }

        private void UpdateImmortality()
        {
            cleric.IsInvincible = IsClericInvincible;
            knight.IsInvincible = IsKnightInvincible;
            ranger.IsInvincible = IsRangerInvincible;
        }
    }
}
