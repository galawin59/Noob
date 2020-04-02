using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingReward : MonoBehaviour
{
    string[] nameSprite = new string[4] { "Espadon", "PoissonRouge", "PoissonVert", "ornithorynque" };
    string[] nameTxt = new string[4] { "Espadon", "Poisson Rouge", "Poisson Vert", "ornithorynque" };
    Text txt;
    int idFish;
    public Fishing fishing;
    // Use this for initialization
    void Start()
    {
        Image img = transform.GetChild(1).GetComponent<Image>();
        txt = transform.GetChild(2).GetComponent<Text>();
        idFish = Random.Range(0, 4);
        img.sprite = Resources.Load<Sprite>("Sprites/Fishing/reward/" + nameSprite[idFish]);
        img.rectTransform.sizeDelta = new Vector2(img.sprite.texture.width, img.sprite.texture.height) * 4.0f;
        txt.text = nameTxt[idFish];
        transform.GetChild(0).localScale = Vector2.zero;
        transform.GetChild(1).localScale = Vector2.zero;
        transform.GetChild(2).localScale = Vector2.zero;
    }

    void Update()
    {
        transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, Vector2.one, 0.08f);
        transform.GetChild(1).localScale = transform.GetChild(0).localScale;
        transform.GetChild(2).localScale = transform.GetChild(0).localScale;
    }

    public void Yes()
    {
        if (!InventoryManager.GetInventoryManager.IsFull())
        {
            if (ResourcesInventoryManager.GetResourcesInventoryManager.HaveResources(CraftResources.TypeResources.wood, 15))
            {
                ResourcesInventoryManager.GetResourcesInventoryManager.RemoveResources(CraftResources.TypeResources.wood, 15);
                InventoryManager.GetInventoryManager.AddItem(new Item((Item.TypeItem.swordfishTrophey + idFish)));
                ChatManager.GetChatManager.SendInfoMsg("Trophee ajoute a l'inventaire", Message.MessageType.info);
            }
            else
            {
                ChatManager.GetChatManager.SendInfoMsg("Vous n'avez pas assez de bois.", Message.MessageType.info);
            }
        }
        else
        {
            ChatManager.GetChatManager.SendInfoMsg("Vous n'avez plus de place dans votre inventaire", Message.MessageType.info);
        }
        Destroy(gameObject);
    }

    public void No()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        fishing.IsFishing = false;
    }
}
