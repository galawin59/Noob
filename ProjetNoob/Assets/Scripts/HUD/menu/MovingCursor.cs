using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCursor : MonoBehaviour {
    float offset = 0.1f;
    RectTransform rect;
	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + offset*Mathf.Sin(Time.time), rect.localPosition.z);
	}
}
