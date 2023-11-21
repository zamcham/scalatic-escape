using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerAnimations : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    string currentAnimation;

    void Start()
    {
        // Need to dynamically update
        skeletonAnimation = FindObjectOfType<SkeletonAnimation>();
        currentAnimation = "Idle";
        
    }

    void Update()
    {
        
    }

        void SetAnimation(AnimationReferenceAsset animationName, bool loop, float timeScale)
    {
        if (animationName.name.Equals(currentAnimation))
        {
            return;
        }
        skeletonAnimation.state.SetAnimation(0, animationName, loop).TimeScale = timeScale;
        currentAnimation = animationName.name;
    }
}
