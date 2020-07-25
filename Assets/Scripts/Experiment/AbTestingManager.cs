using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment
{
    /// <summary>
    /// This component can determine the experiment group the current player belongs to.
    /// Is a DontDestroyOnLoad object, there should be only one instance of this object between loads.
    /// </summary>
    public class AbTestingManager: MonoBehaviour
    {
        [HideInInspector]
        public bool IsPendingKill;

        private void Awake()
        {
            if (FindObjectsOfType<AbTestingManager>().Length > 1)
            {
                IsPendingKill = true;
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            ResetTestingGroup();
        }
        /// <summary>
        /// Randomly select the testing group for this player.
        /// </summary>
        public void ResetTestingGroup()
        {
            CurrentExperimentGroup = UnityEngine.Random.Range(0f, 1f) > 0.5f
                ? ExperimentGroup.FirstGeneratedThenStatic
                : ExperimentGroup.FirstStaticThenGenerated;
        }
        /// <summary>
        /// The experiment group of this player.
        /// </summary>
        public ExperimentGroup CurrentExperimentGroup;
    }
}
