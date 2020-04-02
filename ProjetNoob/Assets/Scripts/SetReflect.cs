using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetReflect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().SetReflectOffset(new Vector2(0f, -1.5f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().SetReflectOffset(Vector2.zero);
        }
    }
}
