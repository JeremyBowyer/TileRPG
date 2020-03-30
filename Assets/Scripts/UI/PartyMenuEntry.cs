using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class PartyMenuEntry : MonoBehaviour
{
    [SerializeField] public Image mAvatar;
    [SerializeField] public Text mName;
    [SerializeField] public Text mClass;
    [SerializeField] public Text mLevel;
    [SerializeField] public Text mHp;
    [SerializeField] public Text mMana;
    [SerializeField] public Text mStamina;
    [SerializeField] Button button;

    public PartyMember character;

    private WorldMenuPanelController wmpController { get { return WorldMenuPanelController.instance; } }

    public string Name
    {
        get { return mName.text; }
        set { mName.text = value; }
    }

    void Awake()
    {
    }

    public void SetOnClick(UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void LoadMember(PartyMember _member)
    {
        mAvatar.sprite = _member.avatar;
        mName.text = _member.cName;
        mClass.text = _member.cClass;
        mHp.text = _member.stats.curHP.ToString() + " / " + _member.stats.maxHPTemp.ToString() + " / " + _member.stats.maxHP.ToString();
        mMana.text = _member.stats.curMP.ToString() + " / " + _member.stats.maxMPTemp.ToString() + " / " + _member.stats.maxMP.ToString();
        mStamina.text = _member.stats.curAP.ToString() + " / " + _member.stats.maxAPTemp.ToString() + " / " + _member.stats.maxAP.ToString();
        mLevel.text = _member.level.ToString();
        character = _member;
    }

    public void OnHoverEnter()
    {
        wmpController.OnHoverEnterMember(this);
    }

    public void OnHoverExit()
    {
        wmpController.OnHoverExitMember(this);
    }
}