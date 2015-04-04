using UnityEngine;
using System.Collections;

public class ScreenFit : MonoBehaviour
{
	private float screenScaleWH;
	public float screenDpiWidth, screenDpiHeight;
	private float screenW, screenH;
	public int width, height;
	
	public void init () 
	{
		float w, h;
	//	float wScale, hScale;
		float aspectW, aspectH;
		
		aspectW = (float)Screen.height / (float)Screen.width;
		aspectH = (float)Screen.width / (float)Screen.height;
		
		w = 640.0f / Screen.width;
		h = 960.0f / Screen.height;
		if (aspectW < 1.5f) {
			screenW = h * (float)Screen.width;
			screenH = h * (float)Screen.height;
			screenDpiWidth = (float)Screen.width / screenW;
			screenDpiHeight = (float)Screen.height / screenH;
			//width = (int)(w * (float)Screen.width);
			width = (int)screenW;
			height = (int)screenH;
			//	Debug.Log ("screenWH = " + screenW + "x" + screenH + " " + width + "x" + height);
			//	Debug.Log("01 - iPad");
		}else {
			screenW = w * (float)Screen.width;
			screenH = w * (float)Screen.height;
			screenDpiWidth = (float)Screen.width / screenW;
			screenDpiHeight = (float)Screen.height / screenH;
			//screenDpiWidth = screenW / (float)Screen.width;
			//screenDpiHeight = screenH / (float)Screen.height;
			width = (int)screenW;
			height = (int)screenH;
			//	Debug.Log ("screenWH = " + screenW + "x" + screenH + " " + width + "x" + height);
			//	Debug.Log("01 - iPhone");
		}
		
		//Debug.Log ("Screen " + Screen.width + "x" + Screen.height + " Aspect " + aspectW + "x" + aspectH + " size " + screenDpiWidth + "x" + screenDpiHeight);
	}
	
	public void setScaledScreen()
	{
		Matrix4x4 scaledMatrix = Matrix4x4.Scale(new Vector3(screenDpiWidth, screenDpiHeight, 1));
		GUI.matrix = scaledMatrix;
	}
}
