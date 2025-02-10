using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VRClickablePanel : MonoBehaviour
{
    [SerializeField] GameObject textButtonPrefab;
    [SerializeField] TextMeshPro textComponent;
    [SerializeField] List<TextButton> textButtons;

    public struct ButtonAction
    {
        public ButtonAction(string text, UnityAction action)
        {
            this.text = text;
            this.action = action;
        }
        public ButtonAction(UnityAction action)
        {
            this.text = string.Empty;
            this.action = action;
        }
        string text;
        UnityAction action;

        public string Text => text;
        public UnityAction Action => action;
    }

    void Start()
    {
        foreach (var textButton in textButtons)
        {
            textButton.Button.onClick.AddListener(ClosePanel);
        }
    }

    void OnDestroy()
    {
        foreach (var textButton in textButtons)
        {
            if (textButton != null)
                textButton.Button?.onClick.RemoveListener(ClosePanel);
        }
    }

    public void ShowPanel(string text, ButtonAction[] buttonActions)
    {
        foreach (var textButton in textButtons)
        {
            textButton.gameObject.SetActive(false);
        }
        for (int i = 0; i < buttonActions.Length; i++)
        {
            if (i >= textButtons.Count)
            {
                var newButton = Instantiate(textButtonPrefab, transform).GetComponent<TextButton>();
                textButtons.Add(newButton);
            }

            string buttonText = buttonActions[i].Text;
            if (buttonText != string.Empty)
                textButtons[i].SetText(buttonText);

            textButtons[i].SetButtonAction(buttonActions[i].Action);
            textButtons[i].gameObject.SetActive(true);
        }
        textComponent.text = text;
        gameObject.SetActive(true);
    }

    void ClosePanel()
    {
        foreach (var textButton in textButtons)
        {
            if (textButton.RemoveActionOnClose)
            {
                textButton.RemoveButtonAction();
            }
        }
        gameObject.SetActive(false);
    }
}
