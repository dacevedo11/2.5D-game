using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private AudioSource audioSource;
    public AudioClip hitSound;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage()
    {
        animator.SetTrigger("Hurt");
        audioSource.PlayOneShot(hitSound);
    }
}
