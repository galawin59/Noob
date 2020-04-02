using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSmourbiff : MonoBehaviour
{
    public static EncyclopediaSmourbiff GetEncyclopediaSmourbiff { get; private set; }

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }

        private set
        {
            isOpen = value;
        }
    }

    public event InventoryEvent EncyclopediaOpen = () => { };
    public event InventoryEvent EncyclopediaClose = () => { };
    [SerializeField] Sprite[] arraySmourbiff;
    public Dictionary<int, bool> dicoSmourbiff;
    // [SerializeField] Button closeButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] Image background;
    [SerializeField] Sprite backgroundSmourbiff;
    [SerializeField] Vector2 sizeSmourbiffPicture;
    [SerializeField] Vector2 offsetPicture;
    [SerializeField] Vector2 offsetBorder;
    Image[] smourbiffPictures;
    int nbPicturePerline;
    int nbPicturePerColumn;
    bool isOpen;
    int numberPicture;
    int currentIndexPage;
    int nbPicturePerPage;
    int nbMaxPage;

    // Use this for initialization
    IEnumerator Start()
    {
        dicoSmourbiff = new Dictionary<int, bool>();
        arraySmourbiff = new Sprite[SpriteManager.GetSpriteManager.NbSmourbiff];
        numberPicture = arraySmourbiff.Length;
        isOpen = false;
        for (int i = 0; i < numberPicture; i++)
        {
            dicoSmourbiff[i] = false;
            arraySmourbiff[i] = SpriteManager.GetSpriteManager.smourbiffList[i][0];
        }
        dicoSmourbiff[12] = true;
        smourbiffPictures = new Image[numberPicture];
        nbPicturePerline = (int)((GetComponent<RectTransform>().rect.width - offsetBorder.x * 2.0f) / (sizeSmourbiffPicture.x + offsetPicture.x)) + 1;

        nbPicturePerColumn = (int)((GetComponent<RectTransform>().rect.height - offsetBorder.y * 2.0f) / (sizeSmourbiffPicture.y + offsetPicture.y)) + 1;
        nbPicturePerPage = nbPicturePerline * nbPicturePerColumn;
        nbMaxPage = (int)(numberPicture / nbPicturePerPage);
        currentIndexPage = 0;
        int tmpLine = 0;
        int tmpColumn = 0;
        for (int i = 0; i < numberPicture; i++)
        {

            if (tmpLine >= nbPicturePerline)
            {
                tmpLine = 0;
                tmpColumn++;
            }
            if (tmpColumn >= nbPicturePerColumn)
            {
                tmpLine = 0;
                tmpColumn = 0;
            }

            GameObject tmpImg = new GameObject();
            tmpImg.name = "Smourbiff " + i;
            tmpImg.AddComponent<RectTransform>();
            tmpImg.transform.SetParent(transform);
            Vector2 tmpPos = Vector3.zero;
            tmpPos.x += offsetBorder.x + tmpLine * (sizeSmourbiffPicture.x + offsetPicture.x);
            tmpPos.y -= offsetBorder.y + tmpColumn * (sizeSmourbiffPicture.y + offsetPicture.y);
            RectTransform tmpRect = tmpImg.GetComponent<RectTransform>();
            tmpRect.pivot = Vector2.up;
            tmpRect.anchorMin = Vector2.up;
            tmpRect.anchorMax = Vector2.up;
            tmpRect.sizeDelta = sizeSmourbiffPicture;
            tmpRect.localScale = Vector3.one;
            tmpRect.anchoredPosition = tmpPos;
            smourbiffPictures[i] = tmpImg.AddComponent<Image>();
            smourbiffPictures[i].sprite = arraySmourbiff[i];

            tmpLine++;
        }
        while (HUDManager.GetHUDManager == null ||
           InventoryManager.GetInventoryManager == null ||
           CraftManager.GetCraftManager == null ||
            ResourcesInventoryManager.GetResourcesInventoryManager == null)
        {
            yield return null;
        }
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen += CloseEncyclopedia;
        CraftManager.GetCraftManager.CraftOpen += CloseEncyclopedia;
        InventoryManager.GetInventoryManager.InventoryOpen += CloseEncyclopedia;
        HUDManager.GetHUDManager.HUDClose += CloseEncyclopedia;

        CloseEncyclopedia();
    }

    private void Awake()
    {
        if (GetEncyclopediaSmourbiff == null)
        {
            GetEncyclopediaSmourbiff = this;
        }
        else if (GetEncyclopediaSmourbiff != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetInputManager.GetButtonDown("Collection", false))
        {
            if (!isOpen)
                OpenEncyclopedia();
            else
            {
                CloseEncyclopedia();
                HUDManager.GetHUDManager.Close();
            }
        }
    }

    public void OpenEncyclopedia()
    {
        HUDManager.GetHUDManager.Open();
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
        background.enabled = true;
        isOpen = true;
        UpdateEncyclopedia();
        EncyclopediaOpen();
    }

    void UpdateEncyclopedia()
    {
        for (int i = 0; i < numberPicture; i++)
        {
            if (currentIndexPage * nbPicturePerPage <= i && i < (currentIndexPage + 1) * nbPicturePerPage)
            {
                smourbiffPictures[i].enabled = true;
                if (dicoSmourbiff[i])
                {
                    smourbiffPictures[i].sprite = arraySmourbiff[i];
                }
                else
                {
                    smourbiffPictures[i].sprite = backgroundSmourbiff;
                }
            }
            else
            {
                smourbiffPictures[i].enabled = false;
            }
        }

        if (currentIndexPage == 0)
        {
            previousButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
        }

        if (currentIndexPage >= nbMaxPage)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }

    }

    public void AddEntryToEncyclopedia(int index)
    {
        dicoSmourbiff[index] = true;
    }

    void DeleteEntryToEncyclopedia(int index)
    {
        dicoSmourbiff[index] = false;
    }

    public void NextPage()
    {
        if (currentIndexPage < nbMaxPage)
        {
            currentIndexPage++;
            UpdateEncyclopedia();
        }
    }

    public void PreviousPage()
    {
        if (currentIndexPage > 0)
        {
            currentIndexPage--;
            UpdateEncyclopedia();
        }
    }

    public void CloseEncyclopedia()
    {
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        background.enabled = false;
        isOpen = false;
        EncyclopediaClose();
        for (int i = 0; i < numberPicture; i++)
        {
            smourbiffPictures[i].enabled = false;
        }
    }

    public void PrepareForCapture()
    {
        Smourbiff.OnCapture += AddEntryToEncyclopedia;
    }

    public void CancelCapture()
    {
        Smourbiff.OnCapture -= AddEntryToEncyclopedia;
    }

    void LoadDictionnary()
    {

    }

    void SaveDictionnary()
    {

    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
        EncyclopediaOpen -= OpenEncyclopedia;
        EncyclopediaClose -= CloseEncyclopedia;
    }
}
