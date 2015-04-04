using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class ConcIndex : MonoBehaviour
{
	// Classes
	private ScreenFit cScreenFit;
	private ConcSound cSound;

	// Skins
	public GUISkin SetSkin;
	
	// Get Texture
	private Texture2D[] Textures = new Texture2D[1];
	// Where we load Images from
	private string ResourcesLocation = "Concentration/Index/";

	// rectangles
	private Rect rcBackground;	// Background Image
	private Rect rcDrawArea;	// where we draw GUILayout
	private Rect rcStartButton;
	private Rect rcEndButton;

	// Screen Settings
	private int newWidth;		// New Aspect screen width
	private int newHeight;		// New Aspect screen height

	// Animations
	private float anim = 0.0f;
	private int animCount = 0;
	private bool animFlag = false;
	private float sinTheta = 0;
	private float animSpeed = 0.2f;
	private float animSize = 6.0f;

	// Use this for initialization
	void Start ()
	{
		// Set FPS
		Application.targetFrameRate = 30;
		// Gets ScreenFit Class
		cScreenFit = GetComponent<ScreenFit>();
		// Initialize Screen Settings
		cScreenFit.init();
		cSound = GetComponent<ConcSound>();

		// Add Images
		Object textures = Resources.Load(ResourcesLocation + "BackIndex", typeof(Texture2D));
		Textures[0] = (Texture2D)textures;

		// Setting Screen sizes
		newWidth = cScreenFit.width;
		newHeight = cScreenFit.height;

		// Rectangle Images
		rcBackground	= new Rect(0, 0, Screen.width, Screen.height);
		rcDrawArea		= new Rect(0, 0, newWidth,	   newHeight);
		rcStartButton	= new Rect(getCenterPos(newWidth, 383) - (anim * .5f),	620.0f - (anim * .5f), 383.0f + anim, 93.0f + anim);
		rcEndButton		= new Rect(getCenterPos(newWidth, 383), 760.0f, 383.0f, 93.0f);

		// Remove previous memory when entering here
		Resources.UnloadUnusedAssets();
	}

	// Will Display the menu
	void OnGUI()
	{
		if(SetSkin)
			GUI.skin = SetSkin;
		
		GUIStyle IndexStart;
		GUIStyle IndexQuit;
		IndexStart = GUI.skin.GetStyle("IndexStart");
		IndexQuit = GUI.skin.GetStyle("IndexQuit");
		
		// Background picture
		GUI.DrawTexture(rcBackground, Textures[0]);

		// Set Aspect Ratio
		cScreenFit.setScaledScreen();

		GUILayout.BeginArea(rcDrawArea);	// Start Drawing Area

		GUILayout.EndArea();	// End DrawingArea
		
		// Start Button animation, in order to not create more unnecessary rects.
		rcStartButton.x = getCenterPos(newWidth, 383) - (anim * .5f);
		rcStartButton.y = 620.0f - (anim * .5f);
		rcStartButton.width = 383.0f + anim;
		rcStartButton.height = 93.0f + anim;

		// Game Start Button		// 80									480
		if( GUI.Button(rcStartButton, "", IndexStart) )
		{
			Application.LoadLevel("ConcGameStart");
		}

		// Game End Button
		if( GUI.Button(rcEndButton, "", IndexQuit) )
		{
			Application.Quit();
		}

		Animation();
	}

	private void Animation()
	{
		sinTheta += animSpeed;
		++animCount;
		float temp = animSize * (Mathf.Sin(sinTheta) + 1f);
		if (animCount >= 61)
		{
			animCount = 0;
			animFlag = !animFlag;
			sinTheta = 0;
		}
		if (animFlag)
			anim = temp;
	}

	private float getCenterPos (float screenSize,int TargetSize)
	{
		return (screenSize / 2.0f) - (TargetSize / 2.0f);
	}
}
