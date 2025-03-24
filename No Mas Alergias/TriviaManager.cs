/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class TriviaManager : MonoBehaviour
{
    [Header("Trivia sources")]
    [SerializeField] TriviaPanelTemplate allergiesTrivia;
    [SerializeField] TriviaPanelTemplate allergensTrivia;
    [SerializeField] TriviaPanelTemplate vaccinesTrivia;

    [Header("Trivia Display")]
    [SerializeField] GameObject triviaBox;
    [SerializeField] Image triviaBoxImage; //The image containing all the trivia info
    [SerializeField] Image triviaImage; //An image in the corner of the box to give the trivia some flavor
    [SerializeField] TextMeshProUGUI triviaTitle;
    [SerializeField] TextMeshProUGUI triviaInfo;
    [SerializeField] Image triviaButtonImage;

    void Start(){
        triviaBox.SetActive(false);
    }

    public void SetCategory(TriviaCategory.Category category){
        switch(category){
            case TriviaCategory.Category.Allergens:
                GenerateTrivia(allergensTrivia);
                break;
            case TriviaCategory.Category.Allergies:
                GenerateTrivia(allergiesTrivia);
                break;
            case TriviaCategory.Category.Vaccines:
                GenerateTrivia(vaccinesTrivia);
                break;
        }
    }

    void GenerateTrivia(TriviaPanelTemplate triviaPanel){        
        triviaBoxImage.sprite = triviaPanel.triviaPanelImage;
        triviaButtonImage.sprite = triviaPanel.triviaContinueBTN;
        //Select a random trivia from the retrieved trivia object
        TriviaBox selectedTrivia = triviaPanel.triviaBoxes[Random.Range(0, triviaPanel.triviaBoxes.Count)];
        triviaTitle.text = selectedTrivia.triviaTitle;
        triviaInfo.text = selectedTrivia.triviaInfo;
        triviaImage.sprite = selectedTrivia.triviaImage;

        triviaBox.SetActive(true);
    }
}