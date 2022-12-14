using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSeats : MonoBehaviour
{
    public static PlayerSeats Instance { get; private set; }

    public const int MaxSeats = 5;

    public event Action<Player, int> PlayerSitEvent;
    public event Action<Player, int> PlayerLeaveEvent;
    
    public List<Player> Players => _players.ToList();
    [ReadOnly] [SerializeField] private List<Player> _players;

    public int TakenSeatsAmount => _players.Count(x => x != null);

    [SerializeField] private float _conncetionLostCheckInterval;

    private void OnValidate()
    {
        _players = new List<Player>(MaxSeats);
        for (var i = 0; i < MaxSeats; i++)
        {
            _players.Add(null);
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckForConnectonLost());
    }

    public bool TryTake(Player player, int seatNumber)
    {
        if (_players[seatNumber] != null)
        {
            Log.WriteLine($"Player ('{player.NickName}') can`t take the {seatNumber} seat, its already taken by Player('{_players[seatNumber].NickName}).'");
            return false;
        }

        TryLeave(player);

        _players[seatNumber] = player;

        Log.WriteLine($"Player ('{player.NickName}') sit on {seatNumber} seat.");

        PlayerSitEvent?.Invoke(player, seatNumber);
        return true;
    }

    public bool TryLeave(Player player)
    {
        if (_players.Contains(player) == false)
        {
            return false;
        }

        int seatNumber = _players.IndexOf(player);
        _players[seatNumber] = null;

        Log.WriteLine($"Player ('{player.NickName}') leave from {seatNumber} seat.");

        PlayerLeaveEvent?.Invoke(player, seatNumber);
        return true;
    }

    public bool IsFree(int seatNumber)
    {
        return _players[seatNumber] == null;
    }

    private IEnumerator CheckForConnectonLost()
    {
        while (true)
        {
            Log.WriteToFile($"CheckForConnectionLost cycle", $"{Application.persistentDataPath}\\CustomLog.log");

            for (var i = 0; i < _players.Count; i++)
            {
                try
                {
                    GameObject gameObjectName = _players[i].gameObject;
                    Log.WriteToFile($"Connection stable on '{_players[i].NickName}'. GameObject name '{gameObjectName}'", $"{Application.persistentDataPath}\\CustomLog.log");
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        // Check for MissingReferenceException ("Kolhoz" because cant catch the real MissingReferenceException in build).
                        string nick = _players[i].NickName;
                        Log.WriteToFile($"Connection lost on player ('{nick}') on {i} seat.", $"{Application.persistentDataPath}\\CustomLog.log");
                        TryLeave(_players[i]);
                    }
                    catch (NullReferenceException) { }
                }
            }

            yield return new WaitForSeconds(_conncetionLostCheckInterval);
        }
    }
}
