using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VRClickablePanel : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] Button button;

    bool removeActionOnClose = true;
    UnityAction registeredAction;

    void Start()
    {
        button.onClick.AddListener(() => DefaultButtonAction());
    }

    public void ShowPanel(string text, UnityAction action=null)
    {
        SetText(text);
        // Remove previous action if it was set to be removed
        if (registeredAction != null && removeActionOnClose)
        {
            button.onClick.RemoveListener(registeredAction);
        }
        // Set new action if provided
        if (action != null)
        {
            SetButtonAction(action);
        }
        gameObject.SetActive(true);
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

    void DefaultButtonAction()
    {
        gameObject.SetActive(false);
    }
}
