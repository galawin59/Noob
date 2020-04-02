using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HarvestableResources : NetworkBehaviour, IHarvestable, IInteractable
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite spriteHarvest;
    [SerializeField] GameObject emitter;
    [SerializeField] GameObject infoHarvest;

    public CraftResources.TypeResources ResourcesHarvest { get { return resourcesHarvest; } }
    public short Min { get { return min; } }
    public short Max { get { return max; } }

    [SerializeField] CraftResources.TypeResources resourcesHarvest = CraftResources.TypeResources.wood;
    [SerializeField] short min = 1;
    [SerializeField] short max = 12;
    [SerializeField] float timeForRespawnMin = 2f;
    [SerializeField] float timeForRespawnMax = 2f;
    [SerializeField] string nameSound = "";
    [SerializeField] float volumeSound = 0.5f;
    [SerializeField] bool haveSound = false;
    [SerializeField] int xpGive = 5;

    [SyncVar]
    bool isAlreadyHarvesting;

    [SyncVar(hook = "IsHit")]
    bool isHit;

    public bool IsAlreadyHarvesting
    {
        get
        {
            return isAlreadyHarvesting;
        }
    }


    SpriteRenderer sr;
    SeeThrough seeTr;

    [SyncVar(hook = "ChangeSprite")]
    bool isCut = false;

    public bool IsCut
    {
        get
        {
            return isCut;
        }

        private set
        {
            isCut = value;
        }
    }

    public event OnInteract onInteract;

    [Command]
    public void CmdHarvest()
    {
        IsCut = true;
        StartCoroutine(Repop());
    }

    [Command]
    public void CmdIsHit()
    {
        isHit = true;
    }

    [Command]
    public void CmdIsHarvesting()
    {
        isAlreadyHarvesting = true;
    }

    public void Harvest()
    {
        if (!IsCut)
        {
            onInteract();
            PlayerManager.GetPlayerManager.AddXpToPlayer(xpGive);
            int nbResources = 0;
            if (ResourcesInventoryManager.GetResourcesInventoryManager != null)
            {
                nbResources = UnityEngine.Random.Range(Min, Max + 1);
                ResourcesInventoryManager.GetResourcesInventoryManager.AddRessource(ResourcesHarvest, nbResources);
                ResourcesInventoryManager.GetResourcesInventoryManager.UpdateInvetory();
            }
            CmdHarvest();
            if (GetComponent<OutlineSprite>())
            {
                GetComponent<OutlineSprite>().DesactivateHelp();
            }
            GameObject tmp = Instantiate(infoHarvest, transform.position, Quaternion.identity);
            tmp.GetComponent<InfoHarvest>().SetText("+" + nbResources.ToString());
            tmp.GetComponent<InfoHarvest>().SetSprite(SpriteManager.GetSpriteManager.ResourcesSpriteHud[(int)ResourcesHarvest]);
            if (haveSound)
            {
                SoundManager.GetSoundManager.PlaySound(nameSound, volumeSound);
            }
        }
    }

    void ChangeSprite(bool isCut)
    {
        this.IsCut = isCut;
        sr.sprite = isCut ? spriteHarvest : baseSprite;
        if (!isCut && PlayerManager.GetPlayerManager.playerController.isHelpActive && GetComponent<OutlineSprite>())
        {
            GetComponent<OutlineSprite>().ActivateHelp();
        }

        if (isCut && emitter != null)
        {

            if (sr.enabled && gameObject.activeSelf)
            {
                Instantiate(emitter, transform.position, transform.rotation);
            }

            if (seeTr)
            {
                seeTr.gameObject.SetActive(false);
            }
        }
        else
        {
            if (seeTr)
            {
                seeTr.gameObject.SetActive(true);
            }
        }
    }

    void IsHit(bool _isHit)
    {
        isHit = _isHit;
        if (_isHit)
        {
            if (emitter != null)
            {
                Instantiate(emitter, transform.position, transform.rotation);
            }

            if (haveSound)
            {
                SoundManager.GetSoundManager.PlaySound(nameSound, volumeSound);
            }
        }
    }

    IEnumerator Repop()
    {
        yield return new WaitForSeconds(Random.Range(timeForRespawnMin, timeForRespawnMax));
        IsCut = false;
        isAlreadyHarvesting = false;
    }

    IEnumerator OnHarvest()
    {
        CmdIsHarvesting();
        for (int i = 0; i < 3; i++)
        {
            isHit = false;
            if (!isServer)
            {
                CmdIsHit();
                IsHit(true);
            }
            else
                isHit = true;
            yield return new WaitForSeconds(2.0f / 3.0f);
        }
        isHit = false;
        Harvest();
    }

    // Use this for initialization
    void Start()
    {
        onInteract += () => { };
        seeTr = GetComponentInChildren<SeeThrough>();
        sr = GetComponent<SpriteRenderer>();
        ChangeSprite(IsCut);
    }

    bool IInteractable.Interact()
    {
        if (!isCut && !isAlreadyHarvesting)
        {
            StartCoroutine(OnHarvest());
            // Harvest();
            return true;
        }
        else return false;
    }
}
