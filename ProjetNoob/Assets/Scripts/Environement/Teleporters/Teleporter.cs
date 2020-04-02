using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : Link
{
    Renderer currentRend;
    MaterialPropertyBlock propBlock;
    float radius;
    bool isTriggered;
    List<Collider2D> players;

    public float Radius
    {
        get
        {
            return radius;
        }

        set
        {
            radius = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        currentRend = transform.GetChild(1).transform.GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        radius = 1.0f;
        isTriggered = false;
        players = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            if (radius > 0.0f)
            {
                currentRend.GetPropertyBlock(propBlock);
                radius -= Time.deltaTime * 0.6f;
                radius = Mathf.Clamp(radius, 0.0f, 1.0f);
                propBlock.SetFloat("_rangeRadius", radius);
                currentRend.SetPropertyBlock(propBlock);
            }

            if (radius <= 0f)
            {
                foreach (Collider2D playerCollider in players)
                {
                    StartCoroutine(ChangeSceneOnTrigger(playerCollider));
                    propBlock.SetFloat("_rangeRadius", radius);
                    currentRend.SetPropertyBlock(propBlock);
                }
                players.Clear();
                isTriggered = false;
            }
        }
        else
        {
            if (radius < 1.0f)
            {
                currentRend.GetPropertyBlock(propBlock);
                radius += Time.deltaTime * 0.6f;
                radius = Mathf.Clamp(radius, 0.0f, 1.0f);
                propBlock.SetFloat("_rangeRadius", radius);
                currentRend.SetPropertyBlock(propBlock);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController p = collision.GetComponent<PlayerController>();
        if (p)
        {
            if (!players.Contains(collision))
            {
                players.Add(collision);
                isTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController p = collision.GetComponent<PlayerController>();
        if (p)
        {
            if (players.Contains(collision))
            {
                players.Remove(collision);
                if (players.Count <= 0)
                {
                    isTriggered = false;
                } 
            }
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
    }
}
