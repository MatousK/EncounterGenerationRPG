using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class LevelTransition: MonoBehaviour
    {
        private bool didTransition;
        private void Start()
        {
            GetComponent<InteractableObject>().OnInteractionTriggered += LevelTransitionClicked;
        }

        private void LevelTransitionClicked(object chest, Hero hero)
        {
            if (!didTransition)
            {
                FindObjectOfType<LevelLoader>().LoadNextLevel();
                didTransition = true;
            }
        }
    }
}
