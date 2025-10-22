using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FinalResultPopupView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Color _correctColour, _medianColour, _incorrectColour;

    public void SetResultText(float userGuess)
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
        float previousGuess = PlayerPrefs.GetFloat(GlobalData.Instance.CurrentActiveScenario.name);
        Debug.Log($"Get float {GlobalData.Instance.CurrentActiveScenario.name} : {previousGuess}");


        gameObject.SetActive(true);
        _resultText.text = $"Your guess was <color=#{hexColour}>{userGuess.ToString("0.00")}</color> meters from the fault";

        if (previousGuess != -1f)
        {
            _resultText.text += $"\nYour previous best guess was {previousGuess.ToString("0.00")}";
        }

        //if (!userGuess._cableTypesCorrect)
        //{
        //    _resultText.text += "\n\nSome of your <color=#CD0000>cable type</color> inputs were incorrect";
        //}

        //if (!userGuess._cableThicknessCorrect)
        //{
        //    _resultText.text += "\n\nSome of your <color=#CD0000>cable thickness</color> inputs were incorrect";
        //}
    }
}
