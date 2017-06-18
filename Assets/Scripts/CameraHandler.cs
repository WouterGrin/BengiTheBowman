using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour
{
    public const float CAMERA_SPEED = 4f;
    GameObject playerObj;
    float zPos;
	void Start () 
    {
        zPos = transform.position.z;
        playerObj = GameObject.Find("Player");
	}
	
	void Update () 
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, transform.position.z), Time.deltaTime * CAMERA_SPEED);

        //float restX = transform.position.x % (1f / 4f);
        //float restY = transform.position.x % (1f / 4f);
        //transform.position = new Vector3(transform.position.x, transform.position.y, zPos);

	}
}
