using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnerSmourbiff : MonoBehaviour
{
    public int idSmourbiff;
    [SerializeField] GameObject prefabsSmourbiff;
    [SerializeField] GameObject prefabsPortal;
    GameObject currentSmourbiff = null;
    GameObject link;
    GameObject portal;
    Vector2 posStart = new Vector2(10.0f, -8.0f);
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        idSmourbiff = -1;
        link = null;
        Smourbiff.OnCapture += OnCapture;
        Smourbiff.OnFlee += OnFlee;
        StartCoroutine(GiveRightID());
    }

    IEnumerator GiveRightID()
    {
        while (idSmourbiff == -1)
        {
            yield return null;
        }
        while(PlayerManager.GetPlayerManager.playerController.IsTeleporting)
        {
            yield return null;
        }
        GameObject[] gms = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < gms.Length; i++)
        {
            if (gms[i].GetComponent<Link>() != null)
            {
                link = gms[i];
            }
            else if (gms[i].GetComponentInChildren<Link>() != null)
            {

                link = gms[i].transform.Find("@Link(Clone)").gameObject;
            }
            if (link != null)
            {
                if (link.GetComponentInChildren<Link>().NextId != null)
                    link.GetComponentInChildren<Link>().NextId = "l" + idSmourbiff.ToString() + "Smourbiff";
                link.GetComponentInChildren<SpriteRenderer>().color = SpriteManager.GetSpriteManager.WhiteTransparentColor;
                currentSmourbiff = Instantiate(prefabsSmourbiff, posStart, Quaternion.identity);
                currentSmourbiff.GetComponent<Smourbiff>().IDSmourbiff = idSmourbiff;
                link.SetActive(false);
                i = gms.Length;
            }
        }
    }

    void OnCapture(int index)
    {
        portal = Instantiate(prefabsPortal, link.transform.position, Quaternion.identity);
        link.SetActive(true);
    }

    void OnFlee()
    {
        portal = Instantiate(prefabsPortal, link.transform.position, Quaternion.identity);
        link.SetActive(true);
    }

    private void OnDisable()
    {
        Destroy(currentSmourbiff);
        Destroy(portal);
        currentSmourbiff = null;
        Smourbiff.OnCapture -= OnCapture;
        Smourbiff.OnFlee -= OnFlee;
    }
}
