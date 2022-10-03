using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip deathAudio;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (deathAudio != null)
        {
            audioSource = GetComponentInChildren<AudioSource>();
            audioSource.PlayOneShot(deathAudio);
        }

        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isDead", true);
        Destroy(gameObject, 10);
    }
}
