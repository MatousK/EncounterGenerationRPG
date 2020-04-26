using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PauseIndicator: MonoBehaviour
    {
        public GameObject TextGameObject;
        private PauseManager pauseManager;

        private void Start()
        {
            pauseManager = FindObjectOfType<PauseManager>();
        }

        private void Update()
        {
            if (pauseManager.IsPaused != TextGameObject.activeSelf)
            {
                TextGameObject.SetActive(pauseManager.IsPaused);
            }
        }
    }
}
