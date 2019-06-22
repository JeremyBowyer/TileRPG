using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnEntry : MonoBehaviour
{
    public CharController character;
    public Image background;
    public Text characterName;
    public Text characterClass;

    public void Init(CharController controller)
    {
        character = controller;
        controller.turnEntry = this;
        characterName.text = controller.character.cName;
        characterClass.text = controller.character.cClass;

        if (character is PlayerController)
            background.color = CustomColors.PlayerUI;
        else if (character is EnemyController)
            background.color = CustomColors.EnemyUI;
    }

}
