using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    /* [SerializeField]
     float winingArea;*/
    GameObject fishEndu;
    Rigidbody2D rb;
    float timerMove;
    float pushTimer;
    //  float winTimerMax;
    //  float winTimer;
    float fishForce;
    float startingForce;
    float Xlimit;
    bool flee;
    bool win;
    Vector2 hitForce;

    public bool Flee
    {
        get
        {
            return flee;
        }

        set
        {
            flee = value;
        }
    }

    public bool Win
    {
        get
        {
            return win;
        }

        set
        {
            win = value;
        }
    }

    void Start()
    {
        hitForce = new Vector2(40.0f, 0.0f);
        rb = GetComponent<Rigidbody2D>();
        timerMove = Random.Range(0.2f, 0.6f);
        pushTimer = 0.6f;
        /*     winTimerMax = 8.0f;
             winTimer = 0.0f;*/
        Xlimit = (transform.parent.GetChild(0).GetComponent<RectTransform>().rect.width - GetComponent<RectTransform>().rect.width) * 0.5f;
        flee = false;
        win = false;

        fishForce = Random.Range(9, 10);
        startingForce = fishForce;
        fishEndu = transform.parent.GetChild(2).gameObject;
        transform.localPosition = new Vector2(transform.localPosition.x + Random.Range(-0.1f,0.1f), transform.localPosition.y);
    }


    void Update()
    {
        if (fishForce <= 0.0f)
        {
            win = true;
        }

        if (pushTimer > timerMove)
        {
            Vector2 v = Vector2.right * ((transform.localPosition.x >= 0.0f) ? 1 : -1) * Random.Range(20.0f, 30.0f) * fishForce;
            rb.AddForce(v, ForceMode2D.Impulse);
            pushTimer = 0.0f;
            timerMove = Random.Range(0.2f, 0.6f);
        }
        else pushTimer += Time.deltaTime;

        if (InputManager.GetInputManager.GetButton("Horizontal"))
        {

            rb.AddForce(hitForce * InputManager.GetInputManager.GetAxisRaw("Horizontal") * 0.5f, ForceMode2D.Impulse);

        }

        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -Xlimit, Xlimit), transform.localPosition.y, transform.localPosition.z);

        if (transform.localPosition.x >= Xlimit ||
           transform.localPosition.x <= -Xlimit)
        {
            flee = true;
        }

        fishForce = Mathf.Clamp(fishForce - Time.deltaTime * 0.5f, 0, 20);
        fishEndu.transform.localScale = new Vector3(8.2f * (fishForce / startingForce), fishEndu.transform.localScale.y, fishEndu.transform.localScale.z);
    }
}
