
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public enum TileType
    {
        BlueFruit,
        Lemon,
        Tomato,
        Watermelon,
    }

    [SerializeField]
    private int Row;
    [SerializeField]
    private int Column;
    [SerializeField]
    private GameObject DestroyEffect;

    [SerializeField]
    public TileType FruitType { get; set; }
    private bool bMatchCheck { get; set;}

    Tile() : base()
    {
    }

    static public void SwapTile(GameObject _tile1, GameObject _tile2)
    {
        int Tile1Row = _tile1.GetComponent<Tile>().Row;
        int Tile1Col = _tile1.GetComponent<Tile>().Column;
        int Tile2Row = _tile2.GetComponent<Tile>().Row;
        int Tile2Col = _tile2.GetComponent<Tile>().Column;
        
        Vector3 Pos = _tile1.transform.position;
        _tile1.transform.position = _tile2.transform.position;
        _tile2.transform.position = Pos;

        Transform ParentTransform = _tile1.transform.parent;
        _tile1.transform.parent = _tile2.transform.parent;
        _tile2.transform.parent = ParentTransform;

        _tile1.transform.parent.GetComponent<TileBackground>().SetTileObject(ref _tile2);
        _tile2.transform.parent.GetComponent<TileBackground>().SetTileObject(ref _tile1);

        _tile1.GetComponent<Tile>().SetPosition(Tile2Row, Tile2Col);
        _tile2.GetComponent<Tile>().SetPosition(Tile1Row, Tile1Col);

        _tile1.transform.parent.parent.GetComponent<Board>().Check(Tile1Row, Tile1Col);
        _tile2.transform.parent.parent.GetComponent<Board>().Check(Tile2Row, Tile2Col);
    }
    
    static public bool IsTileNearBy(GameObject _tile1, GameObject _tile2)
    {
        int Tile1Row = _tile1.GetComponent<Tile>().Row;
        int Tile1Col = _tile1.GetComponent<Tile>().Column;
        int Tile2Row = _tile2.GetComponent<Tile>().Row;
        int Tile2Col = _tile2.GetComponent<Tile>().Column;

        int[] dx = { 1, 0, -1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        for (int i = 0; i < 4; ++i)
        {
            int NearX = Tile1Col + dx[i];
            int NearY = Tile1Row + dy[i];

            if (Tile2Row == NearY && Tile2Col == NearX)
            {
                return true;
            }
        }

        return false;
    }

    public void SetPosition(int _Row, int _Column)
    {
        Row = _Row;
        Column = _Column;
    }
    
    public void SetFruitType(TileType _Type)
    {
        FruitType = _Type;
    }
    
    public TileType GetFruitType()
    {
        return FruitType;
    }

    public void SetMatchCheck(bool _bEnable)
    {
        bMatchCheck = _bEnable;
    }

    void Start()
    {
        bMatchCheck = false;
    }

    void Update()
    {
        if (bMatchCheck)
        {
            TileBackground TileBack = this.transform.parent.gameObject.GetComponent<TileBackground>();

            TileBack.CreateFruit();

            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
