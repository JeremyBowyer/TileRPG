using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioController : AudioController
{
    private float _volumeMultiplier = 0.5f;
    public float VolumeMultiplier
    {
        get { return _volumeMultiplier; }
        set
        {
            _volumeMultiplier = Mathf.Clamp01(value);
            onVolumeChange?.Invoke(_volumeMultiplier);
        }
    }

    public delegate void OnVolumeChange(float mult);
    public OnVolumeChange onVolumeChange;

    public GlobalUIAudioProfile uiProfile;

    public static GlobalAudioController instance;

    public override void Awake()
    {
        instance = this;

        base.Awake();

        LoadProfile();
        //DontDestroyOnLoad(gameObject);
    }

    public void LoadProfile()
    {
        if (uiProfile == null)
            return;

        CreateSoundFromClip(uiProfile.defaultConfirmClick, "default_confirm_click");
        CreateSoundFromClip(uiProfile.defaultCancelClick, "default_cancel_click");
        CreateSoundFromClip(uiProfile.defaultMajorClick, "default_major_click");
        CreateSoundFromClip(uiProfile.defaultHover, "default_hover");
    }
}
