/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance { get; set; }
    public int prickTestScore = 0;
    public int kitScore = 0;
    public int totalScore = 0;

    public const int MAXSCORE = 200;
    public const int MAXLEVELSCORE = 100; //Highest possible score in a level
    public const int MINLEVELSCORE = 60; //Minimum score required to progress to the next level

    void Awake(){ //Singleton
        if(_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }

        //event from KitLevelEvents
        LevelKitEvents.OnRetryLevel += ResetKitScore;
        //event from PrickTestLevelEvents
        LevelPrickEvents.OnRetryLevel += ResetPrickTestScore;
    }

    public int CalculateTotalScore(){
        return prickTestScore + kitScore;
    }

    public int GetTotalScorePercentage(){
        totalScore = CalculateTotalScore();
        int percentageTotalScore = Mathf.RoundToInt(totalScore*100/MAXSCORE);
        return percentageTotalScore;
    }

    public void PrintScores(){
        Debug.Log("Prick Test Score: "+prickTestScore);
        Debug.Log("Kit Score: "+kitScore);
        Debug.Log("TotalScore: "+GetTotalScorePercentage()+"%");
    }

    //Reset Methods, for reloading scenes or the game
    public void ResetPrickTestScore(){
        prickTestScore = 0;
    }

    public void ResetKitScore(){
        kitScore = 0;
    }

    public void ResetAll(){
        Destroy(_instance);
    }

    void OnDestroy(){
        LevelKitEvents.OnRetryLevel -= ResetKitScore;
        LevelPrickEvents.OnRetryLevel += ResetPrickTestScore;
    }
}
