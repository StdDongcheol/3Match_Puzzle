using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    // 에디터에서 수동으로 퍼즐보드를 미리 배치할수 있도록 구현해주세요.

    [SerializeField]
    private int Width;
    [SerializeField]
    private int Height;

    private int[] dx = new int[4] {1, -1, 0, 0 };
    private int[] dy = new int[4] { 0, 0, 1, -1 };


    [SerializeField]
    private GameObject TilePrefab;

    private GameObject[,] Tiles;

    void Start()
    {
        Tiles = new GameObject[Width, Height];
        Setup();
    }

    /// BFS 탐색 수행
    private bool BFS(int _StartRow, int _StartCol, bool bIsHorizontal)
    {
        int Count = 1;
        GameObject StartTile = Tiles[_StartCol, _StartRow];
        Tile.TileType CurType = 0;
        Tile CurTileComp = null;
        if (StartTile.IsUnityNull() == false)
        {
            TileBackground TileBackComponent = StartTile.GetComponent<TileBackground>();

            if (TileBackComponent.IsUnityNull() == false)
            {
                CurTileComp = TileBackComponent.GetTileComponent();
                CurType = CurTileComp.GetFruitType();
            }
            else
                return false;
        }
        else 
            return false;


        bool[,] CheckBoard = new bool[Height, Width];
        CheckBoard[_StartCol, _StartRow] = true;
        Queue<Vector2Int> CoordQueue = new Queue<Vector2Int>();
        Queue<Vector2Int> CheckQueue = new Queue<Vector2Int>();
        CoordQueue.Enqueue(new Vector2Int(_StartCol, _StartRow));

        while (CoordQueue.TryPeek(out Vector2Int res) != false)
        {
            Vector2Int CurPos = CoordQueue.Dequeue();
            int CurX = CurPos.x; 
            int CurY = CurPos.y;

            CheckBoard[CurX, CurY] = true;
            int Index = 2, Max = 4;
            if (bIsHorizontal)
            {
                Index = 0;
                Max = 2;
            }

            for (int i = Index; i < Max; ++i)
            {
                int NextX = CurX + dx[i];
                int NextY = CurY + dy[i];
                
                if (0 <= NextX && NextX < Width && 0 <= NextY && NextY < Height && CheckBoard[NextX, NextY] == false)
                {
                    Tile TileComp = Tiles[NextX, NextY].GetComponent<TileBackground>().GetTileComponent();
                    if (CurType == TileComp.GetFruitType())
                    {
                        CheckBoard[NextX, NextY] = true;
                        CoordQueue.Enqueue(new Vector2Int(NextX, NextY));
                        CheckQueue.Enqueue(new Vector2Int(NextX, NextY));

                        ++Count;
                    }
                }
            }
        }

        if (Count >= 3)
        {
            while (CheckQueue.Count > 0)
            {
                Vector2Int CurPos = CheckQueue.Dequeue();

                Tile TileComp = Tiles[CurPos.x, CurPos.y].GetComponent<TileBackground>().GetTileComponent();
                TileComp.SetMatchCheck(true);
            }

            CurTileComp.SetMatchCheck(true);

            Debug.Log("Matched by " + Count + ". (" + _StartRow + ", " + _StartCol + ")");
            return true;
        }
        else
            return false;
    }

    public void Check(int _TargetRow, int _TargetCol)
    {
        BFS(_TargetRow, _TargetCol, false);
        BFS(_TargetRow, _TargetCol, true);
    }

    private void Setup()
    {
        for(int i = 0 ; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                GameObject TileObj = Instantiate(TilePrefab, new Vector3(i, j, transform.position.z), Quaternion.identity) as GameObject;

                TileObj.transform.parent = this.transform;
                TileObj.name = "(" +  i + ", " + j + ")";
                TileObj.GetComponent<TileBackground>().SetPosition(j, i);
                Tiles[i, j] = TileObj;
            }
        }
    }
}
