using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    public class ReconstructionPartyDefinition : IPartyDefinition
    {
        public ReconstructionPartyDefinition(Dictionary<HeroProfession, float> partyHitpoints, Dictionary<HeroProfession, float> partyAttack)
        {
            this.partyAttack = partyAttack;
            this.partyHitpoints = partyHitpoints;
        }

        private Dictionary<HeroProfession, float> partyHitpoints;
        private Dictionary<HeroProfession, float> partyAttack;
        public float GetAttackForHero(HeroProfession heroProfession)
        {
            return partyAttack[heroProfession];
        }

        public IEnumerable<HeroProfession> GetHeroProfessions()
        {
            return partyHitpoints.Keys;
        }

        public float GetHpForHero(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession];
        }

        public float GetMaxHpForHero(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession];
        }

        public bool IsDown(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession] == 0;
        }
    }
}
