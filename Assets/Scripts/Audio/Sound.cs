using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public enum SoundGroup { None, Run, Walk };
    public enum SoundType { Misc, Dialogue, Ambience, Music, Effect, UI };

    public string soundName;
    public SoundGroup soundGroup;
    public SoundType soundType;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.5f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;
    public bool playOnAwake;
    public bool playOnEnable;

    public AudioSource source;

    public bool isPlaying
    {
        get { return source != null && source.isPlaying; }
    }

    public void Start()
    {
        GlobalAudioController.instance.onVolumeChange += OnVolumeChange;

        if (source == null)
            LoadSource(gameObject.AddComponent<AudioSource>());
        else
            LoadSource(source);

        if (playOnAwake)
            Play();

        if (playOnEnable && gameObject.activeInHierarchy)
            Play();
    }

    public void LoadSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.volume = volume * GlobalAudioController.instance.VolumeMultiplier;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
    }

    public void Play()
    {

        if (source == null)
        {
            Debug.LogWarning("Sound: " + soundName + " on " + gameObject.name + " has no audio source.");
            return;
        }
        source.Stop();
        source.Play();
    }

    public void Stop()
    {
        if (source == null)
        {
            Debug.LogWarning("Sound: " + soundName + " on " + gameObject.name + " has no audio source.");
            return;
        }
        source.Stop();
    }

    public void OnVolumeChange(float mult)
    {
        source.volume = volume * GlobalAudioController.instance.VolumeMultiplier;
    }

    public void OnEnable()
    {
        if (playOnEnable)
            Play();
    }
}
