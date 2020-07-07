using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    class ExperimentSummaryCsvLine
    {
        public string UserId;
        public string Group;
        public ExperimentHalfSummary GeneratedLevelsSummary;
        public ExperimentHalfSummary StaticLevelsSummary;
        public ExperimentDemographics Demographics;

        public static void WriteHeader(StreamWriter sw, char separator = ';')
        {
            sw.Write($"UserId{separator}");
            sw.Write($"Group{separator}");
            ExperimentHalfSummary.WriteHeader(sw, "Generated", separator);
            ExperimentHalfSummary.WriteHeader(sw, "Static", separator);
            ExperimentDemographics.WriteHeader(sw, separator);
        }

        public void WriteLine(StreamWriter sw, char separator = ';')
        {
            sw.Write(UserId + separator);
            sw.Write(Group + separator);
            GeneratedLevelsSummary.WriteLine(sw, separator);
            StaticLevelsSummary.WriteLine(sw, separator);
            Demographics.WriteLine(sw, separator);
        }
    }
    
    class ExperimentHalfSummary
    {
        public int Rating;
        public double FlowScore;
        public double PerceivedDifficultyScore;
        public int EasyDoorRating;
        public int MediumDoorRating;
        public int HardDoorRating;
        public bool DidOrderDoorsCorrectly => (EasyDoorRating <= MediumDoorRating) && (MediumDoorRating <= HardDoorRating);

        public static void WriteHeader(StreamWriter sw, string headerPrefix, char separator)
        {
            sw.Write($"{headerPrefix}Rating{separator}");
            sw.Write($"{headerPrefix}FlowScore{separator}");
            sw.Write($"{headerPrefix}PerceivedDifficultyScore{separator}");
            sw.Write($"{headerPrefix}EasyDoorRating{separator}");
            sw.Write($"{headerPrefix}MediumDoorRating{separator}");
            sw.Write($"{headerPrefix}HardDoorRating{separator}");
            sw.Write($"{headerPrefix}DidOrderDoorsCorrectly{separator}");
        }

        public void WriteLine(StreamWriter sw, char separator)
        {
            sw.Write($"{Rating}{separator}");
            sw.Write($"{FlowScore}{separator}");
            sw.Write($"{PerceivedDifficultyScore}{separator}");
            sw.Write($"{EasyDoorRating}{separator}");
            sw.Write($"{MediumDoorRating}{separator}");
            sw.Write($"{HardDoorRating}{separator}");
            sw.Write($"{(DidOrderDoorsCorrectly ? 1 : 0)}{separator}");
        }
    }

    public class ExperimentDemographics
    {
        public string Gender;
        public string Age;
        public string Education;
        public int RpgsPlayed;

        public static void WriteHeader(StreamWriter sw, char separator)
        {
            sw.Write($"Gender{separator}");
            sw.Write($"Age{separator}");
            sw.Write($"Education{separator}");
            sw.WriteLine($"RpgsPlayed");
        }

        public void WriteLine(StreamWriter sw, char separator)
        {
            sw.Write($"{Gender}{separator}");
            sw.Write($"{Age}{separator}");
            sw.Write($"{Education}{separator}");
            sw.WriteLine($"{RpgsPlayed}");
        }
    }
}
