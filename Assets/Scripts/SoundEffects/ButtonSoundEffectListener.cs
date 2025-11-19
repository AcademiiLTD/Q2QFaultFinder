using UnityEngine;

public class ButtonSoundEffectListener : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        ApplicationEvents.OnButtonSoundEffect += OnButtonSoundEffect;
    }

    private void OnDisable()
    {
        ApplicationEvents.OnButtonSoundEffect -= OnButtonSoundEffect;
    }

    private void OnButtonSoundEffect()
    {
        _audioSource.Play();
    }
}
