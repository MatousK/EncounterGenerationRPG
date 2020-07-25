using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Information about the party for the encounter generator.
    /// Uses the actual party from the game as its data source.
    /// </summary>
    public class PartyDefinition: IPartyDefinition
    {
        /// <summary>
        /// List of heroes in the party.
        /// </summary>
        public List<Hero> PartyMembers;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetAttackForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).Attributes.DealtDamageMultiplier;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public IEnumerable<HeroProfession> GetHeroProfessions()
        {
            return PartyMembers.Select(partyMember => partyMember.HeroProfession);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetHpForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).HitPoints;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public float GetMaxHpForHero(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).MaxHitpoints;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="heroProfession"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public bool IsDown(HeroProfession heroProfession)
        {
            return GetHero(heroProfession).IsDown;
        }
        /// <summary>
        /// Calculates the strength of this party.
        /// </summary>
        /// <returns>The strength of this party.</returns>
        public float GetPartyStrength()
        {
            float toReturn = 0;
            foreach (var partyMember in PartyMembers)
            {
                toReturn += partyMember.MaxHitpoints * partyMember.Attributes.DealtDamageMultiplier;
            }
            return toReturn;
        }
        /// <summary>
        /// Finds the hero with this profession represented by this party.
        /// </summary>
        /// <param name="profession">The hero with this profession.</param>
        /// <returns>The hero with this profession.</returns>
        private Hero GetHero(HeroProfession profession)
        {
            return PartyMembers.First(hero => hero.HeroProfession == profession);
        }
    }
}
