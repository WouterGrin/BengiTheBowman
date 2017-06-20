using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : MonoBehaviour {

    public GameObject player;
    public Sprite otherWallSprite;
    SpriteRenderer renderer;
    GameObject lowerTile;
    GameObject upperTile;
    void Awake () {
        renderer = GetComponent<SpriteRenderer>();
    }

	void Update () {
        renderer.sortingOrder = 0;
        if (player != null && player.transform.position.y > transform.position.y)
        {
            renderer.sortingOrder = 3;
        }
        if (lowerTile != null && lowerTile.tag == "Block")
        {
            renderer.sortingOrder = 3;
        }
    }

    public void AdjustSpriteAndHitbox(GameObject[,] grid, Vector2 gridPos)
    {
        lowerTile = null;
        upperTile = null;
        if (gridPos.y - 1 >= 0)
        {
            lowerTile = grid[(int)gridPos.x, (int)gridPos.y - 1];
            if (lowerTile != null && lowerTile.tag != "Block")
            {          
                renderer.sprite = otherWallSprite;
            }
        }
        if (gridPos.y + 1 < LevelGenerator.HEIGHT)
        {
            upperTile = grid[(int)gridPos.x, (int)gridPos.y + 1];
            if (upperTile != null && upperTile.tag != "Block")
            {
                GetComponent<BoxCollider>().size = new Vector3(1, 0.4f, 1);
                GetComponent<BoxCollider>().center = new Vector3(0, -0.3f, 0);
               
            }
        }
        if (lowerTile != null && lowerTile.tag == "Block" && upperTile != null && upperTile.tag == "Block")
        {
            //GetComponent<BoxCollider>().size = new Vector3(1, 1.05f, 1);
        }
    }

    
}
