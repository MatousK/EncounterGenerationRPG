using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class TargetedGestureSkill : TargetedSkill
{

    public TargetedGestureSkill()
    {
        SkillAnimationName = "Gesturing";
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
}