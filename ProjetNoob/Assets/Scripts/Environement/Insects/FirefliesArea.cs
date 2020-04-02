using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirefliesArea : Area
{
    List<Firefly>[] list;
    [SerializeField]
    GameObject prefab;
    PlayerController player;
    [SerializeField]
    bool needToStream = false;
    [SerializeField]
    float distanceStream = 30f;

    IEnumerator Start()
    {
        list = new List<Firefly>[2];
        list[0] = new List<Firefly>();
        list[1] = new List<Firefly>();
        float radius = GetComponent<CircleCollider2D>().radius;
        string actualSceneName = SceneManager.GetActiveScene().name;
        //birds = new List<GameObject>();
        for (int i = 0; i < Nb; i++)
        {
            GameObject gm = Instantiate(prefab, transform);
            if (actualSceneName != GetComponent<ObjectSceneId>().SceneName)
            {
                SpriteRenderer[] spriteRenderers = gm.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.enabled = false;
                }
            }
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distanceToCenter = Random.Range(0f, 100f) / 100f;
            distanceToCenter = (1f - Mathf.Pow(distanceToCenter, 4)) * radius;
            gm.transform.position = new Vector2(transform.position.x + distanceToCenter * Mathf.Cos(angle),
                transform.position.y + distanceToCenter * Mathf.Sin(angle));
            list[0].Add(gm.GetComponent<Firefly>());
        }
        while (player == null)
        {
            yield return null;
            player = PlayerManager.GetPlayerManager.playerController;
        }
        if (needToStream)
        {
            Streamer streamer = gameObject.AddComponent<Streamer>();
            streamer.distanceStream = distanceStream;
            streamer.nbSecondsForRefreshing = 4f;
            streamer.target = gameObject;
            streamer.StartStream();
        }
    }
    private void Update()
    {
        foreach (Firefly b in list[1])
        {
            b.changeDirection(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Firefly b = collision.GetComponent<Firefly>();
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
        Firefly b = collision.GetComponent<Firefly>();
        if (b)
        {
            if (!list[1].Contains(b) && list[0].Contains(b))
            {
                list[1].Add(b);
            }
        }
    }
}
