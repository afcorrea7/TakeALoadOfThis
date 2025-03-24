//Andrés Correa 2024
using System.Collections;
using UnityEngine;

public class CameraBlendTriggerZone : MonoBehaviour
{
    //A zone that triggers a cinematic camera blend
    //It only triggers once
    [SerializeField] GameObject endVirtualCamera;
    [Tooltip("Essentially the main camera")]
    [SerializeField] GameObject playerFollowVirtualCamera;
    [Tooltip("How much time before the camera goes back to the main one")]
    [SerializeField] int cameraHoldTime; 

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            StartCoroutine(CameraBlend());
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public IEnumerator CameraBlend(){
        SwitchCameras(true);
        GameManager.instance.TogglePlayerHasControl(false);
        yield return new WaitForSeconds(cameraHoldTime);
        if(PlayerIsAllowedToMove()){
            GameManager.instance.TogglePlayerHasControl(true); //if there are not dialogues actives give control back to the player
        }
        SwitchCameras(false);
    }

    //When the camera is done panning, is the player currently in a dialogue or fight?
    bool PlayerIsAllowedToMove(){
        //2025 HINDSIGHT: Could have really used a State Machine for player control, instead I had to jump through these bool hoops nonsense
        return DialogueManager.DMInstance != null && !DialogueManager.DMInstance.dialogueIsActive && !GameManager.instance.playerIsInFight;
    }

    void SwitchCameras(bool state){
        endVirtualCamera.SetActive(state);
        playerFollowVirtualCamera.SetActive(!state);
    }
}
