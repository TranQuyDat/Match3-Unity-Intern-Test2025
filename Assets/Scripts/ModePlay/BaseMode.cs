using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMode 
{

    public Action playLogicMethod = delegate { };
    protected bool m_isTapping = false;
    protected bool m_isClearing = false;
    protected BottomCell m_bottomCell;
    protected Board m_board;
    protected GameManager m_gameManager;

    protected float m_TimeDelay = 0;
    private bool m_hintIsShown;
    public BaseMode(GameManager gameManager) 
    {
        m_gameManager = gameManager;
    }
    public virtual void init( Board board , BottomCell bottomCell)
    {
        m_board = board;
        m_bottomCell = bottomCell;
    }
    protected void AddItemfrom(Cell c1)
    {
        m_bottomCell.insertItem(c1.Item);
        c1.Free();
    }
    protected void clearCollapse()
    {
        List<Cell> cells = m_bottomCell.GetHorizontalcellSame();
        if (cells != null && cells.Count >= 3)
            m_gameManager.StartCoroutine(clearAndCollapseBtmCell(cells));
        else if (cells != null && cells.Count <= 0 && m_bottomCell.isFull)
        {
            //game lose
            m_gameManager.GameOver();
        }
    }

    IEnumerator clearAndCollapseBtmCell(List<Cell> cells)
    {

        m_isClearing = true;
        yield return new WaitForSeconds(1f);
        clear3iteminBtmcless(cells);
        yield return new WaitForSeconds(0.2f);

    }

    void clear3iteminBtmcless(List<Cell> cells)
    {
        if (cells == null || cells.Count <= 0)
        {
            m_isClearing = false;
            return;
        }
        cells.Reverse();
        foreach (Cell cell in cells)
        {
            m_bottomCell.removeitemFrom(cell);
        }

        m_isClearing = false;
    }
}

public class ModeMove : BaseMode
{

    private Collider2D m_hitCollider;
    private Camera m_cam;
    public ModeMove(GameManager gameManager) : base(gameManager)
    {
        playLogicMethod = playModeMove;
        m_cam = Camera.main;
    }
    private void playModeMove()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && !m_isTapping && !m_isClearing)
            {
                m_isTapping = true;
                m_hitCollider = hit.collider;
                Cell c1 = m_hitCollider.GetComponent<Cell>();
                if (c1.cellType == CellType.bottomCell || c1.Item == null) return;
                AddItemfrom(c1);
                clearCollapse();

            }
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ResetRayCast();
        }
    }

    private void ResetRayCast()
    {
        m_isTapping = false;
        m_hitCollider = null;
    }

}
public class ModeAutoLose : BaseMode
{
    private int boardx;
    private int boardy;
    (Item, int)[] freq ;
    int freqSize = 0;
    public ModeAutoLose(GameManager gameManager) : base(gameManager)
    {
        playLogicMethod = AutoLose;
    }

    public override void init(Board board, BottomCell bottomCell)
    {
        base.init(board, bottomCell);

        boardx = m_board.XY().Item1;
        boardy = m_board.XY().Item2;
        GetCommonItemsFromBoard(m_board.getCells());
        freqSize = 0;
    }

    private void AutoLose()
    {
        if (m_isTapping)
        {
            m_TimeDelay -= Time.deltaTime; // Giảm TimeDelay dần về 0
            if (m_TimeDelay <= 0)
            {
                m_isTapping = false; // Khi hết thời gian delay, cho phép thực hiện hành động tiếp theo
            }
            return;
        }

        if (m_isClearing) return; // Nếu đang clear, không làm gì cả

        m_isTapping = true;  // Đánh dấu đang thực hiện hành động

       
        if (freqSize<5)
        {
            Cell c = freq[freqSize].Item1.Cell;
            if (c != null)
            {
                AddItemfrom(c);
                m_TimeDelay = 0.5f;
                freqSize++;
            }
            return;
        }
        else if (m_bottomCell.isFull)
        {
            m_gameManager.GameOver();
        }
    }

    private void GetCommonItemsFromBoard(Cell[,] m_cell)
    {
        freq = new (Item, int)[m_bottomCell.getX()];
        freqSize = 0;

        foreach (var cell in m_cell)
        {
            Item item = cell.Item; // Đổi từ value thành item

            // Kiểm tra xem item đã tồn tại trong freq chưa
            int index = Array.FindIndex(freq, 0, freqSize, x => x.Item1.Equals(item));

            if (index != -1)
                freq[index].Item2++; // Tăng số lần xuất hiện
            else if (freqSize < 5)
                freq[freqSize++] = (item, 1); // Thêm item mới

            if (freqSize == 5) return; // Đủ 5 phần tử thì thoát
        }
    }

}
public class ModeAutoWin : BaseMode
{
    public ModeAutoWin(GameManager gameManager) : base(gameManager)
    {
        playLogicMethod= AutoWin;
    }


    private void AutoWin()
    {
        if (m_isTapping)
        {
            m_TimeDelay -= Time.deltaTime; // Giảm TimeDelay dần về 0
            if (m_TimeDelay <= 0)
            {
                m_isTapping = false; // Khi hết thời gian delay, cho phép thực hiện hành động tiếp theo
            }
            return;
        }

        if (m_isClearing) return; // Nếu đang clear, không làm gì cả

        m_isTapping = true;  // Đánh dấu đang thực hiện hành động

        //===>btmcell chưa có item => random 1 item trên board<===
        if (m_bottomCell.getItems().Count <= 0)
        {
            Cell cell = m_board.getRandomCell();
            if (cell != null)
            {
                AddItemfrom(cell);
                m_TimeDelay = 0.5f;
            }
            return;
        }

        //===> Đã có item trong btmCell => tìm item giống type trên board để add vào <===
        else if (m_bottomCell.getItems().Count > 0 && m_bottomCell.getItems().Count <= 3 && m_bottomCell.getCells()[2].IsEmpty)
        {
            Cell c = m_board.getCellsameItem(m_bottomCell.getCells()[0].Item);
            if (c != null)
            {
                AddItemfrom(c);
                m_TimeDelay = 1f;
            }
            return;
        }

        //===> Đủ 3 item giống nhau => bắt đầu clear <===
        else
        {
            clearCollapse();
            m_TimeDelay = 0.5f;
        }
    }


}