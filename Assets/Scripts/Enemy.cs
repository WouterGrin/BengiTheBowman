using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    public float touchRadius;
    public int maxHealth;
    public Texture healthBar;
    int currentHealth;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    protected virtual void OnGUI()
    {
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            GUI.color = Color.red;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.y = Screen.height - screenPos.y;
            GUI.DrawTexture(new Rect(screenPos.x - 20, screenPos.y - 30, 45f / maxHealth * currentHealth, 7), healthBar);
            GUI.color = Color.white;
        }
    }

    public virtual void DealDamage(Vector3 attackerPos, float power, int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }


    protected virtual void Start () {
        CurrentHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}
}
