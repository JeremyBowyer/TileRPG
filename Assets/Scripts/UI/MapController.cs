using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public LevelController lc;
    public GameObject mapRoomPrefab;
    public BSPController bspController;
    public Vector2 mapSize;
    public Canvas canvas;
    public GameObject map;
    public RectTransform mapRect;
    public Sprite battleSprite;
    public Sprite currentLocationSprite;
    public float mapBuffer = 0f;
    Vector3[] mapRectCorners = new Vector3[4];
    public List<MapRoom> mapRooms;

    // Start is called before the first frame update
    public void Init()
    {
        mapRooms = new List<MapRoom>();
        mapRect.GetWorldCorners(mapRectCorners);
        mapRoomPrefab = (GameObject)Resources.Load("Prefabs/UI/MapRoom");
        bspController = GameObject.FindGameObjectWithTag("LevelController").GetComponent<BSPController>();
        lc = GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>();
        mapSize = bspController.rootNode.GetSize();
    }

    public void GenerateMapUI()
    {
        foreach (BSPRoom room in bspController.roomList)
        {
            CreateRoomButton(room);
        }
    }

    public void UpdateMapUI()
    {
        foreach(MapRoom mapRoom in mapRooms)
        {
            if (mapRoom.bspRoom == lc.protag.room)
            {
                mapRoom.SetIcon(currentLocationSprite);
            }
            else if (mapRoom.bspRoom is BSPBattleRoom)
            {
                BSPBattleRoom bRoom = mapRoom.bspRoom as BSPBattleRoom;
                if (!bRoom.completed)
                    mapRoom.SetIcon(battleSprite);
                else
                    mapRoom.SetIcon(null);
            }
            else
            {
                mapRoom.SetIcon(null);
            }
        }
    }

    void CreateRoomButton(BSPRoom room)
    {
        GameObject btn = Instantiate(mapRoomPrefab, map.transform);
        MapRoom mapRoom = btn.GetComponent<MapRoom>();
        mapRoom.bspRoom = room;
        room.mapRoom = mapRoom;
        mapRooms.Add(mapRoom);
        mapRoom.Init();

        if(room == lc.protag.room)
        {
            mapRoom.SetIcon(currentLocationSprite);
        } else if (room is BSPBattleRoom)
        {
            mapRoom.SetIcon(battleSprite);
        } else
        {
            mapRoom.SetIcon(null);
        }

        // Scale Room
        float roomScaleX = mapRect.rect.width * room.xSize / mapSize.x * (1 - mapBuffer);
        float roomScaleY = mapRect.rect.height * room.zSize / mapSize.y * (1 - mapBuffer);
        mapRoom.Scale(roomScaleX, roomScaleY);

        // Position Room
        Vector2 roomPos = room.Node.GetRelativePosition();
        Vector2 mapRectSize = GetSize(mapRect);

        float posX = mapRectCorners[0].x + (mapRectSize.x * mapBuffer / 2) + (mapRectSize.x * roomPos.x * (1 - mapBuffer));
        float posY = mapRectCorners[0].y + (mapRectSize.y * mapBuffer / 2) + (mapRectSize.y * roomPos.y * (1 - mapBuffer));
        mapRoom.SetPosition(new Vector3(posX, posY, 0f));
    }

    protected Vector2 GetSize(RectTransform bar)
    {
        Vector3[] corners = new Vector3[4];
        bar.GetWorldCorners(corners);
        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;
        return new Vector2(width, height);
    }
}
