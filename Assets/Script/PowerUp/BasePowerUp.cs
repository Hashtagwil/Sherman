using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePowerUp : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] AudioClip spawningAudio;
    [SerializeField] AudioClip pickUpAudio;

    void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.PlayOneShot(spawningAudio);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Since the gameObject powrUps get destroyed on pickup, the audio source is always inexistant
        // AudioSource.PlayClipAtPoint creates a new audio source at transform.position and play the sounds effect.
        // Then it disposes of the audiosource
        if (collision.CompareTag("Player") || collision.CompareTag("AI_Tank"))
        {
            if (pickUpAudio != null)
                AudioSource.PlayClipAtPoint(pickUpAudio, transform.position);
        }
    }
}
