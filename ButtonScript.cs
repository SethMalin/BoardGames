//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//ButtonScript handles any UI menu interaction

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject loadMenuUI;
    public GameObject ruleMenuUI;
    public GameObject quitMenuUI;
    public GameObject saveManager;
    public GameObject scrollbar;
    public GameObject musicManager;
    public PieceManager pieceManager;
    public Button menuButton;
    public bool isGame;

    private float time = -1;

    /// <summary>
    /// Call at start of execution. Find MusicManager gameobject and set the sound image accordingly.
    /// </summary>
    private void Start() {
        musicManager = GameObject.Find("MusicManager");

        if (pauseMenuUI != null && musicManager.GetComponent<MusicScript>().isGame) {
            pauseMenuUI.SetActive(true);

            if (musicManager.GetComponent<MusicScript>().playing) {
                GameObject.Find("SoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
                GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = true;
                GameObject.Find("NoSoundButton").GetComponent<Image>().color = Color.clear;
                GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = false;
            }
            else {
                GameObject.Find("SoundButton").GetComponent<Image>().color = Color.clear;
                GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = false;
                GameObject.Find("NoSoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
                GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = true;
            }
            pauseMenuUI.SetActive(false);
            musicManager.GetComponent<MusicScript>().isGame = false;
        }
        else {
            if (musicManager.GetComponent<MusicScript>().playing) {
                GameObject.Find("SoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
                GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = true;
                GameObject.Find("NoSoundButton").GetComponent<Image>().color = Color.clear;
                GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = false;
            }
            else {
                GameObject.Find("SoundButton").GetComponent<Image>().color = Color.clear;
                GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = false;
                GameObject.Find("NoSoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
                GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = true;
            }
        }
    }
    
    /// <summary>
    /// Load given scene.
    /// </summary>
    /// <param name="scene">scene name</param>
    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Open pause menu.
    /// </summary>
    public void PauseMenu() {
        Time.timeScale = 0f;
        menuButton.GetComponent<Button>().interactable = false;
        pauseMenuUI.SetActive(true);
    }

    /// <summary>
    /// Resume game.
    /// </summary>
    public void Resume() {
        Time.timeScale = 1f;
        menuButton.GetComponent<Button>().interactable = true;
        pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// Turn the music on and off.
    /// </summary>
    public void MusicOnOff() {
        if (musicManager.GetComponent<AudioSource>().isPlaying) {
            musicManager.GetComponent<AudioSource>().Pause();
            musicManager.GetComponent<MusicScript>().playing = false;
            GameObject.Find("SoundButton").GetComponent<Image>().color = Color.clear;
            GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = false;
            GameObject.Find("NoSoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
            GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = true;
        }
        else {
            musicManager.GetComponent<AudioSource>().Play();
            musicManager.GetComponent<MusicScript>().playing = true;
            GameObject.Find("SoundButton").GetComponent<Image>().color = new Color32(255, 255, 255, 170);
            GameObject.Find("SoundButton").GetComponent<Image>().raycastTarget = true;
            GameObject.Find("NoSoundButton").GetComponent<Image>().color = Color.clear;
            GameObject.Find("NoSoundButton").GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// Select promotion type for pawns in a chess game.
    /// </summary>
    /// <param name="pieceName">name of the type to promote into: knight, bishop, queen or rook</param>
    public void Promote(string pieceName) {
        pieceManager.Promotion(pieceName);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Open the load menu.
    /// </summary>
    /// <param name="b">true if menu is to be on, false otherwise</param>
    public void LoadMenu(bool b) {
        pauseMenuUI.SetActive(!b);
        loadMenuUI.SetActive(b);
    }

    /// <summary>
    /// Open the quit menu in game.
    /// </summary>
    /// <param name="b">true if the menu is to be on, false otherwise</param>
    public void QuitPrompt(bool b) {
        if (time == -1 || Time.timeSinceLevelLoad > time + 10) {
            pauseMenuUI.SetActive(!b);
            quitMenuUI.SetActive(b);
        }
        else {
            SceneManager.LoadScene("Main Menu");
        }
    }

    /// <summary>
    /// Set the value time to the current time in seconds since the level loaded.
    /// </summary>
    public void SetTime() {
        time = Time.timeSinceLevelLoad;
    }

    /// <summary>
    /// Open the rules.
    /// </summary>
    /// <param name="b">true if the menu is to be on, false otherwise</param>
    public void RuleMenu(bool b) {
        scrollbar.GetComponent<Scrollbar>().value = 1;
        pauseMenuUI.SetActive(!b);
        ruleMenuUI.SetActive(b);
    }

    /// <summary>
    /// Set isGame to true when a game is started.
    /// </summary>
    public void SetIsGame() {
        musicManager.GetComponent<MusicScript>().isGame = true;
    }

    /// <summary>
    /// Call SetSlots function.
    /// </summary>
    /// <param name="num">database table number</param>
    public void SlotButtonStates(int num) {
        SetSlots(num, "Slot1", "Slot1Txt", "DelSlot1", "Game 1");
        SetSlots(num + 1, "Slot2", "Slot2Txt", "DelSlot2", "Game 2");
        SetSlots(num + 2, "Slot3", "Slot3Txt", "DelSlot3", "Game 3");
    }

    /// <summary>
    /// Switch load buttons from interactable to empty based on whether a saved file exists.
    /// </summary>
    /// <param name="num">GAME_DATA database ID</param>
    /// <param name="slot">slot name</param>
    /// <param name="slotTxt">slot text</param>
    /// <param name="delSlot">delete slot</param>
    /// <param name="game">game number</param>
    private void SetSlots(int num, string slot, string slotTxt, string delSlot, string game) {
        if (saveManager.GetComponent<DataScript>().GetLoadState(num) == 1) {
            loadMenuUI.transform.Find(slot).transform.Find(slotTxt).GetComponent<Text>().text = game;
            loadMenuUI.transform.Find(slot).GetComponent<Button>().interactable = true;
            if (loadMenuUI.transform.Find(delSlot) != null)
                loadMenuUI.transform.Find(delSlot).GetComponent<Button>().interactable = true;
        }
        else {
            loadMenuUI.transform.Find(slot).Find(slotTxt).GetComponent<Text>().text = "Empty";
            loadMenuUI.transform.Find(slot).GetComponent<Button>().interactable = false;
            if (loadMenuUI.transform.Find(delSlot) != null)
                loadMenuUI.transform.Find(delSlot).GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Quit the application entirely.
    /// </summary>
    public void Quit() {
        Application.Quit();
    }
}
