using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance;

    public GridLayout gridLayout;
    public Tilemap mainTileMap;
    public Tilemap tempTileMap;

    public Button instantiateButton;
    public GameObject tempPrefabs;

    public enum TileType
    {
        Empty,
        White,
        Green,
        Red
    }

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private Placement temp;
    private Vector3 prevPos;
    private BoundsInt prevArea; 


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        string tilePath = @"Tiles\";
        tileBases.TryAdd(TileType.Empty, null);
        tileBases.TryAdd(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.TryAdd(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.TryAdd(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
        
        instantiateButton.onClick.AddListener(() => InitializeWithBuilding(tempPrefabs));
    }

    // Update is called once per frame
    void Update()
    {
        if (!temp)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                Debug.Log("Pointer Over GameObject");
                return;
            }
            if(!temp.placed)
            {
                Vector3 touchPos = GetWorldPositionOnPlane(Input.mousePosition, gridLayout.transform.position.z);
                Debug.Log(touchPos);
                
                Vector3Int cellPos = gridLayout.LocalToCell(touchPos);

                Debug.Log(cellPos);
                if (prevPos != cellPos)
                {
                    temp.transform.position = gridLayout.LocalToCellInterpolated(cellPos + new Vector3(.5f, .5f, 0f));
                    prevPos = cellPos;
                    FollowBuilding();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Try to Place");
            if (temp.CanBePlaced())
            {
                Debug.Log("Placed");
                temp.Place();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("Cancel Place");
            ClearArea();
            Destroy(temp.gameObject);
        }
    }

    public void InitializeWithBuilding(GameObject building)
    {
        temp = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<Placement>();
        FollowBuilding();
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    #region TileMap Managing
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tileMap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tileMap.GetTile(pos);
            counter++;
        }
        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tileMap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);

        tileMap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    #endregion

    #region Building Placement
    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        tempTileMap.SetTilesBlock(prevArea, toClear);
    }

    private void FollowBuilding()
    {
        ClearArea();
        
        temp.area.position = gridLayout.WorldToCell(temp.gameObject.transform.position);
        BoundsInt buildingArea = temp.area;

        Debug.Log(buildingArea);

        TileBase[] baseArray = GetTilesBlock(buildingArea, mainTileMap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for(int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == null)
            {
                FillTiles(tileArray, TileType.Red);
                Debug.Log("Red");
                break;
            }

            if (baseArray[i].name == "white")
            {
                tileArray[i] = tileBases[TileType.Green];
                Debug.Log("Green");
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                Debug.Log("Red");
                break; 
            }
            
        }

        tempTileMap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArea = GetTilesBlock(area, mainTileMap);
        foreach (TileBase b in baseArea)
        {
            if(b.name == "green")
            {
                return false;
            }
        }

        return true;
    }
    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, tempTileMap);
        SetTilesBlock(area, TileType.Green, mainTileMap);
    }

    #endregion
}
