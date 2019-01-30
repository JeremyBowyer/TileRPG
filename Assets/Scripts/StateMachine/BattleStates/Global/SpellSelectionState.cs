using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellSelectionState : BaseAbilityMenuState
{

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(CommandSelectionState),
            typeof(SpellCharacterTargetState),
            typeof(SpellEnvironmentTargetState)
            };
        }
        set { }
    }

    protected override void LoadMenu()
    {
        abilityMenuPanelController.Show("Spells");
        foreach (SpellAbility spell in bc.CurrentCharacter.Spells)
        {
            string spellName = spell.AbilityName;
            bool canCast = spell.ValidateCost(bc.CurrentCharacter);
            if (spell is EnvironmentSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellEnvironment(spell as EnvironmentSpellAbility)), canCast);
            }
            else if (spell is TargetSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellTarget(spell as TargetSpellAbility)), canCast);
            }

        }

        /*
        menuTitle = "Spells";
        menuOptions = new Dictionary<string, UnityAction>();
        foreach (SpellAbility spell in gc.currentCharacter.Spells)
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
        */
    }

    protected override void Confirm()
    {

    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }

    protected void SpellTarget(TargetSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        bc.ChangeState<SpellCharacterTargetState>(spellTargetArgs);
    }

    protected void SpellEnvironment(EnvironmentSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        bc.ChangeState<SpellEnvironmentTargetState>(spellTargetArgs);
    }

}