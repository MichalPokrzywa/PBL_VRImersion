using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Axe : MonoBehaviour
{
    public GameObject axe;
    private float rotation=180;
    [SerializeField] float speed;
    public static Action onDeath;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level_F");
        }
    }
    // Update is called once per frame
    void Update()
    {
        float target = Mathf.PingPong(Time.time*speed, rotation);
        transform.rotation = Quaternion.Euler(target-90,0,0);
    }
}
