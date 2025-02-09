using UnityEngine;
using System.Collections;
using TMPro;

namespace HR
{
public class HeartRateDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text worldTextLine1; 
    [SerializeField]
    private TMP_Text worldTextLine2;

    private Coroutine coroutine;

    void Start()
    {
        worldTextLine1.text = "IP: ---.---.---.--- : Port: ----";
        worldTextLine2.text = "BPM: -.-";
        InvokeRepeating(nameof(UpdateCanvas), 0f, 1f);
        coroutine = StartCoroutine(UpdateCanvas());
    }

    void OnDestroy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator UpdateCanvas()
    {
        while (true)
        {
            worldTextLine1.text = $"IP: {HeartRateServer.instance.localIP} : Port: {HeartRateServer.instance.port}";
            worldTextLine2.text = $"BPM: {HeartRateServer.instance.currentBPM}";
            yield return new WaitForSeconds(1f);
        }
    }
}
}