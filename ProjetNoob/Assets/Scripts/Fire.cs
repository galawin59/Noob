using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    // Use this for initialization
   
    IEnumerator Start()
    {
        while (transform.parent.GetComponent<SpriteRenderer>().sortingOrder ==0)
        {
            yield return null;
        }
        GetComponent<SpriteRenderer>().sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

}
