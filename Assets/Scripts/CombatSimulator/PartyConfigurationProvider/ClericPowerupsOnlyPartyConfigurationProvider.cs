using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: powerups, baseAttack: ClericBaseAttack, attackPowerups: powerups),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: 0, baseAttack: KnightBaseAttack, attackPowerups: 0),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: 0, baseAttack: RangerBaseAttack, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "Cleric Powerups only " + TierIndex.ToString();
        }
    }
}
