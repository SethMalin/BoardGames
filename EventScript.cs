using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    //Variable instantiation
    public GameObject saveData;
    public GameObject winMenu;
    public GameObject spinButton;
    public GameObject skipButton;
    public GameObject questionButton;
    public GameObject categories;
    public GameObject correctDot;
    public GameObject wrongDot;
    public GameObject[] dots;
    public GameObject[] chalkMarks;
    public GameObject[] cheeses;
    public Button[] card;
    public Sprite[] cardColors;
    public Sprite[] questColors;
    public AudioClip correctAudio;
    public AudioClip wrongAudio;
    public AudioClip winAudio;
    public int histIndex = 0;
    public int langIndex = 0;
    public int ricIndex = 0;
    public int vgIndex = 0;
    public int steps = 0;
    public int before = 3;
    public int right = 0;
    public int currentCat;
    public int[] points = { 0, 0, 0, 0 };
    public bool[] cheeseWon = { false, false, false, false };

    private AudioSource audioSource;
    private IEnumerator co;
    private int rgn;

    /// <summary>
    /// Call when game starts.
    /// Set audio sources.
    /// Load saved data when game loaded.
    /// </summary>
    private void Start() {
        Time.timeScale = 1f;
        if(gameObject.GetComponent<AudioSource>() != null)
            audioSource = gameObject.GetComponent<AudioSource>();
        if (categories != null)
            categories.GetComponent<CategoryScript>().ShuffleLists();
        if (saveData != null)
            if (saveData.GetComponent<DataScript>().LoadTriviaData()) {
                for (int i = 0; i < 4; i++) {
                    if (cheeseWon[i])
                        cheeses[i].SetActive(true);
                }

                //Display points on screen
                DisplayPoints(0, 15);
                DisplayPoints(1, 10);
                DisplayPoints(2, 5);
                DisplayPoints(3, 0);

                if (dots.Length > 0)
                    dots[currentCat].SetActive(true);

                if (card.Length > 0) {
                    if (card[0].transform.Find("cardTxt").GetComponent<Text>().text != "") {
                        spinButton.GetComponent<Button>().interactable = false;
                        spinButton.GetComponent<Image>().color = Color.clear;
                        questionButton.SetActive(true);
                        //questionButton.transform.Find("questionTxt").GetComponent<Text>().text = arr[0];
                        questionButton.transform.Find("questionTxt").GetComponent<Text>().fontSize = 60;
                        questionButton.GetComponent<Image>().sprite = questColors[currentCat];
                        foreach (Button c in card) {
                            c.GetComponent<Image>().sprite = cardColors[currentCat];
                            c.interactable = true;
                        }
                    }
                    else {
                        foreach (Button c in card) {
                            c.GetComponent<Image>().sprite = cardColors[4];
                            c.interactable = false;
                        }
                    }
                }
            }
    }

    /// <summary>
    /// Action when mouse hovers over gameobject.
    /// </summary>
    public void MouseHover() {
        transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);
    }

    /// <summary>
    /// Action when mouse no longer hovers over gameobject.
    /// </summary>
    public void MouseExit() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 20, transform.position.z);
    }

    /// <summary>
    /// Action when a card is clicked.
    /// </summary>
    /// <param name="num">clicked card number</param>
    public void CardClick(int num) {

        //Turn SPIN! button back on
        spinButton.GetComponent<Button>().interactable = true;
        spinButton.GetComponent<Image>().color = Color.white;

        //Disable cards
        foreach (Button button in card) {
            button.GetComponent<Image>().sprite = cardColors[4];
            button.transform.Find("cardTxt").GetComponent<Text>().text = "";
            button.GetComponent<Button>().interactable = false;
        }
        questionButton.SetActive(false);

        foreach (GameObject dot in dots)
            dot.SetActive(false);

        //Check if correct answer is selected
        if (num == right) { 
            if (points[currentCat] < 5) { 
                points[currentCat]++;
                if (points[currentCat] == 5) {
                    cheeses[currentCat].SetActive(true);
                    cheeseWon[currentCat] = true;
                    audioSource.PlayOneShot(winAudio, 1f);
                }
            }
            correctDot.SetActive(true);
            audioSource.PlayOneShot(correctAudio, 0.7f);
        }
        else {
            wrongDot.SetActive(true);
            audioSource.PlayOneShot(wrongAudio, 0.4f);
        }

        //Display points on screen
        DisplayPoints(0, 15);
        DisplayPoints(1, 10);
        DisplayPoints(2, 5);
        DisplayPoints(3, 0);

        int i = 0;

        //Displays cheeses
        foreach (GameObject cheese in cheeses) {
            if (cheese.activeSelf) {
                cheeseWon[i] = true;       
            }
            i++;
        }

        //Check if all cheeses are active, if true, game is won
        if (cheeseWon[0] && cheeseWon[1] && cheeseWon[2] && cheeseWon[3]) {
            winMenu.SetActive(true);
            spinButton.GetComponent<Button>().interactable = false;
            spinButton.GetComponent<Image>().color = Color.clear;
        }
    }

    /// <summary>
    /// Acion when spin button is clicked.
    /// </summary>
    public void Spin() {
        spinButton.GetComponent<Button>().interactable = false;
        spinButton.GetComponent<Image>().color = Color.clear;
        skipButton.SetActive(true);
        correctDot.SetActive(false);
        wrongDot.SetActive(false);
        rgn = Random.Range(20, 30);

        co = BlinkingDots(rgn);
        StartCoroutine(co);
    }

    /// <summary>
    /// Action when skip button is clicked.
    /// </summary>
    public void Skip() {

        StopCoroutine(co);

        int count = 0;

        while (count != rgn) {

            //Loop
            dots[before].SetActive(false);
            dots[steps].SetActive(true);
            audioSource.Play();
            before++;
            steps++;
            count++;

            if (count == rgn)
                break;
            if (steps == 4)
                steps = 0;
            if (before == 4)
                before = 0;
        }
        skipButton.SetActive(false);
        CheckForDots();
    }

    /// <summary>
    /// Blinking animations.
    /// </summary>
    /// <param name="rgn">randomly generated number</param>
    /// <returns></returns>
    public IEnumerator BlinkingDots(int rgn) {
        float speed = 0.1f;
        int count = 0;

        while (count != rgn) {

            //Slow down speed
            if (count > rgn * 0.85)
                speed = 0.5f;
            else if (count > rgn * 0.6)
                speed = 0.3f;
            else if (count > rgn * 0.35)
                speed = 0.15f;
            //Loop
            dots[before].SetActive(false);
            dots[steps].SetActive(true);
            audioSource.Play();
            before++;
            steps++;
            count++;

            if (count == rgn)
                break;
            if (steps == 4)
                steps = 0;
            if (before == 4)
                before = 0;

            yield return new WaitForSeconds(speed);
        }
        skipButton.SetActive(false);
        CheckForDots();
    }

    /// <summary>
    /// Check which dot is active and draw corresponding card.
    /// </summary>
    private void CheckForDots() {

        if (dots[0].activeSelf) {
            DisplayOnCard(0, 3, 0, categories.GetComponent<CategoryScript>().histMixed[histIndex]);
            histIndex++;
            if (histIndex == 35) {
                histIndex = 0;
                categories.GetComponent<CategoryScript>().histMixed = categories.GetComponent<CategoryScript>().GetMixedList(categories.GetComponent<CategoryScript>().histMixed);
            }
        }

        if (dots[1].activeSelf) {
            DisplayOnCard(1, 0, 1, categories.GetComponent<CategoryScript>().langMixed[langIndex]);
            langIndex++;
            if (langIndex == 35) {
                langIndex = 0;
                categories.GetComponent<CategoryScript>().langMixed = categories.GetComponent<CategoryScript>().GetMixedList(categories.GetComponent<CategoryScript>().langMixed);
            }
        }

        if (dots[2].activeSelf) {
            DisplayOnCard(2, 1, 2, categories.GetComponent<CategoryScript>().ricMixed[ricIndex]);
            ricIndex++;
            if (ricIndex == 35) {
                ricIndex = 0;
                categories.GetComponent<CategoryScript>().ricMixed = categories.GetComponent<CategoryScript>().GetMixedList(categories.GetComponent<CategoryScript>().ricMixed);
            }
        }

        if (dots[3].activeSelf) {
            DisplayOnCard(3, 2, 3, categories.GetComponent<CategoryScript>().vgMixed[vgIndex]);
            vgIndex++;
            if (vgIndex == 35) {
                vgIndex = 0;
                categories.GetComponent<CategoryScript>().vgMixed = categories.GetComponent<CategoryScript>().GetMixedList(categories.GetComponent<CategoryScript>().vgMixed);
            }
        }
    }

    /// <summary>
    /// Shuffle answers and display proper information on cards.
    /// </summary>
    /// <param name="index">current category array index</param>
    /// <param name="bef">dot that comes before current category</param>
    /// <param name="ste">dot for current category</param>
    /// <param name="arr">question and its answers</param>
    private void DisplayOnCard(int index, int bef, int ste, string[] arr) {

        string[] ans = Shuffle(arr[1], arr[2], arr[3], arr[4]);
        currentCat = index;

        int i = 0;

        if (dots[index].activeSelf) {
            before = bef;
            steps = ste;

            foreach (Button button in card) {
                button.GetComponent<Image>().sprite = cardColors[index];
                button.transform.Find("cardTxt").GetComponent<Text>().text = ans[i];
                button.GetComponent<Button>().interactable = true;
                i++;
            }

            questionButton.SetActive(true);
            questionButton.transform.Find("questionTxt").GetComponent<Text>().text = arr[0];
            questionButton.transform.Find("questionTxt").GetComponent<Text>().fontSize = 60;
            questionButton.GetComponent<Image>().sprite = questColors[index];
        }
    }

    /// <summary>
    /// Shuffle the four answers
    /// </summary>
    /// <param name="a">answer 1</param>
    /// <param name="b">answer 2</param>
    /// <param name="c">answer 3</param>
    /// <param name="d">answer 4</param>
    /// <returns></returns>
    private string[] Shuffle(string a, string b, string c, string d) {
        string[] s = new string[] { a, b, c, d };

        for (int i = 0; i < s.Length; i++) {
            int rgn = Random.Range(0, 3);
            string temp = s[i];
            s[i] = s[rgn];
            s[rgn] = temp;
        }

        for (int i = 0; i < s.Length; i++)
            if (s[i].Equals(a))
                right = i;

        return s;
    }

    /// <summary>
    /// Display points.
    /// </summary>
    /// <param name="num">category number</param>
    /// <param name="index">checkmark index</param>
    private void DisplayPoints(int num, int index) {

        for (int i = 0; i < points[num]; i++)
            chalkMarks[index + i].SetActive(true);
    }

    /// <summary>
    /// Reset game to its original state in order for a new game to begin.
    /// </summary>
    public void ResetGame() {

        categories.GetComponent<CategoryScript>().ShuffleLists();

        foreach (GameObject cheese in cheeses)
            cheese.SetActive(false);

        foreach (GameObject chalk in chalkMarks)
            chalk.SetActive(false);

        foreach (GameObject dot in dots)
            dot.SetActive(false);

        histIndex = 0;
        langIndex = 0;
        ricIndex = 0;
        vgIndex = 0;
        cheeseWon = new bool[] { false, false, false, false };
        points = new int[] { 0, 0, 0, 0 };

        winMenu.SetActive(false);
        correctDot.SetActive(false);
        wrongDot.SetActive(false);

        spinButton.GetComponent<Button>().interactable = true;
        spinButton.GetComponent<Image>().color = Color.white;
    }
}
