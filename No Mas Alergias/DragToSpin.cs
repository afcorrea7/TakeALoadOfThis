/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToSpin : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private bool isSpinning;
    private float spinSpeed;
    private float totalRotation;
    [SerializeField] float decelerationFactor = 0.95f;
    [SerializeField] float speedFactor = 10f;
    private RectTransform wheelTransform;   
    private Collider2D[] wheelSegments;
    [SerializeField] Collider2D wheelArrow;
    [SerializeField] TriviaManager triviaBoxManager;
 
    void Start()
    {
        wheelTransform = GetComponent<RectTransform>();
        wheelSegments = GetComponentsInChildren<Collider2D>();
    }

    public void OnDrag(PointerEventData eventData){
        if(!isSpinning){ //only allow dragging the wheel if it's not spinning
            if(eventData.delta != Vector2.zero){
                totalRotation += eventData.delta.x + eventData.delta.y;
                wheelTransform.localRotation = Quaternion.Euler(0,0, -totalRotation);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        if(!isSpinning){
            spinSpeed = totalRotation / speedFactor;
            isSpinning = true;
        }
    }

    void FixedUpdate()
    {
        if(SpinsTracker.trackerInstance.spinsCounter == 0){
            enabled = false; //disable this script to prevent further spinning, player has used all their spins
        }

        if(isSpinning){
            wheelTransform.localRotation = Quaternion.Euler(0,0, -totalRotation);
            totalRotation += spinSpeed;
            spinSpeed *= decelerationFactor;
            if(Math.Abs(spinSpeed) < 0.1){
                isSpinning = false;
                //reset totalRotation to the final angle of the wheel to prevent total rotation accumulating speed and being too fast
                totalRotation = -wheelTransform.localEulerAngles.z;
                DetermineResult();
                UpdateSpinTracker();
            }
        }
    }

    void DetermineResult(){
        Debug.Log("The wheel has STOPPED spinning");
        foreach(var collider in wheelSegments){
            if(collider.IsTouching(wheelArrow)){
                Debug.Log("Wheel has stopped in "+collider.gameObject.name);
                ShowTriviaPanel(collider.gameObject);
                break; //break the foreach cycle to only account for the first collider touching the arrow. Will avoid double results in a spin
            }
        }
    }

    void UpdateSpinTracker(){
        SpinsTracker.trackerInstance.spinsCounter--;
        SpinsTracker.trackerInstance.SetText();
    }

    void ShowTriviaPanel(GameObject wheelSegment){
        TriviaCategory category = wheelSegment.GetComponent<TriviaCategory>();
        if(category != null){
            triviaBoxManager.SetCategory(category.sectionCategory);
        }
    }
}