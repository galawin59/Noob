using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class SCamera : MonoBehaviour
{
    [SerializeField]
    Transform target;
    Camera cam;
    //float speed = 2.0f;
    //Tilemap currentTilemap;
    float camheight;
    float camwidth;
    public Vector2Int bounds;

    bool stopFollowingPlayer;
    bool firstApparition;

    #region assessors
    public Vector2 MinSizeMap
    {
        get
        {
            return new Vector2(0.0f, -bounds.y + 1);
        }
    }

    public Vector2 MaxSizeMap
    {
        get
        {
            return new Vector2(bounds.x, 1);
        }
    }

    public bool StopFollowingPlayer
    {
        get
        {
            return stopFollowingPlayer;
        }

        set
        {
            stopFollowingPlayer = value;
        }
    }
    #endregion

    public bool IsObjectVisible(Renderer renderer)
    {

        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), renderer.bounds);
    }

    // Use this for initialization
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
        bounds = Vector2Int.zero;
        camwidth = 0.0f;
        camheight = 0.0f;
        cam = GetComponent<Camera>();

        stopFollowingPlayer = false;
        firstApparition = false;
    }

    private void FixedUpdate()
    {
        if (target)
        {
            if (!stopFollowingPlayer)
            {
                if (firstApparition)
                    FollowObject(target.position, 0.2f);
                else
                {
                    FollowObject(target.position);
                    firstApparition = true;
                }

            }
        }
        else
        {
            if (PlayerManager.GetPlayerManager.currentPlayer && ManagerScene.GetManagerScene.GetActiveSceneName() != "World")
            {
                target = PlayerManager.GetPlayerManager.currentPlayer.transform;
                camheight = GetComponent<Camera>().orthographicSize * 2.0f;
                camwidth = camheight * GetComponent<Camera>().aspect;
                camwidth *= 0.5f;
                camheight *= 0.5f;

                bounds = MapManager.GetMapManager.FindBound();
            }
        }
    }

    public void FollowObject(Vector2 position, float _lerpValue = 0.0f)
    {

        float X;
        float Y;
        if (_lerpValue != 0.0f)
        {
            X = Mathf.Lerp(transform.position.x, Mathf.Clamp(position.x, camwidth, bounds.x - camwidth), _lerpValue);
            Y = Mathf.Lerp(transform.position.y, Mathf.Clamp(position.y, -bounds.y + camheight + 1, -camheight + 1), _lerpValue);
        }
        else
        {
            X = Mathf.Clamp(position.x, camwidth, bounds.x - camwidth);
            Y = Mathf.Clamp(position.y, -bounds.y + camheight + 1, -camheight + 1);
        }

        transform.position = new Vector3(X, Y, -1.0f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }
}
