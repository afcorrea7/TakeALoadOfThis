/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TriviaBox{
    public Sprite triviaImage;
    public string triviaTitle;
    [TextArea(2, 5)]
    public string triviaInfo;
}

[CreateAssetMenu(fileName = "Trivia", menuName = "ScriptableObjects/Trivia")]
public class TriviaPanelTemplate : ScriptableObject
{
    public Sprite triviaPanelImage;
    public Sprite triviaContinueBTN;
    public List<TriviaBox> triviaBoxes;
}
