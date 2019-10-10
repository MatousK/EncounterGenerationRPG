using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// As there can be multiple skill object on an animation object that might want to listen to an animation event, we have one listener here for all fo those.
/// </summary>
public class AnimationEventsListener : MonoBehaviour
{
    public event EventHandler ApplySkillEffect;
    public event EventHandler SkillAnimationFinished;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationCallbackApplySkillEffect()
    {
        ApplySkillEffect?.Invoke(this, new EventArgs());
    }

    public void AnimationCallbackSkillAnimationFinished()
    {
        SkillAnimationFinished?.Invoke(this, new EventArgs());
    }
}
