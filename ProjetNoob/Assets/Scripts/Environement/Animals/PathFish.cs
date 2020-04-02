using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PathFish : NetworkBehaviour
{

    Vector3[] path;
    [SerializeField] GameObject fishPrefab;
    GameObject fish;
    [SerializeField]
    GameObject waypointPref;
    List<Waypoint> waypoints = new List<Waypoint>();
    public string sceneName;

    public class Waypoint
    {
        public int index;
        public int nbWaypoint;
        public Vector2 position;

        public Waypoint(int _index, int _nbWaypoint, Vector2 _position)
        {
            index = _index;
            nbWaypoint = _nbWaypoint;
            position = _position;
        }
    }

    public Vector3[] Path
    {
        get
        {
            return path;
        }

        set
        {
            path = value;
        }
    }

    public void DisableGameobject()
    {
        StartCoroutine(IDisable());
    }

    IEnumerator IDisable()
    {
        while (fish == null)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public List<Waypoint> Waypoints
    {
        get
        {
            return waypoints;
        }

        set
        {
            waypoints = value;
        }
    }

    // Use this for initialization
    public override void OnStartServer()
    {
        foreach (Waypoint w in waypoints)
        {
            CreateWayPoint(w.index, w.nbWaypoint, w.position);
        }

        waypoints.Clear();
    }

    IEnumerator waitParent()
    {
        while (transform.childCount == 0)
        {
            yield return null;
        }
        Vector2 velocity = Vector2.zero;
        Vector2 dir = Vector2.zero;
        float angle = 0.0f;
        fish = Instantiate(fishPrefab);

        float scale = Random.Range(0.3f, 1.0f);

        fish.transform.localPosition = transform.GetChild(0).position;

        velocity = (transform.GetChild(1).transform.position - fish.transform.position).normalized * fish.GetComponent<Fish>().Speed;
        dir = fish.GetComponent<Rigidbody2D>().velocity;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        fish.GetComponent<Fish>().InitializeFishsFriends();

        fish.transform.parent = transform;
        fish.GetComponent<Rigidbody2D>().velocity = velocity;
        fish.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        fish.transform.localScale = Vector2.one * scale;
        for (int i = 1; i < 5; i++)
        {
            scale = Random.Range(0.3f, 1.0f);
            GameObject gm = Instantiate(fishPrefab);

            Vector3 extend = Random.insideUnitCircle * Random.Range(0.3f, 0.8f);
            gm.transform.localPosition = transform.GetChild(0).position + extend;
            fish.GetComponent<Fish>().FishsFriends.Add(gm);

            gm.transform.parent = transform;
            gm.GetComponent<Rigidbody2D>().velocity = velocity;
            gm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            gm.transform.localScale = Vector2.one * scale;
        }
    }

    public override void OnStartClient()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(waitParent());
        }
    }

    public void CreateWayPoint(int index, int nbWaypoint, Vector2 position)
    {
        GameObject gm = Instantiate(waypointPref);
        gm.name = "waypoint" + index;

        WaypointTrigger w = gm.GetComponent<WaypointTrigger>();

        w.Index = index;
        w.NbWaypoint = nbWaypoint;
        w.ParentID = transform;
        gm.transform.parent = transform;
        gm.transform.localPosition = position;

        NetworkServer.Spawn(gm);
    }

    private void OnEnable()
    {

        if (fish && fish.GetComponent<Fish>().FishsFriends != null)
        {

            Vector2 velocity = Vector2.zero;
            Vector2 dir = Vector2.zero;
            float angle = 0.0f;

            float scale = Random.Range(0.3f, 1.0f);

            fish.transform.localPosition = transform.GetChild(0).position - transform.position;

            velocity = (transform.GetChild(1).transform.position - fish.transform.position).normalized * fish.GetComponent<Fish>().Speed;
            dir = fish.GetComponent<Rigidbody2D>().velocity;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            fish.GetComponent<Rigidbody2D>().velocity = velocity;
            fish.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            fish.transform.localScale = Vector2.one * scale;
            foreach (GameObject f in fish.GetComponent<Fish>().FishsFriends)
            {
                scale = Random.Range(0.3f, 1.0f);

                Vector3 extend = Random.insideUnitCircle * Random.Range(0.3f, 0.8f);

                f.transform.localPosition = transform.GetChild(0).position + extend - transform.position;

                f.GetComponent<Rigidbody2D>().velocity = velocity;
                f.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                f.transform.localScale = Vector2.one * scale;
            }
        }
        else if (isClient)
        {
            StartCoroutine(waitParent());
        }
    }
}
