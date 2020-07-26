using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// A line in the result CSV line summarizing the results of the experiment for some specific user.
    /// </summary>
    class ExperimentSummaryCsvLine
    {
        /// <summary>
        /// ID of the player this line summarizes.
        /// </summary>
        public string UserId;
        /// <summary>
        /// Which group did the user belong to.
        /// </summary>
        public string Group;
        /// <summary>
        /// Summary of the results of the generated levels.
        /// </summary>
        public ExperimentHalfSummary GeneratedLevelsSummary;
        /// <summary>
        /// Summary of the results of the static levels.
        /// </summary>
        public ExperimentHalfSummary StaticLevelsSummary;
        /// <summary>
        /// Demographic data about the player.
        /// </summary>
        public ExperimentDemographics Demographics;
        /// <summary>
        /// What was the average encounter error throughout the experiment.
        /// </summary>
        public float AverageErrorAll;
        /// <summary>
        /// What was the average error in the first and second half of the experiment.
        /// </summary>
        public float[] AverageErrorsHalves = new float[2];
        /// <summary>
        /// What was the average error in the first and second, third and fourth quarter of the experiment.
        /// </summary>
        public float[] AverageErrorsQuarters = new float[4];
        /// <summary>
        /// Writes the header for this kind of CSV data to the output file.
        /// </summary>
        /// <param name="sw">Output stream where the header will be written.</param>
        /// <param name="separator">CSV separator used.</param>
        public static void WriteHeader(StreamWriter sw, char separator = ';')
        {
            sw.Write($"UserId{separator}");
            sw.Write($"Group{separator}");
            ExperimentHalfSummary.WriteHeader(sw, "Generated", separator);
            ExperimentHalfSummary.WriteHeader(sw, "Static", separator);
            ExperimentDemographics.WriteHeader(sw, separator);
            sw.Write($"AverageError{separator}");
            sw.Write($"AverageErrorHalf1{separator}");
            sw.Write($"AverageErrorHalf2{separator}");
            sw.Write($"AverageErrorQuarter1{separator}");
            sw.Write($"AverageErrorQuarter2{separator}");
            sw.Write($"AverageErrorQuarter3{separator}");
            sw.WriteLine($"AverageErrorQuarter4");
        }
        /// <summary>
        /// Writes the line represented by this line to the output.
        /// </summary>
        /// <param name="sw">Output in which the line will be written.</param>
        /// <param name="separator">CSV separator used.</param>
        public void WriteLine(StreamWriter sw, char separator = ';')
        {
            sw.Write(UserId + separator);
            sw.Write(Group + separator);
            GeneratedLevelsSummary.WriteLine(sw, separator);
            StaticLevelsSummary.WriteLine(sw, separator);
            Demographics.WriteLine(sw, separator);
            sw.Write($"{AverageErrorAll}{separator}");
            sw.Write($"{AverageErrorsHalves[0]}{separator}");
            sw.Write($"{AverageErrorsHalves[1]}{separator}");
            sw.Write($"{AverageErrorsQuarters[0]}{separator}");
            sw.Write($"{AverageErrorsQuarters[1]}{separator}");
            sw.Write($"{AverageErrorsQuarters[2]}{separator}");
            sw.WriteLine($"{AverageErrorsQuarters[3]}");
        }
    }
    /// <summary>
    /// Information about the results of a single phase of the experiment.
    /// </summary>
    class ExperimentHalfSummary
    {
        /// <summary>
        /// How did the user rate the experience.
        /// </summary>
        public int Rating;
        /// <summary>
        /// How much was the user in the flowed.
        /// </summary>
        public double FlowScore;
        /// <summary>
        /// How difficult did the player think the phase was.
        /// </summary>
        public double PerceivedDifficultyScore;
        /// <summary>
        /// How difficult were the easy doors according to the player.
        /// </summary>
        public int EasyDoorRating;
        /// <summary>
        /// How difficult were the medium doors according to the player..
        /// </summary>
        public int MediumDoorRating;
        /// <summary>
        /// How difficult were the hard doors according to the player.
        /// </summary>
        public int HardDoorRating;
        /// <summary>
        /// If true, the easy doors were rated as easier or the same then medium doors and medium doors were easier than or the same as the hard doors.
        /// </summary>
        public bool DidOrderDoorsCorrectly => (EasyDoorRating <= MediumDoorRating) && (MediumDoorRating <= HardDoorRating);

        /// <summary>
        /// Writes the header for this kind of CSV data to the output file.
        /// </summary>
        /// <param name="sw">Output stream where the header will be written.</param>
        /// <param name="separator">CSV separator used.</param>
        /// <param name="headerPrefix">Each of the header names will be prefixed by this string.</param>
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

        /// <summary>
        /// Writes the line represented by this line to the output.
        /// </summary>
        /// <param name="sw">Output in which the line will be written.</param>
        /// <param name="separator">CSV separator used.</param>
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
    /// <summary>
    /// Demographic information about a single player.
    /// </summary>
    public class ExperimentDemographics
    {
        /// <summary>
        /// What was the player's gender.
        /// </summary>
        public string Gender;
        /// <summary>
        /// Player's age, range of ten years.
        /// </summary>
        public string Age;
        /// <summary>
        /// The highest achieved education of the player.
        /// </summary>
        public string Education;
        /// <summary>
        /// How experienced is the player at playing RPGs.
        /// </summary>
        public int RpgsPlayed;

        /// <summary>
        /// Writes the header for this kind of CSV data to the output file.
        /// </summary>
        /// <param name="sw">Output stream where the header will be written.</param>
        /// <param name="separator">CSV separator used.</param>
        public static void WriteHeader(StreamWriter sw, char separator)
        {
            sw.Write($"Gender{separator}");
            sw.Write($"Age{separator}");
            sw.Write($"Education{separator}");
            sw.Write($"RpgsPlayed{separator}");
        }

        /// <summary>
        /// Writes the line represented by this line to the output.
        /// </summary>
        /// <param name="sw">Output in which the line will be written.</param>
        /// <param name="separator">CSV separator used.</param>
        public void WriteLine(StreamWriter sw, char separator)
        {
            sw.Write($"{Gender}{separator}");
            sw.Write($"{Age}{separator}");
            sw.Write($"{Education}{separator}");
            sw.Write($"{RpgsPlayed}{separator}");
        }
    }
}
