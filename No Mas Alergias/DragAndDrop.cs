/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//IPointerDownHandler is an interface related to pointer (aka mouse, touchscreen, etc) detection
//Layman's terms, interface for managing the object being clicked/tapped
[RequireComponent(typeof(CanvasGroup))]
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler 
{
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform originPoint;
    [SerializeField] private float returnSpeed;
    [SerializeField] private bool enableDragging;
    private RectTransform rectTransform; //RectTransform since we are handling an UI object
    private bool shouldReturn;
    [HideInInspector] public bool droppedOnDropSlot;
    [Range(0,1)] public float darkenModifier;
    private AudioSource audioSource;
    [SerializeField] private AudioClip grabSound;

    void Awake(){
        enableDragging = true;
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.position = originPoint.position;
    }

    public void OnBeginDrag(PointerEventData eventData){
        if(enableDragging){
            audioSource?.PlayOneShot(grabSound);
            canvasGroup.blocksRaycasts = false;
            droppedOnDropSlot = false; //when the object is dragged, assume it's not on a Drop Zone
        }
    }

    public void OnDrag(PointerEventData eventData){
        if(enableDragging){
            //take the current canva's size into consideration
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        if(enableDragging){
            canvasGroup.blocksRaycasts = true;
            if(!droppedOnDropSlot){
                shouldReturn = true;
            }
        }
    }

    void Update(){
        if(shouldReturn){
            ReturnToOriginSlot();
        }
    }

    void ReturnToOriginSlot(){
        //gradually move to the origin
        rectTransform.position = Vector2.MoveTowards(rectTransform.position, originPoint.position, returnSpeed * Time.deltaTime);
        //snap to origin once the distance to it is small enough
        if(Vector2.Distance(rectTransform.position, originPoint.position) < 0.01f){
            rectTransform.position = originPoint.position;
            shouldReturn = false; //object is finally on the origin, doesn't need to return anymore
            if(!enableDragging){
                DarkenImage();
            }
        }
    }

    public void LockObject(){ //To be called by the animation script
        DisableDragging();
        shouldReturn = true;
    }

    public void DisableDragging(){ //To be called by UseSlotBehaviour script
        enableDragging = false;
    }

    public void DarkenImage(){
        Image[] image = GetComponentsInChildren<Image>();
        foreach(Image imageItem in image){
            Color darkenColor = imageItem.color;

            darkenColor.r -= darkenModifier; 
            darkenColor.g -= darkenModifier; 
            darkenColor.b -= darkenModifier; 
            darkenColor.r = Mathf.Clamp01(darkenColor.r);
            darkenColor.g = Mathf.Clamp01(darkenColor.g);
            darkenColor.b = Mathf.Clamp01(darkenColor.b);

            imageItem.color = darkenColor;
        }
    }

}