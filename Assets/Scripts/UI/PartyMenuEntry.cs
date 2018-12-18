using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class PartyMenuEntry : MonoBehaviour
{
    [SerializeField] public Text mName;
    [SerializeField] public Text mClass;
    [SerializeField] public Text mLevel;
    [SerializeField] public Text mHp;
    [SerializeField] public Text mMana;
    [SerializeField] public Text mStamina;
    [SerializeField] Button button;

    public PartyMember member;

    public string Name
    {
        get { return mName.text; }
        set { mName.text = value; }
    }

    [System.Flags]
    enum States
    {
        None = 0,
        Selected = 1 << 0,
        Locked = 1 << 1
    }

    public bool IsLocked
    {
        get { return (State & States.Locked) != States.None; }
        set
        {
            if (value)
                State |= States.Locked;
            else
                State &= ~States.Locked;
        }
    }
    public bool IsSelected
    {
        get { return (State & States.Selected) != States.None; }
        set
        {
            if (value)
                State |= States.Selected;
            else
                State &= ~States.Selected;
        }
    }
    States State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            state = value;

            if (IsLocked)
            {
                mName.color = Color.gray;
            }
            else if (IsSelected)
            {
                //label.color = new Color32(249, 210, 118, 255);
            }
            else
            {
                //label.color = Color.white;
            }
        }
    }

    States state;


    void Awake()
    {
    }

    public void setOnClick(UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void Reset()
    {
        State = States.None;
    }

}