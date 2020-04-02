using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    Renderer currentRend;
    MaterialPropertyBlock propBlock;

    Material firstMaterial;
    [SerializeField]
    Material secondMaterial;
    float multiplier;
    float speedTimer;
    bool istrigger;

    void Start()
    {
        currentRend = transform.parent.GetComponent<Renderer>();

        propBlock = new MaterialPropertyBlock();

        firstMaterial = currentRend.material;
        multiplier = 0.0f;
        speedTimer = 1.5f;
        istrigger = false;
    }

    private void Update()
    {
        currentRend.GetPropertyBlock(propBlock);
        if (istrigger)
        {

            multiplier += Time.deltaTime * speedTimer;
            multiplier = Mathf.Clamp(multiplier, 0.0f, 1.0f);
            propBlock.SetFloat("_multiplier", multiplier);
            currentRend.SetPropertyBlock(propBlock);
        }
        else
        {
            if (multiplier > 0.0f)
            {
                multiplier -= Time.deltaTime * speedTimer;
                multiplier = Mathf.Clamp(multiplier, 0.0f, 1.0f);
                propBlock.SetFloat("_multiplier", multiplier);
                currentRend.SetPropertyBlock(propBlock);
            }
            else
            {
                currentRend.material = firstMaterial;
            }

        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController ctrl = other.GetComponent<PlayerController>();
        if (ctrl)
        {
            if (ctrl.isLocalPlayer)
            {
                istrigger = true;
                currentRend.material = secondMaterial;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController ctrl = other.GetComponent<PlayerController>();
        if (ctrl)
        {
            if (ctrl.isLocalPlayer)
            {
                istrigger = false;
            }
        }
    }

    private void OnDisable()
    {
        if (firstMaterial)
        {
            istrigger = false;
            currentRend.material = firstMaterial;
        }
    }
}
