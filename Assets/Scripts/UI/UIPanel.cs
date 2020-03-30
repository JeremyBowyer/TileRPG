using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LayoutAnchor))]
public class UIPanel : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.25f;
    [SerializeField]
    private Sound clip;
    public Position CurrentPosition { get; private set; }
    public Tweener Transition { get; private set; }
    public bool InTransition { get { return Transition != null; } }
    public Position this[string name]
    {
        get
        {
            if (positionMap != null && positionMap.ContainsKey(name))
                return positionMap[name];
            return null;
        }
    }

    [Serializable]
    public class Position
    {
        public string name;
        public TextAnchor myAnchor;
        public TextAnchor parentAnchor;
        public Vector2 offset;

        public Position(string name)
        {
            this.name = name;
        }

        public Position(string name, TextAnchor myAnchor, TextAnchor parentAnchor) : this(name)
        {
            this.myAnchor = myAnchor;
            this.parentAnchor = parentAnchor;
        }

        public Position(string name, TextAnchor myAnchor, TextAnchor parentAnchor, Vector2 offset) : this(name, myAnchor, parentAnchor)
        {
            this.offset = offset;
        }
    }

    [SerializeField] List<Position> positionList;
    Dictionary<string, Position> positionMap;
    LayoutAnchor anchor;
    void Awake()
    {
        anchor = GetComponent<LayoutAnchor>();
        positionMap = new Dictionary<string, Position>(positionList.Count);
        for (int i = positionList.Count - 1; i >= 0; --i)
            AddPosition(positionList[i]);
    }

    void Start()
    {
        if (CurrentPosition == null && positionList.Count > 0)
            SetPosition(positionList[0], false);
    }

    public void AddPosition(Position p)
    {
        positionMap[p.name] = p;
    }

    public void RemovePosition(Position p)
    {
        if (positionMap.ContainsKey(p.name))
            positionMap.Remove(p.name);
    }

    public Tweener SetPosition(string positionName, bool animated)
    {
        return SetPosition(this[positionName], animated);
    }
    public Tweener SetPosition(Position p, bool animated)
    {
        if (CurrentPosition == p)
            return null;

        CurrentPosition = p;
        if (CurrentPosition == null)
            return null;

        if (InTransition)
            Transition.easingControl.Stop();
        if (animated)
        {
            if (clip != null)
                clip.Play();

            Transition = anchor.MoveToAnchorPosition(p.myAnchor, p.parentAnchor, p.offset);
            //Transition.easingControl.duration = GetComponent<EasingControl>().duration;
            Transition.easingControl.duration = duration;
            return Transition;
        }
        else
        {
            anchor.SnapToAnchorPosition(p.myAnchor, p.parentAnchor, p.offset);
            return null;
        }
    }

}