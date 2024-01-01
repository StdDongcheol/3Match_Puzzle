using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class Board : MonoBehaviour
{
    // 에디터에서 수동으로 퍼즐보드를 미리 배치할수 있도록 구현해주세요.

    [SerializeField]
    private int Width;
    [SerializeField]
    private int Height;
    [SerializeField]
    private GameObject TilePrefab;

    private static int[] dx = new int[4] {1, -1, 0, 0 };
    private static int[] dy = new int[4] { 0, 0, 1, -1 };
    private int[] ColumnLineArr;
    private int[] ColumnFallLine;

    private GameObject[,] Tiles;
    private List<Vector2Int> CheckList;

    void Start()
    {
        Tiles = new GameObject[Width, Height];
        ColumnLineArr = new int[Width];
        ColumnFallLine = new int[Width];
        Setup();
    }

    /// BFS 탐색 수행
    private List<Vector2Int> BFS(int _StartRow, int _StartCol, bool bIsHorizontal)
    {
        List<Vector2Int> ListResult = new List<Vector2Int>();
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
                return ListResult;
        }
        else 
            return ListResult;


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
                    if (TileComp.IsUnityNull())
                    {
                        Debug.Assert(TileComp.IsUnityNull() == false, "Board.cs : TilePos(" + NextX + ", " + NextY + ") " + "TileComp is Null!"); 
                    }
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
                ListResult.Add(CurPos);
                ++ColumnLineArr[CurPos.x];
                ColumnFallLine[CurPos.x] = Mathf.Max(ColumnFallLine[CurPos.x], CurPos.y);
            }

            CurTileComp.SetMatchCheck(true);

            // _StartRow = YPos, _StartCol = XPos
            ListResult.Add(new Vector2Int(_StartCol, _StartRow));

            Debug.Log("Matched by " + Count + ". (" + _StartCol + ", " + _StartRow + ")");
            return ListResult;
        }
        else
            return ListResult;
    }

    private List<Vector2Int> BFS()
    {
        int Count = 0;
        List<Vector2Int> ListResult = new List<Vector2Int>();

        bool[,] CheckBoard = new bool[Height, Width];

        Queue<Vector2Int> CheckQueue = new Queue<Vector2Int>();
        Queue<Vector2Int> CoordQueue = new Queue<Vector2Int>();
        CoordQueue.Enqueue(new Vector2Int(0, 0));

        for (int i = 0; i < Height; ++i)
        {
            for (int j = 0; j < Width; ++j)
            {
                Tile.TileType CurType = 0;

                if (CheckBoard[i, j] == true)
                    continue;

                while (CoordQueue.TryPeek(out Vector2Int res) != false)
                {
                    Vector2Int CurPos = CoordQueue.Dequeue();
                    int CurX = CurPos.x;
                    int CurY = CurPos.y;

                    CheckBoard[CurX, CurY] = true;
                    int Index = 0, Max = 4;

                    for (int Dir = Index; Dir < Max; ++Dir)
                    {
                        int NextX = CurX + dx[Dir];
                        int NextY = CurY + dy[Dir];

                        if (0 <= NextX && NextX < Width && 0 <= NextY && NextY < Height && CheckBoard[NextX, NextY] == false)
                        {
                            Tile TileComp = Tiles[NextX, NextY].GetComponent<TileBackground>().GetTileComponent();
                            if (TileComp.IsUnityNull())
                            {
                                Debug.Assert(TileComp.IsUnityNull() == false, "Board.cs : TilePos(" + NextX + ", " + NextY + ") " + "TileComp is Null!");
                            }
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
            }
        }

        if (Count >= 3)
        {
            while (CheckQueue.Count > 0)
            {
                Vector2Int CurPos = CheckQueue.Dequeue();

                Tile TileComp = Tiles[CurPos.x, CurPos.y].GetComponent<TileBackground>().GetTileComponent();
                TileComp.SetMatchCheck(true);
                ListResult.Add(CurPos);
                ++ColumnLineArr[CurPos.x];
                ColumnFallLine[CurPos.x] = Mathf.Max(ColumnFallLine[CurPos.x], CurPos.y);
            }

            return ListResult;
        }

        return ListResult; 
    }

    public void MoveTiles()
    {
        for (int i = 0; i < Width; ++i)
            ColumnFallLine[i] = 0;

        for (int i = 0; i < CheckList.Count; ++i)
        {
            Vector2Int CurPos = CheckList[i];
            Tile CurTile = Tiles[CurPos.x, CurPos.y].GetComponent<TileBackground>().GetTileComponent();

            // 예외처리: 해당 타일컴포넌트에서 TileComponent를 불러오지 못함
            if (CurTile.IsUnityNull() == true)
            {
                Debug.LogWarning("TileComponent check error occured !! (Board.cs)");
                continue;
            }
            // 예외처리: 해당 열에서 ColumnLineArr이 처리되지 않았음
            if (ColumnLineArr[CurPos.x] <= 0)
            {
                Debug.LogWarning("Board ColumnLineArr number : " + CurPos.x + " check error occured !! (Board.cs)");
                continue;
            }

            // 현재타일 이동 수식 -> (보드의 최대 높이 - 현재타일.YPos) - 현재 타일의 열에서 사라질 타일 갯수
            int MoveHeight = (Height - ColumnLineArr[CurPos.x]);

            // 현재타일을 부모(Tilebackground) 오브젝트 아래로 위치시켜야함.
            TileBackground MovedTileBack = Tiles[CurPos.x, MoveHeight].GetComponent<TileBackground>();
            if (MovedTileBack.IsUnityNull() == false) 
            {
                CurTile.transform.parent = MovedTileBack.transform;
                CurTile.transform.position = new Vector2(MovedTileBack.transform.position.x, MovedTileBack.transform.position.y + ColumnLineArr[CurPos.x]);
                MovedTileBack.SetTileObject(CurTile.gameObject);
                CurTile.SetPosition(MoveHeight, CurPos.x);
            }

            CurTile.transform.parent.GetComponent<TileBackground>().SetTileObject(CurTile.gameObject);


            // 현재타일 바로 위에있는 윗 타일들 모두 이동
            for (int j = CurPos.y + 1; j < Height; ++j)
            {
                Tile TileComp = Tiles[CurPos.x, j].GetComponent<TileBackground>().GetTileComponent();

                if (TileComp && TileComp.IsTileMoved() == false)
                {
                    int MoveCount = ColumnLineArr[CurPos.x];

                    TileComp.transform.parent = Tiles[CurPos.x, j - MoveCount].transform;
                    Tiles[CurPos.x, j - MoveCount].GetComponent<TileBackground>().SetTileObject(TileComp.gameObject);
                    TileComp.SetPosition(j - MoveCount, CurPos.x);
                }
            }
        }

        CheckList.Clear();
    }

    public void Check(int _TargetRow, int _TargetCol)
    {
        List<Vector2Int> CurrentCheckList = new List<Vector2Int>();
        List<Vector2Int> VerticalList = BFS(_TargetRow, _TargetCol, false); // Vertical BFS
        
        // Check 성공시, 체크한 위치의 ColumnLineArr 증가.
        if(VerticalList.Count > 0) 
        {
            ++ColumnLineArr[_TargetCol];
        }
        
        List<Vector2Int> HorizonList = BFS(_TargetRow, _TargetCol, true);  // Horizontal BFS

        CurrentCheckList.AddRange(VerticalList);
        CurrentCheckList.AddRange(HorizonList);
        CheckList = CurrentCheckList.Distinct().ToList();

        //if (CheckList.Count >= 3)
        //{
        //    MoveTiles();
        //}
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
