//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDestruction : MonoBehaviour
{
    public GameObject destroyedHouse;
    Collider2D houseCollider;

    private void Start() {
        houseCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D coll){
        if(coll.gameObject.CompareTag("Player")){
            //Retrieve the current state of the player via the State Machine HamsterStateManager script
            HamsterStateManager playerState = coll.gameObject.GetComponent<HamsterStateManager>();
            if(playerState.CurrentState.GetType() == typeof(HamsterChubbyState)){
                Instantiate(destroyedHouse,transform.position,Quaternion.identity);
                gameObject.SetActive(false);
            }
        }
    }


}
