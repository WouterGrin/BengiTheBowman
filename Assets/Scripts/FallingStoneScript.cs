using UnityEngine;
using System.Collections;

public class FallingStoneScript : MonoBehaviour 
{
    GameObject player;
    public float fallSpeed = 10f;
    bool playsDestroyAnimation;
    Animator animation;
    float destroyTimer;
    float destroyDuration = 0.2f;
    public GameObject shadowObj;
	void Start ()
    {
        player = GameObject.Find("Player");
        animation = GetComponent<Animator>();
	}
	
	void Update () 
    {
        if (playsDestroyAnimation)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer > destroyDuration)
            {
                destroyTimer = 0;
                Destroy(shadowObj);
                Destroy(this.gameObject);

            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime, transform.position.z);
        }
	}

    public void SetDestroyed()
    {
        playsDestroyAnimation = true;
        animation.SetBool("destroy", true);

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= 0.75f)
        {
            Vector3 randomDir = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            player.GetComponent<PlayerHandler>().DealDamage(player.transform.position + randomDir, 1);
        }

    }
}
