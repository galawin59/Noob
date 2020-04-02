using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streamer : MonoBehaviour
{
    PlayerController player;
    public GameObject target;
    public bool needToStream = false;
    public float distanceStream = 30f;
    public float nbSecondsForRefreshing = 4f;



    // Use this for initialization
    IEnumerator Start()
    {
        while (player == null)
        {
            yield return null;
            player = PlayerManager.GetPlayerManager.playerController;
        }
    }

    public void StartStream()
    {
        needToStream = true;
        if (gameObject.activeSelf)
        {
            StartCoroutine(Stream());
        }
    }

    public void StopStream()
    {
        needToStream = false;
        StopCoroutine(Stream());
    }

    IEnumerator Stream()
    {
        while (player == null)
        {
            yield return null;
            player = PlayerManager.GetPlayerManager.playerController;
        }
        while (player.IsTeleporting)
        {
            yield return null;
        }
        while (true)
        {
            float distanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
            if (distanceToPlayer <= distanceStream * distanceStream)
            {
                target.SetActive(true);
            }
            else
            {
                target.SetActive(false);
            }

            yield return new WaitForSeconds(nbSecondsForRefreshing);
        }
    }

    private void OnEnable()
    {
        if (needToStream)
        {
            StartCoroutine(Stream());
        }
        else
        {
            StartCoroutine(Stream());
        }
    }

    private void OnDisable()
    {
        StopCoroutine(Stream());
    }
}
