using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerAnimations : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;

    public enum PlayerAnimationState
    {
        Idle,
        Run,
        Jump,
        GroundPound
    }
    public string currentAnimation;

    void Awake()
    {
        skeletonAnimation = FindObjectOfType<SkeletonAnimation>();
        currentAnimation = PlayerAnimationState.Idle.ToString();
        SetAnimation("Run", true, 1f);   
    }

    public void SetAnimation(string animationName, bool loop, float timeScale)
    {
        Debug.Log("SetAnimation: " + animationName);

        if (animationName == currentAnimation)
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animationName, loop).TimeScale = timeScale;
        currentAnimation = animationName;
    }
}
