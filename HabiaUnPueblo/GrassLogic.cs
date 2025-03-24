//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrassLogic : InteractableFlora
{
    private Animator thisAnim;
    [Range(0.0f, 1.0f)] public float magicBottleChance; //chance of finding a magic bottle in the grass;
    [SerializeField] float waitBeforeNextPickUp; //Have a cooldown after the grass produces a magic bottle
    private bool canProduceItem;

    protected override void Start(){ //separate from InteractableFlora's Start()
        base.Start();
        canProduceItem = true;
        thisAnim = GetComponent<Animator>();
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        if(other.gameObject.CompareTag("Player")){
            if(GameManager.instance != null){
                TryToGetMagicBottle();
            }else{
                Debug.LogWarning("GAME MANAGER NOT FOUND. REQUIRED FOR GRASS LOGIC");
            }
        }
    }

    void TryToGetMagicBottle(){
        float chance = magicBottleChance;
        int randomChance = Random.Range(0, 100);
        if(GameManager.instance.currentMagicSlots == GameManager.MAXMAGICSLOTS){
            chance = HalveChances();
        }
        if(chance*100 >= randomChance & canProduceItem){
            GiveMagicBottle();
            StartCoroutine(ItemCooldown());
        }
    }

    float HalveChances(){        
        return magicBottleChance/2;
    }

    void GiveMagicBottle(){
        GameManager.instance.PickedUpMagicEvent();
        thisAnim?.SetTrigger("ItemFound"); //Show the bottle of magic being found
    }

    //After a succesful pick up appearance, wait a bit before thi grass unit grants the player another item
    IEnumerator ItemCooldown(){
        canProduceItem = false;
        yield return new WaitForSeconds(waitBeforeNextPickUp);
        canProduceItem = true;
    }
}
