using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    class RangerPowerupsOnlyPartConfigurationProvider: PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
        public override PartyConfiguration GetPartyConfiguration()
        {
            int powerups = TierIndex * TierIncrement;
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: 0, attackPowerups: 0),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: 0, attackPowerups: 0),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: powerups, attackPowerups: powerups),
            };
        }

        public override string ToString()
        {
            return "Ranger Powerups only " + TierIndex.ToString();
        }
    }
}
