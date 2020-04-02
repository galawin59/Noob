using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Bird : MonoBehaviour
{

    Animator anim;
    SpriteRenderer rend;
    Rigidbody2D rb;
    Vector3 velocity;
    float speed;
    float flyingSpeed;
    float statetimer;
    float maxtimer;
    int eat = 0;
    int animalsLayer;
    bool isFlying;

    public bool IsFlying
    {
        get
        {
            return isFlying;
        }

        private set
        {
            isFlying = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        speed = 0.15f;
        flyingSpeed = Random.Range(3.0f, 6.0f);
        statetimer = 0.0f;
        velocity = new Vector2(speed, 0.0f);
        maxtimer = Random.Range(2.0f, 4.0f);
        if (Random.Range(0, 2) == 1)
        {
            velocity.x *= -1.0f;
            rend.flipX = !rend.flipX;
        }
        rb.velocity = velocity;
        animalsLayer = 1 << LayerMask.NameToLayer("Animals");
        isFlying = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (anim.GetBool("isWalking"))
        {
            if (Mathf.Abs(statetimer) > maxtimer)
            {
                if (rb.velocity.y == 0)
                {
                    statetimer = 0.0f;
                    velocity.x *= -1.0f;
                    rend.flipX = !rend.flipX;
                    rb.velocity = velocity;
                }
            }
            else statetimer += Time.deltaTime;


            if (Random.Range(0, 200) == 100)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isEating", true);
                if (rb.velocity.y == 0)
                    rb.velocity = Vector2.zero;
            }

            if (Random.Range(0, 500) == 250)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isTurning", true);
                if (rb.velocity.y == 0)
                    rb.velocity = Vector2.zero;
            }

        }

    }

    public void endTurning()
    {
        anim.SetBool("isTurning", false);
        anim.SetBool("isWalking", true);
        maxtimer = Random.Range(2.0f, 4.0f);
        rb.velocity = velocity;
    }

    public void endEating()
    {
        if (eat > 1)
        {
            anim.SetBool("isEating", false);
            anim.SetBool("isWalking", true);
            maxtimer = Random.Range(2.0f, 4.0f);
            rb.velocity = velocity;
            eat = 0;
        }
        else eat++;
    }

    public void FlyAway(Vector3 posPlayer)
    {
        anim.SetBool("isEating", false);
        anim.SetBool("isTurning", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isFlying", true);
        isFlying = true;

        int dir;
        if (posPlayer.x < transform.position.x)
            dir = 1;
        else
            dir = -1;

        velocity = (new Vector2(Random.Range(0.5f, 1.0f) * dir, Random.Range(0.1f, 0.5f))).normalized * flyingSpeed;

        if (velocity.x > 0.0f)
        {
            rend.flipX = false;
        }
        else
        {
            rend.flipX = false;
            rend.flipY = true;
        }

        rb.velocity = velocity;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Destroy(gameObject, 10f);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder = 2000;
        spriteRenderer.sortingLayerName = "Objects";

        //pour que les oiseaux s'envolent d'un coup
        //  Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, animalsLayer);
        //  foreach (Collider2D collider in colliders)
        //  {
        //      Basic_Bird bird = collider.GetComponent<Basic_Bird>();
        //      if (bird != null && !bird.isFlying)
        //      {
        //          bird.FlyAway(posPlayer);
        //      }
        //  }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!anim.GetBool("isFlying"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController)
            {
                FlyAway(collision.transform.position);
            }
        }
    }
}
