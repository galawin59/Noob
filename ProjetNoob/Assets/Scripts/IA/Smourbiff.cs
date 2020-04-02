using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smourbiff : MonoBehaviour
{
    public delegate void DelegateTryCatch(int numberTry);
    static public event DelegateTryCatch OnTryCatch = (int numberTry) => { };

    public delegate void DelegateCapture(int index);
    static public event DelegateCapture OnCapture = (int index) => { };

    public delegate void DelegateFlee();
    static public event DelegateFlee OnFlee = () => { };

    Rigidbody2D rgbd;
    Vector2 velocity;
    Vector2 direction;
    Animator anim;
    SpriteManager sm;
    [SerializeField] float speed = 15.0f;
    [SerializeField] float distanceCatch = 15.0f;
    [SerializeField] int numberTryCatch = 4;
    [SerializeField] float timerBeforeFlee = 30.0f;
    [SerializeField] float timerInvulnerability = 3.0f;
    [SerializeField] int idSmourbiff = 0;

    public int IDSmourbiff
    {
        set
        {
            idSmourbiff = value;
        }
    }

    bool isVulnerable;
    float distanceToRun;
    float runDistance;
    bool isRunning;
    SCamera sCam;
    SpriteRenderer sprite;
    new CircleCollider2D collider;
    bool isRunningIntoWall;
    bool hasFlee;

    // Use this for initialization
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        isRunning = false;
        distanceToRun = 0.0f;
        sCam = Camera.main.GetComponent<SCamera>();
        collider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        isVulnerable = true;
        hasFlee = false;
        sm = SpriteManager.GetSpriteManager;
        GameObject.Find("EncyclopediaSmourbiff").GetComponent<EncyclopediaSmourbiff>().PrepareForCapture();
        StartCoroutine(DecreaseTimerFlee());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
            StartCoroutine(Run());
        if (isVulnerable)
            CatchSmourbiff();
        if (hasFlee)
        {
            Destroy(gameObject);
            OnFlee();
        }
    }

    IEnumerator Run()
    {
        isRunning = true;
        runDistance = 0.0f;
        velocity = DecideDirection();
        CalculateAngle(velocity);
        distanceToRun = Random.Range(250.0f, 400.0f);
        isRunningIntoWall = false;
        yield return new WaitForSeconds(2.0f);
        while (runDistance < distanceToRun && !isRunningIntoWall)
        {
            runDistance += (velocity * speed).magnitude;
            rgbd.velocity = velocity * speed;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(transform.position.x, sCam.MinSizeMap.x + collider.radius, sCam.MaxSizeMap.x - collider.radius);
            pos.y = Mathf.Clamp(transform.position.y, sCam.MinSizeMap.y + collider.radius, sCam.MaxSizeMap.y - collider.radius);
            transform.position = pos;

            yield return null;
        }
        rgbd.velocity = Vector2.zero;
        isRunning = false;
    }

    Vector2 DecideDirection()
    {
        Vector2 direction;
        direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        if (Mathf.Abs(transform.position.x - sCam.MinSizeMap.x) < 7.0f)
        {
            direction.x = Random.Range(0.0f, 1.0f);
        }
        if (Mathf.Abs(transform.position.x - sCam.MaxSizeMap.x) < 7.0f)
        {
            direction.x = Random.Range(-1.0f, 0.0f);
        }
        if (Mathf.Abs(transform.position.y - sCam.MinSizeMap.y) < 7.0f)
        {
            direction.y = Random.Range(0.0f, 1.0f);
        }
        if (Mathf.Abs(transform.position.y - sCam.MaxSizeMap.y) < 7.0f)
        {
            direction.y = Random.Range(-1.0f, 0.0f);
        }

        direction.Normalize();
        return direction;
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

    void CatchSmourbiff()
    {
        if (InputManager.GetInputManager.GetMouseButtonDown(0))
        {
            if (Vector2.Distance(PlayerManager.GetPlayerManager.currentPlayer.transform.position, transform.position) < distanceCatch)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(mousePos, transform.position) < collider.radius)
                {
                    numberTryCatch--;
                    if (numberTryCatch <= 0)
                        numberTryCatch = 0;

                    StartCoroutine(DecreaseTimer());
                    if (numberTryCatch <= 0)
                    {
                        OnCapture(idSmourbiff);
                        GameObject.Find("EncyclopediaSmourbiff").GetComponent<EncyclopediaSmourbiff>().CancelCapture();
                        Destroy(gameObject);
                    }
                    else
                        OnTryCatch(numberTryCatch);
                    isVulnerable = false;
                }
            }
        }
    }

    private void LateUpdate()
    {
        ReskinAnimation();
    }

    void ReskinAnimation()
    {
        Sprite newSprite = sm.smourbiffList[idSmourbiff].Find(item => item.name == sprite.sprite.name);
        if (newSprite)
            sprite.sprite = newSprite;
        else
            sprite.sprite = null;
    }

    IEnumerator DecreaseTimerFlee()
    {
        while (timerBeforeFlee > 0.0f)
        {
            timerBeforeFlee -= Time.deltaTime;
            yield return null;
        }
        hasFlee = true;
    }

    IEnumerator DecreaseTimer()
    {
        yield return new WaitForSeconds(timerInvulnerability);
        isVulnerable = true;
    }
}
