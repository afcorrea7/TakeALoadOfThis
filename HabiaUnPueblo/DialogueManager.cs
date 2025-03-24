//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager DMInstance; //Singleton

    void Awake(){
        if (DMInstance != null && DMInstance != this){
            Destroy(gameObject);
        }
        else{
            DMInstance = this;
        }
    }

    /*DIALOGUE MANAGER tells the DialogueCanvas what panel, information and sprites it should display
        A reference to each type of panel is needed so that the manager can know what Panel it should assign values to,
        since each panel has slighly different visual components.
    */
    public AudioClip endDialogueSound;
    public AudioClip nextDialogueSound;

    public GameObject daniPanel;
    public GameObject mythPanel;
    public GameObject NPCPanel;
    public GameObject critterPanel;
    public GameObject infoPanel;
    
    private AudioSource thisAudioSource;
    private GameObject currentPanel;
    private Animator panelAnimator;
    private TMP_Text panelNameText;
    private TMP_Text  panelLineOfText;
    private TMP_Text panelSpeciesText;
    private TMP_Text infoPanelText;
    private Image panelCharacterPortrait;
    private Queue<Line> lines;
    private Queue<string> infoLines;
    private DialoguesTemplate.PostDialogueEvent postDialogue;
    private InfoTemplate.PostInfoBoxEvent postInfo;
    [HideInInspector] public bool dialogueIsActive; //Some scripts need to check if the player is currently in Dialogue
    [HideInInspector] public bool infoBoxIsActive; //or reading an info box
    private bool lineCompleted; //Is the current line done being typed?
    private Line currentLine;
    private string currentInfoText;

    void Start(){
        thisAudioSource = GetComponent<AudioSource>();
        lines = new Queue<Line>();
        infoLines = new Queue<string>();
    }

    void LateUpdate(){
        if(dialogueIsActive){ //if there's a conversation or info queue right now 
            if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)){ //'continue dialogue' input
                if(!lineCompleted){
                    lineCompleted = true; //autocomplete line in corroutine if it hasn't been completed yet
                }else{
                    ProduceNextLine(); //if it is completed, hop onto the next dialogue line
                }
            }
        }
        if(infoBoxIsActive){
            if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)){
                if(!lineCompleted){
                    lineCompleted = true;
                }else{
                    ProduceNextInfoLine();                
                }
            }
        }
    }

//DIALOGUE ----------------------------------------------------------------------------------------------------------------------------------
    public void GetConversation(DialoguesTemplate conversation){
        //Populate Queue
        lines.Clear();
        foreach(var dialogueLine in conversation.dialogueLines){
            lines.Enqueue(dialogueLine);
        }
        postDialogue = conversation.postDialogueEvent;
        //Display first line, all lines from this point on are shown through a 'continue dialogue' input
        ProduceNextLine();
    }

    void ProduceNextLine(){
        PlaySoundByDialogueLine(lines);
        if(lines.Count == 0){ //if there are no dialogue lines left
            EndDialogue();
            return;
        }
        currentLine = lines.Dequeue();
        //PrintConversation(currentLine);
        AssignDialoguePanel(currentLine.character.characterType);
        DisplayDialogue(currentLine);
        if(dialogueIsActive == false){ //dialogueIsActive is set to true inmediately aftet this
            //this asures open dialogue anim only plays in the very first dialogue line
            panelAnimator.SetTrigger("DialogueOpen");
        } 
        StartCoroutine(WaitAndActivateDialogueBool()); //set to true to allow for advancing dialogue with input
        GameManager.instance.TogglePlayerHasControl(false); //don't let the player move or open menus while in dialogue
    }

    void DisplayDialogue(Line currentLine){
        //Display the information from currentline using the assigned panel variables
        panelNameText.text = currentLine.character.speakerName;
        StopAllCoroutines(); //Stop the text coroutine in case it was running before the next line is displayed
        StartCoroutine(TypeText(panelLineOfText, currentLine.text));
        if(IsCritter(currentLine.character.characterType) && panelSpeciesText != null){
            panelSpeciesText.text = currentLine.character.speciesName;
        }
        panelCharacterPortrait.sprite = currentLine.portraitSprite;
    }

    void AssignDialoguePanel(CharacterTemplate.CharacterType characterType){
        if(currentPanel != null){
            currentPanel.SetActive(false);
        }
        //Select Panel and its animator based on character type
        GameObject nextPanel = SelectPanel(characterType);
        panelAnimator = nextPanel?.GetComponent<Animator>();
        //If the next (line of dialogue) is not by the same character as the previous line, assign a panel
        if(currentPanel != nextPanel){
            currentPanel = nextPanel;
            //The variables to fill currentPanel get defined as the components belonging to the selected panel
            panelNameText = FindWithRecursiveSearch(currentPanel.transform, "NameText").GetComponent<TMP_Text>();
            panelLineOfText = FindWithRecursiveSearch(currentPanel.transform, "LineOfText").GetComponent<TMP_Text>();

            if(IsCritter(characterType)){
                panelSpeciesText = FindWithRecursiveSearch(currentPanel.transform, "SpeciesText").GetComponent<TMP_Text>();
            }
            panelCharacterPortrait = FindWithRecursiveSearch(currentPanel.transform, "Portrait").GetComponent<Image>();
        }
        currentPanel.SetActive(true);
    }

    bool IsCritter(CharacterTemplate.CharacterType characterType){
        if(characterType == CharacterTemplate.CharacterType.Critter){
            return true;
        }
        return false;
    }

    GameObject SelectPanel(CharacterTemplate.CharacterType characterType){
        switch(characterType){
            case CharacterTemplate.CharacterType.Protagonist:
                return daniPanel;
            case CharacterTemplate.CharacterType.NPC:
                return NPCPanel;
            case CharacterTemplate.CharacterType.Myth:
                return mythPanel;
            case CharacterTemplate.CharacterType.Critter:
                return critterPanel;
            default: //Shouldn't happen but if it does, default to a dani panel
                return daniPanel;
        }
    }
    
    void EndDialogue(){
        HandlePostDialogueEvent(postDialogue);
        panelAnimator.SetTrigger("DialogueClose"); //activate dialogue close animation
        dialogueIsActive = false;
        StartCoroutine(WaitBeforeDisable(currentPanel));
        Debug.Log("Dialogue has ended");
        Debug.Log(postDialogue);
    }

    void HandlePostDialogueEvent(DialoguesTemplate.PostDialogueEvent postDialogueEvent){
        switch(postDialogueEvent){
            case DialoguesTemplate.PostDialogueEvent.None:
                GameManager.instance.TogglePlayerHasControl(true); //allow player to move again;
                break;

            case DialoguesTemplate.PostDialogueEvent.TundaTransformation:
                DialogueEventManager.instance.TundaTransformationEvent();
                break;

            case DialoguesTemplate.PostDialogueEvent.TundaFightStart:
                DialogueEventManager.instance.TundaFightStartEvent();
                break;

            case DialoguesTemplate.PostDialogueEvent.GoblinFightStart:
                DialogueEventManager.instance.GoblinFightStartEvent();
                break;     

            case DialoguesTemplate.PostDialogueEvent.PipeLeaving:
                DialogueEventManager.instance.PipeLeavingEvent();
                break;

            case DialoguesTemplate.PostDialogueEvent.MarabeliFightStart:
                DialogueEventManager.instance.MarabeliFightStartEvent();
                break;

            case DialoguesTemplate.PostDialogueEvent.TigrilloStepAside:
                DialogueEventManager.instance.TigrilloStepAsideEvent();
                break;

            case DialoguesTemplate.PostDialogueEvent.RivielFightStart:
                DialogueEventManager.instance.RivielFightStartEvent();
                break;
            case DialoguesTemplate.PostDialogueEvent.DemoEnd:
                DialogueEventManager.instance.SetEndOfDemoEvent();
                break;
        }
    }

    void PrintConversation(Line currentLine){
        Debug.Log("Name: "+currentLine.character.speakerName);
        if(currentLine.character.speciesName != ""){
            Debug.Log("Species: "+currentLine.character.speciesName);
        }
        Debug.Log("CharacterType: "+currentLine.character.characterType);
        Debug.Log("Emotion: "+currentLine.portraitSprite);
        Debug.Log("Line: "+currentLine.text);
    }

 //INFORMATION ------------------------------------------------------------------------------------------------------------------------------
    public void GetInfoBox(InfoTemplate info){
        //Populate info Queue
        infoLines.Clear();
        foreach(var infoLine in info.texts){
            infoLines.Enqueue(infoLine);
        }
        postInfo = info.postInfoEvent;
        //Display the first line, all lines from this point on are shown through a 'continue dialogue' input
        infoPanel.SetActive(true);
        ProduceNextInfoLine();
    }

    void ProduceNextInfoLine(){
        PlaySoundByInfoLine(infoLines);
        if(infoLines.Count == 0){ //if there are no lines left
            EndInfo();
            return;
        }
        currentInfoText = infoLines.Dequeue();
        //PrintInfo(currentInfoText);
        DisplayInfo(currentInfoText);
        if(infoBoxIsActive == false){ //is set to true inmediately aftet this
            //this makes sure the anim only plays in the very first line in the info box
            infoPanel.GetComponent<Animator>().SetTrigger("InfoOpen");
        }
        StartCoroutine(WaitAndActivateInfoBool());
        GameManager.instance.TogglePlayerHasControl(false); //don't let the player move or open menus while an info box is present
    }

    void PrintInfo(string currentInfoText){
        Debug.Log("Info: "+currentInfoText);
    }

    void DisplayInfo(string currentInfoText){
        infoPanelText = FindWithRecursiveSearch(infoPanel.transform, "LineOfText").GetComponent<TMP_Text>();
        StopAllCoroutines(); //Stop the text coroutine in case it was running before the next line is displayed
        StartCoroutine(TypeText(infoPanelText ,currentInfoText));
    }

    void EndInfo(){
        HandlePostInfoEvent(postInfo);
        infoPanel.GetComponent<Animator>().SetTrigger("InfoClose");
        infoBoxIsActive = false;
        StartCoroutine(WaitBeforeDisable(infoPanel));
    }

    //2025 HINDSIGHT: Alright, this is a mess. What I know now is that I could have used an scriptable object as a custom game event.
    void HandlePostInfoEvent(InfoTemplate.PostInfoBoxEvent postInfoEvent){
        switch(postInfoEvent){
            case InfoTemplate.PostInfoBoxEvent.None:
                GameManager.instance.TogglePlayerHasControl(true); //let the player move after closing the infobox
                break;
            case InfoTemplate.PostInfoBoxEvent.TundaEntryUnlocked:
                DialogueEventManager.instance.TundaEntryUnlockedEvent();
                break; 
            case InfoTemplate.PostInfoBoxEvent.GoblinEntryUnlocked:
                DialogueEventManager.instance.GoblinEntryUnlockedEvent();
                break;
            case InfoTemplate.PostInfoBoxEvent.MaraveliEntryUnlocked:
                DialogueEventManager.instance.MaraveliEntryUnlockedEvent();
                break;
            case InfoTemplate.PostInfoBoxEvent.RivielEntryUnlocked:
                DialogueEventManager.instance.RivielEntryUnlockedEvent(); 
                break;
        }
    }

 //UTILITY ----------------------------------------------------------------------------------------------------------------------------------
    //Play a sound when advancing dialogue
    void PlaySoundByDialogueLine(Queue<Line> lines){
        if(!dialogueIsActive){ //if false, it means a dialogue is about to activate. Play woosh sound
            thisAudioSource.PlayOneShot(endDialogueSound);
            return;
        }
        if(lines.Count > 0){ //play keypress sound that represents advancing dialogue
            thisAudioSource.PlayOneShot(nextDialogueSound);
            return;
        }
        if(lines.Count == 0){ //dialogue is over. Play woosh sound
            thisAudioSource.PlayOneShot(endDialogueSound);
            return;
        }
    }

    //Play a sound when advancing info text
    void PlaySoundByInfoLine(Queue<string> infoLines){
        //We don't want a woosh sound on infobox start because a notification sound (Myth Captured!) already exists for it.
        if(infoLines.Count > 0 && dialogueIsActive){ //play keypress sound that represents advancing text
            thisAudioSource.PlayOneShot(nextDialogueSound);
            return;
        }
        if(infoLines.Count == 0){ //info is over. Play woosh sound
            thisAudioSource.PlayOneShot(endDialogueSound);
            return;
        }
    }
    
    //To prevent false positive inputs when starting dialogues, introduce a delay before allowing the input to advance them
    IEnumerator WaitAndActivateDialogueBool(){
        yield return new WaitForSeconds(0.3f);
        dialogueIsActive = true;
    }

    IEnumerator WaitAndActivateInfoBool(){
        yield return new WaitForSeconds(0.3f);
        infoBoxIsActive = true;
    }

    //Find a neeeded component that may reside as a child of one of the children of the panel
    GameObject FindWithRecursiveSearch(Transform parent, string searchName){
        for(int i=0; i<parent.transform.childCount; i++){
            if(parent.transform.GetChild(i).name == searchName){
                return parent.transform.GetChild(i).gameObject; //return child name once found
            }
            //Search the children of the child and so on
            GameObject tempParent = FindWithRecursiveSearch(parent.transform.GetChild(i), searchName);
            if(tempParent != null){
                return tempParent;
            }
        }
        return null; //if the object was not found
    }

    //Type the line's text character by character into a text mesh component
    IEnumerator TypeText(TMP_Text panelTextMesh,string lineToType){
        lineCompleted = false;
        panelTextMesh.text = "";
        foreach(var character in lineToType.ToCharArray()){
            panelTextMesh.text += character;
            yield return new WaitForSeconds(0.02f);
            if(lineCompleted){ //player can autocomplete a line through input
                panelTextMesh.text = lineToType;
                break;
            }
        }
        lineCompleted = true; //foreach loop completed, line is now fully typed
    }

    //wait a bit before disabling panel to allow its end animation to play
    IEnumerator WaitBeforeDisable(GameObject panel){
        yield return new WaitForSeconds(0.5f);
        panel.SetActive(false);
    }
}