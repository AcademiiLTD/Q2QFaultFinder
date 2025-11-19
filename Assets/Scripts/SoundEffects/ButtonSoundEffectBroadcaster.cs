using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundEffectBroadcaster : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;

    private void OnEnable()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(ButtonClicked);
        }
    }

    private void OnDisable()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void ButtonClicked()
    {
        ApplicationEvents.InvokeOnButtonSoundEffect();
    }
}
