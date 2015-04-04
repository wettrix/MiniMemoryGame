using UnityEngine;
using System.Collections;

// Set Enums outside classes so they become global
public enum GameState
{
	ChooseDifficulty = 0,
	SetupGame = 1,
	PlayGame = 2,
	GameClear = 3
}

public enum DifficultyState
{
	easy = 0,
	medium = 1,
	hard = 2
}

public class GameStates : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
