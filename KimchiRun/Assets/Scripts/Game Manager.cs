using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    Intro,
    Playing,
    Dead
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State = GameState.Intro;
    public float PlayStartTime;
    public int lives = 3;

    [Header("References")]
    public GameObject IntroUI;
    public GameObject DeadUI;
    public GameObject EnemySpawner;
    public GameObject FlyEnemySpawner;
    public GameObject FoodSpawner;
    public GameObject GoldenSpawner;
    public Player PlayerScript;
    public TMP_Text scoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IntroUI.SetActive(true);
    }

    float CalculateScore()
    {
        return Time.time - PlayStartTime;
    }

    void SaveHighScore()
    {
        int score = Mathf.FloorToInt(CalculateScore());
        int currentHighScore = PlayerPrefs.GetInt("highScore");
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
    }

    int GetHighScore()
    {
        return PlayerPrefs.GetInt("highScore");
    }

    public float CalculateGameSpeed()
    {
        if(State != GameState.Playing)
        {
            return 5f;
        }
        float speed = 8f + (0.5f * Mathf.Floor(CalculateScore() / 10f));
        return Mathf.Min(speed, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        if (State == GameState.Playing)
        {
            scoreText.text = "SCORE: " + Mathf.FloorToInt(CalculateScore());
        }
        else if (State == GameState.Dead)
        {
            scoreText.text = "HIGH SCORE: " + GetHighScore();
        }
        if (State == GameState.Intro && Input.GetKeyDown(KeyCode.Space))
        {
            State = GameState.Playing;
            IntroUI.SetActive(false);
            EnemySpawner.SetActive(true);
            FlyEnemySpawner.SetActive(true);
            FoodSpawner.SetActive(true);
            GoldenSpawner.SetActive(true);
            PlayStartTime = Time.time;
        }
        if (State == GameState.Playing && lives == 0)
        {
            PlayerScript.KillPlayer();
            EnemySpawner.SetActive(false);
            FlyEnemySpawner.SetActive(false);
            FoodSpawner.SetActive(false);
            GoldenSpawner.SetActive(false);
            SaveHighScore();
            State = GameState.Dead;
            DeadUI.SetActive(true);
        }
        if (State == GameState.Dead && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("main");
        }
    }
}
