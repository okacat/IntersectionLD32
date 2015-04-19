using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    // Scoring
    public static float scoreTargetTruck = 4;
    public static float scoreTargetCar = 2;
    public static float penaltyCar = -1;
    public static float penaltyTruck = -2;
    public float secondsForPoint = 1.0f;
    public float score;
    public float lastScore = -1;

    // State of game
    public enum GameState { Title, Play, ReplayScreen};
    public GameState gameState = GameState.Title;

    // UI Elements
    public Text scoreText;
    public Button playButton;
    public Button exitButton;
    public Text titleText;
    private Text playText;
    private Text exitText;
    public Text aText;
    public Text sText;
    public Text helpText;
    private Image backButtonImage;
    private Image retryButtonImage;
    public Button backButton;
    public Button retryButton;
    public GameObject timerBar;
    private Image timerBarImage;
    public Image screenOverlay;

    // Stuff to tween
    public GameObject vehiclesRoot;

    // Spawners
    public Spawner[] spawners;


    // Light to fade
    public Light sun;
    public Color sunDay;
    public Color sunNight;

    // Play timing
    public float playTimer = 0.0f;
    public float maxTimer;
    private Vector3 barScale;


    // Singleton stuff
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {

            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UIManager>();
            }
                
            return instance;
        }
    }

    
	void Start () {

        gameState = GameState.Title;

        sun.intensity = 0.1f;
        sun.color = sunNight;

        playText = playButton.transform.GetChild(0).GetComponent<Text>();
        exitText = exitButton.transform.GetChild(0).GetComponent<Text>();
        
        backButtonImage = backButton.gameObject.GetComponent<Image>();
        retryButtonImage = retryButton.gameObject.GetComponent<Image>();
        timerBarImage = timerBar.gameObject.GetComponent<Image>();

        backButtonImage.color = new Color(1, 1, 1, 0.0f);
        retryButtonImage.color = new Color(1, 1, 1, 0.0f);
        timerBarImage.color = new Color(1, 1, 1, 0.0f);

       // scoreText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        aText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        sText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        barScale = Vector3.one;

        ResetScore();



        
	}
	
	void Update () {
        if (gameState == GameState.Play)
        {
            if (playTimer > 0.0f)
            {
                SetTimer(playTimer - Time.deltaTime);
            }
            else
            {
                // Time's out
                lastScore = score;
                // Disable the spawners
                for (int i = 0; i < spawners.Length; i++)
                {
                    spawners[i].DisableSpawner();
                }
                StartCoroutine(FadeOutOfGame());
            }
        }
	}

    public void AddToScore(float add)
    {
        score += add;
        scoreText.text = Mathf.RoundToInt(score).ToString();

        if(add > 0) {
            SetTimer(playTimer + add * secondsForPoint);
        }
        

        Debug.Log("Added " + add + " to score");
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();

        playTimer = maxTimer;
    }

    public void SetTimer(float time)
    {
        playTimer = time;
        playTimer = Mathf.Clamp(playTimer, 0.0f, maxTimer);
        barScale.x = time / maxTimer;
        timerBar.transform.localScale = barScale;
    }

    public void PlayClicked()
    {
        Debug.Log("Play clicked!");

        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].EnableSpawner();
        }

        ResetScore();

        StartCoroutine(FadeIntoGame());

        
    }

    public void BackClicked()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].DisableSpawner();
        }

        ResetScore();

        StartCoroutine(FadeOutOfGame());

    }

    public void ExitClicked()
    {
        Application.Quit();
    }

    public IEnumerator FadeIntoGame()
    {
        playButton.interactable = false;
        exitButton.interactable = false;

        StartCoroutine(MyAudioManager.Instance.FadeInMusic());

        SetTimer(maxTimer);

        // Move the vehicles root back to 0
        vehiclesRoot.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        float duration = 1.5f;

        Color guiColorFadeOut = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Color guiColorFadeIn = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime*(1.0f / duration))
        {

            // If this is the last iteration, tween to end (alpha could stay at eg. 0.0532 and still be noticable )
            if (i > 0.95f)
            {
                i = 1.0f;
            }

            // Tween the color of the gui elements
            guiColorFadeIn.a = Mathf.Lerp(0.0f, 1.0f, i);
            guiColorFadeOut.a = Mathf.Lerp(1.0f, 0.0f, i);

            // Tween sun intensity
            sun.intensity = Mathf.Lerp(0.1f, 1.0f, i);
            sun.color = Color.Lerp(sunNight, sunDay, i);

            // Tween the color
            guiColorFadeIn.a = Mathf.Lerp(0.0f, 1.0f, i);
            guiColorFadeOut.a = Mathf.Lerp(1.0f, 0.0f, i);

            // Tween alphas of the title screen text
            titleText.color = guiColorFadeOut;
            playText.color = guiColorFadeOut;
            exitText.color = guiColorFadeOut;
            helpText.color = guiColorFadeOut;

            // Tween score text and others
            //scoreText.color = guiColorFadeIn;
            backButtonImage.color = guiColorFadeIn;
            retryButtonImage.color = guiColorFadeIn;
            timerBarImage.color = guiColorFadeIn;
            aText.color = guiColorFadeIn;
            sText.color = guiColorFadeIn;
            if (lastScore == -1)
            {
               // scoreText.color = guiColorFadeIn;
            }

            yield return new WaitForEndOfFrame();
        }

        retryButton.interactable = true;
        backButton.interactable = true;

        gameState = GameState.Play;

        yield break;
    }

    public IEnumerator FadeOutOfGame()
    {
        gameState = GameState.Title;
        retryButton.interactable = false;
        backButton.interactable = false;

        StartCoroutine(MyAudioManager.Instance.FadeOutMusic());


        float duration = 1.5f;

        Color guiColorFadeOut = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Color guiColorFadeIn = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * (1.0f / duration))
        {
            // Tween the color
            guiColorFadeIn.a = Mathf.Lerp(0.0f, 1.0f, i);
            guiColorFadeOut.a = Mathf.Lerp(1.0f, 0.0f, i);

            // Tween sun intensity
            sun.intensity = Mathf.Lerp(1.0f, 0.1f, i);
            sun.color = Color.Lerp(sunDay, sunNight, i);

            // Tween alphas of the title screen text
            titleText.color = guiColorFadeIn;
            playText.color = guiColorFadeIn;
            exitText.color = guiColorFadeIn;
            helpText.color = guiColorFadeIn;


            // Tween score text and buttons
            backButtonImage.color = guiColorFadeOut;
            retryButtonImage.color = guiColorFadeOut;
            timerBarImage.color = guiColorFadeOut;
            aText.color = guiColorFadeOut;
            sText.color = guiColorFadeOut;
            
            if (lastScore != -1)
            {
                //scoreText.color = guiColorFadeOut;
            }

            // Tween the position of the vehicles root, sink into the ground
            vehiclesRoot.transform.position = new Vector3(0.0f, Mathf.Lerp(0.0f, -2.5f, i*2.0f), 0.0f);

            
            yield return new WaitForEndOfFrame();
        }

        // Destroy all the vehicles
        DestroyAllVehicles();

        playButton.interactable = true;
        exitButton.interactable = true;

        yield break;
    }

    public void DestroyAllVehicles()
    {
        for (int i = 0; i < vehiclesRoot.transform.childCount; i++)
        {
            Destroy(vehiclesRoot.transform.GetChild(i).gameObject);
        }
    }
}
