using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FinalResultPopupView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Color _correctColour, _medianColour, _incorrectColour;

    public void SetResultText(float finalResultDifference)
    {
        Color textColour = Color.black;
        switch (finalResultDifference)
        {
            case > 5f:
                textColour = _incorrectColour;
                break;
            case > 3f:
                textColour = _medianColour;
                break;
            case <= 1f:
                textColour = _correctColour;
                break;
        }

        string hexColour = textColour.ToHexString();

        gameObject.SetActive(true);
        _resultText.text = $"Your guess was <color=#{hexColour}>{finalResultDifference.ToString("0.00")}</color> meters from the fault";
    }
}
