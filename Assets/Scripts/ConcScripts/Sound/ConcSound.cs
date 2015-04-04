using UnityEngine;
using System.Collections;

public class ConcSound : MonoBehaviour
{
	public AudioClip clipIndex;			// Start Index sound
	public AudioClip clipSelectDif;		// Select Difficu
	public AudioClip clipInGameLoop;	// In game sound
	public AudioClip clipCorrect;		// Correct Sound
	public AudioClip clipWrong;			// Wrong sound
	public AudioClip clipTurnCard;		// Turn Card
	public AudioClip clipHint;			// Hint Cards

	private AudioSource BgmIndex;		// Index BGM Sound
	private AudioSource BgmSelect;		// Select BGM Sound
	private AudioSource BgmGamePlay;	// Game BGM Sound

	void Start ()
	{
		// If Index only load Index AudioSource, if GameStart only load Select and Game music

		if(Application.loadedLevelName == "ConcIndex")
		{
			// Index
			BgmIndex = GameObject.Find("Main Camera").AddComponent<AudioSource>();
			BgmIndex.loop = true;
			BgmIndex.playOnAwake = false;
			BgmIndex.dopplerLevel = 1;
			BgmIndex.volume = 1.0f;
			BgmIndex.priority = 128;
			BgmIndex.pitch = 1.0f;
			BgmIndex.minDistance = 1;
			BgmIndex.maxDistance = 500;
			BgmIndex.panLevel = 1;
			BgmIndex.clip = clipIndex;

			playBGMIndex();
		}
		else if(Application.loadedLevelName == "ConcGameStart")
		{
			// Select Menu
			BgmSelect = GameObject.Find("Main Camera").AddComponent<AudioSource>();
			BgmSelect.loop = true;
			BgmSelect.playOnAwake = false;
			BgmSelect.dopplerLevel = 1;
			BgmSelect.volume = 1.0f;
			BgmSelect.priority = 128;
			BgmSelect.pitch = 1.0f;
			BgmSelect.minDistance = 1;
			BgmSelect.maxDistance = 500;
			BgmSelect.panLevel = 1;
			BgmSelect.clip = clipSelectDif;
			
			// In game music
			BgmGamePlay = GameObject.Find("Main Camera").AddComponent<AudioSource>();
			BgmGamePlay.loop = true;
			BgmGamePlay.playOnAwake = false;
			BgmGamePlay.dopplerLevel = 1;
			BgmGamePlay.volume = 1.0f;
			BgmGamePlay.priority = 128;
			BgmGamePlay.pitch = 1.0f;
			BgmGamePlay.minDistance = 1;
			BgmGamePlay.maxDistance = 500;
			BgmGamePlay.panLevel = 1;
			BgmGamePlay.clip = clipInGameLoop;

			playBGMSelect();
		}
	}
	
	//
	// Play AudioSource
	// 

	internal void playBGMIndex(){
		BgmIndex.clip = clipIndex;
		BgmIndex.pan = 0;
		BgmIndex.Play();
	}

	internal void playBGMSelect(){
		BgmSelect.clip = clipSelectDif;
		BgmSelect.pan = 0;
		BgmSelect.Play();
	}

	internal void playBGMGamePlay(){
		BgmGamePlay.clip = clipInGameLoop;
		BgmGamePlay.pan = 0;
		BgmGamePlay.Play();
	}
	
	//
	// Stop AudioSource
	// 
	
	internal void stopBGMIndex(){
		BgmSelect.Stop();
	}

	internal void stopBGMSelectDif(){
		BgmSelect.Stop();
	}

	internal void stopBGMGamePlay(){
		BgmGamePlay.Stop();
	//	DestroyObject(BgmGamePlay);
	}

	//
	// Game Clips
	// 

	internal void playCorrectSound(){
		audio.PlayOneShot(clipCorrect);
	}

	internal void playWrongSound(){
		audio.PlayOneShot(clipWrong);
	}

	internal void playTurnCard(){
		audio.PlayOneShot(clipTurnCard);
	}

	internal void playHint(){
		audio.PlayOneShot(clipHint);
	}

}
