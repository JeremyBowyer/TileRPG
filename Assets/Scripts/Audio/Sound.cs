using UnityEngine.Audio;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public enum SoundGroup { None, Run, Walk };

    public string soundName;
    public SoundGroup soundGroup;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.5f;
    [Range(0.1f, 3f)]
    public float pitch = 1.5f;

    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;
}
