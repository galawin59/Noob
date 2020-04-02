using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    List<GameObject> fishsFriends = null;
    Vector2 velocity = Vector2.zero;
    float speed = 0.5f;
    public List<GameObject> FishsFriends
    {
        get
        {
            return fishsFriends;
        }

        set
        {
            fishsFriends = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    private void Start()
    {
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    public void InitializeFishsFriends()
    {
        fishsFriends = new List<GameObject>();
    }
}
