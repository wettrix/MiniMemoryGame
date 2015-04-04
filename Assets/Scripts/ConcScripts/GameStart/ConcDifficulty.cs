using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class ConcDifficulty : MonoBehaviour 
{
	// Classes
	private ConcGameStart cGameStart;
	private ScreenFit cScreenFit;
	private ConcTimer cTimer;
	private ConcSound cSound;

	// Skins
	public GUISkin SetSkin;
	
	// Get Texture
	private Texture2D[] Textures = new Texture2D[4];
	// Where we load Images from
	private string ResourcesLocation = "Concentration/";
	
	// rectangles
	private Rect rcBackground;	// Background Image
	private Rect rcDrawArea;	// where we draw GUILayout
	private Rect rcReallyQuit;
	private Rect rcYesButton;
	private Rect rcNoButton;

	// Screen Settings
	internal int newWidth;		// New Aspect screen width
	internal int newHeight;		// New Aspect screen height

	internal GameState CurrentState;
	internal DifficultyState CurrentDifficulty;

	// Game stuff
	internal int GameCountdownValue = 0;	// Start value for countdown before game starts
	internal int GameCorrectOrIncorrect = 0;	// Start value Showing if it's the correct train or not
	internal int GameVictory = 0;	// Start value Showing if it's the correct train or not
	internal bool bShowReallyQuit;

	// Animation bools
	internal bool bShowPuzzlePair;
	internal bool bShowPuzzleClear;
	internal bool bShowCountdown;
	internal bool bShowFading;

	// Use this for initialization
	void Start () 
	{
		// Set FPS
		Application.targetFrameRate = 60;
		// Gets GameStart Class
		cGameStart = GetComponent<ConcGameStart>();
		// Gets ScreenFit Class
		cScreenFit = GetComponent<ScreenFit>();
		// Initialize Screen Settings
		cScreenFit.init();
		// Get Timer Class
		cTimer = GetComponent<ConcTimer>();
		cSound = GetComponent<ConcSound>();

		// Add Images
		Object textures01 = Resources.Load(ResourcesLocation + "Difficulty/BackDifficulty", typeof(Texture2D));
		Object textures02 = Resources.Load(ResourcesLocation + "Game/BackGame", typeof(Texture2D));
		Object textures03 = Resources.Load(ResourcesLocation + "Game/QuitText", typeof(Texture2D));
		Object textures04 = Resources.Load(ResourcesLocation + "Game/invisible_layer", typeof(Texture2D));
		Textures[0] = (Texture2D)textures01;
		Textures[1] = (Texture2D)textures02;
		Textures[2] = (Texture2D)textures03;
		Textures[3] = (Texture2D)textures04;


		// Setting Screen sizes
		newWidth = cScreenFit.width;
		newHeight = cScreenFit.height;
		
		// Rectangle Images
		rcBackground	= new Rect(0, 0, Screen.width, Screen.height);
		rcDrawArea		= new Rect(0, 0, newWidth,	   newHeight);

		rcReallyQuit	= new Rect( (newWidth / 2 * 1) - (400) 			/ 2,	newHeight / 10 * 3,	400,			200			);
		rcYesButton		= new Rect( (newWidth / 4 * 1) - (360 / 10 * 9) / 2,	newHeight / 10 * 5,	360 / 10 * 9,	240 / 10 * 9);
		rcNoButton		= new Rect( (newWidth / 4 * 3) - (360 / 10 * 9) / 2,	newHeight / 10 * 5,	360 / 10 * 9,	240 / 10 * 9);
		// Everytime we open this file the game state is always choose Difficulty
		CurrentState = GameState.ChooseDifficulty;
		// By default Choose easy
		CurrentDifficulty = DifficultyState.easy;

		// Remove previous memory when entering here
		Resources.UnloadUnusedAssets();
	}

	// Will Display the menu
	void OnGUI()
	{
		if(SetSkin)
			GUI.skin = SetSkin;
		
		// Background picture
		if(CurrentState == GameState.ChooseDifficulty)
			GUI.DrawTexture(rcBackground, Textures[0]);
		else if(CurrentState == GameState.PlayGame)
		{
			if(CurrentDifficulty == DifficultyState.easy)
				GUI.DrawTexture(rcBackground, Textures[1]);
			else if(CurrentDifficulty == DifficultyState.medium)
				GUI.DrawTexture(rcBackground, Textures[1]);
			else if(CurrentDifficulty == DifficultyState.hard)
				GUI.DrawTexture(rcBackground, Textures[1]);
		}

		// Set Aspect Ratio
		cScreenFit.setScaledScreen();

		// Show Select Difficulty Screen
		if(CurrentState == GameState.ChooseDifficulty)
		{
			GUILayout.BeginArea(rcDrawArea);	// Start Drawing Area

			GetGUIChooseDifficulty();
			GUILayout.EndArea();	// End DrawingArea
		}
		// Show Game Screen
		else if(CurrentState == GameState.PlayGame)
		{
			GUILayout.BeginArea(rcDrawArea);	// Start Drawing Area
		
			// Show Cards game and menu
			cGameStart.GetGUICardPlacement();

			// Show messages 
			GetGUICountDown();
			GetGUIQuit();

			// Timer Animations
			if(bShowPuzzlePair)
				cTimer.GetGUIConcPair();
			if(bShowPuzzleClear)
				cTimer.GetGUIConcClear();
			if(bShowFading)
				cTimer.GetGUIFadingTimer();

			GUILayout.EndArea();	// End DrawingArea
		}

		// -----------------------------------------------------------------------------------------------------------------------------------------------------
		// Debug information
	//	GUI.Label(new Rect(0,0,150,20),  "State = " + CurrentState);
	//	GUI.Label(new Rect(0,20,150,20), "Diffi = " + CurrentDifficulty);
	//	GUI.Label(new Rect(0,60,150,20), "Fail = " + cGameStart.CurrentFail);
		/*
		for(int i = 0 ; i < cGameStart.bIsCardVisible.Length ; i++)
		{
			GUI.Label(new Rect(10,i * 20,150,20),  "bIsCardVisible[" + i + "] = " + cGameStart.bIsCardVisible[i]);
			GUI.Label(new Rect(100,i * 20,500,20),  "rcCard[" + i + "] = " + cGameStart.rcCard[i]);
		}
		// -----------------------------------------------------------------------------------------------------------------------------------------------------
	//	*/
	}

	/*
	 * Show Select Difficulty menu
	 */
	void GetGUIChooseDifficulty()
	{
		GUIStyle DifficultyEasy;
		GUIStyle DifficultyMedium;
		GUIStyle DifficultyHard;
		GUIStyle SelectReturn;
		DifficultyEasy = 	GUI.skin.GetStyle("DifficultyEasy");
		DifficultyMedium =	GUI.skin.GetStyle("DifficultyMedium");
		DifficultyHard = 	GUI.skin.GetStyle("DifficultyHard");
		SelectReturn = 		GUI.skin.GetStyle("SelectReturn");

		// Game Easy Button
		GUILayout.Space(300);
		GUILayout.BeginVertical("Box");
		// ----------------------------------------
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		
		if( GUILayout.Button("", DifficultyEasy, GUILayout.Width(512), GUILayout.Height(118) ) )
		{
//			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
//				GAITrackerPlugin.sendButtonTracker("uiButton", "actionDifficult", "ConcLevel1");
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.easy;
			cGameStart.initializeCards(6);
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Game Medium Button
		GUILayout.Space(45);
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", DifficultyMedium, GUILayout.Width(512), GUILayout.Height(118) ) )
		{
//			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
//				GAITrackerPlugin.sendButtonTracker("uiButton", "actionDifficult", "ConcLevel2");
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.medium;
			cGameStart.initializeCards(6);
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Game Hard Button
		GUILayout.Space(45);
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", DifficultyHard, GUILayout.Width(512), GUILayout.Height(118) ) )
		{
//			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
//				GAITrackerPlugin.sendButtonTracker("uiButton", "actionDifficult", "ConcLevel3");
			CurrentState = GameState.SetupGame;
			CurrentDifficulty = DifficultyState.hard;
			cGameStart.initializeCards(6);
			cSound.stopBGMSelectDif();
			cSound.playBGMGamePlay();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		// ----------------------------------------
		GUILayout.EndVertical();
		
		
		// Go Back to Title Button
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if( GUILayout.Button("", SelectReturn, GUILayout.Width(267), GUILayout.Height(94) ) )
		{
//			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
//				GAITrackerPlugin.sendButtonTracker("uiButton", "actionBack", "ConcBackToTitle");
			Application.LoadLevel("ConcIndex");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
	}

	/*
	 * Function that sets of the delays for Starting Countdown
	 * Called from ConcGameStart.cs initializeCards at the end
	*/
	public IEnumerator GetStartCountDown()
	{
		// Turn off buttons at countdown
		cGameStart.GetShowButtonInfo(false);

		bShowCountdown = true;
		GameCountdownValue = 1;
		yield return new WaitForSeconds(1.0f);
		GameCountdownValue = 2;
		yield return new WaitForSeconds(1.0f);
		GameCountdownValue = 3;
		yield return new WaitForSeconds(1.0f);
		GameCountdownValue = 0;

		// We can click buttons
		cGameStart.GetShowButtonInfo(true);

		yield return new WaitForSeconds(0.10f);
		// Start Hints if we have available
		cGameStart.ShowHints(1);
	}

	/*
	 * Countdown timer at start of a new Game
	 */
	private void GetGUICountDown()
	{
		// Show The CountDown
		if(bShowCountdown)
		{
			cTimer.GetGUICountdown();
		}
	}

	/*
	 * Shows text if you got the Correct Answer or Wrong one
	 * @NOTUSING
	 */
	private void GetGUICorrectOrIncorrect()
	{
		int LabWi = 200;
		int LabHe = 50;

		// GameCorrectOrIncorrect
		if(GameCorrectOrIncorrect == 1)
		{
			//	Debug.Log("Correct!");
			GUI.Label(new Rect(getCenterPos(newWidth, LabWi),
			                   getCenterPos(newHeight, LabHe),
			                   LabWi,
			                   LabHe), "");
		}else if(GameCorrectOrIncorrect == 2)
		{
			//	Debug.Log("Fail, try again");
			GUI.Label(new Rect(getCenterPos(newWidth, LabWi),
			                   getCenterPos(newHeight, LabHe),
			                   LabWi,
			                   LabHe), "");
		}
	}

	/*
	 * Show Message After we have cleared the Game
	 * @NOTUSING
	 */
	private void GetGUIVictory()
	{
		int LabWi = 200;
		int LabHe = 50;
		
		// GameCorrectOrIncorrect
		if(GameVictory == 1)
		{
			//	Debug.Log("Clear!");
			GUI.Label(new Rect(getCenterPos(newWidth, LabWi),
			                   getCenterPos(newHeight, LabHe),
			                   LabWi,
			                   LabHe), "");
		}
	}

	/*
	 * Shows the Quit from Game buttons
	 */
	private void GetGUIQuit()
	{
		if(bShowReallyQuit)
		{
			GUIStyle GameYes;
			GUIStyle GameNo;
			GameYes = 	GUI.skin.GetStyle("GameYes");
			GameNo =	GUI.skin.GetStyle("GameNo");

			//	Invisible layer
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), Textures[3]);

			// Show Hontou ni yameru
			GUI.DrawTexture(rcReallyQuit, Textures[2]);
		
			// Yes Button
			if( GUI.Button(rcYesButton, "", GameYes) )
			{
				// Go back to index
				Application.LoadLevel("ConcIndex");
			}

			// No button
			if( GUI.Button(rcNoButton, "", GameNo) )
			{
				// turn off showing this
				bShowReallyQuit = false;

				// Turn off when we showing really quit menu
				for(int i = 0; i < cGameStart.bShowButton.Length ; i++)
				{
					cGameStart.bShowButton[i] = true;
				}
			}
		}
	}


	/*
	 * Calculate Center Position
	 */
	private float getCenterPos (float screenSize,int TargetSize)
	{
		return (screenSize / 2.0f) - (TargetSize / 2.0f);
	}




	/*
	 * Sounds if we have
	 */
	internal void playCorrectSound(){
		cSound.playCorrectSound();
	}
	
	internal void playWrongSound(){
		cSound.playWrongSound();
	}
	
	internal void playTurnCard(){
		cSound.playTurnCard();
	}
	
	internal void playHint(){
		cSound.playHint();
	}
}
