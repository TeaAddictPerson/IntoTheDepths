using TMPro;
using UnityEngine;

public class UsernameInputValidator : MonoBehaviour
{
    public TMP_InputField inputField;
    private const int maxCharacters = 10; 

    void Start()
    {
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    void ValidateInput(string text)
    {
        string filteredText = "";
        foreach (char c in text)
        {
            if (c >= 'a' && c <= 'z')
            {
                filteredText += c;
            }
        }


        if (filteredText.Length > maxCharacters)
        {
            filteredText = filteredText.Substring(0, maxCharacters);
        }

        if (filteredText != text)
        {
            inputField.text = filteredText;
        }
    }

    void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(ValidateInput);
    }
}
