using Assets.Scripts.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.EncounterGenerator.Model
{
    public interface IPartyDefinition
    {
        IEnumerable<HeroProfession> GetHeroProfessions();
        float GetHpForHero(HeroProfession heroProfession);
        float GetMaxHpForHero(HeroProfession heroProfession);
        float GetAttackForHero(HeroProfession heroProfession);

        bool IsDown(HeroProfession heroProfession);
    }
}
