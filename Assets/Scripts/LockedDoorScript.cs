using UnityEngine;
using System.Collections;

public class LockedDoorScript : MonoBehaviour
{
    public GameObject player;
    bool isLocked;
    Color color;
    public Sprite otherSprite;
    SpriteRenderer renderer;
    void Start () 
    {
        renderer = GetComponent<SpriteRenderer>();
    }
	
    
    void Unlock()
    {
        Destroy(this.gameObject);
    }

	void Update () 
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= 0.5f && PlayerHasKey())
            {
                Unlock();
            }
        }
        renderer.sortingOrder = 1;
        if (player != null && player.transform.position.y > transform.position.y)
        {
            renderer.sortingOrder = 3;
        }
    }


    bool PlayerHasKey()
    {
        PlayerHandler playerScript = player.GetComponent<PlayerHandler>();
        for (int i = 0; i < playerScript.keys.Count; i++)
        {
            Color currColor = playerScript.keys[i];
            if (currColor.Equals(color))
                return true;
        }
        return false;
    }

    public void SetColor(Color _color)
    {
        color = _color;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void AdjustSprite(GameObject[,] tiles, Vector2 gridPos)
    {
        GameObject rightTile = null;
        GameObject leftTile = null;
        GameObject lowerTile = null;
        GameObject upperTile = null;
        if (gridPos.x - 1 >= 0)
        {
            leftTile = tiles[(int)gridPos.x - 1, (int)gridPos.y];
        }
        if (gridPos.x + 1 < LevelGenerator.STARTING_WIDTH)
        {
            rightTile = tiles[(int)gridPos.x + 1, (int)gridPos.y];
        }
        if (gridPos.y - 1 >= 0)
        {
            lowerTile = tiles[(int)gridPos.x, (int)gridPos.y - 1];
        }
        if (gridPos.y + 1 < LevelGenerator.STARTING_HEIGHT)
        {
            upperTile = tiles[(int)gridPos.x, (int)gridPos.y + 1];
        }

        /*
        if (lowerTile != null && lowerTile.tag == "Block" && upperTile != null && upperTile.tag == "Block")
        {

        }
        */
        if (rightTile != null && rightTile.tag == "Block" && leftTile != null && leftTile.tag == "Block")
        {
            GetComponent<SpriteRenderer>().sprite = otherSprite;
        }
    }
}
