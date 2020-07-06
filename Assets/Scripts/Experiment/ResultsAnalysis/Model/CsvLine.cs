using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    public abstract class CsvLine
    {
        public string RawLineData;
        public string UserId;
        public DateTime LogTime;
        public string LineType;
        public int Version;
    }
}
