using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetAnimator : MonoBehaviour
{
    public Animator Animator { get; set; }
    
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void AnimateThrust()
    {
        Animator.Play("Thrust");
    }

    private void ThrustEndedCallback()
    {
        Animator.Play("Idle");
    }
}
