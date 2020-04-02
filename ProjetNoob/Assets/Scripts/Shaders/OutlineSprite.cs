using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSprite : MonoBehaviour
{
    public Color color = Color.white;
    public Color color2 = Color.white;

    [SerializeField] float distance;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        PlayerManager.GetPlayerManager.playerController.OnHelp += ActivateHelp;
        PlayerManager.GetPlayerManager.playerController.OnDesactivate += DesactivateHelp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_OutlineColor", color);
        spriteRenderer.material.SetColor("_SecondOutlineColor", color2);
        spriteRenderer.material.SetFloat("isActive", 0);
    }

    public void ActivateHelp()
    {
        spriteRenderer.material.SetInt("isActive", 1);
    }

    public void DesactivateHelp()
    {
        spriteRenderer.material.SetInt("isActive", 0);
    }

}
