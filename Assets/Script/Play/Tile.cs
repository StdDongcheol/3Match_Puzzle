
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

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
    private GameObject DestroyEffectObject;

    [SerializeField]
    public TileType FruitType { get; set; }
    private bool bMatchCheck { get; set;}
    private bool bMoveTile { get; set;}

    Tile() : base()
    {
    }

    static public void SwapTile(GameObject _tile1, GameObject _tile2)
    {
        int Tile1Row = _tile1.GetComponent<Tile>().Row;
        int Tile1Col = _tile1.GetComponent<Tile>().Column;
        int Tile2Row = _tile2.GetComponent<Tile>().Row;
        int Tile2Col = _tile2.GetComponent<Tile>().Column;

        Transform ParentTransform = _tile1.transform.parent;
        _tile1.transform.parent = _tile2.transform.parent;
        _tile2.transform.parent = ParentTransform;

        _tile1.transform.parent.GetComponent<TileBackground>().SetTileObject(ref _tile2);
        _tile2.transform.parent.GetComponent<TileBackground>().SetTileObject(ref _tile1);

        _tile1.GetComponent<Tile>().SetPosition(Tile2Row, Tile2Col);
        _tile2.GetComponent<Tile>().SetPosition(Tile1Row, Tile1Col);
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
        bMoveTile = true;
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
        bMoveTile = false;
    }

    void Update()
    {
        if (bMoveTile)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, Time.deltaTime * 10.0f);

            if (Mathf.Approximately(transform.position.x, transform.parent.position.x) && 
                Mathf.Approximately(transform.position.y, transform.parent.position.y))
            {
                bMoveTile = false;

                transform.parent.parent.GetComponent<Board>().Check(Row, Column);
            }
        }

        if (bMatchCheck)
        {
            TileBackground TileBack = this.transform.parent.gameObject.GetComponent<TileBackground>();

            TileBack.CreateFruit();

            GameObject EffectObject = Instantiate(DestroyEffectObject, this.transform.parent);
            Destroy(EffectObject, 1.0f);
            Destroy(this.gameObject);
        }
    }
}
