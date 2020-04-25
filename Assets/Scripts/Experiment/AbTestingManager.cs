using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment
{
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
            }
            // TODO: Get if from unity analytics maybe.
            CurrentExperimentGroup = UnityEngine.Random.Range(0f, 1f) > 0.5f
                ? ExperimentGroup.FirstGeneratedThenStatic
                : ExperimentGroup.FirstStaticThenGenerated;
        }

        public ExperimentGroup CurrentExperimentGroup;
    }
}
