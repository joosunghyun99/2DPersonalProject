using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public bool isNeedRotate;

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

        isNeedRotate = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isNeedRotate)
        {
            gameObject.transform.Rotate(0f, 0f, 180f);
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
            collision.transform.GetComponent<Character>().GetDamage(1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void ReturnPool()
    {
        PoolManager.Instance.ReturnPool(this, this);
    }
}
