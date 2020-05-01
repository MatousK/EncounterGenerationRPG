using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    class MainMenuController: MonoBehaviour
    {
        private LevelLoader levelLoader;

        private void Start()
        {
            levelLoader = FindObjectsOfType<LevelLoader>().FirstOrDefault(loader => !loader.IsPendingKill);
        }

        public void StartStoryMode()
        {
            levelLoader.StartStoryMode();
        }

        public void StartFreeMode()
        {
            levelLoader.StartFreeMode();
        }

        public void ShowCredits()
        {
            levelLoader.OpenCredits();
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
