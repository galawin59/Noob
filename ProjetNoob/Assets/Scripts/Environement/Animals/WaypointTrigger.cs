using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaypointTrigger : NetworkBehaviour
{
    [SyncVar]
    int index;
    [SyncVar]
    int nbWaypoint;
    [SyncVar(hook = "OnChangeTransform")]
    Transform parentID;
   
    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
        }
    }

    void OnChangeTransform(Transform transform)
    {
        parentID = transform;
    }

    public int NbWaypoint
    {
        get
        {
            return nbWaypoint;
        }

        set
        {
            nbWaypoint = value;
        }
    }

    public Transform ParentID
    {
        get
        {
            return parentID;
        }

        set
        {
            parentID = value;
        }
    }

    public override void OnStartClient()
    {
        if (!PersonalNetworkManager.isSERVER)
        {
            transform.parent = parentID;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PathFish p = null;
        if (collision.gameObject.transform.parent)
            p = collision.gameObject.transform.parent.gameObject.GetComponent<PathFish>();

        if (p == transform.parent.GetComponent<PathFish>())
        {
            Fish fscript = collision.gameObject.GetComponent<Fish>();
            if (fscript.FishsFriends != null)
            {

                int nextIndex = index + 1;
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (nextIndex > nbWaypoint - 1)
                {
                    rb.velocity = (transform.parent.GetChild(0).transform.position - collision.gameObject.transform.position).normalized * fscript.Speed;
                }
                else rb.velocity = (transform.parent.GetChild(nextIndex).transform.position - collision.gameObject.transform.position).normalized * fscript.Speed;

                Vector2 dir = rb.GetComponent<Rigidbody2D>().velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                collision.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


                foreach (GameObject f in fscript.FishsFriends)
                {

                    f.GetComponent<Rigidbody2D>().velocity = rb.velocity;
                    f.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }
    }
}
