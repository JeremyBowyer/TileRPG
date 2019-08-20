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
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            AddSoundToGroup(s);

            if (s.source.playOnAwake)
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

    public void Play(Sound sound)
    {
        if (sound.source == null)
        {
            Debug.LogWarning("Sound: " + sound.soundName + " on " + gameObject.name + " has no audio source.");
            return;
        }
        sound.source.Play();
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found on " + gameObject.name);
            return;
        }

        if(s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " on " + gameObject.name + " has no audio source.");
            return;
        }
        Play(s);
    }

    public void PlayRandomFromGroup(Sound.SoundGroup group)
    {
        if (!soundGroups.ContainsKey(group))
            return;

        Sound[] sounds = soundGroups[group];
        int idx = GlobalRandom.GetRandom(0, sounds.Length);
        Play(sounds[idx]);
    }
}
