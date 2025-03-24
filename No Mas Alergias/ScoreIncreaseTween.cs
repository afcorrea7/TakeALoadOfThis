/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreIncreaseTween : MonoBehaviour
{
    public enum Scenes{
        Prick,
        Kit,
        FinalScoreScreen
    }
    private TextMeshProUGUI scoreText;
    [SerializeField] private Scenes sceneScoreSource; 
    private int playerScore = 0;
    [SerializeField] private float animDuration;

    [SerializeField] private ScoreBasedButtons scoreBasedButtons;

    void Start()
    {
        scoreBasedButtons.DeactivateButtons();
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        playerScore = SelectScoreSource();

        if(sceneScoreSource != Scenes.FinalScoreScreen){ //Only show panel buttons if it's a level score
            scoreBasedButtons.DisplayButtons(playerScore);
        }


        StartCoroutine(AnimateScore(0, playerScore, animDuration));
    }

    private int SelectScoreSource(){ //Which score should be chosen to display? Should depend on what level is the player in at the moment.
        switch(sceneScoreSource){
            case Scenes.Prick:
                return ScoreManager._instance.prickTestScore;
            case Scenes.Kit:
                return ScoreManager._instance.kitScore;
            case Scenes.FinalScoreScreen:
                return ScoreManager._instance.GetTotalScorePercentage();
            default: //Should not happen
                return ScoreManager._instance.prickTestScore;
        }
    }

    //Rapidly animate the score increasing from 0 to its actual value
    private IEnumerator AnimateScore(int startScore, int endScore, float animDuration)
    {
        float elapsedTime = 0f;
        while(elapsedTime < animDuration){
            elapsedTime += Time.deltaTime;
            int currentScore = Mathf.FloorToInt(Mathf.Lerp(startScore, endScore, elapsedTime / animDuration));
            scoreText.text = currentScore.ToString()+"%";
            yield return null;
        }

        scoreText.text = endScore.ToString()+"%";
    }
}


[Serializable]
public class ScoreBasedButtons{
    //Show Panel Buttons based on how the player performed
    [SerializeField] private bool ableToShowButtons; 
    [SerializeField] private GameObject resetBTN;
    [SerializeField] private GameObject continueBTN;
    [SerializeField] private float offsetPosition;

    public void DeactivateButtons(){
        if(ableToShowButtons){
            resetBTN.SetActive(false);
            continueBTN.SetActive(false);
        }
    }

    public void DisplayButtons(int playerScore){
        if(ableToShowButtons){
            switch(playerScore){
                case < ScoreManager.MINLEVELSCORE:
                    ActivateButton(resetBTN, 0);
                    break;
                case ScoreManager.MAXLEVELSCORE:
                    ActivateButton(continueBTN, 0);
                    break;
                default:
                    ActivateButton(resetBTN, -offsetPosition); //place button to the left
                    ActivateButton(continueBTN, offsetPosition); //place button to the right
                    break;
            }
        }
    }

    void ActivateButton(GameObject button, float offsetPosition){
        button.SetActive(true);
        RectTransform buttonPos = button.GetComponent<RectTransform>();
        buttonPos.anchoredPosition = OffsetPosition(buttonPos, offsetPosition);
    }

    Vector2 OffsetPosition(RectTransform rectTransform, float offset){
        Vector2 btnPos = rectTransform.anchoredPosition;
        btnPos.x = offset;
        return btnPos;
    }
}
