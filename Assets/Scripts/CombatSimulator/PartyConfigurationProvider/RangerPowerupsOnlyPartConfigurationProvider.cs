using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: 0, baseAttack: ClericBaseAttack, attackPowerups: 0),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: 0, baseAttack: KnightBaseAttack, attackPowerups: 0),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: powerups, baseAttack: RangerBaseAttack, attackPowerups: powerups),
            };
        }

        public override string ToString()
        {
            return "Ranger Powerups only " + TierIndex.ToString();
        }
    }
}
