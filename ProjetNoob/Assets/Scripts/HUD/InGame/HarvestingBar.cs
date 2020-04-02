using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestingBar : MonoBehaviour
{
    [SerializeField] GameObject chargement;
    [SerializeField] GameObject contour;
    // Use this for initialization
    void Start()
    {
        contour.gameObject.SetActive(false);
        PlayerManager.GetPlayerManager.playerController.onHarvesting += Harvesting;
        PlayerManager.GetPlayerManager.playerController.onStopHarvesting += StopHarvest;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Harvesting(float _value)
    {
        if (PlayerManager.GetPlayerManager.playerController.IsHarvesting)
        {
            if (!contour.gameObject.activeSelf)
                contour.gameObject.SetActive(true);
            chargement.GetComponent<Image>().fillAmount = (_value / 2.0f);
        }

    }

    void StopHarvest(float _value)
    {
        contour.gameObject.SetActive(false);
        chargement.GetComponent<Image>().fillAmount = (_value);
    }

    private void OnDestroy()
    {
        PlayerManager.GetPlayerManager.playerController.onHarvesting -= Harvesting;
        PlayerManager.GetPlayerManager.playerController.onStopHarvesting -= StopHarvest;
    }
}
