using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : Entity
{
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource winSound;

    [SerializeField] private float speed = 3f;
    [SerializeField] private int health;
    [SerializeField] private int mon=0;
    private int lives = 3;

    [SerializeField] private float jumpForce = 80f;
    private bool isGrounded = false;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHearts;
    [SerializeField] private Sprite deadHearts;


    [SerializeField] private Image[] moneys;

    [SerializeField] private Sprite full_bag;
    [SerializeField] private Sprite empty_bag;

    public bool isAttacking = false;
    public bool isRecharged = true;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        lives = 3;
        health = lives;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        isRecharged = true;

        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        mon = 3;
    }

    private void Run()
    {
        if (isGrounded)
        {
            State = States.run;
        }

            Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }


    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        jumpSound.Play();
    }

    void Update()
    {
        if (isGrounded)
            State = States.idle;

        if (Input.GetButton("Horizontal"))
            Run();

        if (health > lives)
            health = lives;

        for(int i = 0; i < hearts.Length;i++)
        {
            if (i < health)
                hearts[i].sprite = aliveHearts;
            else
                hearts[i].sprite = deadHearts;

            if(i < lives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }

        for (int i = 0; i < moneys.Length; i++)
        {
            if (i < mon)
                moneys[i].sprite = empty_bag;
            else
                moneys[i].sprite = full_bag;
        }

        if (mon == 0)
        {
            if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
                Invoke("FirstLevel", 3f);
            else
                Invoke("NextLevel", 3f);
        }

     
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void FirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if (!isGrounded)
            State = States.jump;
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (isGrounded && Input.GetButton("Jump"))
            Jump();
    }

    public override void GetDamage()
    {
        health -= 1;
        damageSound.Play();
        if(health == 0)
        {
            foreach(var h in hearts)
                h.sprite = deadHearts;
            Die();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
            deathSound.Play();
        }


    }

    public override void GetCoin()
    {
        mon -= 1;
        coinSound.Play();

        if(mon == 0)
            winSound.Play();
    }

}

public enum States
{
    idle,
    run,
    jump
}