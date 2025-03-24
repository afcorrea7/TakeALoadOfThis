//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MitopediaPages : MonoBehaviour
{
    private GameObject previousPage;
    private GameObject currentPage;
    private GameObject nextPage;

    [SerializeField] GameObject goToNextButton;
    [SerializeField] GameObject goToPreviousButton;

    KeyCode[] nextPageKeycodes = {KeyCode.D, KeyCode.RightArrow};
    KeyCode[] previousPageKeyCodes = {KeyCode.A, KeyCode.LeftArrow};

    float inputCooldown = 0.1f; //Prevent player from spamming key/button to cycle through the pages
    bool isOnCooldown; 

    void Start(){
        isOnCooldown = false;
        DisablePages();
        GetNeighbourPages();
    }

    void DisablePages(){
        //Disable all pages in the book except for the first one
        for(int i=0; i < transform.childCount; i++){
            if(i == 0){
                transform.GetChild(i).gameObject.SetActive(true);
            }else{
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    } 

    void GetNeighbourPages(){
        //reset Game Objects
        previousPage = null;
        currentPage = null;
        nextPage = null;

        //Look for the page that is currently active to set as the 'index reference point'
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).gameObject.activeInHierarchy){
                //? checks if there is an existing game object behind or infront of the current instruction
                currentPage = transform.GetChild(i).gameObject;
                if(i-1 >= 0){
                    //prevent previous index from being a negative value
                    previousPage = transform.GetChild(i-1)?.gameObject;
                }
                if(i + 1 < transform.childCount){
                    //prevent next index from being a non-existent value
                    nextPage = transform.GetChild(i+1)?.gameObject;
                }
                ToggleButtonsVisibility();
                break;
            }
        }
    }

    //set these two functions as public to allow buttons to see them
    public void GoToPreviousPage(){
        if(previousPage != null){
            currentPage.SetActive(false);
            previousPage.SetActive(true);
            //Reset positions of which is the next, the current and the previous page
            GetNeighbourPages();
        }
    }

    public void GoToNextPage(){
        if(nextPage != null){
            currentPage.SetActive(false);
            nextPage.SetActive(true);
            //Reset positions of which is the next, the current and the previous page
            GetNeighbourPages();
        }
    }

    void ToggleButtonsVisibility(){
        goToPreviousButton.SetActive(previousPage != null); //if there is no previous page, it means there's no pages before this one
        goToNextButton.SetActive(nextPage != null); //if there is no next page, it means there's no pages further
    }

    void Update(){
        if(!isOnCooldown){
            if(Input.GetKeyDown(nextPageKeycodes[0]) || Input.GetKeyDown(nextPageKeycodes[1])){
                GoToNextPage();
                StartCoroutine(CooldownInput());
            }

            if(Input.GetKeyDown(previousPageKeyCodes[0]) || Input.GetKeyDown(previousPageKeyCodes[1])){
                GoToPreviousPage();
                StartCoroutine(CooldownInput());
            }
        }
    }

    IEnumerator CooldownInput(){
        isOnCooldown = true;
        yield return new WaitForSeconds(inputCooldown);
        isOnCooldown = false;
    }
}