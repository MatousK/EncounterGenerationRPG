using Assets.Scripts.Analytics;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    class GeneralDataParser
    {

        public List<CsvLine> LoadGeneralData(string path)
        {
            List<CsvLine> allCsvLines = new List<CsvLine>();
            using (StreamReader sr = new StreamReader(path))
            {
                do
                {
                    string line = sr.ReadLine();
                    allCsvLines.Add(ParseLine(line));
                } while (!sr.EndOfStream);
            }
            return allCsvLines;
            
        }

        private CsvLine ParseLine(string line)
        {
            var csvElements = line.Split(';');
            string lineType = csvElements[0];
            string userId = csvElements[1];
            string timestampString = csvElements[2];
            DateTime timeStamp = DateTime.FromFileTimeUtc(long.Parse(timestampString, CultureInfo.InvariantCulture));
            switch (lineType)
            {
                case "LevelLoadEnded":
                    return new LevelLoadOverLine
                    {
                        RawLineData = line,
                        LineType = lineType,
                        LogTime = timeStamp,
                        UserId = userId,
                        LevelIndex = int.Parse(csvElements[3], CultureInfo.InvariantCulture),
                        Version = csvElements.Length == 5 ? int.Parse(csvElements[4], CultureInfo.InvariantCulture) : 1
                    };
                case "LevelLoadStarted":
                    return new LevelLoadStartedLine
                    {
                        RawLineData = line,
                        LineType = lineType,
                        LogTime = timeStamp,
                        UserId = userId,
                        LevelIndex = int.Parse(csvElements[3], CultureInfo.InvariantCulture),
                        Version = csvElements.Length == 5 ? int.Parse(csvElements[4], CultureInfo.InvariantCulture) : 1
                    };
                case "RevokeAgreement":
                    return new AgreementRevokedLine
                    {
                        RawLineData = line,
                        LineType = lineType,
                        LogTime = timeStamp,
                        UserId = userId,
                        Version = csvElements.Length == 4 ? int.Parse(csvElements[3], CultureInfo.InvariantCulture) : 1
                    };
                case "Combat":
                    return ParseCombatLine(csvElements, lineType, userId, timeStamp, line);
                default:
                    throw new FormatException("CSV is in an invalid format");
            }
        }

        private CombatOverLine ParseCombatLine(string[] csvElements, string lineType, string userId, DateTime timestamp, string line)
        {
            int currentElementIndex = 3;
            Dictionary<HeroProfession, float> partyStartHitpoints = new Dictionary<HeroProfession, float>
            {
                { HeroProfession.Knight, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Ranger, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Cleric, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) }
            };
            Dictionary<HeroProfession, float> partyEndHitpoints = new Dictionary<HeroProfession, float>
            {
                { HeroProfession.Knight, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Ranger, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Cleric, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) }
            };
            Dictionary<HeroProfession, float> partyAttack = new Dictionary<HeroProfession, float>
            {
                { HeroProfession.Knight, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Ranger, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) },
                { HeroProfession.Cleric, float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture) }
            };
            EncounterDefinition encounter = new EncounterDefinition
            {
                AllEncounterGroups = new List<MonsterGroup>()
            };
            foreach (var monsterType in AnalyticsService.OrderedMonsterTypes)
            {
                var monsterCount = int.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture);
                encounter.AllEncounterGroups.Add(new MonsterGroup(monsterType, monsterCount));
            }
            var expectedDifficulty = float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture);
            var realDifficulty = float.Parse(csvElements[currentElementIndex++], CultureInfo.InvariantCulture);
            var wasGameOver = csvElements[currentElementIndex++] == "1";
            var wasStatic = csvElements[currentElementIndex++] == "1";
            var wasLogged = csvElements[currentElementIndex++] == "1";
            return new CombatOverLine
            {
                RawLineData = line,
                LineType = lineType,
                UserId = userId,
                LogTime = timestamp,
                CombatEncounter = encounter,
                ExpectedDifficulty = expectedDifficulty,
                RealDifficulty = realDifficulty,
                PartyAttack = partyAttack,
                PartyEndHitpoints = partyEndHitpoints,
                PartyStartHitpoints = partyStartHitpoints,
                WasGameOver = wasGameOver,
                WasLogged = wasLogged,
                WasStaticEncounter = wasStatic,
                Version = ++currentElementIndex < csvElements.Length ? int.Parse(csvElements[currentElementIndex], CultureInfo.InvariantCulture) : 1
            };
        }
    }
}
