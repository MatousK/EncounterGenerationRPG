using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Model
{
    public class PartyDefinition
    {
        public List<Hero> PartyMembers;

        public float GetPartyStrength()
        {
            float toReturn = 0;
            foreach (var partyMember in PartyMembers)
            {
                toReturn += (partyMember.MaxHitpoints * 2) * partyMember.Attributes.DealtDamageMultiplier;
            }
            return toReturn;
        }
    }
}
