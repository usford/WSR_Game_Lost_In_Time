using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int gold = 0;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float timeSpeed = 4.5f;
    [SerializeField]
    private float sliderSec = 4;

    private Rigidbody2D rigibody;
    private SpriteRenderer sprite;
    private GameObject slider;

    List<Vector3> lines = new List<Vector3>(); //Создание веторов для отмотки времени
    List<bool> flipesX = new List<bool>(); //Считывание направление игрока для отмотки времени

    private bool isStairs = false; //Находится ли персонаж возле лестницы
    private bool isTrumplet = false; //Находится ли персонаж возле трубы

    private float time;
    private float time2;
    private int indexLines = 9;

    private void Awake()
    {
        rigibody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        time = 0;
        slider = GameObject.Find("Slider");
    }
    private void Update()
    {
        if (Input.GetButton("Horizontal")) Run();
        if (Input.GetButton("Vertical") && (isStairs == true || isTrumplet == true)) Climb();
        if (Input.GetButton("Fire1")) { BackTime(); } else { RegenirationTime(); };
        //if (Input.GetKeyDown(KeyCode.Space)) transform.position = Vector3.MoveTowards(transform.position, lines[0], speed * Time.deltaTime);
        time += Time.deltaTime;
        time2 += Time.deltaTime;
        if (time > 0.33F && !Input.GetButton("Fire1"))
        {
            CreateLines();
            time = 0;
            Debug.Log("Создали вектор");
        }
        //Debug.Log(time2);

    }

    private void CreateLines()
    {
        if (lines.Count == 10)
        {
            lines.RemoveAt(0);
        }
        lines.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        flipesX.Add(sprite.flipX);
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

        if (isStairs == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        }


        if (isTrumplet == true && Input.GetAxis("Vertical") < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        }
    }

    //Перемотка времени
    private void BackTime()
    {
        slider.GetComponent<Slider>().value = slider.GetComponent<Slider>().value - Time.deltaTime * timeSpeed;

        transform.position = Vector3.MoveTowards(transform.position, lines[indexLines], speed * Time.deltaTime);
        sprite.flipX = flipesX[indexLines];
        Debug.Log(transform.position + "::::::::::::::::" + lines[indexLines] + ":::::::::::::::::" + flipesX[indexLines]);
        if (transform.position == lines[indexLines])
        {
            lines.RemoveAt(indexLines);
            flipesX.RemoveAt(indexLines);
            indexLines--;
        }

       
    }

    //Восстановление времени
    private void RegenirationTime()
    {
        slider.GetComponent<Slider>().value = slider.GetComponent<Slider>().value + Time.deltaTime * timeSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gold")
        {
            Destroy(collision.gameObject);
            gold++;
        }

        if (collision.CompareTag("Stairs"))
        {
            rigibody.gravityScale = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Stairs")
        {
            float position = collision.transform.position.x - transform.position.x;
            if (position <= 0.125f && position >= -0.125f)
            {
                isStairs = true;
                rigibody.gravityScale = 0;
            }
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
