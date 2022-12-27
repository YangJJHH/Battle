using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;
/// <summary>
/// 
//
/// </summary>
public class PlayerFootStep : MonoBehaviour
{
    public SoundList[] stepSounds;
    private Animator myAnimator;
    private int index;
    private Transform leftFoot, rightFoot;
    private float dist;
    private int groundedBool, coverBool, aimBool, crouchFloat; //애니메이터 변수
    private bool grounded;
    [Range(0,5)] public float volume;



    public enum Foot 
    {
        LEFT,
        RIGHT,
    }

    private Foot step = Foot.LEFT;
    private float oldDist, maxDist = 0;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        leftFoot = myAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = myAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        groundedBool = Animator.StringToHash(AnimatorKey.Grounded);
        coverBool = Animator.StringToHash(AnimatorKey.Cover);
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        crouchFloat = Animator.StringToHash(AnimatorKey.Crouch);
    }

    private void PlayFootStep()
    {
        if(oldDist < maxDist)
        {
            return;
        }
        oldDist = maxDist = 0;
        int oldIndex = index;
        while(oldIndex == index)
        {
            index = Random.Range(0,stepSounds.Length - 1);
        }
        SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, volume);
    }

    private void Update()
    {
        if(!grounded && myAnimator.GetBool(groundedBool))
        {
            PlayFootStep();
        }
        grounded = myAnimator.GetBool(groundedBool);
        float factor = 0.15f;

        if(grounded && myAnimator.velocity.magnitude > 1.6f)
        {
            oldDist = maxDist;
            switch (step) 
            {
                case Foot.LEFT:
                    dist = leftFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if(dist<= factor)
                    {
                        PlayFootStep();
                        step = Foot.RIGHT;
                    }
                    break;
                case Foot.RIGHT:
                    dist = rightFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if (dist <= factor)
                    {
                        PlayFootStep();
                        step = Foot.LEFT;
                    }
                    break;
            }

        }


    }
}
