using UnityEngine;
using System.Collections;

public class ArrowTrap : MonoBehaviour 
{
    public GameObject arrowPrefab;
    GameObject player;

    bool isOnCooldown;
    float arrowCooldownTimer;
    float arrowCooldownDuration = 2f;
	void Start ()
    {
        player = GameObject.Find("Player");
	}
	
	void Update () 
    {
        if (isOnCooldown)
        {
            arrowCooldownTimer += Time.deltaTime;
            if (arrowCooldownTimer >= arrowCooldownDuration)
            {
                arrowCooldownTimer = 0;
                isOnCooldown = false;
            }
        }
        if (Mathf.Abs(transform.position.x - player.transform.position.x) <= 0.5f && !isOnCooldown)
        {
            isOnCooldown = true;
            Instantiate(arrowPrefab, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);

        }
	}
}
