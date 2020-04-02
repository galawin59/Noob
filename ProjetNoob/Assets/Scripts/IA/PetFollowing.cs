using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollowing : MonoBehaviour
{
    Rigidbody2D rgbd;
    Vector2 velocity;
    Vector2 direction;
    Animator anim;
    SpriteManager sm;
    [SerializeField] int idSmourbiff = 12;
    SpriteRenderer sprite;
    Transform target;
    float basicSpeed;
    public bool isRunning;
    bool gottaGoFast;
    // Use this for initialization
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        sm = SpriteManager.GetSpriteManager;
        target = PlayerManager.GetPlayerManager.currentPlayer.transform;
        basicSpeed = 2.0f;
        gottaGoFast = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = rgbd.velocity;
        float speed;
        gottaGoFast = false;

        if (Vector2.Distance(transform.position, target.position) > 10.0f)
        {
            transform.position = target.position;
        }
        else if (Vector2.Distance(transform.position, target.position) > 3.0f)
        {
            velocity = target.position - transform.position;
            velocity.Normalize();
            CalculateAngle(velocity);
            gottaGoFast = true;
        }
        else if (Vector2.Distance(transform.position, target.position) > 1.0f)
        {
            velocity = target.position - transform.position;
            velocity.Normalize();
            CalculateAngle(velocity);
        }
        else
        {
            velocity = Vector2.zero;
        }
        if (isRunning)
        {
            speed = basicSpeed * 2;
            anim.SetFloat("isRunning", 2.0f);
        }
        else
        {
            speed = basicSpeed;
            anim.SetFloat("isRunning", 1.0f);
        }
        if (gottaGoFast)
            speed *= 2;
        rgbd.velocity = velocity * speed;
    }

    private void LateUpdate()
    {
        ReskinAnimation();
    }

    void CalculateAngle(Vector2 dir)
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        angle = angle > 0.0f ? angle : angle + 360.0f;
        anim.SetBool("Up", false);
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Down", false);
        if (angle >= 45.0f && angle < 135.0f)
        {
            anim.SetBool("Up", true);
        }
        else if (angle >= 135.0f && angle < 225.0f)
        {
            anim.SetBool("Left", true);
        }
        else if (angle >= 225.0f && angle < 315.0f)
        {
            anim.SetBool("Down", true);
        }
        else
        {
            anim.SetBool("Right", true);
        }
    }

    void ReskinAnimation()
    {
        Sprite newSprite = sm.smourbiffList[idSmourbiff].Find(item => item.name == sprite.sprite.name);
        if (newSprite)
            sprite.sprite = newSprite;
        else
            sprite.sprite = null;
    }
}
