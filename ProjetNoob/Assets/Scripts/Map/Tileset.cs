
using UnityEngine;

public class Tileset{
    private int gid;
    private string tilesetName;
    private int columns;
    private int tilesetWidth;
    private int tilesetHeight;
    private int tilecount;
    private int margin;
    private int spacing;
    private int offset;
    private string image;
    private Sprite[] sprites;
    private GameObject[] colliders;

    #region assessors
    public int Gid
    {
        get
        {
            return gid;
        }

        set
        {
            gid = value;
        }
    }

    public string TilesetName
    {
        get
        {
            return tilesetName;
        }

        set
        {
            tilesetName = value;
        }
    }

    public int Columns
    {
        get
        {
            return columns;
        }

        set
        {
            columns = value;
        }
    }

    public int TilesetWidth
    {
        get
        {
            return tilesetWidth;
        }

        set
        {
            tilesetWidth = value;
        }
    }

    public int TilesetHeight
    {
        get
        {
            return tilesetHeight;
        }

        set
        {
            tilesetHeight = value;
        }
    }

    public int Tilecount
    {
        get
        {
            return tilecount;
        }

        set
        {
            tilecount = value;
        }
    }

    public int Margin
    {
        get
        {
            return margin;
        }

        set
        {
            margin = value;
        }
    }

    public int Spacing
    {
        get
        {
            return spacing;
        }

        set
        {
            spacing = value;
        }
    }

    public string Image
    {
        get
        {
            return image;
        }

        set
        {
            image = value;
        }
    }

    public Sprite[] Sprites
    {
        get
        {
            return sprites;
        }

        set
        {
            sprites = value;
        }
    }

    public int Offset
    {
        get
        {
            return offset;
        }

        set
        {
            offset = value;
        }
    }

    public GameObject[] Colliders
    {
        get
        {
            return colliders;
        }

        set
        {
            colliders = value;
        }
    }
    #endregion
}
