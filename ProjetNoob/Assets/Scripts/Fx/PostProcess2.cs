using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess2 : MonoBehaviour
{
    [SerializeField]
    Shader shd;
    [SerializeField]
    Color color;
    private void OnEnable()
    {
        Shader.SetGlobalColor("_ColorST", color);
        GetComponent<Camera>().SetReplacementShader(shd, "");
    }
}
