using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class SoundEffectListener : MonoBehaviour
    {
        [SerializedDictionary("SFX Type", "SFX File")]
        public SerializedDictionary<SoundEffectType, AudioClip> SoundEffects;

        [SerializeField] private AudioSource _audioSource;

        private void OnEnable()
        {
            ApplicationEvents.OnSoundEffect += OnSoundEffect;
        }

        private void OnDisable()
        {
            ApplicationEvents.OnSoundEffect -= OnSoundEffect;
        }

        private void OnSoundEffect(SoundEffectType soundEffectType)
        {
            _audioSource.PlayOneShot(SoundEffects[soundEffectType]);
        }
    }
}

public enum SoundEffectType
{
    BUTTON_CLICK,
    SCREEN_TAP,
    CLIP_ATTACH
}
