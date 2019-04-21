using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int gold = 0;
    [SerializeField]
    private float speed = 3.0f;

    private Rigidbody2D rigibody;
    private SpriteRenderer sprite;

    private bool isStairs = false; //Находится ли персонаж возле лестницы
    private bool isTrumplet = false; //Находится ли персонаж возле трубы

    private void Awake()
    {
        rigibody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetButton("Horizontal")) Run();
        if (Input.GetButton("Vertical") && isStairs == true) Climb();
    }

    /// <summary>
    /// Передвижение влево-вправо
    /// </summary>
    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x > 0.0F;
    }

    /// <summary>
    /// Карабкаться вверх-вниз
    /// </summary>
    private void Climb()
    {
        Vector3 direction = transform.up * Input.GetAxis("Vertical");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gold")
        {
            Destroy(collision.gameObject);
            gold++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Stairs")
        {
            isStairs = true;
            rigibody.gravityScale = 0;
        }

        if (collision.tag == "Trumplet")
        {
            if (collision.transform.position.y - transform.position.y >= 0.1f)
            {
                isTrumplet = true;
                rigibody.bodyType = RigidbodyType2D.Kinematic;
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Stairs")
        {
            isStairs = false;
            rigibody.gravityScale = 1;
        }

        if (collision.tag == "Trumplet")
        {
            isTrumplet = false;
            rigibody.gravityScale = 1;
            rigibody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
