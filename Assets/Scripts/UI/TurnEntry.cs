using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnEntry : MonoBehaviour
{
    public CharController character;
    public Image background;
    public Image characterAvatar;
    public Text characterName;
    public Text characterClass;

    public void Init(CharController controller)
    {
        character = controller;
        controller.turnEntry = this;

        if(characterName != null)
            characterName.text = controller.character.cName;

        if(characterClass != null)
            characterClass.text = controller.character.cClass;

        if (characterAvatar != null)
            characterAvatar.sprite = character.character.avatar;

        if (character is PlayerController)
            background.color = CustomColors.PlayerUI;
        else if (character is EnemyController)
            background.color = CustomColors.EnemyUI;
    }

    public void OnHoverEnter()
    {
        InputEventHandler.instance.LoadTargetCharacter(character);
        Color clr = character is EnemyController ? CustomColors.Hostile : CustomColors.Heal;
        InputEventHandler.instance.OutlineCharacter(character, clr, _mode: Outline.Mode.OutlineAndSilhouette, _width: 2f);
    }

    public void OnHoverExit()
    {
        InputEventHandler.instance.UnloadTargetCharacter();
        InputEventHandler.instance.RemoveOutlines();
    }

}
