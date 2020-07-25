using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    /// <summary>
    /// The class for parsing the difficulty matrix created by the <see cref="CombatSimulator.CombatSimulator"./>
    /// </summary>
    public static class DifficultyMatrixParser
    {
        /// <summary>
        /// Parses the entire CSV file and return the lines in some more readable format.
        /// </summary>
        /// <param name="reader">The stream containing the CSV file.</param>
        /// <returns>All matrix lines in the CSV file.</returns>
        public static List<DifficultyMatrixSourceLine> ParseFile(TextReader reader)
        {
            var toReturn = new List<DifficultyMatrixSourceLine>();
            // Skip through separator and headers.
            reader.ReadLine();
            reader.ReadLine();
            // Read line by line until the matrix is empty, parsing a line and adding it to the output list.
            var currentLine = reader.ReadLine();
            while (!string.IsNullOrEmpty(currentLine))
            {
                var parsedLine = ParseLine(currentLine);
                if (parsedLine != null)
                {
                    toReturn.Add(parsedLine);
                }
                currentLine = reader.ReadLine();
            }
            return toReturn;
        }
        /// <summary>
        /// Parse a single line from the CSV.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>Parsed line from the CSV.</returns>
        private static DifficultyMatrixSourceLine ParseLine(string line)
        {
            string[] values = line.Split(';');
            UnityEngine.Debug.Assert(values.Length == 34);
            // Hardcoded column indexes, not exactly pretty, but for this case we do not need extensibility, we definitely wont be rerunning the simulator with different matrix format.
            return new DifficultyMatrixSourceLine
            {
                TestIndex = int.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat),
                MonsterTier = int.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                PartyConfiguration = values[2],
                PartyStrength = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                MaxHpLost = float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat),
                HpLost = float.Parse(values[5], CultureInfo.InvariantCulture.NumberFormat),
                HeroCombatStatuses = new Dictionary<HeroProfession, HeroCombatStatus>
                    {
                        {HeroProfession.Ranger, new HeroCombatStatus
                        {
                            WasKilled = values[6] == "1",
                            Attack = float.Parse(values[9], CultureInfo.InvariantCulture.NumberFormat),
                            Hp = float.Parse(values[10], CultureInfo.InvariantCulture.NumberFormat),
                            MaxHpLost = float.Parse(values[11], CultureInfo.InvariantCulture.NumberFormat),
                            HpLost = float.Parse(values[12], CultureInfo.InvariantCulture.NumberFormat),
                        } },
                        {HeroProfession.Cleric, new HeroCombatStatus
                        {
                            WasKilled = values[7] == "1",
                            Attack = float.Parse(values[13], CultureInfo.InvariantCulture.NumberFormat),
                            Hp = float.Parse(values[14], CultureInfo.InvariantCulture.NumberFormat),
                            MaxHpLost = float.Parse(values[15], CultureInfo.InvariantCulture.NumberFormat),
                            HpLost = float.Parse(values[16], CultureInfo.InvariantCulture.NumberFormat),
                        } },
                        {HeroProfession.Knight, new HeroCombatStatus
                        {
                            WasKilled = values[8] == "1",
                            Attack = float.Parse(values[17], CultureInfo.InvariantCulture.NumberFormat),
                            Hp = float.Parse(values[18], CultureInfo.InvariantCulture.NumberFormat),
                            MaxHpLost = float.Parse(values[19], CultureInfo.InvariantCulture.NumberFormat),
                            HpLost = float.Parse(values[20], CultureInfo.InvariantCulture.NumberFormat),
                        } }
                    },
                EncounterDefinition = new EncounterDefinition
                {
                    AllEncounterGroups = new List<MonsterGroup> {
                        { new MonsterGroup(new MonsterType(MonsterRank.Minion, MonsterRole.Minion), int.Parse(values[21], CultureInfo.InvariantCulture.NumberFormat
                        )) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Brute), int.Parse(values[22], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Brute), int.Parse(values[23], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Brute), int.Parse(values[24], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Leader), int.Parse(values[25], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Leader), int.Parse(values[26], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Leader), int.Parse(values[27], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Lurker), int.Parse(values[28], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Lurker), int.Parse(values[29], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Lurker), int.Parse(values[30], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Regular, MonsterRole.Sniper), int.Parse(values[31], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Elite, MonsterRole.Sniper), int.Parse(values[32], CultureInfo.InvariantCulture.NumberFormat)) },
                        { new MonsterGroup(new MonsterType(MonsterRank.Boss, MonsterRole.Sniper), int.Parse(values[33], CultureInfo.InvariantCulture.NumberFormat)) },
                    }
                }
            };

        }
    }
    /// <summary>
    /// The class with all information from the matrix source file.
    /// </summary>
    public class DifficultyMatrixSourceLine
    {
        /// <summary>
        /// Index of the test this line represents.
        /// </summary>
        public int TestIndex;
        /// <summary>
        /// Tier of monsters generated in the encounter.
        /// </summary>
        public int MonsterTier;
        /// <summary>
        /// Which party configuration provider was used for this line.
        /// </summary>
        public string PartyConfiguration;
        /// <summary>
        /// How strong is the party in this encounter.
        /// </summary>
        public float PartyStrength;
        /// <summary>
        /// How many percent of Max HP were lost in this encounter.
        /// </summary>
        public float MaxHpLost;
        /// <summary>
        /// How many percent of HP were lost in this encounter
        /// </summary>
        public float HpLost;
        /// <summary>
        /// For each hero, information about his status at the start of the encounter and how did he survive it.
        /// </summary>
        public Dictionary<HeroProfession, HeroCombatStatus> HeroCombatStatuses;
        /// <summary>
        /// The monsters that appeared in this encounter.
        /// </summary>
        public EncounterDefinition EncounterDefinition;
    }
    /// <summary>
    /// Status of the hero at the start of some encounter and how much was he hurt during it.
    /// </summary>
    public class HeroCombatStatus
    {
        /// <summary>
        /// If true, the hero was killed in this encounter.
        /// </summary>
        public bool WasKilled;
        /// <summary>
        /// The attack value of the hero.
        /// </summary>
        public float Attack;
        /// <summary>
        /// Hero Max HP at the startof combat.
        /// </summary>
        public float MaxHp;
        /// <summary>
        /// Hero HP at the start of combat.
        /// </summary>
        public float Hp;
        /// <summary>
        /// How many max HP percent did the hero lose.
        /// </summary>
        public float MaxHpLost;
        /// <summary>
        /// How many HP percent did the hero lose.
        /// </summary>
        public float HpLost;
    }
}
