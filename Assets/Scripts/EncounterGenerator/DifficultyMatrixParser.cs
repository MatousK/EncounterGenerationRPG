using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    public static class DifficultyMatrixParser
    {
        public static List<DifficultyMatrixSourceLine> ParseFile(TextReader reader)
        {
            var toReturn = new List<DifficultyMatrixSourceLine>();
            // Skip through separator and headers.
            reader.ReadLine();
            reader.ReadLine();
            var currentLine = reader.ReadLine();
            // To match the line number in excel, we start at line index 2;
            var currentIndex = 2;
            while (!string.IsNullOrEmpty(currentLine))
            {
                var parsedLine = ParseLine(currentLine, currentIndex++);
                if (parsedLine != null)
                {
                    toReturn.Add(parsedLine);
                }
                currentLine = reader.ReadLine();
            }
            return toReturn;
        }
        private static DifficultyMatrixSourceLine ParseLine(String line, int lineIndex)
        {
            string[] values = line.Split(';');
            Debug.Assert(values.Length == 34);
            // Hardcoded column indexes, not exactly pretty, but for this case we do not need extensibility, we definitely wont be rerunning the simulator with different matrix format.
            return new DifficultyMatrixSourceLine
            {
                TestIndex = int.Parse(values[0]),
                MonsterTier = int.Parse(values[1]),
                PartyConfiguration = values[2],
                PartyStrength = float.Parse(values[3]),
                MaxHpLost = float.Parse(values[4]),
                HpLost = float.Parse(values[5]),
                HeroCombatStatuses = new Dictionary<HeroProfession, HeroCombatStatus>
                    {
                        {HeroProfession.Ranger, new HeroCombatStatus
                        {
                            WasKilled = values[6] == "1",
                            Attack = float.Parse(values[9]),
                            Hp = float.Parse(values[10]),
                            MaxHpLost = float.Parse(values[11]),
                            HpLost = float.Parse(values[12]),
                        } },
                        {HeroProfession.Cleric, new HeroCombatStatus
                        {
                            WasKilled = values[7] == "1",
                            Attack = float.Parse(values[13]),
                            Hp = float.Parse(values[14]),
                            MaxHpLost = float.Parse(values[15]),
                            HpLost = float.Parse(values[16]),
                        } },
                        {HeroProfession.Knight, new HeroCombatStatus
                        {
                            WasKilled = values[8] == "1",
                            Attack = float.Parse(values[17]),
                            Hp = float.Parse(values[18]),
                            MaxHpLost = float.Parse(values[19]),
                            HpLost = float.Parse(values[20]),
                        } }
                    },
                EncounterDefinition = new EncounterDefinition
                {
                    AllEncounterGroups = new List<MonsterGroup> {
                        { new MonsterGroup(new MonsterType(MonsterRank.Minion, MonsterRole.Minion), int.Parse(values[21])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Brute), int.Parse(values[22])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Brute), int.Parse(values[23])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Brute), int.Parse(values[24])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Leader), int.Parse(values[25])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Leader), int.Parse(values[26])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Leader), int.Parse(values[27])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Lurker), int.Parse(values[28])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Lurker), int.Parse(values[29])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Lurker), int.Parse(values[30])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Sniper), int.Parse(values[31])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Sniper), int.Parse(values[32])) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Sniper), int.Parse(values[33])) },
                    }
                }
            };

        }
    }

    public class DifficultyMatrixSourceLine
    {
        public int TestIndex;
        public int MonsterTier;
        public string PartyConfiguration;
        public float PartyStrength;
        public float MaxHpLost;
        public float HpLost;
        public Dictionary<HeroProfession, HeroCombatStatus> HeroCombatStatuses;
        public EncounterDefinition EncounterDefinition;
    }

    public class HeroCombatStatus
    {
        public bool WasKilled;
        public float Attack;
        public float MaxHp;
        public float Hp;
        public float MaxHpLost;
        public float HpLost;
    }
}
