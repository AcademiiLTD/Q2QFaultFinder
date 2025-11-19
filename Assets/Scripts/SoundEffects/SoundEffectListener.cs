using UnityEngine;

public class SoundEffectListener : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        ApplicationEvents.OnSoundEffect += OnSoundEffect;
    }

    private void OnDisable()
    {
        ApplicationEvents.OnSoundEffect -= OnSoundEffect;
    }

    private void OnSoundEffect(AudioClip soundEffect)
    {
        _audioSource.PlayOneShot(soundEffect);
    }
}
