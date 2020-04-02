using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    Animator animator;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAnim(bool isOpen)
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen);
        }
    }
}
