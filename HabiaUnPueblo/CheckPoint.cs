//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckPoint : MonoBehaviour
{
    //2025 HINDSIGHT: This could have been easily done by using static bools

    //Dialogue Triggers that the player has interacted with
    public List<Collider2D> dialogueTriggers = new List<Collider2D>();
    public List<GameObject> actors = new List<GameObject>(); //Actors that the checkpoint has recorded have appeared or disappeared
    private Transform playerPositionPoint; //Position the player will return to if they lose.
    private Collider2D checkPointTrigger;
    private bool isTracking; //This checkpoint is keeping track of what the player does (i.e dialogue triggers or appearing characters)

    void Start(){
        checkPointTrigger = GetComponent<Collider2D>();
        playerPositionPoint = transform;
    }

    void OnTriggerEnter2D(Collider2D other) {
        isTracking = true; 
        //player only needs to pass through a checkpoint once, disable its collider after that
        checkPointTrigger.enabled = false;   
    }

    //Checkpoints save dialogue triggers because they tipically get disabled after the player sees them
    //If the player loses a myth fight, they need to be reenabled so that game sequences doesn't get scrambled.
    //Checkpoints must reenable these triggers when loaded.
    public void SaveDialogueTrigger(Collider2D colliderToSave){ //add a recently enabled dialogue to the list
        if(isTracking){
            dialogueTriggers.Add(colliderToSave);
        }
    }

    public void ReenableFights(){
        StartMythFightBase[] mythFightStarters = FindObjectsOfType<StartMythFightBase>(true); //true to also consider inactive objects
        Debug.Log("FIGHTS FOUND IN SCENE: "+mythFightStarters.Length);
        foreach(var mythFight in mythFightStarters){
            //If this fight was activated before the player lost and went back to a checkpoint
            //Allow it to be activated again
            if(mythFight.alreadyLoaded){
                mythFight.ResetAlreadyLoaded();
            }
        }
    }

    public void SaveActor(GameObject actor){
        if(isTracking){
            actors.Add(actor);
        }
    }

    public void ResetActors(){
        foreach(GameObject actor in actors){
            //Enable actors that have been disabled while this checkpoint is active
            //Disable actors that have been enabled while this checkpoiint is active
            actor.SetActive(!actor.activeInHierarchy);
        }
        actors.Clear(); //Clear the lit so it can again take one single instance of each actor that appears or disappears
    }

    public void ReenableDialogueTriggers(){
        foreach(var trigger in dialogueTriggers){
            trigger.enabled = true; //reenable collision
        }
        dialogueTriggers.Clear(); //Clear the list so it can again take one single instance of each dialogue that the player sees
    }

    public void RelocatePlayer(Transform playerTransform){
        playerTransform.position = playerPositionPoint.position;
    }

    void OnDisable(){
        isTracking = false;
        dialogueTriggers.Clear(); //Also clear list when this checkpoint is disabled
    }
}
