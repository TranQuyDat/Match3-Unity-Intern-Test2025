using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCell 
{
    private int sizeX;
    private Cell[] m_cells;
    private Transform m_root;
    List<Item> its;

    public bool isFull => its.Count >= sizeX;
    public BottomCell(Transform parent, GameSettings gameSettings) 
    {
        m_root = parent;
        sizeX = gameSettings.BottomCellSizeX;
        m_cells = new Cell[sizeX];
        its = new List<Item>();
        createBottonCell();
    }

    public void createBottonCell()
    {
        Vector3 origin = new Vector3(-sizeX * 0.5f + 0.5f, -1 * 0.5f -3.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int y = 0; y < sizeX; y++) 
        {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(y,0 , 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(0, y, CellType.bottomCell);

                m_cells[y] = cell;
            
        }

        //set neighbours
        for (int i = 0; i < sizeX; i++)
        {
            if (i + 1 < sizeX) m_cells[i].NeighbourRight = m_cells[i+1];
            if (i > 0) m_cells[i].NeighbourLeft = m_cells[i-1];
        }

    }


    public void insertItem(Item it)
    {
        if (it == null || isFull )
        {
            return;
        }
        if(its == null || its.Count<=0)
        {
            its.Add(it);

            m_cells[0].Assign(it);
            m_cells[0].Item.View.DOMove(m_cells[0].transform.position, 0.3f);
            return;
        }
        (bool, int) isContain = checkContainIts((NormalItem)it);
        if (!isContain.Item1)
        {
            m_cells[its.Count].Assign(it);
            m_cells[its.Count].Item.View.DOMove(m_cells[its.Count].transform.position, 0.3f);
            its.Add(it);
            return;
        }
        its.Insert(isContain.Item2 + 1,it);
        sortItem();

    }
    public void sortItem()
    {
        for (int i = its.Count-1; i>=0 ;i--)
        {
            its[i].Cell.Free();
            Cell c = m_cells[its.IndexOf(its[i])];
            c.Free();
            c.Assign(its[i]);
            c.Item.View.DOMove(c.transform.position, 0.3f);
        }
    }
    public void removeitemFrom(Cell c) 
    {
        Item it = c.Item;
        c.Free();
        its.Remove(it);
        it.ShowAppearAnimation(() =>
        {
            it.Clear();
        });

        sortItem();
    }

    (bool, int) checkContainIts(NormalItem it)
    {
        int idx = 0;
        bool b = false;
        foreach (NormalItem other in its)
        {
            if (other.ItemType == it.ItemType)
            {
                b = true;
                idx = its.IndexOf(other);

            }
        }
        return (b, idx);
    }

    public List<Cell> GetHorizontalcellSame()
    {
        List<Cell> cells = new List<Cell>();
        for(int i = 1;i< sizeX-1; i++)
        {
            if (!checkValidThirdCell(m_cells[i])) 
                continue;
            if(m_cells[i].IsSameType(m_cells[i].NeighbourLeft) && 
                m_cells[i].NeighbourRight.IsSameType(m_cells[i].NeighbourLeft))
            {
                cells.Add(m_cells[i].NeighbourLeft);
                cells.Add(m_cells[i]);
                cells.Add(m_cells[i].NeighbourRight);
                return cells;
            }
            
        }
        return cells;
    }
    private bool checkValidThirdCell(Cell c)
    {
        bool b=(c!=null ||c.NeighbourLeft!=null ||c.NeighbourRight !=null
            || c.IsEmpty || c.NeighbourLeft.IsEmpty || c.NeighbourRight.IsEmpty
            );
        return b;
    }
    private Cell CheckThirdCell(Cell target, Cell main)
    {
        if (target != null && target != main && target.IsSameType(main))
        {
            return target;
        }

        return null;
    }

    
    internal Cell[] getCells()
    {
        return m_cells;
    }
    internal List<Item> getItems()
    {
        return its;
    }
    internal void addItem(Item it)
    {
        its.Add(it);
    }

    internal int getX()
    {
        return sizeX;
    }
}
