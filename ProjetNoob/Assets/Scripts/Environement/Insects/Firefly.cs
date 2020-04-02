using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{

    Rigidbody2D rb;
    float speed = 0.5f;
    float time;
    float change = 0.25f;
    float angle = 0.0f;
    float angleMax = 0.8f;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        angle += Random.Range(0.0f, 360.0f);
        rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > change)
        {
            angle += Random.Range(-angleMax, angleMax);
            rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            time = 0.0f;
        }
        else time += Time.deltaTime;
    }

    float getAngle(Vector2 _p1, Vector2 _p2)
    {
        return Mathf.Atan2(_p2.y - _p1.y, _p2.x - _p1.x) * 180 / Mathf.PI;
    }


    public void changeDirection(Vector3 _to)
    {
        angle = getAngle(transform.position, _to);
        rb.velocity = (_to - transform.position).normalized * speed;
    }
}
