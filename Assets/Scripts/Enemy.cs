using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsObjectCustom)), RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IKillable, IAttacker
{
    Collider2D[] results;
    Rigidbody2D rb2d;
    PolygonCollider2D collider;
    PhysicsObjectCustom physics;
    ContactFilter2D contactFilter;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        physics = GetComponent<PhysicsObjectCustom>();
        collider = GetComponent<PolygonCollider2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        results = new Collider2D[16];
    }
    public void ApplyDamage(GameObject obj)
    {
        throw new System.NotImplementedException();
    }

    public void CheckIfDamage()
    {
        rb2d.OverlapCollider(contactFilter, results);
        foreach (Collider2D result in results)
        {
            if (result != null && DoesColliderHitCorner(result))
            {
                ApplyDamage(result.gameObject);
            }
        }
    }

    bool DoesColliderHitCorner(Collider2D collision)
    {
        if (collision == null) return false;

        return collision.OverlapPoint(collider.bounds.min) ||
                collision.OverlapPoint(collider.bounds.max) ||
                collision.OverlapPoint(new Vector2(collider.bounds.min.x, collider.bounds.max.y)) ||
                collision.OverlapPoint(new Vector2(collider.bounds.max.x, collider.bounds.min.y));
    }

    float killdelay = 0;
    public void TakeDamage()
    {
        if (Time.time > killdelay)
        {
            Debug.Log("I died!");
            killdelay = Time.time + 1;
        }
        //Destroy(gameObject);
        //transform.position = Vector3.zero;
        
        //Play effects
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
