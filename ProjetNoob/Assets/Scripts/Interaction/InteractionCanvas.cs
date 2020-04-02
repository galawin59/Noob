using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionCanvas : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject prefabButton;
    [SerializeField] Vector2 sizeButton;
    [SerializeField] string[] nameInteraction;
    [SerializeField] GameObject canvas;

    [SerializeField] GameObject answer;
    [SerializeField] Text answerText;

    bool isInitialize = false;

    void EnableInteractionHud()
    {
        background.GetComponent<RectTransform>().anchoredPosition = GetCanvasPosition(Input.mousePosition);
        background.SetActive(true);
    }

    void DisableInteractionHud()
    {
        background.SetActive(false);
    }

    // Use this for initialization
    IEnumerator Start()
    {
        while (InteractionManager.GetInteractionManager == null)
        {
            yield return null;
        }
        for (int i = 0; i < (int)InteractionManager.Interactions.nbInteractions; i++)
        {
            GameObject newButton = Instantiate(prefabButton);
            newButton.transform.SetParent(background.transform);
            newButton.transform.localScale = Vector3.one;
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            Text text = newButton.GetComponentInChildren<Text>();
            text.fontSize = (int)(sizeButton.y / rectTransform.sizeDelta.y * text.fontSize);
            rectTransform.sizeDelta = sizeButton;
            rectTransform.anchoredPosition = new Vector2(0, -sizeButton.y * i);
            text.text = nameInteraction[i];
            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => Interact(index));
        }
        background.GetComponent<RectTransform>().sizeDelta = sizeButton * new Vector2(1, (int)InteractionManager.Interactions.nbInteractions);
        InteractionManager.GetInteractionManager.InteractionOpen += EnableInteractionHud;
        InteractionManager.GetInteractionManager.InteractionClose += DisableInteractionHud;
        InteractionManager.GetInteractionManager.NeedAnswer += NeedAnswer;
        isInitialize = true;
    }

    private void OnDestroy()
    {
        if (isInitialize)
        {
            InteractionManager.GetInteractionManager.InteractionOpen -= EnableInteractionHud;
            InteractionManager.GetInteractionManager.InteractionClose -= DisableInteractionHud;
            InteractionManager.GetInteractionManager.NeedAnswer -= NeedAnswer;
        }
    }

    void NeedAnswer(string targetName, InteractionManager.TypeRequest typeRequest)
    {
        if (typeRequest == InteractionManager.TypeRequest.trade)
        {
            answer.SetActive(true);
            answerText.text = targetName + " vous propose un echange";
        }
    }

    public void SetAnswer(bool answer)
    {
        this.answer.SetActive(false);
        InteractionManager.GetInteractionManager.SetAnswer(answer);
    }

    void Interact(int id)
    {
        InteractionManager.GetInteractionManager.Interact(id);
    }

    Vector2 GetCanvasPosition(Vector2 screenPosition)
    {
        Vector2 sizeCanvas = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 position = new Vector2();
        position.x = screenPosition.x / Screen.width * sizeCanvas.x;
        position.y = screenPosition.y / Screen.height * sizeCanvas.y;
        return position;
    }
}
