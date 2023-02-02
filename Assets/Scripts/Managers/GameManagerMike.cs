using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMike : MonoBehaviour
{
    public int colour; 

    [Header("Inspector Variables")]
    public GameObject pauseMenu;
    public GameObject topBar;
    public Text highScoreText;
    public Text currentScoreText;
    public Text pauseHighScoreText;

    [Header("Timer Variables")]
    public GameObject timerScreen;
    public Text timerText;
    private float timer = 3f;
    private bool loadTimer = false;

    public static bool isPause = false;

    public TrailRenderer playerTrailRenderer;

    [Header("Colours")]
    public Color[] RedPallete = { new Color(255, 0, 0, 255)};
    public Color[] GreenPallete = { new Color(193, 255, 28, 255)};
    public Color[] BluePallete = { new Color(141, 189, 255, 255)};
    public Color[] OrangePallete = { new Color(255, 175, 122, 255)};

    private void Awake()
    {
        colour = Random.Range(0, 4);
        highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString("0");

        playerTrailRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<TrailRenderer>();

        ColourChecker();
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        loadTimer = false;
        isPause = false;
    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause();
        }

        if (loadTimer)
        {
            Timer();
        }
        else
        {
            timerScreen.SetActive(false);
        }

        currentScoreText.text = GrappleHookController.score.ToString("0");

        if (PlayerPrefs.GetInt("HighScore") < GrappleHookController.score)
        {
            highScoreText.text = GrappleHookController.score.ToString("0");
            pauseHighScoreText.text = GrappleHookController.score.ToString("0");

            if (GrappleHookController.score < 5)
            {
                GrappleHookController.highScoreAudioPlayed = true;
            }

            if (!GrappleHookController.highScoreAudioPlayed && GrappleHookController.score != 0)
            {
                AudioManager.highScoreAudio.Play();

                GrappleHookController.highScoreAudioPlayed = true;
            }
        }
    }

    public void ColourChecker()
    {
        switch (colour)
        {
            case 0:
                // this.gameObject.GetComponent<Renderer>().material.color= RedPallete[Random.Range(0, 4)];
                //  transform.GetComponent<Renderer>().material.color = RedPallete[Random.Range(0, 4)];

                ChangeTrailRenderer(RedPallete[0]);

                break;
            case 1:
                //this.gameObject.GetComponent<Renderer>().material.color = GreenPallete[Random.Range(0, 4)];
                //   transform.GetComponent<Renderer>().material.color = GreenPallete[Random.Range(0, 4)];

                ChangeTrailRenderer(GreenPallete[0]);
                break;
            case 2:
                // this.gameObject.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];
                //  transform.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];

                ChangeTrailRenderer(BluePallete[0]);

                break;

            case 3:
                // this.gameObject.GetComponent<Renderer>().material.color = OrangePallete[Random.Range(0, 4)];
                //  transform.GetComponent<Renderer>().material.color =OrangePallete[Random.Range(0, 4)];

                ChangeTrailRenderer(OrangePallete[0]);

                break;

            default:
                ChangeTrailRenderer(BluePallete[0]);

                ChangeTrailRenderer(BluePallete[0]);

                break;
        }
    }

    public void ChangeTrailRenderer(Color color)
    {
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        playerTrailRenderer.colorGradient = gradient;
    }

    public void OnPause()
    {
        if (isPause)
        {
            pauseMenu.SetActive(false);

            loadTimer = true;

        }
        else
        {
            pauseMenu.SetActive(true);
            topBar.SetActive(false);
            isPause = true;

            timer = 3f;

            Time.timeScale = 0;
        }
    }

    private void Timer()
    {
        timerScreen.SetActive(true);

        if (timer> 0f)
        {
            timer -= Time.unscaledDeltaTime;

            timerText.text = timer.ToString("0");
        }
        else
        {
            loadTimer = false;
            timerScreen.SetActive(false);

            topBar.SetActive(true);
            isPause = false;

            Time.timeScale = 1;
        }
    }

    public void LoadNewScene(string sceneName)
    {
        if (PlayerPrefs.GetInt("HighScore") < GrappleHookController.score)
        {
            PlayerPrefs.SetInt("HighScore", GrappleHookController.score);
        }
        SceneManager.LoadScene(sceneName);
    }

    public void OnQuit()
    {
        if (PlayerPrefs.GetInt("HighScore") < GrappleHookController.score)
        {
            PlayerPrefs.SetInt("HighScore", GrappleHookController.score);
        }
        Application.Quit();
    }
}
