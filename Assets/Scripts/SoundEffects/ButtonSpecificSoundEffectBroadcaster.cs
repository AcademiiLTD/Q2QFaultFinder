using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpecificSoundEffectBroadcaster : MonoBehaviour
{
    [SerializeField] private SoundEffectType soundEffectType;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(PlayButtonClick); 
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners(); 
    }

    public void PlayButtonClick()
    {
        ApplicationEvents.InvokeOnSoundEffect(soundEffectType);
    }
}
