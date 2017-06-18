using UnityEngine;
using System.Collections;

public class StoneShadowScript : MonoBehaviour 
{
    float lerpSize = 2;
    float lerpSpeed = 4.5f;
    public GameObject fallingStonePrefab;
    GameObject fallingStone;
    bool spawnedStone;
	void Start ()
    {
        transform.localScale = new Vector3(0, 0, 0);
	}
	
	void Update () 
    {
        float newScale = Mathf.Lerp(transform.localScale.x, lerpSize, Time.deltaTime * lerpSpeed);
        transform.localScale = new Vector3(newScale, newScale, newScale);
        if (transform.localScale.x > 1 && !spawnedStone)
        {
            fallingStone = Instantiate(fallingStonePrefab, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), Quaternion.identity) as GameObject;
            fallingStone.GetComponent<FallingStoneScript>().shadowObj = this.gameObject;
            spawnedStone = true;
        }
        if (fallingStone != null)
        {
            if (fallingStone.transform.position.y < transform.position.y + 0.1f)
            {
                fallingStone.GetComponent<FallingStoneScript>().SetDestroyed();
            }
        }
        
	}
}
