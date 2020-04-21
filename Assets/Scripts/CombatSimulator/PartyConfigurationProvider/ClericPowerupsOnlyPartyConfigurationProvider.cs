using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    class ClericPowerupsOnlyPartyConfigurationProvider: PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
        public override PartyConfiguration GetPartyConfiguration()
        {
            int powerups = TierIndex * TierIncrement;
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: powerups, attackPowerups: powerups),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: 0, attackPowerups: 0),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: 0, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "Cleric Powerups only " + TierIndex.ToString();
        }
    }
}
