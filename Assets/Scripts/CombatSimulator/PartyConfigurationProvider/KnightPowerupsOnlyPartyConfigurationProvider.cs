using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    public class KnightPowerupsOnlyPartyConfigurationProvider : PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
        public override PartyConfiguration GetPartyConfiguration()
        {
            int powerups = TierIndex * TierIncrement;
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: 0, attackPowerups: 0),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: powerups, attackPowerups: powerups),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: 0, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "Knight Powerups only " + TierIndex.ToString();
        }
    }
}
