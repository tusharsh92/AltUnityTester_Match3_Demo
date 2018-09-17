using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Holoville.HOTween;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
		public GameObject _Logo;//Your Game Logo
		public GameObject _PlayButton;//The Play Button
		public GameObject _BestScore;//The Best Score
	    public GameObject _BestLevel;//The  Best Level
		public string _NextScene;//Next scene to load
		public AudioClip MenuSound;//The Sound Played when you click on the play button

		void Awake ()
		{
				Time.timeScale = 1; //Setting the timescale to the standard value of 1
		}

		void Start ()
		{

				AnimateLogo ();
            
				(_BestScore.GetComponent (typeof(TextMesh))as TextMesh).text = "" + PlayerPrefs.GetInt ("HighScore");
				(_BestLevel.GetComponent (typeof(TextMesh))as TextMesh).text = "" + PlayerPrefs.GetInt ("HighLevel");

		}


		
		// Update is called once per frame
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Escape)) {
						Application.Quit ();
				}
     
        //Detecting if the player clicked on the left mouse button and also if there is no animation playing
        //		if (UnityEngine.Input.GetButtonDown ("Fire1")) {
         if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began) {
            //The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
          //  RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (UnityEngine.Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.transform != null) {
								if ((hit.transform.gameObject.name == _PlayButton.name)) {
											GetComponent<AudioSource>().PlayOneShot (MenuSound);   
										hit.transform.localScale = new Vector3 (0.7f, 0.7f, 0);
                    //	Application.LoadLevel (_NextScene);
                    SceneManager.LoadScene(_NextScene);
										Time.timeScale = 1;
								}
			
						}
				}
		}

	//Animating the logo in loop
		void AnimateLogo ()
		{   	
				Sequence mySequence = new Sequence (new SequenceParms ().Loops (-1));
				TweenParms parms;

				Color oldColor = _Logo.GetComponent<Renderer>().material.color;
				parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0.4f)).Ease (EaseType.EaseInQuart);
				
				parms = new TweenParms ().Prop ("localScale", new Vector3 (1.1f, 1.1f, -2)).Ease (EaseType.EaseOutElastic);
				mySequence.Append (HOTween.To (_Logo.transform, 6f, parms));

				parms = new TweenParms ().Prop ("localScale", new Vector3 (0.9f, 0.9f, -2)).Ease (EaseType.EaseOutElastic);
				mySequence.Append (HOTween.To (_Logo.transform, 5f, parms));

				mySequence.Play ();
		
		
		
		
		}

}