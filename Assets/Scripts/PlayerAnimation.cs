using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    Animator animator;
    Vector3 beginScale;

    bool isPlayingAttack;
    float attackTimer;

    delegate void PrevAnim();
    PrevAnim prevAnim;

    const float ANIMATION_DURATION = 0.2f;

	void Start () {
        animator = GetComponent<Animator>();
        beginScale = transform.localScale;
        PlayIdleRight();
    }
	

    public void PlayIdleUp()
    {
        animator.SetBool("up", true);
        animator.SetBool("down", false);
        animator.SetBool("horizontal", false);
        animator.SetBool("attack", false);
        isPlayingAttack = false;
        attackTimer = 0;
        transform.localScale = new Vector3(beginScale.x, beginScale.y, beginScale.z);
        prevAnim = PlayIdleUp;
    }

    public void PlayIdleDown()
    {
        animator.SetBool("up", false);
        animator.SetBool("down", true);
        animator.SetBool("horizontal", false);
        transform.localScale = new Vector3(beginScale.x, beginScale.y, beginScale.z);
        prevAnim = PlayIdleDown;
    }

    public void PlayIdleRight()
    {
        animator.SetBool("up", false);
        animator.SetBool("down", false);
        animator.SetBool("horizontal", true);
        transform.localScale = new Vector3(beginScale.x, beginScale.y, beginScale.z);
        prevAnim = PlayIdleRight;
    }

    public void PlayIdleLeft()
    {
        animator.SetBool("up", false);
        animator.SetBool("down", false);
        animator.SetBool("horizontal", true);
        transform.localScale = new Vector3(-beginScale.x, beginScale.y, beginScale.z);
        prevAnim = PlayIdleLeft;
    }

    public void PlayAttack()
    {
        
        animator.SetBool("up", false);
        animator.SetBool("down", false);
        animator.SetBool("horizontal", false);
        animator.SetBool("attack", true);
        isPlayingAttack = true;

    }

     void Update () {
        if (isPlayingAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > ANIMATION_DURATION)
            {
                isPlayingAttack = false;
                animator.SetBool("attack", false);
                animator.ResetTrigger("attack");
                attackTimer = 0;
                prevAnim();
            }
        }
	}
}
