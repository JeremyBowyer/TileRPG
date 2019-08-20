using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioController : AudioController
{

    public static GlobalAudioController instance;

    public override void Awake()
    {
        base.Awake();

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
