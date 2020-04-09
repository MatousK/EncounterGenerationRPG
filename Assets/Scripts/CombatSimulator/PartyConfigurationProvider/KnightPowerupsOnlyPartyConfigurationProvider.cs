using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: 0, baseAttack: ClericBaseAttack, attackPowerups: 0),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: powerups, baseAttack: KnightBaseAttack, attackPowerups: powerups),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: 0, baseAttack: RangerBaseAttack, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "Knight Powerups only " + TierIndex.ToString();
        }
    }
}
