using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultFindingView : MonoBehaviour
{
    [SerializeField] private FinalResultPopupView _finalResultView;

    public void DisplayuserGuess(float userGuess)
    {

        _finalResultView.SetResultText(userGuess);
        gameObject.SetActive(true);
    }
}
