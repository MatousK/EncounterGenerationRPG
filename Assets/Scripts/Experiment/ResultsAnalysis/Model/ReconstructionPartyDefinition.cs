using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EncounterGenerator.Algorithm;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// Party definition for the <see cref="EncounterMatrixUpdater"/>.
    /// </summary>
    public class ReconstructionPartyDefinition : IPartyDefinition
    {
        /// <summary>
        /// Create the party definition from party hit points and attack.
        /// </summary>
        /// <param name="partyHitpoints">The current hit points of the party.</param>
        /// <param name="partyAttack">The current attack of the party.</param>
        public ReconstructionPartyDefinition(Dictionary<HeroProfession, float> partyHitpoints, Dictionary<HeroProfession, float> partyAttack)
        {
            this.partyAttack = partyAttack;
            this.partyHitpoints = partyHitpoints;
        }
        /// <summary>
        /// How many hit points does each party member have.
        /// </summary>
        private Dictionary<HeroProfession, float> partyHitpoints;
        /// <summary>
        /// Attack of each party member.
        /// </summary>
        private Dictionary<HeroProfession, float> partyAttack;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetAttackForHero(HeroProfession heroProfession)
        {
            return partyAttack[heroProfession];
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public IEnumerable<HeroProfession> GetHeroProfessions()
        {
            return partyHitpoints.Keys;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetHpForHero(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession];
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetMaxHpForHero(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession];
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool IsDown(HeroProfession heroProfession)
        {
            return partyHitpoints[heroProfession] == 0;
        }
    }
}
