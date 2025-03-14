using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour 
{
    public event Action OnMoveEvent = delegate { };
    public BaseMode mode;
    public bool IsBusy { get; private set; }

    private Board m_board;
    private BottomCell m_bottomCell;
    private GameManager m_gameManager;
    private Camera m_cam;
    private GameSettings m_gameSettings;
    private List<Cell> m_potentialMatch;
    private float m_timeAfterFill;
    private bool m_hintIsShown;
    private bool m_gameOver;
    private bool m_gameWin;
    private bool isInit = false;
    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;


        m_board = new Board(this.transform, gameSettings);
        m_bottomCell = new BottomCell(this.transform, gameSettings);

        Fill();

    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
            case GameManager.eStateGame.GAME_WIN:
                m_gameWin = true;
                break;
        }
    }

    public void init()
    {
        mode.init( m_board, m_bottomCell);
    }
    public void Update()
    {
        if(mode!=null && !isInit)
        {
            init();
            isInit = true;
        }

        if (m_gameOver || m_gameWin) return;
        if (IsBusy) return;

        if (m_board.IsEmpty())
        {
            m_gameManager.GameWin();
        }

        mode.playLogicMethod?.Invoke();
        
    }
    
    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }


}
