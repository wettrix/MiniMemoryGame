using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConcGameStart : MonoBehaviour 
{
	// Classes
	private ScreenFit cScreenFit;
	private ConcDifficulty cDifficulty;

	// Skins
	public GUISkin SetSkin;

	// Choose Cards from this
	private string[] CardsToChooseFrom = new string[]
	{
		"BandAid","Battery","Computer","Egg","Flower","Heart","MadSmiley","Magnifier","Peach","Scissors"
	};
	// Where we load cards from
	private string ResourcesLocation = "Concentration/PictureCards/";

	// Cards We will use to play with
	private string[] PlayingCards;									// Stores Card info
	internal bool[] bIsCardCorrect = new bool[6];					// Stores Card if we got matches
	internal List<Texture2D> CardImage = new List<Texture2D>();		// Card Textures
	private List<string[]> CompareTwoCards = new List<string[]>();	// What Cards we will compare for the right awnser

	// How many times we failed to get the correct card
	internal int CurrentFail;

	// --------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Textures
	private Texture2D Shadow;		// Shadow Texture
	private Texture2D CardBack;		// Card Texture
	
	// Card Animation setup
	private bool[] bTurnFirst = new bool[6];		// First card turning
	private bool[] bTurnSecond = new bool[6];		// Second card turning to show result
	private Rect[] rcShadow = new Rect[6];			// Shadow placements
	internal Rect[] rcCard = new Rect[6];			// Card placements
	private bool[] bIsCardVisible = new bool[6]; 	// When flipping shows either train or card, used for flipping forward and back
	
	// GUI placement calculation
	private float startHe;						// 200 px start height
	private float[] cardLeft = new float[2];	// Card Startpoint 80px, 360px
	private float cardTopDif;					// Inbetween Cards
	private float shadowCalc;					// Where to place shadow
	
	private int aspectWidth;
	// --------------------------------------------------------------------------------------------------------------------------------------------------------------
	internal bool[] bShowButton = new bool[6];	// Turn Buttons On & Off

	void Start()
	{
		// Gets GameStart Class
		cDifficulty = GetComponent<ConcDifficulty>();

		// Add Images
		Object textures01 = Resources.Load("Concentration/MainCards/card_shadow", typeof(Texture2D));
		Object textures02 = Resources.Load("Concentration/MainCards/card_back", typeof(Texture2D));
		Shadow = (Texture2D)textures01;
		CardBack = (Texture2D)textures02;

		// Fix aspect for iPad or Android Card placement
		if(cDifficulty.newWidth > 640)
		{
			aspectWidth = (cDifficulty.newWidth - 640) / 2;
		}else
		{
			aspectWidth = 0;
		}
	}
	
	/*
	 * Will Set Cards
	 * @param	CardAmount		How many cards we will setup
	 */
	public void initializeCards(int CardAmount)
	{
		List<string> CardListRand = new List<string>();		// Cards we use to find what train we should choose randomly	int
		List<string> CardsHalfRand = new List<string>();	// Place Selected Cards randomly for placement in PlayingCards	int

		// Random 6 cards according to what we have & The bools for turning cards
		PlayingCards = new string[CardAmount];
		bIsCardCorrect = new bool[CardAmount];
		// Setting up the rectangles for all the Cards
		SetCardRectangles();

		// Adds Cards we can use in the deck so that we can randomize from them
		for(int i = 0; i < CardsToChooseFrom.Length ; i++)
		{
			CardListRand.Add(CardsToChooseFrom[i]);
		}

		// Randomize Cards for half of the deck and add 2 of them to the store CardsHalfRand
		for(int i = 0; i < CardAmount / 2 ; i++)
		{
			// Get randomized Cards
			int thisNumber = Random.Range(0, CardListRand.Count);

			// Add the texture
			Object textures = Resources.Load(ResourcesLocation + CardListRand[thisNumber], typeof(Texture2D));
			CardImage.Add((Texture2D)textures);

			// Add 2 Cards
			CardsHalfRand.Add(CardListRand[thisNumber]);
			CardsHalfRand.Add(CardListRand[thisNumber]);

			// Remove Cards so we don't find it again
			CardListRand.RemoveAt(thisNumber);
		}
		
		// Randomly Place the Cards in the main deck
		for(int i = 0; i < PlayingCards.Length ; i++)
		{
			int thisNumber = Random.Range(0, CardsHalfRand.Count);
			PlayingCards[i] = CardsHalfRand[thisNumber];
			CardsHalfRand.RemoveAt(thisNumber);
		}

		// Start off by setting all the buttons as active
		for(int i = 0; i < bShowButton.Length ; i++)
		{
			bShowButton[i] = true;
		}

		// Remove extra true function if comparison doesn't have time to remove it self
		if(CompareTwoCards.Count > 0)
		{
			CompareTwoCards.RemoveRange(0,CompareTwoCards.Count);
		}

		// Reset animatin so they don't lock if you click too much
		for(int i = 0; i < bIsCardCorrect.Length ; i++)
		{
			bIsCardCorrect[i] = false;
		}
		
		// Setting Card Fails to 0 at start (reset for more games)
		CurrentFail = 0;

		// Set Next Game State
		cDifficulty.CurrentState = GameState.PlayGame;
		StartCoroutine( cDifficulty.GetStartCountDown() );
	}

	/*
	 * Setup Where we Place Cards and shadows
	*/
	private void SetCardRectangles()
	{
		bTurnFirst = new bool[6];
		bTurnSecond = new bool[6];
		rcShadow = new Rect[6];
		rcCard = new Rect[6];
		bIsCardVisible = new bool[6];

		// Card setup settings for placement on screen
		cardLeft[0] = 80;
		cardLeft[1] = 360;
		cardTopDif = 250;	// before 220
		startHe = 125;		// before 200
		shadowCalc = 7;
		
		// Setting Rectangles after rows
		for(int i = 0; i < 6; i+=2)
		{
			for(int ii = 0; ii < 2; ii++)
			{
				rcCard[i+ii] 	= new Rect(aspectWidth + cardLeft[ii],   			startHe+(cardTopDif * i / 2),				CardBack.width,  CardBack.height);
				rcShadow[i+ii]  = new Rect(aspectWidth + cardLeft[ii]+shadowCalc,	startHe+(cardTopDif * i / 2)+shadowCalc,	Shadow.width,	Shadow.height);
			}
		}
	}

	/*
	 * Will show Flipping Card animation, Old, didn't change
	 */
	void FixedUpdate ()
	{
		for(int i = 0; i < bIsCardCorrect.Length; i++)
		{
			if(bIsCardCorrect[i])
			{
				if(!bIsItHint)
				{
					rcCard[i] = CardAnimation(rcCard[i], false, i);
					rcShadow[i] = CardAnimation(rcShadow[i], true, i);
				}
			}
		}
	}

	/*
	 * Shows Hint animation
	 */
	void Update()
	{
		//	TurnFunction();
		for(int i = 0; i < bIsCardCorrect.Length; i++)
		{
			if(bIsCardCorrect[i])
			{
				if(bIsItHint && !bFlipDone && !bIfMatch[i])
				{
					rcCard[i] = HintAnimation(rcCard[i], false, i);
					rcShadow[i] = HintAnimation(rcShadow[i], true, i);
				}
			}
		}
	}
	
	private int[] iFlipCount = new int[6];			// Card flipping calculation
	private int[] iFlipCountShadow = new int[6];	// Shadow Calculation
	private bool[] bIfMatch = new bool[6];			// set so that we can only update if it is not a match
	private bool bIsItHint;		// Hint mode on
	private bool bFlipDone;		// Check to see if Hint flip is done
	private Rect HintAnimation(Rect FlipCard, bool bHasShadow, int ImgIndex)
	{

		if(!bFlipDone);
		{
			if(!bHasShadow)
				++iFlipCount[ImgIndex];
			else
				++iFlipCountShadow[ImgIndex];

			if (iFlipCount[ImgIndex] <= 1 || iFlipCountShadow[ImgIndex] <= 1)
			{
			//	Time.timeScale = 1;
				// Sound play
			//	Camera.main.GetComponent<PlaySE>().playFanfare();
			}
			else if(iFlipCount[ImgIndex] <= 21 || iFlipCountShadow[ImgIndex] <= 21)	// Goes to show Card
			{
				// Move Card
				if(!bHasShadow)
				{
					FlipCard.x += 5;
					FlipCard.width -= 10;
					FlipCard.y -= 2;
				}
				// Move Shadow
				else
				{
					FlipCard.x += 5;
					FlipCard.width -= 10;
				}

				if(FlipCard.width == 0)
					bIsCardVisible[ImgIndex] = true;
			}
			
			else if(iFlipCount[ImgIndex] <= 41 || iFlipCountShadow[ImgIndex] <= 41) // Goes to Show Card full
			{
				// Move Card
				if(!bHasShadow)
				{
					FlipCard.x -= 5;
					FlipCard.width += 10;
					FlipCard.y -= 2;
				}
				// Move Shadow
				else
				{
					FlipCard.x -= 5;
					FlipCard.width += 10;
				}
				
			}
			else if(iFlipCount[ImgIndex] <= 61 || iFlipCountShadow[ImgIndex] <= 61) // Goes to Show Card
			{
				// Move Card
				if(!bHasShadow)
				{
					FlipCard.x += 5;
					FlipCard.width -= 10;
					FlipCard.y += 2;
				}
				// Move Shadow
				else
				{
					FlipCard.x += 5;
					FlipCard.width-= 10;
				}

				if(FlipCard.width == 0)
					bIsCardVisible[ImgIndex] = false;
			}
			else if(iFlipCount[ImgIndex] <= 81 || iFlipCountShadow[ImgIndex] <= 81) // Goes to Show Card full
			{
								// Move Card
				if(!bHasShadow)
				{
					FlipCard.x -= 5;
					FlipCard.width += 10;
					FlipCard.y += 2;
				}
				// Move Shadow
				else
				{
					FlipCard.x -= 5;
					FlipCard.width+= 10;
				}
			}
			else if (iFlipCount[ImgIndex] >= 82 || iFlipCountShadow[ImgIndex] <= 82)	
			{
//				Debug.Log("AnimTimer= " + ImgIndex);
			}else if(iFlipCount[ImgIndex] >= 85 || iFlipCountShadow[ImgIndex] <= 85)
			{
				// both reached so turn bool off, and later in Hint function we set all coutners to 0
				bFlipDone = true;
			}

		}
		return FlipCard;
	}

	/*
	 * Animates Flipping Cards to show Image
	 * @param	ThisCard		What Rectangle Card
	 * @param	bHasShadow		Is it a shadow
	 * @param	ImgIndex		Where in the for loop is it
	 * @return					returns the Animation process of the rectangle
	*/
	private Rect CardAnimation(Rect ThisCard, bool bHasShadow, int ImgIndex)
	{
		float SpeedValue = 800; // 800
		// Flip card first
		if(bTurnFirst[ImgIndex])
		{
			if(ThisCard.width > 0)
			{
				if(!bHasShadow)
				{
					// no shadow move it up also
					ThisCard.width-= Time.deltaTime * SpeedValue;
					ThisCard.x+= Time.deltaTime * SpeedValue/2;
					ThisCard.y-= Time.deltaTime * SpeedValue/4;
				}
				else
				{
					// shadow just move sideways
					ThisCard.width-= Time.deltaTime * SpeedValue;
					ThisCard.x+= Time.deltaTime * SpeedValue/2;
				}
			}
			else
			{
				// First flip doesn't show the train Label, Second time it does show the Card Label
				if(!bIsCardVisible[ImgIndex])
					bIsCardVisible[ImgIndex] = true;
				else
					bIsCardVisible[ImgIndex] = false;
				bTurnFirst[ImgIndex] = false;
				bTurnSecond[ImgIndex] = true;
			//	Debug.Log("First Flip");
			}
		}
		// Flip Card Again
		if(bTurnSecond[ImgIndex])
		{
			if(ThisCard.width < 200)
			{
				if(!bHasShadow)
				{
					// no shadow move it up also
					ThisCard.width+= Time.deltaTime * SpeedValue;
					ThisCard.x-= Time.deltaTime * SpeedValue/2;
					ThisCard.y+= Time.deltaTime * SpeedValue/4;
				}
				else
				{
					// shadow just move sideways
					ThisCard.width+= Time.deltaTime * SpeedValue;
					ThisCard.x-= Time.deltaTime * SpeedValue/2;
				}
			}else
			{
				// Second time we come here we will switch on Card Button, a reset
				if(!bIsCardVisible[ImgIndex])
					bIsCardCorrect[ImgIndex] = false;
				
				bTurnSecond[ImgIndex] = false;
			//	Debug.Log("Second Flip");
			}
		}
		
		return ThisCard;
	}
	
	/*
	 * Will Display the menu
	 */
	public void GetGUICardPlacement()
	{
		GUIStyle GameQuit;
		GameQuit = GUI.skin.GetStyle("GameQuit");

		// Show Cards
		for(int i = 0; i < 6 ; i++)
		{
			GetGUIButton(i);
		}

		GUILayout.FlexibleSpace();
		// Go Back to Title Button
		GUILayout.BeginHorizontal("Box");
		GUILayout.FlexibleSpace();
		if(bShowButton[0] &&  GUILayout.Button("", GameQuit, GUILayout.Width(180), GUILayout.Height(100) ) )
		{
			// When Pressed show
			cDifficulty.bShowReallyQuit = true;

			// Turn off when we showing really quit menu
			for(int i = 0; i < bShowButton.Length ; i++)
			{
				bShowButton[i] = false;
			}
		}else
		{
			// Show button not pressable at start
			if(!bShowButton[0])
				GUILayout.Label( "", GameQuit, GUILayout.Width(180), GUILayout.Height(100) );
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
	}

	/*
	 * GUI Layour Card button placement & functions
	 * @param	index	what index we are using
	 */
	private void GetGUIButton(int index)
	{
		for(int ii = 0; ii < CardImage.Count ; ii++)
		{
			// If not then it's no animation just a normal button
			if(!bIsCardCorrect[index])
			{
				GUI.DrawTexture(rcShadow[index], Shadow);
				if(bShowButton[index] && GUI.Button( rcCard[index],	CardBack ) )
				{
					// Play Correct Sound
					cDifficulty.playTurnCard();

					string[] AddCard = new string[2];	// Slot for What Card & Where it is		// int
					
					AddCard[0] = PlayingCards[index];	// What card
					AddCard[1] = index.ToString();		// where the card is

					// Starts animation
					bTurnFirst[index] = true;
					// Shows Card Visible
					bIsCardCorrect[index] = true;
					
					// Add The card
					CompareTwoCards.Add(AddCard);
					
					// When we have 2 cards in here do the comparison
					if(CompareTwoCards.Count == 2)
					{
						StartCoroutine(CompareCards(CompareTwoCards));
					}
					// ----------------------
				}else
				{
					GUI.DrawTexture(rcCard[index], CardBack);
				}
				break; // Why?
			}
			// After Pressed show the animation label
			else
			{
				// Show Back Side of the Card
				if(!bIsCardVisible[index])
				{
					GUI.DrawTexture(rcShadow[index], Shadow);
					GUI.DrawTexture(rcCard[index], CardBack);
				}
				// Show Front Side (picture) of the Card
				else
				{
					if(PlayingCards[index] == CardImage[ii].name)
					{
						GUI.DrawTexture(rcShadow[index], Shadow);
						GUI.DrawTexture(rcCard[index], CardImage[ii]);
					}
				}
			}
		}
	}

	/*
	 * Used to turn of buttons when doing CompareCards fucntion
	 * @param	value	what we set them to
	 */
	public void GetShowButtonInfo(bool value)
	{
		for(int i = 0; i < bShowButton.Length ;i++)
		{
			bShowButton[i] = value;
		}
	}

	/*
	 * Comapares 2 cards when they are both up
	 * @param	CompareList		What Cards we will compare
	 */
	private IEnumerator CompareCards(List<string[]> CompareList)
	{
		// Press buttons so it flips, then check if we have 1 or 2 cards open, if 2 cards are in the array then run this
		if(CompareList[0][0] == CompareList[1][0])
		{
			// Set buttons off at start so we can't use them during comparing period
			GetShowButtonInfo(false);

			// Show message
			cDifficulty.bShowPuzzlePair = true;

			// paus for animation to finish playing and then we continue
			yield return new WaitForSeconds(1.0f);
			// Play Correct Sound
			cDifficulty.playCorrectSound();
			yield return new WaitForSeconds(2.7f);

			// Return button status
			GetShowButtonInfo(true);

			// Shows Card Visible forever
			bIsCardCorrect[int.Parse(CompareList[0][1])] = true;
			bIsCardCorrect[int.Parse(CompareList[1][1])] = true;

			// Add so that we never can be updated again
			bIfMatch[int.Parse(CompareList[0][1])] = true;
			bIfMatch[int.Parse(CompareList[1][1])] = true;

			// Remove Cards
			CompareList.RemoveRange(0, CompareList.Count);

			// If all cards are true then we can exit the game
			if(CompareAllCards(bIsCardCorrect,true))
			{
				StartCoroutine(GetVictoryTimer());
			}
		}
		else
		{
			// Set buttons off at start so we can't use them during comparing period
			GetShowButtonInfo(false);
			
			// Show message
			StartCoroutine(GetCorrectOrIncorrectTimer(false));
			yield return new WaitForSeconds(1.0f);	//1.5
			// Play Wrong Sound
			cDifficulty.playWrongSound();
			yield return new WaitForSeconds(1.0f);	//1.5

			// Add a Fail to the game
			CurrentFail++;

			// Turn Cards Back to Hiding
			bTurnFirst[int.Parse(CompareList[0][1])] = true;
			bTurnSecond[int.Parse(CompareList[0][1])] = false;

			bTurnFirst[int.Parse(CompareList[1][1])] = true;
			bTurnSecond[int.Parse(CompareList[1][1])] = false;

			// Remove Cards
			CompareList.RemoveRange(0, CompareList.Count);

			yield return new WaitForSeconds(2.5f);
			// Return button status
			GetShowButtonInfo(true);
			ShowHints(CurrentFail);
		}
	}

	/*
	 * Shows Victory time and then resets the game
	 */
	IEnumerator GetVictoryTimer()
	{
		// Show Victory Message
		cDifficulty.bShowPuzzleClear = true;
		yield return new WaitForSeconds(3.0f);

		// Reset game information
		ResetGameInformation();

		// Switch state
		cDifficulty.CurrentState = GameState.ChooseDifficulty;
	}

	/*
	 * Timer for showing messages in CompareCards
	 * @param	bIsItEven	If even tell it to use GetGUICorrectOrIncorrect in ConcDifficulty.cs
	*/
	IEnumerator GetCorrectOrIncorrectTimer(bool bIsItEven)
	{
		if(bIsItEven)
		{
			// display correct train
			cDifficulty.GameCorrectOrIncorrect = 1;
			yield return new WaitForSeconds(1.5f);
			// Turn Off
			cDifficulty.GameCorrectOrIncorrect = 0;
		}
		else
		{
			// error message
			cDifficulty.GameCorrectOrIncorrect = 2;
			yield return new WaitForSeconds(1.5f);
			// Turn Off
			cDifficulty.GameCorrectOrIncorrect = 0;
		}
	}

	/*
	 * Compares all the bools inside this array and if they are true
	 * @param	BoolArray	What array to check
	 * @param	BoolValue	What value we want to compare it as
	 * @return				return value
	 */
	public bool CompareAllCards(bool[] BoolArray, bool BoolValue)
	{
		bool Answer = true;
		for(int i = 0 ; i < BoolArray.Length ; i++)
		{
			if(BoolArray[i] != BoolValue)
			{
				Answer = false;
				break;
			}
		}
		return Answer;
	}

	/*
	 * Will Display Hint animation when we Fail
	 * @param	hintAmount	How many hints
	 */
	public void ShowHints(int hintAmount)
	{
		List<int> OpenHintSpots = new List<int>();

		// get all the cards that are not completed and add to list
		for(int i = 0; i < bIsCardCorrect.Length ; i++)
		{
			if(!bIsCardCorrect[i])
				OpenHintSpots.Add(i);
		}
		// How many free slots to see how many hints we can display
		int CardsAvailable = OpenHintSpots.Count;

		// if easy
		if(cDifficulty.CurrentDifficulty == DifficultyState.easy)
		{
			if(hintAmount == 1)
			{
			//	Debug.Log("1st = Show 2 cards");
				cDifficulty.bShowFading = true;	// turn on fading animation
				StartCoroutine(GetRandomHint(2));
			}else if (hintAmount == 2)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(4));
				}else if(CardsAvailable == 4)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(2));
				}
			}else if (hintAmount >= 3)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(4));
				}else if(CardsAvailable == 4)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(2));
				}else if(CardsAvailable == 2)
				{
					// no hints
				}
			}
		}
		// if medium
		else if(cDifficulty.CurrentDifficulty == DifficultyState.medium)
		{
			if(hintAmount == 1)
			{
			//	Debug.Log("1st = Show Nothing!");
			}else if (hintAmount == 2)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(3));
				}else if(CardsAvailable == 4)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(1));
				}
			}else if (hintAmount >= 3)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(2));
				}else if(CardsAvailable == 4)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(1));
				}else if(CardsAvailable == 2)
				{
					// no hints
				}
			}
		}
		// if hard
		else if(cDifficulty.CurrentDifficulty == DifficultyState.hard)
		{
			if(hintAmount == 1)
			{
			//	Debug.Log("1st = Show Nothing!");
			}else if (hintAmount == 2)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(2));
				}
			}else if (hintAmount >= 3)
			{
				if(CardsAvailable == 6)
				{
					cDifficulty.bShowFading = true;
					StartCoroutine(GetRandomHint(2));
				}else if(CardsAvailable == 4)
				{
					// no hints
				}else if(CardsAvailable == 2)
				{
					// no hints
				}
			}
		}
		// -----
	}

	/*
	 * Shows how many Hints depending on how many Cards we have left
	 * @param	HintAmount	How many hints
	 */
	private IEnumerator GetRandomHint(int HintAmount)
	{
		List<int> OpenHintSpots = new List<int>();	// what to random
		List<int> HintIndexList = new List<int>();	// what to hint at

		// Play Hint Sound
		cDifficulty.playHint();

		// get all the cards that are not completed and add to list
		for(int i = 0; i < bIsCardCorrect.Length ; i++)
		{
			// if false = not completed, show hint
			if(!bIsCardCorrect[i])
				OpenHintSpots.Add(i);
		}

		// Random Select numbers for Hints
		for(int i = 0; i < HintAmount ; i++)
		{
			int thisIndex = Random.Range(0, OpenHintSpots.Count);
			HintIndexList.Add(OpenHintSpots[thisIndex]);
			OpenHintSpots.RemoveAt(thisIndex);
		}

		// Set buttons off at start so we can't use them during comparing period
		GetShowButtonInfo(false);

		// Set Other Animations On
		bIsItHint = true;
		bFlipDone = false;

		// Show hint
		for(int i = 0; i < HintIndexList.Count ; i++)
			bIsCardCorrect[HintIndexList[i]] = true;

		// Cooldown for Fading Animation & Hints
		yield return new WaitForSeconds(3.5f);

		// Return deck to normal
		for(int i = 0; i < HintIndexList.Count ; i++)
			bIsCardCorrect[HintIndexList[i]] = false;

		// Reset Animation counters
		for(int i = 0; i < iFlipCount.Length ; i++)
		{
			iFlipCount[i] = 0;
			iFlipCountShadow[i] = 0;
		}

		// Reset Other Animation settings
		bIsItHint = false;
		bFlipDone = true;

		// Return button status
		GetShowButtonInfo(true);

		// Removes hints
		OpenHintSpots.RemoveRange(0, OpenHintSpots.Count);
		HintIndexList.RemoveRange(0, HintIndexList.Count);
	}



	/*
	 * Resets the basic Game Information so that we can start a new challenge
	 */
	public void ResetGameInformation()
	{
		// Removes Cards From Deck
		CardImage.RemoveRange(0, CardImage.Count);

		iFlipCount = null;			// Card flipping calculation
		iFlipCountShadow = null;	// Shadow Calculation
		bIfMatch = null;			// set so that we can only update if it is not a match

		// StartCountdown value is set
		cDifficulty.GameCountdownValue = 1;
		// Turn off Victory message
		cDifficulty.GameVictory = 0;
	}
}
