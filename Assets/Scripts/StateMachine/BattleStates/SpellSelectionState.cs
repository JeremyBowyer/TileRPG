using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SpellSelectionState : BaseAbilityMenuState
{

    protected override void LoadMenu()
    {
        menuTitle = "Spells";
        menuOptions = new Dictionary<string, UnityAction>();
        foreach (SpellAbility spell in gc.currentCharacter.spells)
        {
            if (spell is EnvironmentSpellAbility)
            {
                menuOptions.Add(spell.AbilityName, () => SpellEnvironment(spell as EnvironmentSpellAbility));
            } else if (spell is TargetSpellAbility)
            {
                menuOptions.Add(spell.AbilityName, () => SpellTarget(spell as TargetSpellAbility));
            }

        }
        abilityMenuPanelController.Show(menuTitle, menuOptions);
    }

    protected override void Confirm()
    {

    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

    protected void SpellTarget(TargetSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        gc.ChangeState<SpellCharacterTargetState>(spellTargetArgs);
    }

    protected void SpellEnvironment(EnvironmentSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        gc.ChangeState<SpellEnvironmentTargetState>(spellTargetArgs);
    }

}