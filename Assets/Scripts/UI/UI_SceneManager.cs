using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_SceneManager : MonoBehaviour
{
    public GameObject tutorial_Text;

    public static bool showTut = false;

    void Start()
    {
        
    }

    public void Update()
    {
        if (showTut)
        {
            tutorial_Text.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(tutorialText());
            }
        }
    }

    public IEnumerator tutorialText()
    {
        yield return new WaitForSeconds(7f);
        tutorial_Text.SetActive(false);
    }

    public void controlSchemeSelector()
    {
        SceneManager.LoadScene(1);
    }

    public void controlScheme1()
    {
        SceneManager.LoadScene(2);
    }

    public void controlScheme2()
    {
        SceneManager.LoadScene(3);
    }

    public void controlScheme3()
    {
        SceneManager.LoadScene(4);
    }

    public void controlScheme4()
    {
        SceneManager.LoadScene(5);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
