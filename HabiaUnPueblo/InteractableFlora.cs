//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This little piece of nature will appear to move when Dani passes through it
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class InteractableFlora : MonoBehaviour
{
    //For when Dani is on top of the grass
    [SerializeField] protected Sprite regularSprite;
    [SerializeField] protected Sprite steppedOnSprite;
    protected AudioSource thisAudioSource;
    protected SpriteRenderer thisSpriteRenderer;

    protected virtual void Start(){ //Methods are marked as protected to allow access to inheriters, virtual to allow overriding
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.sprite = regularSprite; //Start with the regularSprite as the default.
        thisAudioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            SwapSprite(steppedOnSprite);
            thisSpriteRenderer.sprite = steppedOnSprite;
            PlayGrassSound();
        }
    }    

    protected void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            SwapSprite(regularSprite);
            thisSpriteRenderer.sprite = regularSprite;
        }
    }

    //Replace a sprite with the other one
    void SwapSprite(Sprite newSprite){
        //Disable and reenable sprite while swapping, to allow the component to refresh
        //(This is not needed for play mode, but it solves an issue of sprites overlapping instead of getting swapped in WebGL)
        thisSpriteRenderer.enabled = false;
        thisSpriteRenderer.sprite = newSprite;
        thisSpriteRenderer.enabled = true;
    }

    void PlayGrassSound(){
        thisAudioSource?.Play();
    }
}
