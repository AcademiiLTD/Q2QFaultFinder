using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FinalResultPopupView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Color _correctColour, _medianColour, _incorrectColour;

    public void SetResultText(float userGuess, FaultPositionGuess faultPositionGuess)
    {
        Color textColour = Color.black;
        switch (userGuess)
        {
            case > 10f:
                textColour = _incorrectColour;
                break;
            case > 5f:
                textColour = _medianColour;
                break;
            case <= 3f:
                textColour = _correctColour;
                break;
        }

        string hexColour = textColour.ToHexString();

        _resultText.text = $"Your guess was <color=#{hexColour}>{userGuess.ToString("0.00")}</color> meters from the fault";

        //if (previousGuess != -1f)
        //{
        //    _resultText.text += $"\nYour previous best guess was {previousGuess.ToString("0.00")}";
        //}

        if (!faultPositionGuess.CableTypesCorrect())
        {
            _resultText.text += "\n\nSome of your <color=#CD0000>cable type</color> inputs were incorrect";
        }

        if (!faultPositionGuess.CableThicknessCorrect())
        {
            _resultText.text += "\n\nSome of your <color=#CD0000>cable thickness</color> inputs were incorrect";
        }

        if (!faultPositionGuess.CableLengthsCorrect())
        {
            _resultText.text += "\n\nSome of your <color=#CD0000>cable length</color> inputs were incorrect";
        }
    }

    public void SetPopupActive(bool state)
    {
        gameObject.SetActive(state);
    }
}
