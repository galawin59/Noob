using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory
{
    Item[,] items;
    int nbColumns;
    int nbLines;
    public bool isOpen;

    #region assessors
    public int NbColumns
    {
        get
        {
            return nbColumns;
        }
    }

    public int NbLines
    {
        get
        {
            return nbLines;
        }
    }
    #endregion

    public Inventory(int nbColumns, int nbLines)
    {
        this.nbColumns = nbColumns;
        this.nbLines = nbLines;
        items = new Item[nbColumns, nbLines];
        isOpen = false;
    }

    public Item GetItem(int column, int line)
    {
        if (column >= items.GetLength(0) || line >= items.GetLength(1) || column < 0 || line < 0)
        {
            return null;
        }
        return items[column, line];
    }

    public bool Contain(Item item)
    {
        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] == item)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AddItem(int column, int line, Item item)
    {
        if (column >= items.GetLength(0) || line >= items.GetLength(1) || column < 0 || line < 0)
        {
            return false;
        }

        items[column, line] = item;
        return true;
    }

    public bool AddItem(Item item)
    {
        for (int j = 0; j < nbLines; j++)
        {
            for (int i = 0; i < nbColumns; i++)
            {
                if (items[i, j] == null)
                {
                    items[i, j] = item;
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(Item item)
    {
        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] == item)
                {
                    items[i, j] = null;
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(int column, int line)
    {
        if (column >= items.GetLength(0) || line >= items.GetLength(1) || column < 0 || line < 0)
        {
            return false;
        }
        else if (items[column, line] == null)
        {
            return false;
        }
        items[column, line] = null;
        return true;
    }

    public void Clear()
    {

        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                items[i, j] = null;
            }
        }
    }

    public bool IsEmpty()
    {

        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsFull()
    {
        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Sort()
    {

        Item[] tmpArray = new Item[nbLines * nbColumns];
        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                int index = i + j * nbColumns;
                if (items[i, j] != null)
                {
                    tmpArray[index] = items[i, j];
                }
                else
                {
                    tmpArray[index] = new Item();
                }
            }
        }

        tmpArray = tmpArray.OrderByDescending(x => x.ItemType).ToArray();

        for (int i = 0; i < nbLines * nbColumns; i++)
        {
            if (tmpArray[i].ItemType != Item.TypeItem.unknown)
            {
                items[i % nbColumns, i / nbColumns] = tmpArray[i];
            }
            else
            {
                items[i % nbColumns, i / nbColumns] = null;
            }
        }
    }

    public int GetNbRemainingPlace()
    {
        int count = 0;

        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] == null)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public int GetNbOccupiedPlace()
    {
        int count = 0;

        for (int i = 0; i < nbColumns; i++)
        {
            for (int j = 0; j < nbLines; j++)
            {
                if (items[i, j] != null)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
