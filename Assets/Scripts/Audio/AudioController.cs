using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public Sound[] sounds;
    public Dictionary<Sound.SoundGroup, Sound[]> soundGroups;

    public virtual void Awake()
    {
        sounds = GetComponents<Sound>();
        soundGroups = new Dictionary<Sound.SoundGroup, Sound[]>();
        InitSounds();
    }

    public void InitSounds()
    {
        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            s.LoadSource(source);
            AddSoundToGroup(s);

            if (s.playOnAwake)
                Play(s);

        }
    }

    public void AddSoundToGroup(Sound sound)
    {
        if (soundGroups.ContainsKey(sound.soundGroup) && soundGroups[sound.soundGroup] != null)
        {
            List<Sound> soundList = new List<Sound>(soundGroups[sound.soundGroup]);
            soundList.Add(sound);
            soundGroups[sound.soundGroup] = soundList.ToArray();
        }
        else
        {
            soundGroups[sound.soundGroup] = new Sound[] { sound };
        }
    }

    public void Play(string name)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found on " + gameObject.name);
            return;
        }
        Play(s);
    }

    public void Play(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogWarning("Sound not found on " + gameObject.name);
            return;
        }
        sound.Play();
    }

    public void Stop(Sound sound)
    {
        sound.Stop();
    }

    protected Sound GetSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found on " + gameObject.name);
            return null;
        }

        return s;
    }

    public void Stop(string name)
    {
        Sound s = GetSound(name);
        Stop(s);
    }

    public void PlayRandomFromGroup(Sound.SoundGroup group)
    {
        if (!soundGroups.ContainsKey(group))
            return;

        Sound[] sounds = soundGroups[group];
        int idx = CustomUtils.GetRandom(0, sounds.Length);
        Play(sounds[idx]);
    }

    public void CreateSoundFromClip(AudioClip clip, string name)
    {
        CreateSoundFromClip(clip, name, 0.5f, Sound.SoundGroup.None, Sound.SoundType.Misc);
    }

    public void CreateSoundFromClip(AudioClip clip, string name, float volume, Sound.SoundGroup group, Sound.SoundType type)
    {
        if (clip == null || name == null)
            return;

        Sound s = gameObject.AddComponent<Sound>();
        s.soundName = name;
        s.clip = clip;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        s.LoadSource(source);

        List<Sound> soundsList = new List<Sound>(sounds);
        soundsList.Add(s);
        sounds = soundsList.ToArray();
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        StartCoroutine(PlayThenDestroy(clip));
    }

    public IEnumerator PlayThenDestroy(AudioClip clip)
    {
        Sound s = gameObject.AddComponent<Sound>();
        s.clip = clip;
        yield return new WaitForEndOfFrame();

        s.Play();

        while (s.isPlaying)
            yield return new WaitForEndOfFrame();

        s.source.Stop();
        Destroy(s.source);
        Destroy(s);
    }
}
