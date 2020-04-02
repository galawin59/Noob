using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButterfliesArea : Area
{
    List<Butterfly>[] list;
    [SerializeField] GameObject butterfly;

    private void Start()
    {
        list = new List<Butterfly>[2];
        list[0] = new List<Butterfly>();
        list[1] = new List<Butterfly>();
        float radius = GetComponent<CircleCollider2D>().radius;
        string actualSceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < Nb; i++)
        {
            GameObject gm = Instantiate(butterfly, transform);
            if (actualSceneName != GetComponent<ObjectSceneId>().SceneName)
            {
                gm.GetComponent<SpriteRenderer>().enabled = false;
            }
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distanceToCenter = Random.Range(0f, 100f) / 100f;
            distanceToCenter = (1f - Mathf.Pow(distanceToCenter, 2)) * radius;
            gm.transform.position = new Vector2(transform.position.x + distanceToCenter * Mathf.Cos(angle),
                transform.position.y + distanceToCenter * Mathf.Sin(angle));
            list[0].Add(gm.GetComponent<Butterfly>());
        }
    }

    private void Update()
    {
        foreach (Butterfly b in list[1])
        {
            b.changeDirection(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Butterfly b = collision.GetComponent<Butterfly>();
        if (b)
        {
            if (list[1].Contains(b))
            {
                list[1].Remove(b);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Butterfly b = collision.GetComponent<Butterfly>();
        if (b)
        {
            if (!list[1].Contains(b) && list[0].Contains(b))
            {
                list[1].Add(b);
            }
        }
    }
}
