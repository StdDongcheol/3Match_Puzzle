
using NUnit.Framework.Constraints;
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
