//Andrés Correa 2024
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarSlots : MonoBehaviour
{
    private int basicMagicSlotAmount = 6;
    private bool canGetHurt; //Cooldown for taking damage to prevent taking more than warranted
    void OnEnable(){
        MinigameEventManager.OnPlayerHurt += LoseMagic;
    }

    void Start(){
        //Set magic slots to the current magic according to the game manager
        if(GameManager.instance == null){
            SetMagicCount(basicMagicSlotAmount);
        }else{
            SetMagicCount(GameManager.instance.currentMagicSlots);
        }
        canGetHurt = true;
    }

    public void SetMagicCount(int startingMagicSlots){
        //first, turn off all child slot sprites
        for(int i=0; i< transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //then, turn on childs up to the current number of magic slots according to the game manager
        for(int i=0; i < startingMagicSlots; i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void LoseMagic(){
        if(canGetHurt){
            if(GameManager.instance != null){
                GameManager.instance.currentMagicSlots--;
            }
            //Disable the first enabled slot the method can find
            for(int i = 0; i < transform.childCount; i++){
                GameObject slot = transform.GetChild(i).gameObject;
                if(slot.activeInHierarchy){
                    slot.SetActive(false);
                    if(AllSlotsDepleted()){
                        MinigameEventManager.instance.LoseMinigameEvent();
                    }
                    return; //only deactivate one slot at a time
                }
            }
            StartCoroutine(CanGetHurtCooldoown());
        }

    }

    IEnumerator CanGetHurtCooldoown(){
        canGetHurt = false;
        yield return new WaitForSeconds(0.5f);
        canGetHurt = true;
    }

    bool AllSlotsDepleted(){ //return true if there are no active slots in the magic bar
        int activeSlotCount = 0;
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).gameObject.activeInHierarchy){
                activeSlotCount++;
            }
        }
        return activeSlotCount == 0;
    }

    public void GainMagic(){
        if(GameManager.instance != null){
            if(GameManager.instance.currentMagicSlots < GameManager.MAXMAGICSLOTS){ //If magic bar is not maxed out
                GameManager.instance.currentMagicSlots++;
                for(int i = 0; i < transform.childCount; i++){
                    GameObject slot = transform.GetChild(i).gameObject;
                    if(!slot.activeInHierarchy){
                        slot.SetActive(true);
                        return; //only fill one slot at a time
                    }
                }
            }
        }
    }

    void OnDisable(){
        MinigameEventManager.OnPlayerHurt -= LoseMagic;
    }
}