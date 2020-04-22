using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.UI.Credits
{
    public class CreditsController: MonoBehaviour
    {
        public void CreditsOver()
        {
            FindObjectOfType<LevelLoader>().LoadNextLevel();
        }
    }
}
