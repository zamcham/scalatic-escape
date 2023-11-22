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
        Jump
    }
    string currentAnimation;

    void Awake()
    {
        skeletonAnimation = FindObjectOfType<SkeletonAnimation>();
        currentAnimation = PlayerAnimationState.Idle.ToString();
    }

    void Start()
    {
        SetAnimation(PlayerAnimationState.Idle.ToString(), true, 1f);   
    }

    void Update()
    {
        
    }

    void SetAnimation(string animationName, bool loop, float timeScale)
    {
        if (animationName == currentAnimation)
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animationName, loop).TimeScale = timeScale;
        currentAnimation = animationName;
    }
}
