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
            typeof(SpellEnvironmentSplashTargetState),
            typeof(SpellEmanationCharacterTargetState),
            typeof(SpellEnvironmentPathTargetState)
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
            if (spell is EnvironmentSplashSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellEnvironmentSplash(spell as EnvironmentSplashSpellAbility)), canCast, spell);
            } else if (spell is EnvironmentPathSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellEnvironmentPath(spell as EnvironmentPathSpellAbility)), canCast, spell);
            }
            else if (spell is TargetSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellTarget(spell as TargetSpellAbility)), canCast, spell);
            }
            else if (spell is EmanationSpellAbility)
            {
                abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(spellName, () => SpellEmanation(spell as EmanationSpellAbility)), canCast, spell);
            }

        }

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

    protected void SpellEmanation(EmanationSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        bc.ChangeState<SpellEmanationCharacterTargetState>(spellTargetArgs);
    }

    protected void SpellEnvironmentPath(EnvironmentPathSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        bc.ChangeState<SpellEnvironmentPathTargetState>(spellTargetArgs);
    }

    protected void SpellEnvironmentSplash(EnvironmentSplashSpellAbility spell)
    {
        StateArgs spellTargetArgs = new StateArgs
        {
            spell = spell
        };
        bc.ChangeState<SpellEnvironmentSplashTargetState>(spellTargetArgs);
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        OutlineTargetCharacter(sender, e);
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();
    }

}