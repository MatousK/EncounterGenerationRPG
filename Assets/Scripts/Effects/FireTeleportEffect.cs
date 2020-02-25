﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FireTeleportEffect: MonoBehaviour
{
    public int FireAnimationRepetitions = 2;
    public event EventHandler OnFireMaxSize;
    public event EventHandler OnFireAnimationEnded;
    private int currentFireanimationRepetions;
    public void StartFire()
    {
        currentFireanimationRepetions = 0;
        GetComponent<Animator>().SetBool("FireTeleportationActive", true);
    }

    public void FireMaxSize()
    {
        OnFireMaxSize?.Invoke(this, new EventArgs());
    }

    public void FireEnded()
    {
        OnFireAnimationEnded?.Invoke(this, new EventArgs());
        if (++currentFireanimationRepetions >= FireAnimationRepetitions)
        {
            GetComponent<Animator>().SetBool("FireTeleportationActive", false);
        }
    }
}