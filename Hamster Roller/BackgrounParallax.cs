//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgrounParallax : MonoBehaviour
{
    [SerializeField][Range(0.1f, 1.0f)] private float delayFactor;
    private Transform cameraTransform;
    private Vector2 lastCameraPos; //The object needs to know where the camera was in the last frame
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPos = cameraTransform.position;
    }

    //2025 HINDSIGHT: Probably should've commented why I went with LateUpdate() instead of Update(). I can't remember now.
    void LateUpdate()
    {
        Vector2 deltaMovement = (Vector2)cameraTransform.position - lastCameraPos;
        transform.position -= (Vector3)(deltaMovement * delayFactor);
        lastCameraPos = cameraTransform.position;
    }
}
