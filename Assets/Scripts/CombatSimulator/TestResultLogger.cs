using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator.PartyConfigurationProvider;
using Assets.Scripts.EncounterGenerator.Model;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// This class can log the results of the test into a file.
    /// When created, it will try to open the log file to get the index of the test where we left off the last time.
    /// If no log file is found, create one and assume that we are starting a new round of simulations.
    /// This class is also the authority on what is the current test index. That is because the test index is determined by the current state of the log file.
    /// </summary>
    class TestResultLogger
    {
        // TODO: Refactor this so the matrix data are in some class that can output itself and read itself from an XML file. This is reaaaaally not the best way to do this, but it was quick.
        /// <summary>
        /// The index of the test currently being executed.
        /// </summary>
        public int CurrentTestIndex;
        /// <summary>
        /// The separator to use in the output CSV file.
        /// </summary>
        private const string Separator = ";";
        /// <summary>
        /// The name of the file in which the results of the combat simulations should be stored.
        /// </summary>
        private const string ResultsFileName = "TestResults.csv";
        /// <summary>
        /// Specifies the format of the matrix output file.
        /// </summary>
        private readonly List<Column> outputColumns = new List<Column> {
            new Column { Header = "Test Index", ValueFunction = result => result.TestIndex.ToString() },
            new Column { Header = "Monster Tier", ValueFunction = result => result.MonsterTier.ToString() },
            new Column { Header = "Party config", ValueFunction = result => result.PartyProvider.ToString() },
            new Column { Header = "Party Strength", ValueFunction = result => result.GetPartyStrength().ToString() },
            new Column { Header = "Max HP lost", ValueFunction = result => result.GetMaxHpLost().ToString() },
            new Column { Header = "HP lost", ValueFunction = result => result.GetHpLost().ToString() },
            new Column { Header = "Ranger killed", ValueFunction = result => result.Ranger.IsDown ? "1" : "0" },
            new Column { Header = "Cleric killed", ValueFunction = result => result.Cleric.IsDown ? "1" : "0" },
            new Column { Header = "Knight killed", ValueFunction = result => result.Knight.IsDown ? "1" : "0" },
            new Column { Header = "Ranger attack", ValueFunction = result => result.PartyConfiguration.RangerStats.AttackModifier.ToString() },
            new Column { Header = "Ranger HP", ValueFunction = result => result.PartyConfiguration.RangerStats.MaxHp.ToString() },
            new Column { Header = "Ranger Max HP lost", ValueFunction = result => result.GetMaxHpLost(result.Ranger).ToString() },
            new Column { Header = "Ranger HP lost", ValueFunction = result => result.GetHpLost(result.Ranger).ToString() },
            new Column { Header = "Cleric attack", ValueFunction = result => result.PartyConfiguration.ClericStats.AttackModifier.ToString() },
            new Column { Header = "Cleric HP", ValueFunction = result => result.PartyConfiguration.ClericStats.MaxHp.ToString() },
            new Column { Header = "Cleric Max HP lost", ValueFunction = result => result.GetMaxHpLost(result.Cleric).ToString() },
            new Column { Header = "Cleric HP lost", ValueFunction = result => result.GetHpLost(result.Cleric).ToString() },
            new Column { Header = "Knight attack", ValueFunction = result => result.PartyConfiguration.KnightStats.AttackModifier.ToString() },
            new Column { Header = "Knight HP", ValueFunction = result => result.PartyConfiguration.KnightStats.MaxHp.ToString() },
            new Column { Header = "Knight Max HP lost", ValueFunction = result => result.GetMaxHpLost(result.Knight).ToString() },
            new Column { Header = "Knight HP lost", ValueFunction = result => result.GetHpLost(result.Knight).ToString() },
            new Column { Header = "Minion Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Minion, MonsterRole.Minion).ToString() },
            new Column { Header = "Brute Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Regular, MonsterRole.Brute).ToString() },
            new Column { Header = "Brute elite Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Elite, MonsterRole.Brute).ToString() },
            new Column { Header = "Brute boss Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Boss, MonsterRole.Brute).ToString() },
            new Column { Header = "Leader Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Regular, MonsterRole.Leader).ToString() },
            new Column { Header = "Leader elite Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Elite, MonsterRole.Leader).ToString() },
            new Column { Header = "Leader boss Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Boss, MonsterRole.Leader).ToString() },
            new Column { Header = "Lurker Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Regular, MonsterRole.Lurker).ToString() },
            new Column { Header = "Lurker elite Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Elite, MonsterRole.Lurker).ToString() },
            new Column { Header = "Lurker boss Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Boss, MonsterRole.Lurker).ToString() },
            new Column { Header = "Sniper Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Regular, MonsterRole.Sniper).ToString() },
            new Column { Header = "Sniper elite  Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Elite, MonsterRole.Sniper).ToString() },
            new Column { Header = "Sniper boss Count", ValueFunction = result => result.GetMonsterCount(MonsterRank.Boss, MonsterRole.Sniper).ToString() },
        };
    
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultLogger"/> class. Tries to open the log file and get the current test index.
        /// If no log file is found, create one.
        /// </summary>
        public TestResultLogger()
        {
            UpdateFinishedTestsCount();
            if (!File.Exists(ResultsFileName))
            {
                CreateOutputFile();
            }
        }
        /// <summary>
        /// Logs the result of a test into the log file.
        /// </summary>
        /// <param name="result">Result to log.</param>
        public void LogResult(TestResult result)
        {
            using (var outputStream = new StreamWriter(ResultsFileName, true))
            {
                var line = string.Join(Separator, outputColumns.Select(column => column.ValueFunction(result)));
                outputStream.WriteLine(line);
            }
            CurrentTestIndex++;
        }
        /// <summary>
        /// Try open the log file and update the current test index.
        /// Does nothing if the file does not exist.
        /// </summary>
        void UpdateFinishedTestsCount()
        {
            try
            {
                using (StreamReader sr = new StreamReader(ResultsFileName))
                {
                    int lineCount = 0;
                    while (sr.ReadLine() != null) ++lineCount;
                    // First line is separator, second are column headers.
                    CurrentTestIndex = lineCount - 2;
                }
            } 
            catch (FileNotFoundException)
            {
                // Everything fine, output file just not exist yet. 
            }
        }
        /// <summary>
        /// Initializes the output file.
        /// </summary>
        void CreateOutputFile()
        {
            using (var outputStream = new StreamWriter(ResultsFileName, true))
            {
                outputStream.WriteLine("sep=" + Separator);
                var headers = string.Join(";", outputColumns.Select(column => column.Header));
                outputStream.WriteLine(headers);
            }
        }
        /// <summary>
        /// A class representing a single column in the CSV file.
        /// </summary>
        private class Column
        {
            /// <summary>
            /// The header of the CSV column.
            /// </summary>
            public string Header;
            /// <summary>
            /// A function that can extract the value for this column from a test result.
            /// </summary>
            public Func<TestResult, string> ValueFunction;
        }
    }
    /// <summary>
    /// Results of the finished combat simulation.
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Index of the text.
        /// </summary>
        public int TestIndex;
        /// <summary>
        /// Monster tier used in the test.
        /// </summary>
        public int MonsterTier;
        /// <summary>
        /// Encounter specifying the monsters the party was fighting against in this test.
        /// </summary>
        public EncounterDefinition TestEncounter;
        /// <summary>
        /// The configuration the party that was fighting the monsters in the combat simulation.
        /// </summary>
        public PartyConfiguration PartyConfiguration;
        /// <summary>
        /// The provider used to generate the party in this test.
        /// </summary>
        public PartyConfigurationProvider.PartyConfigurationProvider PartyProvider;
        /// <summary>
        /// The knight class which was fighting, should contain his status at the end of the encounter.
        /// </summary>
        public Hero Knight;
        /// <summary>
        /// The cleric class which was fighting, should contain his status at the end of the encounter.
        /// </summary>
        public Hero Cleric;
        /// <summary>
        /// The ranger class which was fighting, should contain his status at the end of the encounter.
        /// </summary>
        public Hero Ranger;
        /// <summary>
        /// Calculates the strength of the party fighting in the encounter
        /// </summary>
        /// <returns></returns>
        public float GetPartyStrength()
        {
            return PartyConfiguration.ClericStats.AttackModifier * PartyConfiguration.ClericStats.MaxHp +
                   PartyConfiguration.KnightStats.AttackModifier * PartyConfiguration.KnightStats.MaxHp +
                   PartyConfiguration.RangerStats.AttackModifier * PartyConfiguration.RangerStats.MaxHp;
        }
        /// <summary>
        /// Calculates how many Max HP percent did the party lose in total (0-3).
        /// </summary>
        /// <returns>How many Max HP did the entire party lost.</returns>
        public float GetMaxHpLost()
        {
            return GetMaxHpLost(Knight) + GetMaxHpLost(Cleric) + GetMaxHpLost(Ranger);
        }
        /// <summary>
        /// Calculates how many Max HP percent did the hero lose (0-1).
        /// </summary>
        /// <param name="hero">The hero whose lost Max HP is requested.</param>
        /// <returns>How many max HP percent did the hero lose. </returns>
        public float GetMaxHpLost(Hero hero)
        {
            return 1 - hero.MaxHitpoints / hero.TotalMaxHitpoints;
        }
        /// <summary>
        /// Calculates how many HP percent did the party lose in total (0-3).
        /// </summary>
        /// <returns>How many HP percent did the entire party lost.</returns>
        public float GetHpLost()
        {
            return GetHpLost(Knight) + GetHpLost(Cleric) + GetHpLost(Ranger);
        }
        /// <summary>
        /// Calculates how many HP percent did the hero lose (0-1).
        /// </summary>
        /// <param name="hero">The hero whose lost HP is requested.</param>
        /// <returns>How many HP percent did the hero lose. </returns>
        public float GetHpLost(Hero hero)
        {
            return 1 - hero.HitPoints / hero.TotalMaxHitpoints;
        }
        /// <summary>
        /// Retrieve the number of monsters present in the encounter of specified rank an role.
        /// </summary>
        /// <param name="rank">Rank of monsters whose count interests us.</param>
        /// <param name="role">Role of monsters whose count interests us.</param>
        /// <returns>The requested number of monsters.</returns>
        public int GetMonsterCount(MonsterRank rank, MonsterRole role)
        {
            var monsterGroup = TestEncounter.AllEncounterGroups.FirstOrDefault(encounter => encounter.MonsterType.Rank == rank && encounter.MonsterType.Role == role);
            return monsterGroup != null ? monsterGroup.MonsterCount : 0;
        }
    }
}