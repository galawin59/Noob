using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCursorPlayer : MonoBehaviour {

    float offset = 0.1f;
    [SerializeField] Vector3 localStartPos;

    private void Start()
    {
        localStartPos = transform.localPosition;
    }
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(localStartPos.x, localStartPos.y + offset * Mathf.Sin(Time.time*1.5f), transform.localPosition.z);
    }
}
