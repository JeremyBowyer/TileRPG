using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AbilityMenuEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] Button button;

    public string Title
    {
        get { return label.text; }
        set { label.text = value; }
    }

    public string Cost
    {
        get { return cost.text; }
        set { cost.text = value; }
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
                label.color = Color.gray;
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