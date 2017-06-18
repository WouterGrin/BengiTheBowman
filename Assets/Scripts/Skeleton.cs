using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skeleton : Enemy
{
    GameObject player;
    ObjectContainer objContainer;
    List<Vector3> feetPos = new List<Vector3>();
    SpriteRenderer renderer;
    Rigidbody rigidbody;
    
    public bool followPlayer;
    public float followRange;
    public bool canDealDamage;
    public bool canBeKnockedBack;
    bool isOnFollow;
    public float bounciness;

    protected override void Start()
    {
        base.Start();
        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();
        player = GameObject.Find("Player");

        touchRadius = 0.5f;

        //feetPos.Add(new Vector3(-0.3f, -0.4f, 0));
        //feetPos.Add(new Vector3(0.3f, -0.4f, 0));

        PhysicMaterial physicMat = new PhysicMaterial();
        physicMat.name = "DynamicMat";
        physicMat.bounciness = bounciness;
        physicMat.dynamicFriction = 0.0f;
        physicMat.staticFriction = 0.0f;
        physicMat.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = physicMat;

        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        objContainer.enemyList.Add(this.gameObject);
	}

    public override void DealDamage(Vector3 attackerPos, float power, int amount)
    {
        base.DealDamage(attackerPos, power, amount);
        Knockback(attackerPos, power);
        if (CurrentHealth <= 0)
        {
            objContainer.enemyList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void Knockback(Vector3 attackerPos, float power)
    {
        if (canBeKnockedBack)
        {
            Vector2 diff = transform.position - attackerPos;
            diff.Normalize();
            rigidbody.AddForce(diff * 1000 * power, ForceMode.Force);
        }
        
    }

    


    protected override void Update()
    {
        base.Update();

        if (transform.position.y > player.transform.position.y)
        {
            renderer.sortingOrder = 0;
        }  
        else
        {
            renderer.sortingOrder = 2;
        }

        if (followPlayer)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= followRange || isOnFollow)
            {
                isOnFollow = true;
                var direction = Vector3.zero;
                if (Vector3.Distance(transform.position, player.transform.position) > 0.1f)
                {
                    
                    direction = player.transform.position - transform.position;
                    rigidbody.AddRelativeForce(direction.normalized * 20, ForceMode.Force);
                }
            }
            
        }
        

        if (rigidbody.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
	}
}
