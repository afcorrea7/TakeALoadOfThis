/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkAllergenSounds : MonoBehaviour
{
    [Header("Music Jingles")]
    [SerializeField] private AudioSource jinglesAudioSource;
    [SerializeField] private AudioClip allergicJingle;
    [SerializeField] private AudioClip nonAllergicJingle;

    [Header("Noises")]
    [SerializeField] private AudioSource interactionSoundsSource;
    [SerializeField] private AudioClip touchSound;
    [SerializeField] private AudioClip munchOrAnimalSound;
    [SerializeField] private AudioClip reactionSound;

    public void PlayAllergicJingle(){
        jinglesAudioSource.PlayOneShot(allergicJingle);
    }

    public void PlayNonAllergicJingle(){
        jinglesAudioSource.PlayOneShot(nonAllergicJingle);
    }

    public void PlayJingle(AudioClip jingle){
        jinglesAudioSource.PlayOneShot(jingle);
    }

    public void PlayTouchSound(){
        interactionSoundsSource.PlayOneShot(touchSound);
    }

    public void PlayMunchOrAnimalSound(){
        interactionSoundsSource.PlayOneShot(munchOrAnimalSound);
    }

    public void PlayReactionSound(){
        interactionSoundsSource.PlayOneShot(reactionSound);
    }

    public void PlayNoise(AudioClip noise){
        interactionSoundsSource.PlayOneShot(noise);
    }
}
