﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using GeneralAlgorithms.Algorithms.Common;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepSimpleMessage: TutorialStepWithMessageBoxBase
    {

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (didMessageBoxAppear && UnityEngine.Input.anyKeyDown && !completedTutorialAction)
            {
                completedTutorialAction = true;
                messageBox.Hide();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}