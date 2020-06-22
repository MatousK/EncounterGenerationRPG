using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;

namespace Assets.Scripts.EncounterGenerator.Model
{
    public class PartyDefinition: IPartyDefinition
    {
        public List<Hero> PartyMembers;

        public float GetAttackForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).Attributes.DealtDamageMultiplier;
        }

        public IEnumerable<HeroProfession> GetHeroProfessions()
        {
            return PartyMembers.Select(partyMember => partyMember.HeroProfession);
        }

        public float GetHpForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).HitPoints;
        }

        public float GetMaxHpForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).MaxHitpoints;
        }
        public bool IsDown(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).IsDown;
        }

        public float GetPartyStrength()
        {
            float toReturn = 0;
            foreach (var partyMember in PartyMembers)
            {
                toReturn += partyMember.MaxHitpoints * partyMember.Attributes.DealtDamageMultiplier;
            }
            return toReturn;
        }

        private Hero GetHero(HeroProfession profession)
        {
            return PartyMembers.First(hero => hero.HeroProfession == profession);
        }
    }
}
