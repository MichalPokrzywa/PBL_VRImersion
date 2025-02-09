using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static ChangeScene Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("zmiana sceny na menu wygranej");
    }

    public void MenuSecondScene()
    {
        SceneManager.LoadScene("Menu2");
        Debug.Log("zmiana sceny na menu przegranej");
    }
}
