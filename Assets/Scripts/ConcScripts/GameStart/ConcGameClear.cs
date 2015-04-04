using UnityEngine;
using System.Collections;

public class ConcGameClear : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}

	public void GetGUIGameCleared(int failedAmount)
	{
		if(failedAmount < 2)
		{
			Debug.Log("Sugoi!");
		}else if(failedAmount > 2 && failedAmount < 4)
		{
			Debug.Log("Ganbatta");
		}else if(failedAmount > 4)
		{
			Debug.Log("yatta ne");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
