/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;

public class ListTracking : MonoBehaviour
{
    //Tracks the following:
        //How many prick tests have been applied? (Player must test before being able to check boxes, they can't check them right away).
        //How many boxes are currently checked?
        //Has 5 boxes been checked already? (5 is the maximum allowed)
    
    public static ListTracking Instance;
    public GameObject CheckboxGroup;
    public int appliedSlotsCounter; //Track how many slots has been given a prick test
    public int markedBoxesCounter; 
    private int maxChecksAllowed;

    [HideInInspector] public bool listIsShown;

    //event for making the send button appear or disappear
    public delegate void ToggleSendButton(bool value);
    public static event ToggleSendButton OnMarkedEnoughChecks;

    void Awake(){
        Instance = this;
        listIsShown = false;
    }

    void Start(){
        appliedSlotsCounter = 0;
        markedBoxesCounter = 0;
        maxChecksAllowed = AllergiesManager.ALLERGIESNUMBER; //Allow the player to check up to 5 boxes
        VerifyCapacity(); //Start with all checkboxes grayed out
    }

    public bool IsAllowedToCheckBox(){
        return !MetMaxChecksAllowed() && AreBoxesAvailable(); //automatically compare, if is implicit here. Same below
    }

    public bool MetMaxChecksAllowed(){
        //are 5 boxes marked already?
        return markedBoxesCounter == maxChecksAllowed;
    }

    bool AreBoxesAvailable(){
        //Are there equal or less marked boxes as there are prick tests applied?
        return markedBoxesCounter < appliedSlotsCounter  && appliedSlotsCounter != 0;
    } 

    public void IncreaseAppliedSlotsCount(){
        appliedSlotsCounter++;
        //Verify checkboxes gray them out as long as tests haven't surpassed max number of checkmarks (5)
        if(appliedSlotsCounter <= maxChecksAllowed){
            VerifyCapacity();
        }
    }

    public void IncreaseMarkedBoxes(){
        markedBoxesCounter++;
        VerifyCapacity(); //Check if the list needs to gray out the remaining checkboxes
    }

    public void DecreaseMarkedBoxes(){
        markedBoxesCounter--;
        VerifyCapacity();
    }
 
    void VerifyCapacity(){
        //If all (currently) allowed checkboxes are occupied
        if(!IsAllowedToCheckBox()){
            GrayOutBoxes(); //Illustrate the disabling of box-checking
        }else{
            ResetBoxesColor(); //Illustrate the enabling of box-checking
        }

        if(MetMaxChecksAllowed()){ //If all 5 allergies are marked, display the send button
            ActivateSendButton();
        }else{
            DeactivateSendButton(); //Hide it otherwise
        }
    }

    void GrayOutBoxes(){
        for(int i=0; i < CheckboxGroup.transform.childCount;i++){
            GameObject checkbox = CheckboxGroup.transform.GetChild(i).gameObject;
            CheckBoxInteraction checkboxScript = checkbox.GetComponent<CheckBoxInteraction>();
            if(!checkboxScript.isChecked){
                DarkenBox(checkbox);              
            }     
        }
    }

    void DarkenBox(GameObject checkbox){
        Image checkboxImage = GetIconChildImage(checkbox);
        float darkenModifier = 0.45f;
        checkboxImage.color = EditColor.DarkenColor(checkboxImage.color, darkenModifier);
    }

    void ResetBoxesColor(){
        for(int i=0; i < CheckboxGroup.transform.childCount;i++){
            GameObject checkbox = CheckboxGroup.transform.GetChild(i).gameObject;
            Image checkboxImage = GetIconChildImage(checkbox);
            checkboxImage.color = Color.white;              
        }
    }

    Image GetIconChildImage(GameObject checkbox){
        for(int i=0; i < checkbox.transform.childCount; i++){
            Image childIcon = checkbox.transform.GetChild(i).GetComponent<Image>();
            if(childIcon != null){
                return childIcon;
            }
        }
        return null;
    }

    void ActivateSendButton(){
        OnMarkedEnoughChecks?.Invoke(true); //remember the ToggleSendButton delegate expects a bool value
    }

    void DeactivateSendButton(){
        OnMarkedEnoughChecks?.Invoke(false);
    }
}
