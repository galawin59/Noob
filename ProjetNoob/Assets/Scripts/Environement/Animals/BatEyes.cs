using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEyes : MonoBehaviour
{
    float timer = 0f;
    [SerializeField]
    Animator animator;
    // Use this for initialization
    void Start()
    {
        if (animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (timer <= 0f && Random.Range(0, 600) == 0)
        {
            timer = 10f;
            animator.SetTrigger("activate");
        }
    }
}
