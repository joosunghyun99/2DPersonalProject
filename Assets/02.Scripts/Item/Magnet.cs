using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Magnet : Item
{
    public float speed = 5.0f;

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.1f)
        {
            ReturnPool();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = Vector2.left.normalized * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<Character>().ActivateMagnet(2.0f, 2.0f);
            ReturnPool();
        }
    }

    private void ReturnPool()
    {
        PoolManager.Instance.ReturnPool(this, this);
    }
}
