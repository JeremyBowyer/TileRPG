using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPArea : MonoBehaviour
{
    protected List<GameObject> edgeFloors { get; set; }
    protected List<GameObject> cornerFloors { get; set; }
    protected List<GameObject> walls { get; set; }
    public List<GameObject> floors;
    protected List<GameObject> props;
    protected List<GameObject> characters;
    protected LevelController lc;

    protected bool hidden;

    public float xSize;
    public float ySize;
    public float zSize;

    private int buffer;
    private Vector2 bufferBounds;

    protected int Id;

    public void Awake()
    {
        hidden = false;
        edgeFloors = new List<GameObject>();
        cornerFloors = new List<GameObject>();
        walls = new List<GameObject>();
        floors = new List<GameObject>();
        props = new List<GameObject>();
        characters = new List<GameObject>();
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
    }

    public virtual void Init(int _id, Vector2 _bufferBounds)
    {
        Id = _id;
        bufferBounds = _bufferBounds;
        System.Random random = new System.Random(Id);
        buffer = random.Next(Mathf.RoundToInt(bufferBounds.x), Mathf.RoundToInt(bufferBounds.y));
        // Get initial size, so we know what % to scale down, based on buffer size
        xSize = GetComponent<BoxCollider>().bounds.size.x;
        ySize = GetComponent<BoxCollider>().bounds.size.y;
        zSize = GetComponent<BoxCollider>().bounds.size.z;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * ((xSize - buffer * 2) / xSize), gameObject.transform.localScale.y, gameObject.transform.localScale.z * ((zSize - buffer * 2) / zSize));

        // Get scaled down size
        xSize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.x);
        ySize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.y);
        zSize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.z);
    }

    public virtual void AddProps()
    {

    }

    public void AddTorches()
    {

        for (int i = 0; i < walls.Count; i++)
        {
            GameObject wall = walls[i];
            BSPWall wallObj = wall.GetComponent<BSPWall>();
            if (wallObj.prop != null)
                continue;
            if (i % 3 == 0)
            {
                GameObject torch = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/Wall_Torch"));
                torch.transform.position = wall.transform.Find("TorchAnchor").transform.position;
                torch.transform.parent = wall.transform.Find("TorchAnchor").transform;
                torch.transform.localScale = Vector3.one * 2f;

                if (wallObj.facingDirection == Vector3.left)
                {
                    torch.transform.Rotate(new Vector3(0f, 90f, 0f));
                }
                else if (wallObj.facingDirection == Vector3.right)
                {
                    torch.transform.Rotate(new Vector3(0f, -90f, 0f));
                }
                else if (wallObj.facingDirection == Vector3.forward)
                {
                    torch.transform.Rotate(new Vector3(0f, 180f, 0f));
                }
                wallObj.prop = torch;
                AddProp(torch);
            }
        }

    }

    public void SignObject(GameObject _obj)
    {
        MapObject obj = _obj.AddComponent<MapObject>();
        obj.startingPos = _obj.transform.position;
        obj.area = this;
    }

    public void AddCornerFloor(GameObject _floor)
    {
        cornerFloors.Add(_floor);
    }

    public void AddFloor(GameObject _floor)
    {
        SignObject(_floor);
        floors.Add(_floor);
    }

    public void AddWall(GameObject _wall)
    {
        SignObject(_wall);
        walls.Add(_wall);
    }

    public void AddProp(GameObject _prop)
    {
        SignObject(_prop);
        props.Add(_prop);
    }

    public void AddCharacter(GameObject _character)
    {
        SignObject(_character);
        characters.Add(_character);
    }

    public void AddEdgeFloor(GameObject _edgeFloor)
    {
        if(!edgeFloors.Contains(_edgeFloor))
            edgeFloors.Add(_edgeFloor);
    }

    public void RemoveFloor(GameObject _floor)
    {
        if (floors.Contains(_floor))
            floors.Remove(_floor);
    }

    public void RemoveProp(GameObject _prop)
    {
        if (props.Contains(_prop))
            props.Remove(_prop);
    }

    public void RemoveCharacter(GameObject _character)
    {
        if (characters.Contains(_character))
            characters.Remove(_character);
    }

    public void RemoveWall(GameObject _wall)
    {
        if(walls.Contains(_wall))
            walls.Remove(_wall);
    }

    public void ShowRoom()
    {
        foreach (GameObject floor in floors)
        {
            StartCoroutine(ShowObject(floor));
        }

        foreach (GameObject wall in walls)
        {
            StartCoroutine(ShowObject(wall));
        }

        foreach (GameObject prop in props)
        {
            StartCoroutine(ShowObject(prop));
        }

        foreach (GameObject character in characters)
        {
            StartCoroutine(ShowObject(character));
        }
    }

    public void HideRoom()
    {
        foreach (GameObject floor in floors)
        {
            StartCoroutine(HideObject(floor));
        }

        foreach (GameObject wall in walls)
        {
            StartCoroutine(HideObject(wall));
        }

        foreach (GameObject prop in props)
        {
            StartCoroutine(HideObject(prop));
        }

        foreach (GameObject character in characters)
        {
            StartCoroutine(HideObject(character));
        }
    }

    public IEnumerator HideObject(GameObject _obj)
    {
        if (_obj == null || hidden)
            yield break;

        yield return new WaitForSeconds(Random.value / 2);

        float currentTime = 0f;
        float speed = 0.5f;
        float startingY = _obj.transform.position.y;
        float deltaY = 10f;
        float destinationY = startingY - deltaY;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = (1f - 0f) * EasingEquations.EaseInExpo(0.0f, 1.0f, currentTime);
            _obj.transform.position = new Vector3(_obj.transform.position.x, startingY - deltaY * frameValue, _obj.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        _obj.transform.position = new Vector3(_obj.transform.position.x, destinationY, _obj.transform.position.z);
        _obj.SetActive(false);
        hidden = true;
        yield break;
    }

    public IEnumerator ShowObject(GameObject _obj)
    {
        if (_obj == null || !hidden)
            yield break;

        yield return new WaitForSeconds(Random.value / 2);

        _obj.SetActive(true);

        MapObject mapObj = _obj.GetComponent<MapObject>();

        float currentTime = 0f;
        float speed = 0.5f;
        float startingY = _obj.transform.position.y;
        float destinationY = mapObj.startingPos.y;
        float deltaY = destinationY - startingY;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = (1f - 0f) * EasingEquations.EaseOutExpo(0.0f, 1.0f, currentTime);
            _obj.transform.position = new Vector3(_obj.transform.position.x, startingY + deltaY * frameValue, _obj.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        _obj.transform.position = new Vector3(_obj.transform.position.x, destinationY, _obj.transform.position.z);
        hidden = false;
        yield break;
    }

}
