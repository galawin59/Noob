using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : NetworkBehaviour
{
    //To do Change start id
    private int startSceneId = PlayerManager.GetPlayerManager.currentPlayer.GetComponent<PlayerController>().sceneId;
    private static MapManager instance = new MapManager();
    private Vector2Int mapSize = Vector2Int.zero;
    private Vector2Int tileSize = Vector2Int.zero;
    private List<Tileset> tilesetsList = new List<Tileset>();
    private const int tilesByTileset = 225;
    // Tilemap[] tileMapUnity;

    #region assessors
    // Game Instance Singleton
    public static MapManager GetMapManager
    {
        get
        {
            return instance;
        }
    }
    #endregion

    public Vector2Int FindBound()
    {
        TextAsset assetMap = (TextAsset)Resources.Load("Maps/" + ManagerScene.GetManagerScene.GetActiveSceneName(), typeof(TextAsset));
        JsonData root = JsonMapper.ToObject(assetMap.text);
        return new Vector2Int(root["width"].GetHashCode(), root["height"].GetHashCode());
    }

    public void LoadMap(string pathfile)
    {
        GameObject grid = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/Grid"));
        grid.name = "Grid" + pathfile;

        //tileMapUnity = grid.GetComponentsInChildren<Tilemap>();

        Scene scene = SceneManager.GetSceneByName(pathfile);

        bool activeScene = startSceneId != scene.buildIndex ? false : true;
        if (activeScene)
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(startSceneId));

        TextAsset assetMap = (TextAsset)Resources.Load("Maps/" + pathfile, typeof(TextAsset));
        JsonData root = JsonMapper.ToObject(assetMap.text);

        JsonData layers = root["layers"];
        JsonData tilesets = root["tilesets"];

        //main map info
        //string type = root["type"].GetString();
        mapSize.x = root["width"].GetHashCode();
        mapSize.y = root["height"].GetHashCode();
        tileSize.x = root["tilewidth"].GetHashCode();
        tileSize.y = root["tileheight"].GetHashCode();

        GameObject groupCollider = new GameObject("colliders group");
        SceneManager.MoveGameObjectToScene(groupCollider, scene);
        groupCollider.SetActive(activeScene);

        // Debug.Log(pathfile);
        for (int i = 0; i < tilesets.Count; i++)
        {
            JsonData tileset = tilesets[i];
            if (tilesets[i]["tilecount"].GetHashCode() > 1 || tilesets[i]["name"].GetString() == "water_tile")
            {

                Tileset tilesetTmp = new Tileset();
                tilesetTmp.Gid = tilesets[i]["firstgid"].GetHashCode();
                tilesetTmp.TilesetName = tilesets[i]["name"].GetString();
                tilesetTmp.Columns = tilesets[i]["columns"].GetHashCode();
                tilesetTmp.TilesetWidth = tilesets[i]["imagewidth"].GetHashCode();
                tilesetTmp.TilesetHeight = tilesets[i]["imageheight"].GetHashCode();
                tilesetTmp.Tilecount = tilesets[i]["tilecount"].GetHashCode();
                tilesetTmp.Margin = tilesets[i]["margin"].GetHashCode();
                tilesetTmp.Spacing = tilesets[i]["spacing"].GetHashCode();
                tilesetTmp.Image = tilesets[i]["image"].GetString();



                tilesetTmp.Sprites = Resources.LoadAll<Sprite>("Tilesets/" + tilesetTmp.TilesetName);
                try
                {
                    JsonData tiles = tilesets[i]["tiles"];

                    tilesetTmp.Colliders = new GameObject[tilesetTmp.Tilecount];

                    for (int itiles = 0; itiles < tiles.Count; itiles++)
                    {
                        try
                        {
                            int index = tiles[itiles]["id"].GetHashCode();

                            if (tiles[itiles]["objectgroup"]["objects"].Count > 0)
                            {
                                tilesetTmp.Colliders[index] = new GameObject("collider");
                            }
                            for (int j = 0; j < tiles[itiles]["objectgroup"]["objects"].Count; j++)
                            {
                                JsonData objectGrp = tiles[itiles]["objectgroup"]["objects"][j];

                                JsonData polygons = objectGrp["polygon"];




                                /*Debug.Log("x double " + objectGrp["x"].GetNatural());
                                Debug.Log("y double " + objectGrp["y"].GetNatural());*/


                                tilesetTmp.Colliders[index].transform.position = new Vector2(float.Parse(objectGrp["x"].ToString()), -float.Parse(objectGrp["y"].ToString()) + 48.0f) / 48.0f;

                                PolygonCollider2D poly = tilesetTmp.Colliders[index].AddComponent<PolygonCollider2D>();
                                Vector2[] points = new Vector2[polygons.Count];

                                poly.pathCount = 1;
                                for (int iPolygon = 0; iPolygon < polygons.Count; iPolygon++)
                                {
                                    Vector2 pos = Vector2.zero;
                                    pos.x = float.Parse(polygons[iPolygon]["x"].ToString()) / 48.0f;
                                    pos.y = -float.Parse(polygons[iPolygon]["y"].ToString()) / 48.0f;
                                    points[iPolygon] = pos;
                                }
                                poly.SetPath(0, points);


                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch (Exception e)
                {

                }
                tilesetsList.Add(tilesetTmp);
            }
        }
        //To do


        int witdhLayer = 0, heightLayer = 0;
        int layerOrder = 20;
        for (int iLayer = layers.Count - 1; iLayer >= 0; iLayer--)
        {
            string typeLayer = "", nameLayer = "";

            typeLayer = layers[iLayer]["type"].GetString();
            nameLayer = layers[iLayer]["name"].GetString();

            if (typeLayer == "tilelayer")
            {
                GameObject newTileMap = new GameObject();
                Tilemap tMap = newTileMap.AddComponent<Tilemap>();
                TilemapRenderer tRender = newTileMap.AddComponent<TilemapRenderer>();
                tRender.sortingOrder = layerOrder;
                layerOrder--;
                tRender.sortingLayerName = "Background";

                newTileMap.name = nameLayer;
                newTileMap.layer = 11;
                newTileMap.transform.SetParent(grid.transform);
                if (newTileMap.name == "Water")
                {
                    tRender.material = Resources.Load<Material>("Materials/Water");
                    tRender.sortingOrder = -100;
                }
                if (newTileMap.name == "Cascade")
                {
                    tRender.material = Resources.Load<Material>("Materials/Cascade");
                    tRender.sortingOrder = -90;
                }
                if (newTileMap.name == "Under Water")
                {
                    tRender.sortingOrder = -110;
                }
                witdhLayer = layers[iLayer]["width"].GetHashCode();
                heightLayer = layers[iLayer]["height"].GetHashCode();
                JsonData data = layers[iLayer]["data"];
                Tile tile = Instantiate(Resources.Load<Tile>("Tilesets/Base_Tile"));

                for (short i = 0; i < witdhLayer; i++)
                {
                    for (short j = 0; j < heightLayer; j++)
                    {
                        int nIndex = data[j * witdhLayer + i].GetHashCode();
                        int indexTileset = 0;

                        foreach (Tileset t in tilesetsList)
                        {

                            if (nIndex < t.Tilecount + t.Gid && nIndex - t.Gid >= 0)
                            {
                                tile.sprite = tilesetsList[indexTileset].Sprites[nIndex - t.Gid];

                                Vector3Int tilePos = new Vector3Int(i, -j, 0);
                                tMap.SetTile(tilePos, tile);

                                //create collider
                                if (t.Colliders != null)
                                {
                                    if (t.Colliders[nIndex - t.Gid] != null)
                                    {
                                        //  Debug.Log(tilesetsList[indexTileset].Sprites[nIndex - t.Gid].name);
                                        GameObject gm = Instantiate(t.Colliders[nIndex - t.Gid]);
                                        gm.isStatic = true;
                                        gm.transform.position += tMap.WorldToCell(tilePos);
                                        gm.transform.SetParent(groupCollider.transform);
                                    }
                                }

                                break;
                            }
                            indexTileset++;
                        }
                    }
                }
            }

            if (typeLayer == "objectgroup")
            {
                if (nameLayer == "Prefabs")
                {
                    JsonData objects = layers[iLayer]["objects"];

                    GameObject groupPrefabs = new GameObject("prefabs group");

                    SceneManager.MoveGameObjectToScene(groupPrefabs, scene);

                    if (!PersonalNetworkManager.isSERVER)
                        groupPrefabs.SetActive(activeScene);

                    for (int iObject = 0; iObject < objects.Count; iObject++)
                    {

                        float widthObject = (float)objects[iObject]["width"].GetHashCode();
                        //float heightObject = (float)objects[iObject]["height"].GetHashCode();


                        try
                        {
                            if (PersonalNetworkManager.isSERVER)
                            {

                                string path = "";
                                for (int iPropertie = 0; iPropertie < objects[iObject]["properties"].Count; iPropertie++)
                                {
                                    if (objects[iObject]["properties"][iPropertie]["name"].GetString() == "path")
                                    {
                                        path = objects[iObject]["properties"][iPropertie]["value"].GetString();
                                        break;
                                    }
                                }


                                GameObject gm = Resources.Load<GameObject>("Prefabs/" + path + "/" + objects[iObject]["name"]);

                                float X = 0.0f;
                                float Y = 0.0f;

                                X = float.Parse(objects[iObject]["x"].ToString());
                                Y = float.Parse(objects[iObject]["y"].ToString());

                                gm = GameObject.Instantiate(gm);
                                gm.GetComponent<ObjectSceneId>().SceneName = scene.name;

                                Link scptLink = gm.GetComponent<Link>();
                                Teleporter scptTeleporter = gm.GetComponentInChildren<Teleporter>();
                                if (!scptLink)
                                {
                                    scptLink = gm.GetComponentInChildren<Link>();
                                }

                                Area scptArea = gm.GetComponent<Area>();
                                PathFish scptPathFish = gm.GetComponent<PathFish>();
                                SpawnBirdTree scptSpawnBirdTree = gm.GetComponent<SpawnBirdTree>();
                                Crate crate = gm.GetComponent<Crate>();
                                SmourbiffStill smourb = gm.GetComponent<SmourbiffStill>();



                                if (scptPathFish)
                                {
                                    JsonData polygons = objects[iObject]["polygon"];

                                    for (int iPolygon = 0; iPolygon < polygons.Count; iPolygon++)
                                    {
                                        /*scptPathFish.CreateWayPoint(iPolygon, polygons.Count, new Vector2(float.Parse(polygons[iPolygon]["x"].ToString()),
                                            -float.Parse(polygons[iPolygon]["y"].ToString())) / 48.0f);*/

                                        scptPathFish.Waypoints.Add(new PathFish.Waypoint(iPolygon, polygons.Count, new Vector2(float.Parse(polygons[iPolygon]["x"].ToString()),
                                            -float.Parse(polygons[iPolygon]["y"].ToString())) / 48.0f));
                                    }
                                    scptPathFish.DisableGameobject();

                                }

                                if (scptArea)
                                {
                                    float radius;

                                    if (objects[iObject]["width"].IsReal)
                                    {
                                        radius = (float)objects[iObject]["width"].GetReal();
                                    }
                                    else radius = objects[iObject]["width"].GetHashCode();
                                    scptArea.radius = radius;


                                    gm.transform.position = new Vector2(
                                        (X + radius * 0.5f) / tileSize.x,
                                                                (-Y - radius * 0.5f) / tileSize.y + 1.0f);
                                    if (gm.GetComponent<CircleCollider2D>())
                                        gm.GetComponent<CircleCollider2D>().radius = radius * 0.5f / tileSize.x;

                                    scptArea.Nb = objects[iObject]["properties"][0]["value"].GetHashCode();


                                }
                                else
                                {
                                    gm.transform.position = new Vector2(
                                        (X + widthObject * 0.5f) / tileSize.x,
                                                                (-Y) / tileSize.y + 1.0f);


                                }

                                //  NetworkServer.SpawnWithClientAuthority(gm, connectionToClient);
                                gm.transform.SetParent(groupPrefabs.transform);
                                SpriteRenderer[] spr;
                                if (scptLink)
                                {
                                    scptLink.SceneName = scene.name;
                                    scptLink.IdSmourb = "-1";
                                    for (int i = 0; i < objects[iObject]["properties"].Count; i++)
                                    {
                                        if (objects[iObject]["properties"][i]["name"].GetString() == "zSmourbiffID")
                                        {
                                            scptLink.IdSmourb = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                        else if (objects[iObject]["properties"][i]["name"].GetString() == "Direction")
                                        {
                                            scptLink.Direction = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                        else if (objects[iObject]["properties"][i]["name"].GetString() == "LinkId")
                                        {
                                            scptLink.LinkId = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                        else if (objects[iObject]["properties"][i]["name"].GetString() == "NextId")
                                        {
                                            scptLink.NextId = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                        else if (objects[iObject]["properties"][i]["name"].GetString() == "NextRoom")
                                        {
                                            scptLink.NextRoom = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                        else if (objects[iObject]["properties"][i]["name"].GetString() == "Type")
                                        {
                                            scptLink.Type = objects[iObject]["properties"][i]["value"].GetString();
                                        }
                                    }
                                }

                                if (crate)
                                {
                                    for (int iPropertie = 0; iPropertie < objects[iObject]["properties"].Count; iPropertie++)
                                    {
                                        if (objects[iObject]["properties"][iPropertie]["name"].GetString() == "SpriteName")
                                        {
                                            crate.SetSpriteCrate(objects[iObject]["properties"][iPropertie]["value"].GetString());
                                            break;
                                        }
                                    }
                                }

                                if (smourb)
                                {
                                    for (int iPropertie = 0; iPropertie < objects[iObject]["properties"].Count; iPropertie++)
                                    {
                                        if (objects[iObject]["properties"][iPropertie]["name"].GetString() == "IdSmourbiff")
                                        {
                                            smourb.IdSmourbiff = int.Parse(objects[iObject]["properties"][iPropertie]["value"].GetString());
                                            break;
                                        }
                                    }
                                }


                                try
                                {
                                    if (objects[iObject]["properties"][0]["name"].GetString() == "layer")
                                    {
                                        string str = objects[iObject]["properties"][0]["value"].GetString();
                                        spr = gm.GetComponentsInParent<SpriteRenderer>();
                                        foreach (SpriteRenderer sp in spr)
                                        {
                                            sp.sortingLayerName = str;
                                        }

                                    }

                                }
                                catch (Exception e)
                                {

                                }

                                Scene s = SceneManager.GetSceneByName(pathfile);
                                string currentScene = SceneManager.GetSceneByBuildIndex(PlayerManager.GetPlayerManager.playerController.sceneId).name;
                                //Change scene for the server
                                if (currentScene != pathfile)
                                {
                                    if (scptLink)
                                    {
                                        scptLink.enabled = false;
                                    }
                                    if (scptSpawnBirdTree)
                                    {
                                        scptSpawnBirdTree.enabled = false;
                                    }
                                    FindObjectOfType<SpawnerSmourbiff>().enabled = false;
                                    spr = gm.GetComponentsInParent<SpriteRenderer>();
                                    Collider2D[] colliders = gm.GetComponentsInParent<Collider2D>();

                                    if (spr.Length > 0)
                                    {
                                        foreach (SpriteRenderer sp in spr)
                                        {
                                            sp.enabled = false;
                                        }
                                    }

                                    if (colliders.Length > 0)
                                    {
                                        foreach (Collider2D c in colliders)
                                        {
                                            c.enabled = false;
                                        }

                                    }


                                    spr = gm.GetComponentsInChildren<SpriteRenderer>();
                                    colliders = gm.GetComponentsInChildren<Collider2D>();
                                    if (spr.Length > 0)
                                    {
                                        foreach (SpriteRenderer sp in spr)
                                        {

                                            sp.enabled = false;
                                        }
                                    }

                                    if (colliders.Length > 0)
                                    {
                                        foreach (Collider2D c in colliders)
                                        {
                                            c.enabled = false;
                                        }
                                    }

                                    SpawnerSmourbiff smourbScript = gm.GetComponentInChildren<SpawnerSmourbiff>();
                                    if (smourbScript)
                                    {
                                        smourbScript.enabled = false;
                                    }
                                }

                                NetworkServer.Spawn(gm);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log(pathfile);
                            Debug.Log("error spawn game object " + objects[iObject]["name"].GetString());
                        }
                    }
                }

                /* if (nameLayer == "Collisions")
                 {
                     JsonData objects = layers[iLayer]["objects"];

                     GameObject groupColliderSupp = new GameObject("colliders group");

                     SceneManager.MoveGameObjectToScene(groupColliderSupp, scene);

                     groupColliderSupp.SetActive(activeScene);

                     for (int iObject = 0; iObject < objects.Count; iObject++)
                     {
                         //create one collider

                         string name = objects[iObject]["name"].GetString();
                         GameObject gm = new GameObject(name);
                         Vector2 position = new Vector2(float.Parse(objects[iObject]["x"].ToString()),
                             -float.Parse(objects[iObject]["y"].ToString()) + 48.0f) / 48.0f;
                         gm.transform.position = position;

                         JsonData polygons = objects[iObject]["polygon"];
                         PolygonCollider2D poly = gm.AddComponent<PolygonCollider2D>();
                         Vector2[] points = new Vector2[polygons.Count];

                         poly.pathCount = 1;
                         for (int iPolygon = 0; iPolygon < polygons.Count; iPolygon++)
                         {
                             points[iPolygon] = new Vector2(float.Parse(polygons[iPolygon]["x"].ToString()),
                                 -float.Parse(polygons[iPolygon]["y"].ToString())) / 48.0f;
                         }
                         poly.SetPath(0, points);
                         gm.transform.parent = groupColliderSupp.transform;
                     }
                 }*/

            }
        }
        SceneManager.MoveGameObjectToScene(grid, scene);
        foreach (Tileset t in tilesetsList)
        {
            if (t.Colliders != null)
            {
                foreach (GameObject d in t.Colliders)
                {
                    Destroy(d);
                }
            }
        }
        tilesetsList.Clear();
        grid.SetActive(activeScene);
        SetupClusters(groupCollider, new Vector2Int(witdhLayer, heightLayer), activeScene);
    }

    void SetupClusters(GameObject groupCollider, Vector2Int sizeMap, bool activeScene)
    {
        Vector2 sizeCluster = new Vector2(15f, 15f);
        int width = sizeMap.x % sizeCluster.x == 0 ? sizeMap.x / (int)sizeCluster.x : sizeMap.x / (int)sizeCluster.x + 1;
        int height = sizeMap.y % sizeCluster.y == 0 ? sizeMap.y / (int)sizeCluster.y : sizeMap.y / (int)sizeCluster.y + 1;
        GameObject[,] clusters = new GameObject[width, height];
        GameObject[,] parents = new GameObject[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                clusters[i, j] = new GameObject("Cluster " + i + ":" + j);
                clusters[i, j].transform.position = new Vector3(i * sizeCluster.x + sizeCluster.x / 2f, -j * sizeCluster.y - sizeCluster.y / 2f);
                parents[i, j] = new GameObject("parent");
                parents[i, j].transform.SetParent(clusters[i, j].transform);
                parents[i, j].transform.localPosition = Vector3.zero;
            }
        }

        for (int k = 0; k < groupCollider.transform.childCount; k++)
        {
            int i = (int)(groupCollider.transform.GetChild(k).position.x / sizeCluster.x);
            int j = (int)(-groupCollider.transform.GetChild(k).position.y / sizeCluster.y);
            if (i < 0)
            {
                i = 0;
            }
            if (i >= width)
            {
                i = width - 1;
            }
            if (j < 0)
            {
                j = 0;
            }
            if (j >= height)
            {
                j = height - 1;
            }
            groupCollider.transform.GetChild(k).SetParent(parents[i, j].transform);
            k--;
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                clusters[i, j].transform.SetParent(groupCollider.transform);
                Streamer streamer = clusters[i, j].AddComponent<Streamer>();

                streamer.distanceStream = 15f;
                streamer.nbSecondsForRefreshing = 3f;
                streamer.target = parents[i, j];
                if (activeScene)
                {
                    streamer.StartStream();
                }
            }
        }
    }

}
