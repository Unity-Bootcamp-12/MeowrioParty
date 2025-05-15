using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GamePhase
{
    GameReady,
    GamePlay,
    GameEnd
}
public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GamePhase _currentPhase;
    [SerializeField] private int _maxGameTurn; //�׽�Ʈ�� ���� �ν����� ���� ���� ���� ����
    [SerializeField] private int _currentGameTurn;
    [SerializeField] private List<Player> _playerList;
    [SerializeField] private Board _board;
    [SerializeField] private Player _currentPlayer;

    private int _currentPlayerIndex; // ���� �����̰� �ִ� �÷��̾��� �ε�����
    private int _setTurnOrderIndex;
    private List<(int, Player)> playerDiceNumberList = new List<(int, Player)>();

    public Board Board { get { return _board; } }   //_board�� Ŭ���� �ܺο��� �� �� �ֵ��� �ϴ� Property

    public override void Awake()
    {
        base.Awake();
        
        _currentPhase = GamePhase.GameReady;
        _maxGameTurn = 2;
        _currentGameTurn = 0;
        _currentPlayerIndex = 0;
        //players = new List<Player>();
        //initialize board
        _currentPlayer = null;
        _setTurnOrderIndex = 0;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (_currentPhase == GamePhase.GameReady)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_setTurnOrderIndex < _playerList.Count)
                {
                    int playersDiceNum = _playerList[_setTurnOrderIndex].RollDice();
                    AddToPlayerOrderList(playersDiceNum);
                }
                else
                {
                    SetPlayerTurnOrder();
                    _currentPhase = GamePhase.GamePlay;
                }
            }
        }
        
        if (_currentPhase == GamePhase.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentPlayer = _playerList[_currentPlayerIndex];

                int playersDiceNum = _currentPlayer.RollDice();
                for (int i = 0; i < playersDiceNum; i++)
                {
                    int index = (_currentPlayer.currentTile.tileIndex + 1 ) % _board.tiles.Length;
                    _currentPlayer.MoveToNextTile(_board.tiles[index]);
                }
                //int playersDiceNum = _playerList[_currentMovingPlayerIndex].RollDice();
                //_playerList[_currentMovingPlayerIndex].Move(playersDiceNum);
                _currentPlayerIndex++;

                if (_currentPlayerIndex == _playerList.Count)
                {
                    _currentPlayerIndex = 0;
                    _currentGameTurn++;
                }

                if (_currentGameTurn == _maxGameTurn)
                {
                    _currentPhase = GamePhase.GameEnd;
                }
            }
        }
    }

    private void StartGame()
    {
        _currentPhase = GamePhase.GameReady;
    }

    private void SetPlayerTurnOrder()
    {
        playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < playerDiceNumberList.Count; i++)
        {
            _playerList[i] = playerDiceNumberList[i].Item2;
        }

        _currentPhase = GamePhase.GamePlay;
        _currentPlayer = _playerList[0];
    }

    private void AddToPlayerOrderList(int diceValue)
    {
        (int, Player) playerDiceNumber;

        playerDiceNumber.Item1 = diceValue;
        playerDiceNumber.Item2 = _playerList[_setTurnOrderIndex];

        playerDiceNumberList.Add(playerDiceNumber);
        _setTurnOrderIndex++;
    }
}
