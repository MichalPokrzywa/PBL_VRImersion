using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TextButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Text text;
    [SerializeField] bool removeActionOnClose = true;

    public bool RemoveActionOnClose => removeActionOnClose;
    public Button Button => button;

    UnityAction registeredAction;

    void OnDestroy()
    {
        RemoveButtonAction();
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void SetButtonAction(UnityAction newAction)
    {
        registeredAction = newAction;
        button.onClick.AddListener(registeredAction);
    }

    public void RemoveButtonAction()
    {
        if (registeredAction != null)
            button.onClick.RemoveListener(registeredAction);
    }
}
