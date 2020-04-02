using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SmourbiffStill : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] int idSmourbiff = -1;

    public int IdSmourbiff
    {
        set
        {
            idSmourbiff = value;
        }
    }
    SpriteRenderer sprite;
    // Use this for initialization
    void Start()
    {
        Smourbiff.OnCapture += OnCapture;
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = SpriteManager.GetSpriteManager.smourbiffList[idSmourbiff][0];
    }

    void OnCapture(int id)
    {
        if (id == idSmourbiff)
        {
            if (PersonalNetworkManager.isSERVER)
                gameObject.SetActive(false);
            else
               Destroy(gameObject);

            //Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Smourbiff.OnCapture -= OnCapture;
    }
}
