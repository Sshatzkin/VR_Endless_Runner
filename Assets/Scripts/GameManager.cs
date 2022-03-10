using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;
    public GameState State;
    // Start is called before the first frame update

    public float time = 0;
    
    void Awake() {
        Instance = this;
    }
    
    void Start()
    {
        updateGameState(GameState.StartMenu);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (State == GameState.Running){
            time += Time.deltaTime;
        }
        
    }

    public void updateGameState (GameState newState){
        State = newState;

        switch (newState){
            case GameState.StartMenu:
                break;

            case GameState.Running:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    
}

public enum GameState {
    StartMenu,
    Running
}
