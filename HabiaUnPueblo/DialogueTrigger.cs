//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialoguesTemplate[] conversations;
    private SpriteRenderer dialogueBubble;
    private int idIncrease = 0; //for cycling through the collection of conversations this object may have
    [HideInInspector] public bool isInTalkRange = false;

    void Start(){
        dialogueBubble = GetComponentInChildren<SpriteRenderer>();
        if(dialogueBubble != null){
            dialogueBubble.enabled = false; //start with the dialogue bubble invisible
        }
    }
    
    void Update(){
        if(Input.GetKeyDown(KeyCode.E) && MeetsTalkRequirements()){
            if(dialogueBubble != null){
                dialogueBubble.enabled = false; //player has initiated dialogue. No need to show the bubble anymore
            }
            SendDialogue();
        }
    }

    bool MeetsTalkRequirements(){
        return isInTalkRange && GameManager.instance.playerMovementIsAllowed;
    }

    void SendDialogue(){
        DialoguesTemplate selectedConversation = SelectConversation();
        DialogueManager.DMInstance.GetConversation(selectedConversation);
    }

    DialoguesTemplate SelectConversation(){
        DialoguesTemplate selectedConversation;
        if(idIncrease < conversations.Length){
            selectedConversation = conversations[idIncrease];
            idIncrease++;
            return selectedConversation;
        }
        //If IdIncrease has reached max value, send the last value in the array
        return conversations[conversations.Length - 1];
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            if(dialogueBubble != null){
                dialogueBubble.enabled = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            isInTalkRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            isInTalkRange = false;
            if(dialogueBubble != null){
                dialogueBubble.enabled = false;
            }
        }
    }
}
