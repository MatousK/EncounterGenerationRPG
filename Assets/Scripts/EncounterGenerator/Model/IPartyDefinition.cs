using Assets.Scripts.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EncounterGenerator.Algorithm;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// An interface for a representation of the party. Used by the <see cref="EncounterMatrixUpdater"/>
    /// </summary>
    public interface IPartyDefinition
    {
        /// <summary>
        /// Retrieve all professions of heroes in this party.
        /// </summary>
        /// <returns>All professions of heroes in this party.</returns>
        IEnumerable<HeroProfession> GetHeroProfessions();
        /// <summary>
        /// Retrieve the current HP of a specific hero.
        /// </summary>
        /// <param name="heroProfession">Hero whose HP we want.</param>
        /// <returns>HP of the hero.</returns>
        float GetHpForHero(HeroProfession heroProfession);
        /// <summary>
        /// Retrieve the current max HP of a specific hero.
        /// </summary>
        /// <param name="heroProfession">Hero whose max HP we want.</param>
        /// <returns>Max HP of the hero.</returns>
        float GetMaxHpForHero(HeroProfession heroProfession);
        /// <summary>
        /// Retrieve the current attack of a specific hero.
        /// </summary>
        /// <param name="heroProfession">Hero whose attack we want.</param>
        /// <returns>Attack of the hero.</returns>
        float GetAttackForHero(HeroProfession heroProfession);
        /// <summary>
        /// If true, the hero is currently defeated.
        /// </summary>
        /// <param name="heroProfession">Hero whose alive status is in question.</param>
        /// <returns>True if the hero is defeated, otherwise false.</returns>
        bool IsDown(HeroProfession heroProfession);
    }
}
