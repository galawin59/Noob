using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Pnj : MonoBehaviour, IInteractable
{
    [SerializeField] Canvas canvas;
    [SerializeField] Image textBubble;
    [SerializeField] Image nextMessage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Vector3 textOffset;
    [SerializeField] bool isPanneau = false;
    [SerializeField] List<float> timeForSpawnLetter;
    [TextArea(3, 5)]
    [SerializeField] List<string> dialogues;
    int indexDialogue;
    bool needToUpdate = false;
    bool textIsFull = false;
    float timer;

    public bool TextIsFull
    {
        get
        {
            return textIsFull;
        }

        private set
        {
            textIsFull = value;
            nextMessage.gameObject.SetActive(value);
        }
    }

    public event OnInteract onInteract;

    bool IInteractable.Interact()
    {
        if (!canvas.gameObject.activeSelf && timer <= 0f)
        {
            needToUpdate = true;
            StopAllCoroutines();
        }
        return false;
    }

    // Use this for initialization
    void Start()
    {
        indexDialogue = 0;
        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();
        }
        if (textBubble == null && canvas != null)
        {
            textBubble = canvas.transform.Find("Image").transform.Find("ImageBubble").GetComponent<Image>();
        }
        if (text == null && canvas != null)
        {
            text = canvas.transform.Find("Image").transform.Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>();
        }

        if (nextMessage == null && textBubble != null)
        {
            nextMessage = textBubble.transform.Find("ImageFinText").GetComponent<Image>();
        }
    }

    IEnumerator UpdateText()
    {
        TextIsFull = false;
        text.text = "";
        int i = 0;
        while (i < dialogues[indexDialogue].Length && (timeForSpawnLetter == null || timeForSpawnLetter.Count <= indexDialogue || timeForSpawnLetter[indexDialogue] > 0))
        {
            text.text += dialogues[indexDialogue][i++];
            yield return new WaitForSeconds(timeForSpawnLetter != null && timeForSpawnLetter.Count > indexDialogue ? timeForSpawnLetter[indexDialogue] : 0.1f);
        }
        text.text = dialogues[indexDialogue];
        TextIsFull = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (canvas.gameObject.activeSelf && InputManager.GetInputManager.GetButtonDown("Interact"))
        {
            needToUpdate = true;
        }
        if (canvas.gameObject.activeSelf && Vector3.Distance(transform.position, PlayerManager.GetPlayerManager.playerController.transform.position) >= 1.5f)
        {
            canvas.gameObject.SetActive(false);
        }
        if (needToUpdate)
        {
            bool t = false;
            if (canvas.gameObject.activeSelf)
            {
                if (TextIsFull)
                {
                    indexDialogue++;
                }
                else
                {
                    StopAllCoroutines();
                    text.text = dialogues[indexDialogue];
                    TextIsFull = true;
                    t = true;
                }
            }
            else
            {
                canvas.gameObject.SetActive(true);
                indexDialogue = 0;
                StartCoroutine(UpdateText());
            }

            if (indexDialogue < dialogues.Count)
            {
                if (TextIsFull && !t)
                {
                    StartCoroutine(UpdateText());
                }
            }
            else
            {
                canvas.gameObject.SetActive(false);
                timer = 0.5f;
            }
            needToUpdate = false;
        }

        UpdateTextPos();
    }

    private void UpdateTextPos()
    {
        if (textBubble != null && text != null)
        {
            if (isPanneau)
            {
                Vector3 textPos = Vector3.zero;
                Vector3 pnjPos;
                Vector3 playerPos;
                Vector3 scaleFlip = textBubble.rectTransform.localScale;

                pnjPos = transform.position;
                playerPos = PlayerManager.GetPlayerManager.playerController.transform.position;

                textPos.x = playerPos.x > pnjPos.x ? pnjPos.x - 0.8f : pnjPos.x + 0.8f;
                textPos.y = pnjPos.y + 1.2f;

                scaleFlip.x = playerPos.x > pnjPos.x ? -1.0f : 1.0f;

                textBubble.rectTransform.localScale = scaleFlip;

                textBubble.transform.position = textPos;
                text.transform.position = textPos + textOffset;
            }
            else
            {
                Vector3 textPos = Vector3.zero;
                Vector3 pnjPos;
                Vector3 playerPos;
                Vector3 scaleFlip = textBubble.rectTransform.localScale;

                pnjPos = transform.position;
                playerPos = PlayerManager.GetPlayerManager.playerController.transform.position;

                textPos.x = playerPos.x > pnjPos.x ? pnjPos.x - 0.8f : pnjPos.x + 0.8f;
                textPos.y = pnjPos.y + 0.6f;

                scaleFlip.x = playerPos.x > pnjPos.x ? -1.0f : 1.0f;

                textBubble.rectTransform.localScale = scaleFlip;

                textBubble.transform.position = textPos;
                text.transform.position = textPos + textOffset;
            }
        }
    }
}
