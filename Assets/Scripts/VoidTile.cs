using UnityEngine;
using System.Collections;

public class VoidTile : MonoBehaviour 
{

	void Start ()
    {
	
	}

	void Update ()
    {
	

	}

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //if (Vector2.Distance(transform.position, other.transform.position) < 0.58f)
            //{
                PlayerHandler playerScript = other.GetComponent<PlayerHandler>();
                if (playerScript.dashVector.magnitude <= 0.01f)
                {
                    playerScript.SetFallingDown();
                }
            //}
            
            
        }

    }
}
