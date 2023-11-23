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
        
    }

    void Start()
    {
        StartAnimation(PlayerAnimationState.Idle.ToString(), true, 1f);   
    }

    public void StartAnimation(string animationName, bool loop, float timeScale)
    {
        Debug.Log($"Called SetAnimation: {animationName}");

        if (animationName == currentAnimation)
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animationName, loop).TimeScale = timeScale;
        currentAnimation = animationName;
    }
}
