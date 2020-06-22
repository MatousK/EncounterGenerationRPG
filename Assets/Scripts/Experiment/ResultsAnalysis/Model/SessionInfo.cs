using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    public class SessionInfo
    {
        public SessionGroup SessionGroup;
        public int ClearedLevels;
    }

    public enum SessionGroup
    {
        TutorialOnly,
        FirstGeneratedThenStatic,
        FirstStaticThenGenerated, 
        InvalidValues
    }
}
