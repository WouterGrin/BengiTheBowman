using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Enemy
{

    ObjectContainer objContainer;
    public GameObject arrowPrefab;
    public GameObject skeletonPrefab;
    StoneSpawnerScript stoneSpawner;
    GameObject player;


    Animator animator;
    bool isInTransit;

  

    float transitTimer;
    float transitDuration = 0.3f;
    bool isActivated;


    float arrowPhaseMoveSpeed = 3.4f;
    public List<Vector3> arrowMovePos = new List<Vector3>();
    int arrowPosIndex;

    int currentBossState;
    public int CurrentBossState
    {
        get
        {
            return currentBossState;
        }
        set
        {
            currentBossState = value;
        }
    }

    float stoneTimer;
    float stoneDuration = 7.5f;
    Vector3 startPos;

    public List<Vector3> skeletonSpawnPos = new List<Vector3>();
    float skeletonSpawnSpeed = 2.3f;
    int skeletonPosIndex;
    bool resetPos;

    float idleTimer;
    float idleDuration = 1.5f;

    int futureState = 1;

    protected override void Start () 
    {
        base.Start();

        touchRadius = 1.3f;

        stoneSpawner = GameObject.Find("StoneSpawner").GetComponent<StoneSpawnerScript>();
        CurrentHealth = PlayerPrefs.GetInt("bossHealth");
        startPos = transform.position;
        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();
        objContainer.enemyList.Add(this.gameObject);
        animator = GetComponent<Animator>();

        ReadyRobot();

    }

    public override void DealDamage(Vector3 attackerPos, float power, int amount)
    {
        base.DealDamage(attackerPos, power, amount);
        if (CurrentHealth <= 0)
        {
            Application.LoadLevel("EndScreen");
        }
    }

    public void ActivateRobot()
    {
        int transitInt = PlayerPrefs.GetInt("dogIsInBoss");
        if (transitInt == 0)
        {
            isInTransit = true;
            PlayerPrefs.SetInt("dogIsInBoss", 1);
        }
    }

    void ReadyRobot()
    {
        isActivated = true;
        int oldBossState = PlayerPrefs.GetInt("bossState");
        if (oldBossState != 0)
        {
            futureState = oldBossState;
        }
        else
        {
            futureState = 1;
        }
        
        StartIdleState();


    }

    void SetState(int state)
    {
        if (state == 0)
        {
            StartIdleState();
        }
        else if (state == 1)
        {
            StartArrowState();
        }
        else if (state == 2)
        {
            StartRockState();
        }
        else if (state == 3)
        {
            StartSkeletonState();
        }
    }



    void StartArrowState()
    {
        arrowPosIndex = 0;
        CurrentBossState = 1;
    }

    void StartRockState()
    {
        CurrentBossState = 2;
    }

    void StartSkeletonState()
    {
        skeletonPosIndex = 0;
        CurrentBossState = 3;
        resetPos = true;
    }

    void StartIdleState()
    {
        CurrentBossState = 0;
        
    }

    protected override void Update ()
    {
        base.Update();
        if (isInTransit)
        {
            transitTimer += Time.deltaTime;
            if (transitTimer >= transitDuration)
            {
                transitTimer = 0;
                isInTransit = false;
                animator.SetBool("transit", false);
                animator.SetBool("idle_with_dog", true);
                ReadyRobot();
            }
        }

        if (isActivated)
        {
            switch (currentBossState)
            {
                case 0:
                    transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * 3);
                    idleTimer += Time.deltaTime;
                    if (idleTimer >=  idleDuration)
                    {
                        idleTimer = 0;
                        SetState(futureState);
                    }
                    break;
                case 1:
                    if (arrowPosIndex < arrowMovePos.Count)
                    {
                        transform.position = Vector3.Lerp(transform.position, new Vector3(arrowMovePos[arrowPosIndex].x + 0.5f, arrowMovePos[arrowPosIndex].y, transform.position.z), arrowPhaseMoveSpeed * Time.deltaTime);
                        if (Vector2.Distance(transform.position, new Vector3(arrowMovePos[arrowPosIndex].x + 0.5f, arrowMovePos[arrowPosIndex].y, transform.position.z)) < 0.5f)
                        {
                            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                            arrowPosIndex++;
                        }
                    }
                    if (arrowPosIndex >= arrowMovePos.Count)
                    {
                        transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * 2);
                        if (Vector2.Distance(transform.position, startPos) < 0.5f)
                        {
                            futureState = 2;
                            StartIdleState();
                        }
                    }
                    break;
                case 2:
                    transform.position = Vector3.Lerp(transform.position, startPos + new Vector3(0, 3.5f, 0), Time.deltaTime * 2);
                    if (Vector2.Distance(transform.position, startPos + new Vector3(0, 3.5f, 0)) < 0.5f)
                    {
                        if (!stoneSpawner.enabled)
	                    {
		                    stoneSpawner.enabled = true;
	                    }
                    }
                    stoneTimer += Time.deltaTime;
                    if (stoneTimer >= stoneDuration)
                    {
                        stoneTimer = 0;
                        stoneSpawner.enabled = false;
                        futureState = 3;
                        StartIdleState();
                    }
                    break;
                case 3:
                    if (resetPos)
                    {
                        transform.position = Vector3.Lerp(transform.position, startPos + new Vector3(0, 3.5f, 0), Time.deltaTime * 2);
                        if (Vector2.Distance(transform.position, startPos + new Vector3(0, 3.5f, 0)) < 0.5f)
                        {
                            resetPos = false;
                        }
                    }
                    else
                    {
                        if (skeletonPosIndex < skeletonSpawnPos.Count)
                        {
                            transform.position = Vector3.Lerp(transform.position, new Vector3(skeletonSpawnPos[skeletonPosIndex].x, skeletonSpawnPos[skeletonPosIndex].y, transform.position.z), skeletonSpawnSpeed * Time.deltaTime);
                            if (Vector2.Distance(transform.position, skeletonSpawnPos[skeletonPosIndex]) < 0.5f)
                            {
                                GameObject skelObj = Instantiate(skeletonPrefab, transform.position, Quaternion.identity) as GameObject;
                                skelObj.GetComponent<Skeleton>().followRange = 100f;
                                skeletonPosIndex++;
                            }
                        }
                        if (skeletonPosIndex >= skeletonSpawnPos.Count)
                        {
                            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime);
                            if (Vector2.Distance(transform.position, startPos) < 0.5f)
                            {
                                StartArrowState();
                            }
                        }
                    }
                    
                    break;
                
                default:
                    break;
            }
        }

	}
}
