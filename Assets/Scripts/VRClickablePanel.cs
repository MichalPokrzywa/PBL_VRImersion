using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VRClickablePanel : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] Button button;

    UnityAction action;
    bool removeActionOnClose = true;

    void Start()
    {
        button.onClick.AddListener(() => DefaultButtonAction());
    }

    public void ShowPanel(string text, UnityAction action=null)
    {
        SetText(text);
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
        action = newAction;
        button.onClick.AddListener(action);
    }

    void DefaultButtonAction()
    {
        if (action != null && removeActionOnClose)
        {
            button.onClick.RemoveListener(action);
        }
        gameObject.SetActive(false);
    }
}
