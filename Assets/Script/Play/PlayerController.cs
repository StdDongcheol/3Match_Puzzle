
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private GameObject SrcTile;
    private GameObject DstTile;

    void Start()
    {
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        
    }

    void OnClick()
    {
        Vector3 MousePos = Input.mousePosition; 
        Vector3 WorldMousePos = Camera.main.ScreenToWorldPoint(MousePos);
        RaycastHit2D RayHitResult;

        RayHitResult = Physics2D.Raycast(WorldMousePos, Vector3.forward, 10.0f);

        if (RayHitResult && RayHitResult.transform.CompareTag("Puzzle"))
        {
            bool IsSrcTileNull = SrcTile.IsUnityNull();
            bool IsDstTileNull = DstTile.IsUnityNull();

            if (IsSrcTileNull)
            {
                SrcTile = RayHitResult.transform.gameObject;
                Debug.Log(SrcTile.transform.gameObject.name);
            }

            else if (IsSrcTileNull == false)
            {
                GameObject TargetObject = RayHitResult.transform.gameObject;

                if (TargetObject == SrcTile)
                {
                    Debug.Log(SrcTile.name + " Released");
                    SrcTile = null;
                }
                else if (IsDstTileNull)
                {
                    DstTile = RayHitResult.transform.gameObject;
                    Debug.Log(DstTile.transform.gameObject.name);

                    if (Tile.IsTileNearBy(SrcTile, DstTile))
                    {
                        Tile.SwapTile(SrcTile, DstTile);
                        Debug.Log("Swapped");

                        // tilebackground의 tile 까지 교체됐음.
                        // 여기서 Board한테 3매치 체크 요청을 보내.
                        // 그럼 Board가 가지고 있는 tilebackground 배열을 통해서 3매칭 알고리즘을 진행하면 되
                    }

                    SrcTile = null;
                    DstTile = null;
                }
                else
                {
                    Debug.Assert(false, "PlayerController : OnClick() condition check error occured.");
                }
            }

        }
    }
}
