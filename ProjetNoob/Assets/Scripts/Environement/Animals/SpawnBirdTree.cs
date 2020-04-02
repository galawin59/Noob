using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnBirdTree : MonoBehaviour
{
    [SerializeField]
    List<GameObject> prefabBirds;
    [SerializeField] float distance;
    HarvestableResources tree;


    IEnumerator Start()
    {
        yield return null;
        tree = GetComponent<HarvestableResources>();
        if (tree)
        {
            tree.onInteract += SpawnBirdWhenCut;
        }
    }

    void SpawnBirdWhenCut()
    {
        if (Random.Range(0, 1) != 0)
        {
            return;
        }
        Vector3 posPlayer = PlayerManager.GetPlayerManager.playerController.transform.position;
        int nbBirds = Random.Range(0, 4);
        for (int i = 0; i < nbBirds; i++)
        {
            StartCoroutine(SpawnBird(posPlayer));
        }
    }

    IEnumerator SpawnBird(Vector3 posPlayer)
    {
        Vector3 posSpawn = transform.position + (Vector3)Random.insideUnitCircle / 3f;
        Basic_Bird bird = Instantiate(prefabBirds[Random.Range(0, prefabBirds.Count)], posSpawn, Quaternion.identity).GetComponent<Basic_Bird>();
        yield return null;
        bird.FlyAway(posPlayer);
    }

    IEnumerator SpawnBird()
    {
        Vector3 posPlayer;
        float distanceToPlayer;
        float lowestDistanceToPlayer;
        while (true)
        {
            while (tree == null || tree.IsCut)
            {
                yield return new WaitForSeconds(1f);
            }
            lowestDistanceToPlayer = 1000f;
            foreach (GameObject player in PlayerManager.GetPlayerManager.listConnectedPlayers[SceneManager.GetActiveScene().buildIndex])
            {
                posPlayer = PlayerManager.GetPlayerManager.playerController.transform.position;
                distanceToPlayer = Vector3.SqrMagnitude(transform.position - posPlayer);
                if (lowestDistanceToPlayer > distanceToPlayer)
                {
                    lowestDistanceToPlayer = distanceToPlayer;
                }
                if (distanceToPlayer <= distance * distance && Random.Range(0, 1000) == 0)
                {
                    Vector3 posSpawn = transform.position + (Vector3)Random.insideUnitCircle / 3f;
                    Basic_Bird bird = Instantiate(prefabBirds[Random.Range(0, prefabBirds.Count)], posSpawn, Quaternion.identity).GetComponent<Basic_Bird>();
                    yield return null;
                    bird.FlyAway(posPlayer);
                    break;
                }
            }
            if (lowestDistanceToPlayer <= distance * distance * 5)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(4f);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnBird());
    }

}
