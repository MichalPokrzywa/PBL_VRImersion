using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VRHubPanel : MonoBehaviour
{
    [SerializeField] Button button_LvlD;
    [SerializeField] Button button_LvlE;
    [SerializeField] Button button_LvlRooftop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        button_LvlD.onClick.AddListener(() =>LoadLevel(1));
        button_LvlE.onClick.AddListener(() =>LoadLevel(2));
        button_LvlRooftop.onClick.AddListener(() =>LoadLevel(3));

    }

    private void LoadLevel(int v)
    {
        Debug.Log($"Build index: {v}");
        SceneManager.LoadScene(v, LoadSceneMode.Single);
    }

}
