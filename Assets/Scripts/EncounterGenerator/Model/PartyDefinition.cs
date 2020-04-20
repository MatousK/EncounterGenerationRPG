using System.Collections.Generic;
using Assets.Scripts.Combat;

namespace Assets.Scripts.EncounterGenerator.Model
{
    public class PartyDefinition
    {
        public List<Hero> PartyMembers;

        public float GetPartyStrength()
        {
            float toReturn = 0;
            foreach (var partyMember in PartyMembers)
            {
                toReturn += partyMember.MaxHitpoints * partyMember.Attributes.DealtDamageMultiplier;
            }
            return toReturn;
        }
    }
}
