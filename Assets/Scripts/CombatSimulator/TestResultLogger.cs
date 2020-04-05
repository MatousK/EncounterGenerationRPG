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
    class TestResultLogger
    {
        public int CurrentTestIndex;
        private const string Separator = ";";
        private const string ResultsFileName = "TestResults.csv";
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
    

        public TestResultLogger()
        {
            UpdateFinishedTestsCount();
            if (!File.Exists(ResultsFileName))
            {
                CreateOutputFile();
            }
        }

        public void LogResult(TestResult result)
        {
            using (var outputStream = new StreamWriter(ResultsFileName, true))
            {
                var line = string.Join(Separator, outputColumns.Select(column => column.ValueFunction(result)));
                outputStream.WriteLine(line);
            }
            CurrentTestIndex++;
        }

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
        void CreateOutputFile()
        {
            using (var outputStream = new StreamWriter(ResultsFileName, true))
            {
                outputStream.WriteLine("sep=" + Separator);
                var headers = string.Join(";", outputColumns.Select(column => column.Header));
                outputStream.WriteLine(headers);
            }
        }
    
        private class Column
        {
            public string Header;
            public Func<TestResult, string> ValueFunction;
        }
    }

    public class TestResult
    {
        public int TestIndex;
        public int MonsterTier;
        public EncounterDefinition TestEncounter;
        public PartyConfiguration PartyConfiguration;
        public PartyConfigurationProvider.PartyConfigurationProvider PartyProvider;
        public Hero Knight;
        public Hero Cleric;
        public Hero Ranger;

        public float GetPartyStrength()
        {
            return PartyConfiguration.ClericStats.AttackModifier * PartyConfiguration.ClericStats.MaxHp +
                   PartyConfiguration.KnightStats.AttackModifier * PartyConfiguration.KnightStats.MaxHp +
                   PartyConfiguration.RangerStats.AttackModifier * PartyConfiguration.RangerStats.MaxHp;
        }

        public float GetMaxHpLost()
        {
            return GetMaxHpLost(Knight) + GetMaxHpLost(Cleric) + GetMaxHpLost(Ranger);
        }

        public float GetMaxHpLost(Hero hero)
        {
            return 1 - hero.MaxHitpoints / hero.TotalMaxHitpoints;
        }
        public float GetHpLost()
        {
            return GetHpLost(Knight) + GetHpLost(Cleric) + GetHpLost(Ranger);
        }
        public float GetHpLost(Hero hero)
        {
            return 1 - hero.HitPoints / hero.TotalMaxHitpoints;
        }

        public int GetMonsterCount(MonsterRank rank, MonsterRole role)
        {
            var monsterGroup = TestEncounter.AllEncounterGroups.FirstOrDefault(encounter => encounter.MonsterType.Rank == rank && encounter.MonsterType.Role == role);
            return monsterGroup != null ? monsterGroup.MonsterCount : 0;
        }
    }
}