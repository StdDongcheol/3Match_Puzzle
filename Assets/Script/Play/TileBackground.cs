using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class TileBackground : MonoBehaviour
{
    public GameObject[] Tiles;
    private GameObject CurrentTile;

    // 퍼즐 수동 배치를 위한 변수들
    [SerializeField]
    private bool IsSetFruitManual;
    [SerializeField]
    private Tile.TileType FruitType;

    private int Row;
    private int Column;


    public void SetTileManual(int _Index)
    {
        FruitType = (Tile.TileType)_Index;
        IsSetFruitManual = true;
    }


    public void SetPosition(int _Row, int _Column)
    {
        Row = _Row;
        Column = _Column;
    }
    
    public void SetTileObject(GameObject _TileObject)
    {
        CurrentTile = _TileObject;
    }
    
    public GameObject GetTileObject()
    {
        return CurrentTile;
    }
    
    public Tile GetTileComponent()
    {
        return gameObject.GetComponentInChildren<Tile>();
    }

    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        if (IsSetFruitManual)
        {
            if (FruitType != GetTileComponent().GetFruitType())
            {
                IsSetFruitManual = false;
                Destroy(CurrentTile);
                GameObject Tile = Instantiate(Tiles[(int)FruitType], transform.position, Quaternion.identity);
                Tile.transform.parent = this.transform;
                Tile.GetComponent<Tile>().SetPosition(Row, Column);
                Tile.GetComponent<Tile>().SetFruitType(FruitType);
                SetTileObject(Tile);
                CurrentTile = Tile;
            }
        }
    }

    public void CreateFruit()
    {
        //int TileIndex = Random.Range(0, Tiles.Length);
        //
        //GameObject Tile = Instantiate(Tiles[TileIndex], transform.position, Quaternion.identity);
        //Tile.transform.parent = this.transform;
        //Tile.GetComponent<Tile>().SetPosition(Row, Column);
        //Tile.GetComponent<Tile>().SetFruitType((Tile.TileType)TileIndex); 
        //SetTileObject(ref Tile);
    }

    private void Initialize()
    {
        int TileIndex = Random.Range(0, Tiles.Length);
        GameObject Tile;

        if (IsSetFruitManual)
        {
            IsSetFruitManual = false;
            Tile = Instantiate(Tiles[(int)FruitType], transform.position, Quaternion.identity);
            Tile.GetComponent<Tile>().SetFruitType(FruitType);
        }
        else
        {
            Tile = Instantiate(Tiles[TileIndex], transform.position, Quaternion.identity);
            Tile.GetComponent<Tile>().SetFruitType((Tile.TileType)TileIndex);
        }

        Tile.transform.parent = this.transform;
        Tile.GetComponent<Tile>().SetPosition(Row, Column);
        
        SetTileObject(Tile);
    }
}
