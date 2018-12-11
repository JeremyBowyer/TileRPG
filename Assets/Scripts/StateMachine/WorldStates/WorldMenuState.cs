using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldMenuState : WorldBaseMenuState
{
    public WorldMenuPanelController worldMenuPanelController { get { return gc.worldMenuPanelController; } }

    protected string menuTitle;
    protected Dictionary<string, UnityAction> menuOptions;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(WorldExploreState),
            typeof(WorldInventoryState),
            typeof(WorldPartyState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        worldMenuPanelController.ShowMenu();
        LoadMenu();
        LoadPartyMembers();
        worldMenuPanelController.ShowContentPanel("Party");
        inTransition = false;
    }
    public override void Exit()
    {
        base.Exit();
        worldMenuPanelController.HideMenu();
    }

    protected void LoadMenu()
    {
        menuOptions = new Dictionary<string, UnityAction>();
        menuTitle = "Menu";
        menuOptions.Add("Party", Party);
        menuOptions.Add("Inventory", Inventory);
        menuOptions.Add("Exit Game", Quit);
        worldMenuPanelController.ShowNavEntries(menuTitle, menuOptions);
    }

    protected void LoadPartyMembers()
    {
        foreach(PartyMember member in gc.protag.GetComponent<Protag>().partyMembers)
        {
            worldMenuPanelController.AddPartyMember(member);
        }
    }

    protected void Confirm()
    {

    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<WorldExploreState>();
    }

    protected void Inventory()
    {
        worldMenuPanelController.ShowContentPanel("Inventory");
    }

    protected void Party()
    {
        worldMenuPanelController.ShowContentPanel("Party");
    }

    protected void Quit()
    {
        Application.Quit();
    }


}
