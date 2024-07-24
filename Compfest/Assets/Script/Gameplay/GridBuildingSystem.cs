using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance;

    public GridLayout gridLayout;
    public Tilemap mainTileMap;
    public Tilemap tempTileMap;

    public enum TileType
    {
        Empty,
        White,
        Green,
        Red
    }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        string tilePath = @"Tiles\";
        TileBase.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
