using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.Model
{
    public class CombatOverLine: CsvLine
    {
        public Dictionary<HeroProfession, float> PartyStartHitpoints;
        public Dictionary<HeroProfession, float> PartyEndHitpoints;
        public Dictionary<HeroProfession, float> PartAttack;
        public EncounterDefinition CombatEncounter;
        public float ExpectedDifficulty;
        public float RealDifficulty;
        public bool WasGameOver;
        public bool WasStaticEncounter;
        public bool WasLogged;
    }
}