using UnityEngine;
using System.Collections;

public class KeyScript : MonoBehaviour
{
    public GameObject player;
    public Color color;
    SpriteRenderer renderer;
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

	void Start () 
    {
       

    }
	
	void Update ()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= 0.5f)
            {
                player.GetComponent<PlayerHandler>().keys.Add(color);
                Destroy(this.gameObject);
            }
        }
	    
	}

    public void SetColor(Color _color)
    {
        color = _color;
        renderer.color = _color;
    }
}
