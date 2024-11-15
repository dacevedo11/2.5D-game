using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Animator animator;
    
    public void TakeDamage()
    {
        animator.SetTrigger("Hurt");
    }
}
