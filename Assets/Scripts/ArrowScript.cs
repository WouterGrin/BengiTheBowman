using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
    Vector3 direction;
    SpriteRenderer renderer;
    public bool isFriendly;

    const float TRAVEL_SPEED = 11f;

	void Start ()
    {
        renderer = GetComponent<SpriteRenderer>();
	}

    public void SetDir(Vector3 dir)
    {
        direction = dir;
        if (direction.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if (direction.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else if (direction.y > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, -180);
        }
        else if (direction.y < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
	
	void Update ()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + direction * Time.deltaTime * TRAVEL_SPEED;
        if (!renderer.isVisible)
        {
            Destroy(this.gameObject);
        }

       
    }

    void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        if (isFriendly && other.tag == "Enemy")
        {
           Enemy enemyScript = other.GetComponent<Enemy>();
            enemyScript.DealDamage(transform.position, 1, 1);
        }
        else if (!isFriendly && other.tag == "Player")
        {
            PlayerHandler playerScript = other.GetComponent<PlayerHandler>();
            playerScript.DealDamage(transform.position, 1);
        }
        else if (isFriendly && other.tag == "Block")
        {
            Destroy(this.gameObject);
        }
    }
}
