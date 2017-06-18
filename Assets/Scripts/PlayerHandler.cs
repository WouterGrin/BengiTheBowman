using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour
{
    Rigidbody rigidBody;
    PlayerAnimation animation;
    SpriteRenderer renderer;

    public GameObject upperBox;
    public GameObject horizontalBox;
    public GameObject lowerBox;
    public GameObject ownHitbox;

    public GameObject arrowPrefab;

    public Texture heartSprite;
    public Texture keySprite;
    

    const float MOVE_SPEED = 5f;
    const float DASH_SPEED = 46f;

    bool keyPressed;
    bool upPressed;
    bool downPressed;
    bool leftPressed;
    bool rightPressed;

    public bool forcedStop;

    bool canAttack = true;
    float attackCooldownTimer;
    float attackCooldown = 0.4f;
    int faceDirection;

    ObjectContainer objContainer;

   
    

    SpriteRenderer horizontalDashStripesRenderer;
    SpriteRenderer upperDashStripesRenderer;
    SpriteRenderer lowerDashStripesRenderer;


    public Vector3 dashVector;
    
    float dashDiminish = 0.73f;
    int dashDirection;

    Vector3 knockbackVector;
    float knockbackAmount = 15f;
    float knockbackDiminish = 0.85f;
    

    bool isImmune;
    float immuneTimer;
    float immuneDuration = 2.5f;
    float flickerTimer;
    float flickerDuration = 0.3f;
    float flickerSpeed = 1;
    float flickerSpeedIncrease = 0.3f;
    int alphaFlicker;

    public List<Color> keys = new List<Color>();
   

    float fallSpeed = 5;
    bool isFallingDown;
    float fallingDownTimer;
    float fallingDownDuration = 0.75f;

    int lives = 3;

    public bool debug;
    public int Lives
    {
        get 
        { 
            return lives; 
        }
        set 
        { 
            lives = value;
            if (lives <= 0)
            {
                Application.LoadLevel("EndScreen");
            }

        }
    }

    void Awake()
    {
        
        
    }

	void Start ()
    {
        
        renderer = GetComponent<SpriteRenderer>();
        horizontalDashStripesRenderer = transform.FindChild("horizontalDashStripes").GetComponent<SpriteRenderer>();
        upperDashStripesRenderer = transform.FindChild("upperDashStripes").GetComponent<SpriteRenderer>();
        lowerDashStripesRenderer = transform.FindChild("lowerDashStripes").GetComponent<SpriteRenderer>();

        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();

        rigidBody = GetComponent<Rigidbody>();
        animation = GetComponent<PlayerAnimation>();

        faceDirection = 2;

        PhysicMaterial physicMat = new PhysicMaterial();
        physicMat.name = "DynamicMat";
        physicMat.dynamicFriction = 0.0f;
        physicMat.staticFriction = 0.0f;
        physicMat.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = physicMat;

	}
	

    void OnGUI()
    {
        for (int i = 0; i < lives; i++)
        {
            GUI.DrawTexture(new Rect(10 + (i * (heartSprite.width + 6)), 10, heartSprite.width, heartSprite.height), heartSprite);
        }
        GUI.color = new Color(0, 0, 0);
        for (int i = 0; i < keys.Count; i++)
        {
            GUI.color = keys[i];
            float scaleRatio = heartSprite.width / keySprite.width;
            
            GUI.DrawTexture(new Rect(200 + i * (keySprite.width + 20), 10, keySprite.width * scaleRatio, keySprite.height * scaleRatio), keySprite);
        }
        GUI.color = new Color(0, 0, 0);
    }

	void Update ()
    {
        if (forcedStop)
        {
            return;
        }

        if (isFallingDown)
        {
            rigidBody.velocity *= 0.5f;
            fallingDownTimer += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime, transform.position.z);

            if (fallingDownTimer > 0.2f)
            {
                if (transform.localScale.y > 0.1f)
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.8f, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(0, 0, 0);
                }
            }
            
            if (fallingDownTimer > fallingDownDuration)
            {
                fallingDownTimer = 0;
                isFallingDown = false;
                Lives -= 1;
                
            }
            return;
        }

        if (MenuHandler.IsEnabled)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animation.PlayIdleUp();
            keyPressed = true;
            upPressed = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            animation.PlayIdleLeft();
            keyPressed = true;
            leftPressed = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            animation.PlayIdleDown();
            keyPressed = true;
            downPressed = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            animation.PlayIdleRight();
            keyPressed = true;
            rightPressed = true;
        }


        if (upPressed && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            keyPressed = false;
            upPressed = false;
        }
        if (leftPressed && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
        {
            keyPressed = false;
            leftPressed = false;
        }
        if (downPressed && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow))
        {
            keyPressed = false;
            downPressed = false;
        }
        if (rightPressed && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
        {
            keyPressed = false;
            rightPressed = false;
        }


        if (!canAttack)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer > attackCooldown)
            {
                attackCooldownTimer = 0;
                canAttack = true;
            }
        }

        Vector3 deltaMove = Vector3.zero;
        
        if (upPressed && dashDirection != 1 && dashDirection != 3)
        {
            faceDirection = 1;
            deltaMove.y += MOVE_SPEED;
        }
        if (leftPressed && dashDirection != 2 && dashDirection != 4)
        {
            faceDirection = 4;
            deltaMove.x -= MOVE_SPEED;
        }
        if (downPressed && dashDirection != 1 && dashDirection != 3)
        {
            faceDirection = 3;
            deltaMove.y -= MOVE_SPEED;
        }
        if (rightPressed && dashDirection != 2 && dashDirection != 4)
        {
            faceDirection = 2;
            deltaMove.x += MOVE_SPEED;
        }
        

        if (canAttack && Input.GetKey(KeyCode.Z))
        {
            
            canAttack = false;
            animation.PlayAttack();
            keyPressed = false;
            upPressed = false;
            leftPressed = false;
            downPressed = false;
            rightPressed = false;
            ShootArrow();
        }

        if (Input.GetKeyDown(KeyCode.X) && dashVector.magnitude < 0.2f)
        {
            dashDirection = faceDirection;

            switch (faceDirection)
            {
                case 1:
                    dashVector = deltaMove.normalized * DASH_SPEED;
                    lowerDashStripesRenderer.enabled = true;
                    break;
                case 2:
                    dashVector = deltaMove.normalized * DASH_SPEED;
                    horizontalDashStripesRenderer.enabled = true;
                    break;
                case 3:
                    dashVector = deltaMove.normalized * DASH_SPEED;
                    upperDashStripesRenderer.enabled = true;
                    break;
                case 4:
                    dashVector = deltaMove.normalized * DASH_SPEED;
                    horizontalDashStripesRenderer.enabled = true;
                    break;
                default:
                    break;
            }
            
        }

        if (isImmune)
        {
            immuneTimer += Time.deltaTime;
            flickerTimer += Time.deltaTime * flickerSpeed;
           
            Color currColor = renderer.material.color;
            currColor = new Color(currColor.r, currColor.g, currColor.b, 1 - 0.5f * alphaFlicker);
            renderer.material.color = currColor;
            if (flickerTimer > flickerDuration)
            {
                flickerTimer = 0;
                if (alphaFlicker == 1)
                    alphaFlicker = 0;
                else
                    alphaFlicker = 1;
                if (flickerSpeed < 3f)
                {
                    flickerSpeed += flickerSpeedIncrease;
                }
                
            }
            
            if (immuneTimer > immuneDuration)
            {
                flickerTimer = 0;
                immuneTimer = 0;
                isImmune = false;
                currColor = new Color(currColor.r, currColor.g, currColor.b, 1f);
                renderer.material.color = currColor;
            }
        }

        if (!isImmune)
        {
            for (int i = 0; i < objContainer.enemyList.Count; i++)
            {
                GameObject currEnemy = objContainer.enemyList[i];
                Enemy currEnemyScript = currEnemy.GetComponent<Enemy>();
                
                if (Vector2.Distance(transform.position, currEnemy.transform.position) < currEnemyScript.touchRadius)
                {
                    DealDamage(currEnemy.transform.position, 1);

                }
                
            }
        }
        


        rigidBody.velocity = deltaMove + dashVector + knockbackVector;
        dashVector *= dashDiminish;

        if (dashVector.magnitude < 3.75f)
        {
            horizontalDashStripesRenderer.enabled = false;
            lowerDashStripesRenderer.enabled = false;
            upperDashStripesRenderer.enabled = false;
            if (dashVector.magnitude < 2.75f)
            {
                dashDirection = 0;
            }
        }
        

        knockbackVector *= knockbackDiminish;
	}

    public void DealDamage(Vector3 enemyPos, int amount)
    {
        if (!isImmune)
        {
            Lives -= amount;
            Vector2 diff = transform.position - enemyPos;
            diff.Normalize();
            dashVector = Vector3.zero;
            knockbackVector = diff * knockbackAmount;
            SetImmune();
        }
        
    }

    public void SetForceStop(bool value)
    {
        forcedStop = value;
        if (forcedStop)
        {
            dashVector = Vector3.zero;
            knockbackVector = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
        }
    }

    void SetImmune()
    {
        flickerSpeed = 1;
        alphaFlicker = 1;
        isImmune = true;
    }

    public void SetFallingDown()
    {
        if (!isFallingDown)
        {
            isFallingDown = true;
            fallingDownTimer = 0;
            GameObject tileHolder = GameObject.Find("Level");
            int children = tileHolder.transform.childCount;
            for (int i = 0; i < children; ++i)
            {
                GameObject tile = tileHolder.transform.GetChild(i).gameObject;
                if (tile.tag == "Ground")
                {
                    if (tile.transform.position.y < transform.position.y - 0.5f && Mathf.Abs((int)tile.transform.position.x - (int)transform.position.x) <= 1)
                    {
                        tile.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 2;
                    }
                }
            }
        }
        
    }
    


    void ShootArrow()
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity) as GameObject;
        ArrowScript arrScript = newArrow.GetComponent<ArrowScript>();
        arrScript.isFriendly = true;
        Vector3 dir = Vector3.zero;
        switch (faceDirection)
        {
            case 1:
                dir = new Vector3(0, 1, 0);
                break;
            case 2:
                dir = new Vector3(1, 0, 0);
                break;
            case 3:
                dir = new Vector3(0, -1, 0);
               
                break;
            case 4:
                dir = new Vector3(-1, 0, 0);
                break;
            default:
                break;
        }
        newArrow.transform.position += dir;
        arrScript.SetDir(dir);



    }
}
