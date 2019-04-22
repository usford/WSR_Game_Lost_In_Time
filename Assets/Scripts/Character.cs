using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Character : MonoBehaviour
{
    private int gold = 0;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float timeSpeed = 1f;
    [SerializeField]
    private float sliderSec = 4;


    private Rigidbody2D rigibody;
    private SpriteRenderer sprite;
    private GameObject slider;

    private bool isReversing = false; //Включена ли перемотка
    private ArrayList playerPositions; //Позиция игрока для перемотки
    private ArrayList playerFlip; //Направление игрока для перемотки
    private Image imageTimeController; //Эффект перемотки


    List<Vector3> lines = new List<Vector3>(); //Создание веторов для отмотки времени
    List<bool> flipesX = new List<bool>(); //Считывание направление игрока для отмотки времени

    private bool isStairs = false; //Находится ли персонаж возле лестницы
    private bool isTrumplet = false; //Находится ли персонаж возле трубы

    private bool fullSlider; //Проверка на заполненность слайдера

    private float time = 0;
    private int indexLines = 9; //Количество созданных векторов

    private void Awake()
    {
        rigibody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        slider = GameObject.Find("Slider");

        playerPositions = new ArrayList();
        playerFlip = new ArrayList();

        imageTimeController = GameObject.Find("Image_TimeController").GetComponent<Image>();
        imageTimeController.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!isReversing)
        {
            playerPositions.Add(transform.position);
            playerFlip.Add(sprite.flipX);
        }
        else if (isReversing && fullSlider && playerPositions.Count != 0)
        {
            transform.position = (Vector3)playerPositions[playerPositions.Count - 1];
            sprite.flipX = (bool)playerFlip[playerFlip.Count - 1];

            playerPositions.RemoveAt(playerPositions.Count - 1);
            playerFlip.RemoveAt(playerFlip.Count - 1);
        }

        if (playerPositions.Count > 250)
        {
            playerPositions.RemoveAt(0);
            playerFlip.RemoveAt(0);
        }
    }

    private void Update()
    {
        isReversing = Input.GetButton("Fire1");
        if (Input.GetButton("Horizontal") && !isReversing) Run();
        if (Input.GetButton("Vertical") && !isReversing && (isStairs == true || isTrumplet == true)) Climb();
        if (Input.GetButton("Fire1") && fullSlider) { BackTime();} else { RegenirationTime(); };
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

    /// <summary>
    /// Перемотка времени
    /// </summary>
    private void BackTime()
    {
        slider.GetComponent<Slider>().value = slider.GetComponent<Slider>().value - Time.deltaTime * timeSpeed;

        if (slider.GetComponent<Slider>().value == 0)
        {
            fullSlider = false;
        }

        imageTimeController.enabled = true;
    }

    /// <summary>
    /// Восстановление времени
    /// </summary>
    private void RegenirationTime()
    {
        slider.GetComponent<Slider>().value = slider.GetComponent<Slider>().value + Time.deltaTime * timeSpeed;

        fullSlider = (slider.GetComponent<Slider>().value == sliderSec);

        imageTimeController.enabled = false;
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
