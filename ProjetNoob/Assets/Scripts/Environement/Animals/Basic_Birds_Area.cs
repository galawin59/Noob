using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Basic_Birds_Area : Area
{
    //List<GameObject> birds;
    [SerializeField]
    GameObject prefab;
    PlayerController player;
    [SerializeField]
    bool needToStream = false;
    [SerializeField]
    float distanceStream = 30f;
    GameObject parent;

    // Use this for initialization
    IEnumerator Start()
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        //birds = new List<GameObject>();
        parent = new GameObject("Parent");
        parent.transform.SetParent(transform);
        string actualSceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < Nb; i++)
        {
            GameObject gm = Instantiate(prefab, parent.transform);
            if (actualSceneName != GetComponent<ObjectSceneId>().SceneName)
            {
                gm.GetComponent<SpriteRenderer>().enabled = false;
            }
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distanceToCenter = Random.Range(0f, 100f) / 100f;
            distanceToCenter = (1f - Mathf.Pow(distanceToCenter, 2)) * radius;
            gm.transform.position = new Vector2(transform.position.x + distanceToCenter * Mathf.Cos(angle),
                transform.position.y + distanceToCenter * Mathf.Sin(angle));
            //birds.Add(gm);
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
            streamer.target = parent;
            streamer.StartStream();
        }
    }
}
