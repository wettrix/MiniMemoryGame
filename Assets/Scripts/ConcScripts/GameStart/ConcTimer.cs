using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConcTimer : MonoBehaviour 
{
	// Classes
	private ConcDifficulty cDifficulty;
	
	// Get Texture
	private Texture2D[] Textures = new Texture2D[6];

	// Where we load Images from
	private string ResourcesLocation = "Concentration/Game/";
	// Image names
	private string[] TextureNames = new string[6]{"Count01","Count02","Count03","WinMessage01","WinMessage02", "fade"};

	// Rectangles
	private Rect rcPuzzlePair;
	private Rect rcPuzzleClear;
	private Rect rcCountDown;
	private Rect rcFade;

	// Animation
	private int animationCounter = 0;
	private int animationFadeCounter = 0;
	private float alpha = 0;
	private float FadeAlpha = 0;
	private int countNum = 0;

	void Start()
	{
		
		Application.targetFrameRate = 60;
		// Gets GamessStart Class
		cDifficulty = GetComponent<ConcDifficulty>();

		// Add Images
		for(int i = 0; i < TextureNames.Length; i++)
		{
			Object textures = Resources.Load(ResourcesLocation + TextureNames[i], typeof(Texture2D));
			Textures[i] = (Texture2D)textures;
		}

		// Rectangle setups
		rcCountDown = new Rect( getCenterPos(cDifficulty.newWidth,256),  getCenterPos(cDifficulty.newHeight,256),  256, 256);	// height was 320.0f
		rcPuzzlePair = new Rect( getCenterPos(cDifficulty.newWidth,640),  -240,  640, 240);	// height was 200.0f
		rcPuzzleClear = new Rect( getCenterPos(cDifficulty.newWidth,640),  250,  640, 240);
		rcFade = new Rect(0, 0, cDifficulty.newWidth, cDifficulty.newHeight);

		// Animation values
		animationCounter = 0;	//counter that is used for update to animate
		animationFadeCounter = 0;
		FadeAlpha = 0;
		alpha = 0;				// alpha we use on pictures
		countNum = 3;			// Countdown order number
	}

	void Update()
	{
		if(cDifficulty.bShowCountdown)
			PlayCountdown();
		if(cDifficulty.bShowPuzzlePair)
			PlayConcPair();
		if(cDifficulty.bShowPuzzleClear)
			PlayConcClear();
		if(cDifficulty.bShowFading)
			PlayFading();
	}

	/*
	 * Play fading for Hints
	 */
	private void PlayFading()
	{
	//	2.6f i speed, 1.5f + 0.55f + 0.55f	* 60 == 156
		++animationFadeCounter;
		if (animationFadeCounter <= 15)
		{
		//	rcFade
			FadeAlpha += 0.02f;
		}
		else if (animationFadeCounter <= 182)  // flame [10]　countNum [11]
		{
		}
		else if (animationFadeCounter <= 197)  // flame [10]　countNum [11]
		{
			FadeAlpha -= 0.02f;
		}
		else if (animationFadeCounter <= 198)  // flame [10]　countNum [11]
		{
			FadeAlpha = 0;
			animationFadeCounter = 0;
			cDifficulty.bShowFading = false;
		}
	}

	/*
	 * Play Start Countdown
	 */
	private void PlayCountdown()
	{   
		++animationCounter;
		if (animationCounter <= 5)
		{
			rcCountDown.x += 5f;
			rcCountDown.width -= 10f;
			rcCountDown.height -= 10f;
		}
		else if (animationCounter <= 10)  // flame [10]　countNum [11]
		{
			rcCountDown.x -= 5f;
			rcCountDown.width += 10f;
			rcCountDown.height += 10f;
			rcCountDown.y += 10f;			// <-
			alpha += 0.2f;
		}
		else if (animationCounter <= 49) {             }
		else if (animationCounter == 50) {  
			alpha = 0;
			rcCountDown.y = getCenterPos(cDifficulty.newHeight,256);			// <-  320.0f
			--countNum;
		}
		else if (animationCounter <= 55)
		{
			rcCountDown.x += 5f;
			rcCountDown.width -= 10f;
			rcCountDown.height -= 10f;
		}
		else if (animationCounter <= 60)
		{
			rcCountDown.x -= 5f;
			rcCountDown.width += 10f;
			rcCountDown.height += 10f;
			rcCountDown.y += 10f;			// <-
			alpha += 0.2f;
		}
		else if (animationCounter <= 119) {           }
		else if (animationCounter == 120)
		{
			alpha = 0;
			--countNum;					// <-
			rcCountDown.y = getCenterPos(cDifficulty.newHeight,256);	// 320
			
		}
		else if (animationCounter <= 125)
		{
			rcCountDown.x += 5f;
			rcCountDown.width -= 10f;
			rcCountDown.height -= 10f;
		}
		else if (animationCounter <= 130)
		{
			rcCountDown.x -= 5f;
			rcCountDown.width += 10f;
			rcCountDown.height += 10f;
			rcCountDown.y += 10f;			// <-
			alpha += 0.2f;
		}
		else if(animationCounter <= 170)
		{
			
		}
		else if(animationCounter <= 190)
		{
			alpha -= 0.08f;
		//	alphaSub -= 0.05f;
		}
		else
		{
			animationCounter = 0;
			alpha = 0f;
			// Start Event
			cDifficulty.bShowCountdown = false;
			
			rcCountDown = new Rect( getCenterPos(cDifficulty.newWidth,256),  getCenterPos(cDifficulty.newHeight,256),  256.0f, 256.0f);
		}
	}

	/*
	 * Plays animation for Getting a Pair
	 */
	private void PlayConcPair()
	{
		++animationCounter;
		if (animationCounter <= 1)
		{
			alpha = 1f;
			Time.timeScale = 1;
			// Sound play
		}
		else if(animationCounter <= 60)
		{

		}
		else if (animationCounter <= 74)		// 14	Starts by appearing and going down
		{
			// Sets alpha going to max 1.0f
			if(alpha == 1.0f){}
			else
				alpha += 0.1f;
			
			rcPuzzlePair.y += 15f * 3;		// 15
		}
		else if (animationCounter <= 80)			// 20	Comes to a stop and goes up
			rcPuzzlePair.y -= 4f;
		else if (animationCounter <= 87)			// 27	Goes down again
			rcPuzzlePair.y += 3f;
		else if (animationCounter <= 185) { }		// 45	Stand still
		else if (animationCounter <= 210)			// 70	exspands and looses alpha
		{ // 640 240
			rcPuzzlePair.x -= 15f;
			rcPuzzlePair.y -= 10f;
			rcPuzzlePair.width += 30f;
			rcPuzzlePair.height += 20f;
			alpha -= 0.08f;
		}
		else if (animationCounter >= 225)			// 85	dissapears and resets information
		{
//			Debug.Log("AnimTimer Over");
			animationCounter = 0;
			alpha = 0;
			// set bool off
			cDifficulty.bShowPuzzlePair = false;
			// reset size again
			rcPuzzlePair = new Rect( getCenterPos(cDifficulty.newWidth,640),  -240.0f,  640.0f, 240.0f);
		}
	}

	/*
	 * Play Animation for Clearing the game
	 */
	private void PlayConcClear()
	{
		++animationCounter;

		// starts
		if (animationCounter <= 0) {
		//	rcPuzzleClear.y = 600f;
		}
		else if (animationCounter <= 25)
		{
			rcPuzzleClear.x -= 15f;
			rcPuzzleClear.y -= 7.5f;
			rcPuzzleClear.width += 30f;
			rcPuzzleClear.height += 15f;
		}
		else if (animationCounter <= 40)
		{
			rcPuzzleClear.x += 30;
			rcPuzzleClear.y += 15f;
			rcPuzzleClear.width -= 60f;
			rcPuzzleClear.height -= 30f;
		}
		else if (animationCounter <= 45)
		{
			rcPuzzleClear.x -= 10f;
			rcPuzzleClear.y -= 5f;
			rcPuzzleClear.width += 20f;
			rcPuzzleClear.height += 10f;
		}
		else if (animationCounter <= 46){}
		else if (animationCounter <= 180){}	// small break
		else if (animationCounter <= 181)
		{
			// When done reset Start information
			animationCounter = 0;
		//	alphaSub = 1;
			alpha = 0f;
			countNum = 3;
			
			// bool off to hide it
			cDifficulty.bShowPuzzleClear = false;
			rcPuzzleClear = new Rect( getCenterPos(cDifficulty.newWidth,640),  250,  640.0f, 240.0f);
		}
	}

	/*
	 * Used in ConcDifficulty to show Countdown
	 */
	internal void GetGUIFadingTimer()
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, FadeAlpha);
		GUI.DrawTexture(rcFade, Textures[5]);
	}

	/*
	 * Used in ConcDifficulty to show Countdown
	 */
	internal void GetGUICountdown()
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		if (countNum == 3)
			GUI.DrawTexture(rcCountDown, Textures[2]);
		else if (countNum == 2)
			GUI.DrawTexture(rcCountDown, Textures[1]);	// <-
		else if (countNum == 1)
			GUI.DrawTexture(rcCountDown, Textures[0]);	// <-
	}

	/*
	 * Used in ConcDifficulty to show Puzzle Pairs found
	 */
	internal void GetGUIConcPair()
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.DrawTexture(rcPuzzlePair, Textures[3]);
	}

	/*
	 * Used in ConcDifficulty to show Puzzle is cleared
	 */
	internal void GetGUIConcClear()
	{
		GUI.DrawTexture(rcPuzzleClear, Textures[4]);
	}


	private float getCenterPos (float screenSize,int TargetSize)
	{
		return (screenSize / 2.0f) - (TargetSize / 2.0f);
	}
}
