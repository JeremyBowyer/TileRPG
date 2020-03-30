using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioController : AudioController
{
    private CharacterAudioProfile audioProfile;
    public CharController charController;
    public Character character { get { return charController.character; } }

    public void LoadProfile()
    {
        audioProfile = character.audioProfile;

        if (audioProfile == null)
            return;

        CreateSoundFromClip(audioProfile.joinPartyClip, "join_party");
        CreateSoundFromClip(audioProfile.getHitClip, "get_hit");
        CreateSoundFromClip(audioProfile.deathClip, "death");
        CreateSoundFromClip(audioProfile.attackClip, "attack");
        CreateSoundFromClip(audioProfile.moveClip, "move");
    }

}
