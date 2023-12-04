using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(StartGame2());
    }

    public IEnumerator StartGame2()
    {
        AudioManager.Instance.StopMusic();
        //AudioManager.Instance.PlaySE("press_start");
        yield return new WaitForSeconds(2f);
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager != null)
        {
            //GameManager.Instance.NewGame();
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
        
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }

    public void ResetGame()
    {
        //GameManager.Instance.NewGame();
    }
}
