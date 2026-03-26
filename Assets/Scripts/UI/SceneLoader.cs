using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject fade;
    public int sceneIndex;

    private void Start()
    {
        StartCoroutine(fadeDisapear());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
    }
    public void sceneTutorial()
    {
        sceneIndex = 1; 
        StartCoroutine(scene1Load());
    }
    public void sceneBoss()
    {
        sceneIndex = 2;
        StartCoroutine(scene1Load());
    }
    public void endGame()
    {
        sceneIndex = 3;
        StartCoroutine(scene1Load());
    }
    IEnumerator scene1Load()
    {
        fade.GetComponent<Image>().enabled = true;
        fade.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(1f);
        switch(sceneIndex)
        {
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                break;
            case 2:
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                break;
            case 3:
                Application.Quit();
                break;
        }
    }
    IEnumerator fadeDisapear()
    {
        yield return new WaitForSeconds(1.5f);
        fade.GetComponent<Image>().enabled = false;
    }
}
