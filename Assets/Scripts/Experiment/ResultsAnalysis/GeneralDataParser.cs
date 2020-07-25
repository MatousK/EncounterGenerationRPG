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
    /// <summary>
    /// This class can parse the general data from the analytics csv line from the server.
    /// </summary>
    class GeneralDataParser
    {
        /// <summary>
        /// Load the data from the specified file and parses them.
        /// </summary>
        /// <param name="path">The path of where the CSV line is present.</param>
        /// <param name="skipFirstNLines">This many lines of the output file be skipped. This should be the size of the header.</param>
        /// <returns>The list of CSV lines in the specified file.</returns>
        public List<CsvLine> LoadGeneralData(string path, int skipFirstNLines = 0)
        {
            List<CsvLine> allCsvLines = new List<CsvLine>();
            using (StreamReader sr = new StreamReader(path))
            {
                for (int i = 0; i< skipFirstNLines; ++i)
                {
                    sr.ReadLine();
                }
                do
                {
                    string line = sr.ReadLine();
                    allCsvLines.Add(ParseLine(line));
                } while (!sr.EndOfStream);
            }
            return allCsvLines;
            
        }
        /// <summary>
        /// Parses the string line into a CSV line of the correct type.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The CSV line this string represents.</returns>
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
        /// <summary>
        /// Parses the combat over line.
        /// </summary>
        /// <param name="csvElements">List of all cells in this CSV line.</param>
        /// <param name="lineType">The type of the line being parsed.</param>
        /// <param name="userId">ID of the user who logged this line.</param>
        /// <param name="timestamp">When was this line logged.</param>
        /// <param name="line">String representation of the line to be parsed.</param>
        /// <returns></returns>
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
