using UnityEngine;
using System.Collections;
using Holoville.HOTween;

enum thePlayingEffect
{
	None,
	ChangeFate,
	Randomize,
	FlappyBird,
	GemKiller,
	TimeReset,
	BLackHole,
	BombermanBomb,
	Freeze,
	TimeIncrease,
	Blindness
}

/// <summary>
///  This class is the main entry point of the game it should be attached to a gameobject and be instanciate in the scene
/// Author : Pondomaniac Games
/// </summary>
public class Main : MonoBehaviour
{
    
	public GameObject _indicator;//The indicator to know the selected tile
	public GameObject[,]  _arrayOfShapes;//The main array that contain all games tiles
	private GameObject _currentIndicator;//The current indicator to replace and destroy each time the player change the selection
	private GameObject _FirstObject;//The first object selected
	private GameObject _SecondObject;//The second object selected
	public GameObject[] _listOfGems;//The list of tiles we want to see in the game you can remplace them in unity's inspector and choose all what you want

	public GameObject _theJokerGem;
	public float _theJokerGemPurcentToAppear;

	//Randomize
	public GameObject _theRandomizeGem;
	public float _theRandomizePurcentToAppear;
	public GameObject _randomizeGemEffectWhenMatch;
	public int _theRandomizeNumberOfGems;
	public AudioClip RandomizeSound;

	//Change fate 
	public GameObject _theChangeFateGem;
	public float _theChangeFatePurcentToAppear;
	public GameObject _changeFateGemEffectWhenMatch;
	public AudioClip ChangeFateSound;
		
	//Flappy bird
	public GameObject _theFlappyBirdGem;
	public float _theFlappyBirdPurcentToAppear;
	public Vector3  _theFlappyBirdStartingPosition;
	public GameObject _theFlappyBirdCharacter;
	private GameObject _currentFlappyBirdCharacter;
	public int _flappyBirdCharacterXVelocity;
	public GameObject _flappyBirdGemEffectWhenMatch;
	public AudioClip FlappySound;

	//Reset The time
	public GameObject _theResetimeGem;
	public float _theResetimePurcentToAppear;
	public GameObject _theResetimeGemEffectWhenMatch;
	public float _theResetimeAmount;
	public AudioClip ResetTimeSound;

	//The black hole
	public GameObject _theBlackHoleGem;
	public float _theBlackHolePurcentToAppear;
	public GameObject _theBlackHoleGemEffectWhenMatch;
	public GameObject _theBlackHoleToAnimate;
	public int _theBlackNumberOfGemsToAspire;
	public AudioClip BlackHoleSound;

	//The Bomberman bomb
	public GameObject _theBombGem;
	public float _theBombPurcentToAppear;
	public GameObject _theBombGemEffectWhenMatch;
	public GameObject _theBombExplosion;
	public int  _theBombNumberOfGemsToExplode;
	public AudioClip BombermanSound;

	//The Freeze
	public GameObject _theFreezeGem;
	public float _theFreezePurcentToAppear;
	public GameObject _theFreezeGemEffectWhenMatch;
	public GameObject _theFreezeEffect;
	public float  _theFreezeTimeToStay;
	public int  _theFreezeNumberOfGems;
	public AudioClip FreezeSound;

	//Increase The time
	public GameObject _theIncreasetimeGem;
	public float _theIncreasetimePurcentToAppear;
	public GameObject _theIncreasetimeGemEffectWhenMatch;
	public float _theIncreasetimeAmount;
	public AudioClip IncreaseTimeSound;

	//Blindness
	public GameObject _theBlindnessGem;
	public float _theBlindnessPurcentToAppear;
	public GameObject _theBlindnessGemEffectWhenMatch;
	public GameObject _theBlindnessToAnimate;
	public float  _theBlindnessTimeToStay;
	public AudioClip BlindnessSound;

	//Tutorial
	public GameObject _Tutorial;
	public GameObject _emptyGameobject;//After destroying object they are replaced with this one so we will replace them after with new ones
	public GameObject _particleEffect;//The object we want to use in the effect of shining stars 
	public GameObject _particleEffectWhenMatch;//The gameobject of the effect when the objects are matching
	public GameObject _particleEffectWhenMatchTheJoker;//The gameobject of the effect when the objects are matching
	public bool _canTransitDiagonally = false;//Indicate if we can switch diagonally
	public int _scoreIncrement;//The amount of point to increment each time we find matching tiles
	private  int _scoreTotal = 0;//The score 
	private ArrayList _currentParticleEffets = new ArrayList ();//the array that will contain all the matching particle that we will destroy after
	public AudioClip MatchSound;//the sound effect when matched tiles are found
	public AudioClip JokerMatchSound;
	public int _gridWidth;//the grid number of cell horizontally
	public int _gridHeight;//the grid number of cell vertically
	float progress = 0;
	public float _timerCoef  ;
	public GameObject _Time;//The timer
	public GameObject _PauseButton;
	private bool isPaused = false ;
	private bool isEnded = false ;
	private bool isCountingDown = true ;
	private thePlayingEffect _thePlayingEffect = thePlayingEffect.None ;
	public GameObject _ReloadButton;
	public GameObject _PlayButton;
	public GameObject _MenuButton;
	public GameObject _PausedBackground;
	float timing = 0;
	public GameObject _TimeIsUp;
	public GameObject _Message;
	public GameObject _MessageEffectWhenShown;
	bool _BestScoreReached = false ;
	bool _BestLevelReached = false ;
	public GameObject _CurrentScore;
	public GameObject _BestScore;
	public GameObject _CurrentLevel;
	public GameObject _BestLevel;
	public GameObject _ScoreBoard;
	public GameObject _Level;//The timer
	public GameObject _LevelTextValue;//The timer

	public GameObject _CountDown;
	public AudioClip PowerSound;
	public AudioClip MenuSound;
	int   level = 0;
	float maxProgress = 0;
	float ScoreByLevel = 0;
	public float _Amount ;
	public AudioClip LevelUpSound;
	public AudioClip TimeUpSound;
	public AudioClip BestScoreSound;
	public AudioClip EndSound;
	// Use this for initialization
	void Start ()
	{

		UpdateLevel (0);
		progress = (float)(timing * _timerCoef);
		_Time.transform.localScale = new Vector3 (Mathf.Clamp01 (progress), _Time.transform.localScale.y, 0);
		_arrayOfShapes = new GameObject[_gridWidth, _gridHeight];
		for (int i = 0; i <= _gridWidth-1; i++) {
			for (int j = 0; j <= _gridHeight-1; j++) {
				var gameObject = GameObject.Instantiate (_emptyGameobject, new Vector3 (i, j, 0), transform.rotation) as GameObject;
				_arrayOfShapes [i, j] = gameObject;
			}
		}
		InvokeRepeating ("DoShapeEffect", 1f, 0.21F);
		StartCoroutine (Init ());
		DoEmptyDown (ref _arrayOfShapes);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		ArrayList Matches = new ArrayList ();
		bool shouldTransit = false;

        //Detecting if the player clicked on the left mouse button and also if there is no animation playing
        //if (UnityEngine.Input.GetButtonDown ("Fire1")) {
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began)        {
            Destroy (_currentIndicator);
			//The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
		    RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.touches[0].position), Vector2.zero);
           // RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector2.zero);
            if (hit.transform != null) {
				if (hit.transform.gameObject.name == _emptyGameobject.name + "(Clone)") {
					DoEmptyDown (ref _arrayOfShapes);
					return;
				}

				if (hit.transform.gameObject.name == _MenuButton.name) {
					GetComponent<AudioSource> ().PlayOneShot (MenuSound);
					hit.transform.localScale = new Vector3 (1.1f, 1.1f, 0);

					Application.LoadLevel ("MainMenu");
				}
				if (hit.transform.gameObject.name == _ReloadButton.name) {
					GetComponent<AudioSource> ().PlayOneShot (MenuSound);
					Time.timeScale = 1;
					isPaused = false;
					HOTween.Play ();
					hit.transform.localScale = new Vector3 (1.1f, 1.1f, 0);
					Application.LoadLevel (Application.loadedLevelName);
				}
				if (hit.transform.gameObject.name == _PauseButton.name && !isPaused && !isCountingDown && !isEnded && HOTween.GetTweenersByTarget (_PlayButton.transform, false).Count == 0 && HOTween.GetTweenersByTarget (_MenuButton.transform, false).Count == 0) {
					GetComponent<AudioSource> ().PlayOneShot (MenuSound);
					StartCoroutine (ShowMenu ());
					hit.transform.localScale = new Vector3 (1.1f, 1.1f, 0);
				} else if ((hit.transform.gameObject.name == _PauseButton.name || hit.transform.gameObject.name == _PlayButton.name) && !isEnded && !isCountingDown && isPaused && HOTween.GetTweenersByTarget (_PlayButton.transform, false).Count == 0 && HOTween.GetTweenersByTarget (_MenuButton.transform, false).Count == 0) {
					GetComponent<AudioSource> ().PlayOneShot (MenuSound);
					StartCoroutine (HideMenu ());
					hit.transform.localScale = new Vector3 (1f, 1f, 0);
				}



				bool founGem = false;
				bool gemIsTweening = false;
				Vector2 gemPosition = new Vector2 (-1, -1);
				for (var x = 0; x <= _arrayOfShapes.GetUpperBound(0); x++) {
					for (var y = 0; y <= _arrayOfShapes.GetUpperBound(1); y++) {
						if (_arrayOfShapes [x, y].GetInstanceID () == hit.transform.gameObject.GetInstanceID ()) {
							founGem = true;
							gemPosition = new Vector2 (x, y);
						}
						if (HOTween.GetTweenersByTarget (_arrayOfShapes [x, y].transform, false).Count > 0)
							gemIsTweening = true; 
					}
				}
				if (! founGem || isPaused || gemIsTweening)
					return;
				//To know if the user already selected a tile or not yet
				if (_FirstObject == null)
					_FirstObject = hit.transform.gameObject;
				else {
					_SecondObject = hit.transform.gameObject;
					shouldTransit = true;
				}

				_currentIndicator = GameObject.Instantiate (_indicator, new Vector3 (hit.transform.gameObject.transform.position.x, hit.transform.gameObject.transform.position.y, -1), transform.rotation) as GameObject;

					
				if (hit.transform.gameObject.name == _theRandomizeGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theRandomizeGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.Randomize;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];

					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);

					var destroyingParticle = GameObject.Instantiate (_randomizeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
					Destroy (destroyingParticle, 3f);
					//	StartCoroutine( ShowMessage(_Message, "Randomize"));
					StartCoroutine (Randomize ());
					Destroy (_currentIndicator);
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (RandomizeSound);
				} else if (hit.transform.gameObject.name == _theChangeFateGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theChangeFateGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.ChangeFate;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];

					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);
					var destroyingParticle = GameObject.Instantiate (_changeFateGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
					Destroy (destroyingParticle, 10f);
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					//	StartCoroutine( ShowMessage(_Message, "Change of fate"));
					Destroy (_currentIndicator);
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (ChangeFateSound);
				} else if (hit.transform.gameObject.name == _theFlappyBirdGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theFlappyBirdGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.FlappyBird;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					//Destroy(go);
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					//StartCoroutine( ShowMessage(_Message, "Flappy Time"));
					CreateAFlappyBird (_theFlappyBirdStartingPosition);
					StartCoroutine (ShowMessage (_Message, "Tap Tap Tap..."));
					Destroy (_currentIndicator);
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (FlappySound);
				} else if (hit.transform.gameObject.name == _theResetimeGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theResetimeGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.TimeReset;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
				
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)gemPosition.x, (int)gemPosition.y, -1), transform.rotation) as GameObject;
				
					FreezeTime (go);
					Destroy (go);
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					//StartCoroutine( ShowMessage(_Message, "Time saver"));
					Destroy (_currentIndicator);
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (ResetTimeSound);
				} else if (hit.transform.gameObject.name == _theBlackHoleGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theBlackHoleGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.BLackHole;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					var destroyingParticle = GameObject.Instantiate (_theBlackHoleGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
					Destroy (destroyingParticle, 1f);
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);
					


					Destroy (_currentIndicator);
					StartCoroutine (ShowBlackHole (go.transform.position));
					//shouldTransit= false;
					GetComponent<AudioSource> ().PlayOneShot (BlackHoleSound);
				} else if (hit.transform.gameObject.name == _theBombGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theBombGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.BombermanBomb;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					var destroyingParticle = GameObject.Instantiate (_theBombGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
					Destroy (destroyingParticle, 3f);
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);
					

					//StartCoroutine( ShowMessage(_Message, "Bomberman"));
					Destroy (_currentIndicator);
					StartCoroutine (ShowBombermanBomb (go.transform.position));
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (BombermanSound);
				} else if (hit.transform.gameObject.name == _theFreezeGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theFreezeGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.Freeze;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);
					
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					var destroyingParticle = GameObject.Instantiate (_theFreezeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
					Destroy (destroyingParticle, 1f);
					//StartCoroutine( ShowMessage(_Message, "Freeze time"));
					Destroy (_currentIndicator);
					StartCoroutine (FreezeGems (go.transform.position));
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (FreezeSound);
				} else if (hit.transform.gameObject.name == _theIncreasetimeGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theIncreasetimeGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.TimeIncrease;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;

					
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					IncreaseTime (go);
					Destroy (go);
					//StartCoroutine( ShowMessage(_Message, "Time waster"));
					Destroy (_currentIndicator);
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (IncreaseTimeSound);
				} else if (hit.transform.gameObject.name == _theBlindnessGem.name + "(Clone)" || (_SecondObject != null && _SecondObject.name == _theBlindnessGem.name + "(Clone)")) {
					_thePlayingEffect = thePlayingEffect.Blindness;
					GameObject go = _arrayOfShapes [(int)gemPosition.x, (int)gemPosition.y];
					
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					Destroy (go);
					
					if (!Matches.Contains (go)) {
						Matches.Add (go);
					}
					//StartCoroutine( ShowMessage(_Message, "Toxic vision"));
					Destroy (_currentIndicator);
					StartCoroutine (ShowBlindness (go.transform.position));
					shouldTransit = false;
					GetComponent<AudioSource> ().PlayOneShot (BlindnessSound);
				}
				//If the user select the second tile we will swap the two tile and animate them
				if (shouldTransit) {
					//Getting the position between the 2 tiles
					var distance = _FirstObject.transform.position - _SecondObject.transform.position;
					//Testing if the 2 tiles are next to each others otherwise we will not swap them 
					if (Mathf.Abs (distance.x) <= 1 && Mathf.Abs (distance.y) <= 1) {   //If we dont want the player to swap diagonally
						if (!_canTransitDiagonally) {
							if (distance.x != 0 && distance.y != 0) {
								Destroy (_currentIndicator); 
								_FirstObject = null;
								_SecondObject = null; 
								return;
							}
						}
						//Animate the transition
						DoSwapMotion (_FirstObject.transform, _SecondObject.transform);
						//Swap the object in array
						DoSwapTile (_FirstObject, _SecondObject, ref _arrayOfShapes);

                                                
					} else {
						_FirstObject = null;
						_SecondObject = null;

					}
					Destroy (_currentIndicator);
                                       
				}
               
			}
          
		}

		if (isPaused)
			return;
		var Infos2 = HOTween.GetTweenInfos ();
		bool gemIsTweening2 = false;
		if (Infos2 != null) {
						
			for (var x = 0; x <= _arrayOfShapes.GetUpperBound(0); x++) {
				for (var y = 0; y <= _arrayOfShapes.GetUpperBound(1); y++) {
					if (HOTween.GetTweenersByTarget (_arrayOfShapes [x, y].transform, false).Count > 0)
						gemIsTweening2 = true; 
				}
			}
		}
		//If no animation is playing
		if (! gemIsTweening2) {
			Matches.AddRange (FindMatch (_arrayOfShapes));
			//If we find a matched tiles
			if (Matches.Count > 0) {//timing-=0.9f;
				if (timing < 0)
					timing = 0;
				if (Matches.Count == 4) { 
					
					StartCoroutine (ShowMessage (_Message, "Superb"));
				} else if (Matches.Count == 5) { 
					
					StartCoroutine (ShowMessage (_Message, "Outstanding"));
				} else if (Matches.Count >= 6) { 
					
					StartCoroutine (ShowMessage (_Message, "Marvelous"));
				} 
				//Update the score
				_scoreTotal += Matches.Count * _scoreIncrement;
               
				foreach (GameObject go in Matches) {
					Debug.Log (go.tag);
					if (go.tag == "Freeze")
						continue;
					if (go.name == _theJokerGem.name + "(Clone)") {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_particleEffectWhenMatchTheJoker as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theRandomizeGem.name + "(Clone)" || go.tag == "RandomizeMatch" || _thePlayingEffect == thePlayingEffect.Randomize) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_randomizeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theChangeFateGem.name + "(Clone)" || go.tag == "ChangeFateMatch" || _thePlayingEffect == thePlayingEffect.ChangeFate) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
					
					} else if (go.name == _theFlappyBirdGem.name + "(Clone)" || go.tag == "FlappyBirdCollision") {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_flappyBirdGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theResetimeGem.name + "(Clone)" || go.tag == "ResetTime" || _thePlayingEffect == thePlayingEffect.TimeReset) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theResetimeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theBlackHoleGem.name + "(Clone)" || go.tag == "BlackHole" || _thePlayingEffect == thePlayingEffect.BLackHole) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theBlackHoleGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theBombGem.name + "(Clone)" || go.tag == "Bomb" || _thePlayingEffect == thePlayingEffect.BombermanBomb) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theBombGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theFreezeGem.name + "(Clone)" || go.tag == "Freeze" || _thePlayingEffect == thePlayingEffect.Freeze) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theFreezeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theIncreasetimeGem.name + "(Clone)" || go.tag == "IncreaseTime" || _thePlayingEffect == thePlayingEffect.TimeIncrease) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theIncreasetimeGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else if (go.name == _theBlindnessGem.name + "(Clone)" || go.tag == "Blindness" || _thePlayingEffect == thePlayingEffect.Blindness) {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (JokerMatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_theBlindnessGemEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 3f);
					} else {
						//Playing the matching sound
						GetComponent<AudioSource> ().PlayOneShot (MatchSound);
						//Creating and destroying the effect of matching
						var destroyingParticle = GameObject.Instantiate (_particleEffectWhenMatch as GameObject, new Vector3 (go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
						Destroy (destroyingParticle, 1f);
					}
					//Replace the matching tile with an empty one
					_arrayOfShapes [(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;
					foreach (Tweener t in HOTween.GetTweenersByTarget (go.transform,true))
						t.Kill ();
					//Destroy the ancient matching tiles
					Destroy (go);
				}
				_FirstObject = null;
				_SecondObject = null;
				//Moving the tiles down to replace the empty ones
				DoEmptyDown (ref _arrayOfShapes);

			}  
       //If no matching tiles are found remake the tiles at their places
            else if (_FirstObject != null
				&& _SecondObject != null
                     ) {
				//Animate the tiles
				DoSwapMotion (_FirstObject.transform, _SecondObject.transform);
				//Swap the tiles in the array
				DoSwapTile (_FirstObject, _SecondObject, ref _arrayOfShapes);
				_FirstObject = null;
				_SecondObject = null;
            
			} 
		}
		if (! isPaused) {
			timing += 0.001f;
			progress = (float)(timing * _timerCoef);
		
				
			_Time.transform.localScale = new Vector3 (Mathf.Clamp01 (progress), _Time.transform.localScale.y, 0);


		}
		if (Mathf.Clamp01 (progress) >= 1) {
			isEnded = true; 
			isPaused = true;
			TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_TimeIsUp.transform.position.x, -0.85f, -6)).Ease (EaseType.EaseOutQuart);
			HOTween.To (_TimeIsUp.transform, 0.5f, parms).WaitForCompletion ();
			StartCoroutine (ShowBoardScore ());

		}
		//Update the score
		(GetComponent (typeof(TextMesh))as TextMesh).text = _scoreTotal.ToString ();
		if (PlayerPrefs.GetInt ("HighScore") < _scoreTotal && !_BestScoreReached) {
			StartCoroutine (ShowMessage (_Message, "Best score"));
			_BestScoreReached = true;
		}
		if (PlayerPrefs.GetInt ("HighLevel") < level && !_BestLevelReached) {
			StartCoroutine (ShowMessage (_Message, "Best Level"));
			_BestLevelReached = true;
		}
		//Update the Level
		UpdateLevel (Matches.Count * _scoreIncrement);

		
		_thePlayingEffect = thePlayingEffect.None;

	}

	void UpdateLevel (int score)
	{ 
		ScoreByLevel += score;
		maxProgress = (float)Mathf.Floor (250 * (level + 1));
		_Level.transform.localScale = new Vector3 ((float)(ScoreByLevel / maxProgress), _Level.transform.localScale.y, 0);

		if (Mathf.Clamp01 (_Level.transform.localScale.x) >= 1) { 
			// update level and increase difficulty
			level += 1;
			ScoreByLevel = 0;
			timing = 0;
			UpdatePowers (_Amount);
			StartCoroutine (ShowMessage (_Message, "Level UP"));
			TweenParms parms = new TweenParms ().Prop ("localScale", new Vector3 (0, _Level.transform.localScale.y, -6)).Ease (EaseType.EaseOutQuart);
			HOTween.To (_Level.transform, 0.5f, parms).WaitForCompletion ();
			parms = new TweenParms ().Prop ("localScale", new Vector3 (0, _Time.transform.localScale.y, -6)).Ease (EaseType.EaseOutQuart);
			HOTween.To (_Time.transform, 0.5f, parms).WaitForCompletion ();

			(_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text = level.ToString ();

			var destroyingParticle = GameObject.Instantiate (_LevelTextValue as GameObject, new Vector3 (_LevelTextValue.transform.position.x, _LevelTextValue.transform.position.y, _LevelTextValue.transform.position.z - 1), transform.rotation) as GameObject;
			Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
			parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseOutQuart);
			HOTween.To ((destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh), 4f, parms);
			parms = new TweenParms ().Prop ("fontSize", 150).Ease (EaseType.EaseOutQuart);
			HOTween.To ((destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh), 2f, parms);
			Destroy (destroyingParticle, 5);

		} else {
			(_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text = level.ToString ();

		}

	}

	IEnumerator ShowBoardScore ()
	{
		GetComponent<AudioSource> ().Stop ();
		GetComponent<AudioSource> ().PlayOneShot (TimeUpSound);
		yield return new WaitForSeconds (0.5f);
		GetComponent<AudioSource> ().PlayOneShot (EndSound);
		(_BestScore.GetComponent (typeof(TextMesh))as TextMesh).text = "" + PlayerPrefs.GetInt ("HighScore");
		(_BestLevel.GetComponent (typeof(TextMesh))as TextMesh).text = "" + PlayerPrefs.GetInt ("HighLevel");
		(_CurrentScore.GetComponent (typeof(TextMesh))as TextMesh).text = "" + _scoreTotal;
		(_CurrentLevel.GetComponent (typeof(TextMesh))as TextMesh).text = "" + (_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text;
		SetScore (_scoreTotal);
		yield return new WaitForSeconds (1);
		TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_ScoreBoard.transform.position.x, 5f, _ScoreBoard.transform.position.z)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_ScoreBoard.transform, 0.5f, parms);
		parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 1.7f, -8)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_MenuButton.transform, 0.7f, parms).WaitForCompletion ();
		parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 1.7f, -8)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_ReloadButton.transform, 0.9f, parms).WaitForCompletion ();
	}
	// Find Match-3 Tile
	private ArrayList FindMatch (GameObject[,] cells)
	{//creating an arraylist to store the matching tiles
		ArrayList stack = new ArrayList ();
		//Checking the vertical tiles
		for (var x = 0; x <= cells.GetUpperBound(0); x++) {
			for (var y = 0; y <= cells.GetUpperBound(1); y++) {
				var thiscell = cells [x, y];
				//If it's an empty tile continue
				if (thiscell.name == "Empty(Clone)")
					continue;

				if (_thePlayingEffect == thePlayingEffect.ChangeFate) {
					if (!stack.Contains (cells [x, y])) {
						cells [x, y].tag = "ChangeFateMatch";
						stack.Add (cells [x, y]);
					}
				} else if (_currentFlappyBirdCharacter != null) {
					RaycastHit2D hit = Physics2D.Raycast (thiscell.transform.position, Vector2.zero);
					if (hit.transform != null && hit.transform.gameObject.name == _currentFlappyBirdCharacter.name) {
						if (!stack.Contains (cells [x, y])) {
							cells [x, y].tag = "FlappyBirdCollision";
							stack.Add (cells [x, y]);
						}
					}
				}
				
				int matchCount = 0;
				int y2 = cells.GetUpperBound (1);
				int y1;
				//Getting the number of tiles of the same kind
				for (y1 = y + 1; y1 <= y2; y1++) {
					if (cells [x, y1].name == "Empty(Clone)" || (thiscell.name != cells [x, y1].name && thiscell.name != _theJokerGem.name + "(Clone)") && cells [x, y1].name != _theJokerGem.name + "(Clone)")
						break;
					matchCount++;
				}
				//If we found more than 2 tiles close we add them in the array of matching tiles
				if (matchCount >= 2) {
					y1 = Mathf.Min (cells.GetUpperBound (1), y1 - 1);
					for (var y3 = y; y3 <= y1; y3++) {
						if (!stack.Contains (cells [x, y3])) {
							stack.Add (cells [x, y3]);
						}
					}
				}
			}
		}
		//Checking the horizontal tiles , in the following loops we will use the same concept as the previous ones
		for (var y = 0; y < cells.GetUpperBound(1) + 1; y++) {
			for (var x = 0; x < cells.GetUpperBound(0) + 1; x++) {
				var thiscell = cells [x, y];
				if (thiscell.name == "Empty(Clone)")
					continue;


				int matchCount = 0;
				int x2 = cells.GetUpperBound (0);
				int x1;
				for (x1 = x + 1; x1 <= x2; x1++) {
					if (cells [x1, y].name == "Empty(Clone)" || (thiscell.name != cells [x1, y].name && thiscell.name != _theJokerGem.name + "(Clone)") && cells [x1, y].name != _theJokerGem.name + "(Clone)")
						break;
					matchCount++;
				}
				if (matchCount >= 2) {
					x1 = Mathf.Min (cells.GetUpperBound (0), x1 - 1);
					for (var x3 = x; x3 <= x1; x3++) {
						if (!stack.Contains (cells [x3, y])) {
							stack.Add (cells [x3, y]);
						}
					}
				}
			}
		}
		return stack;
	}

	// Swap Motion Animation, to animate the switching arrays
	void DoSwapMotion (Transform a, Transform b)
	{
		Vector3 posA = a.localPosition;
		Vector3 posB = b.localPosition;
		TweenParms parms = new TweenParms ().Prop ("localPosition", posB).Ease (EaseType.EaseOutQuart);
		HOTween.To (a, 0.2f, parms).WaitForCompletion ();
		parms = new TweenParms ().Prop ("localPosition", posA).Ease (EaseType.EaseOutQuart);
		HOTween.To (b, 0.2f, parms).WaitForCompletion ();

	}


	// Swap Two Tile, it swaps the position of two objects in the grid array
	void DoSwapTile (GameObject a, GameObject b, ref GameObject[,] cells)
	{
		GameObject cell = cells [(int)a.transform.position.x, (int)a.transform.position.y];
		cells [(int)a.transform.position.x, (int)a.transform.position.y] = cells [(int)b.transform.position.x, (int)b.transform.position.y];
		cells [(int)b.transform.position.x, (int)b.transform.position.y] = cell;

	}

	// Do Empty Tile Move Down
	private void DoEmptyDown (ref GameObject[,] cells)
	{   //replace the empty tiles with the ones above
		for (int x= 0; x <= cells.GetUpperBound(0); x++) {
			for (int y = 0; y <= cells.GetUpperBound(1); y++) {

				var thisCell = cells [x, y];
				if (thisCell.name == "Empty(Clone)") {

					for (int y2 = y; y2 <= cells.GetUpperBound(1); y2++) {
						if (cells [x, y2].name != "Empty(Clone)") {
							cells [x, y] = cells [x, y2];
							cells [x, y2] = thisCell;
							break;
						}

					}
                
				}

			}
		}
		//Instantiate new tiles to replace the ones destroyed
		for (int x = 0; x <= cells.GetUpperBound(0); x++) {
			for (int y = 0; y <= cells.GetUpperBound(1); y++) {
				if (cells [x, y].name == "Empty(Clone)") { 
					float randomPurcent = Random.Range (0f, 100f);
					Destroy (cells [x, y]);
					if (randomPurcent < _theChangeFatePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theChangeFateGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theBlindnessPurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theBlindnessGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theBlackHolePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theBlackHoleGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theBombPurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theBombGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theFlappyBirdPurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theFlappyBirdGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theFreezePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theFreezeGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theRandomizePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theRandomizeGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theIncreasetimePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theIncreasetimeGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theResetimePurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theResetimeGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else if (randomPurcent < _theJokerGemPurcentToAppear)
						cells [x, y] = GameObject.Instantiate (_theJokerGem as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					else {
						cells [x, y] = GameObject.Instantiate (_listOfGems [Random.Range (0, _listOfGems.Length)] as GameObject, new Vector3 (x, cells.GetUpperBound (1) + 2, 0), transform.rotation) as GameObject;
					}
               
				}
			}
		}

		for (int x = 0; x <= cells.GetUpperBound(0); x++) {
			for (int y = 0; y <= cells.GetUpperBound(1); y++) {

				TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (x, y, -1)).Ease (EaseType.EaseOutQuart);
				HOTween.To (cells [x, y].transform, .4f, parms);
			}
		}

      
      
	}
	//Instantiate the star objects
	void DoShapeEffect ()
	{
		if (isPaused)
			return;
		foreach (GameObject row in _currentParticleEffets)
			Destroy (row);
		for (int i = 0; i <= 2; i++)
			_currentParticleEffets.Add (GameObject.Instantiate (_particleEffect, new Vector3 (Random.Range (0, _arrayOfShapes.GetUpperBound (0) + 1), Random.Range (0, _arrayOfShapes.GetUpperBound (1) + 1), -1), new Quaternion (0, 0, Random.Range (0, 1000f), 100)) as GameObject);
	}

	IEnumerator ShowMenu ()
	{
		isPaused = true;
		HOTween.Pause ();
		GetComponent<AudioSource> ().Pause ();

		TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_PausedBackground.transform.position.x, 4, -5)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_PausedBackground.transform, 0.2f, parms).WaitForCompletion ();

		parms = new TweenParms ().Prop ("position", new Vector3 (_PlayButton.transform.position.x, 3.5f, -6)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_PlayButton.transform, 0.4f, parms).WaitForCompletion ();

		parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 3.5f, -6)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_ReloadButton.transform, 0.5f, parms).WaitForCompletion ();

		parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 3.5f, -6)).Ease (EaseType.EaseOutQuart);
		yield return StartCoroutine (HOTween.To (_MenuButton.transform, 0.6f, parms).WaitForCompletion ());


		Time.timeScale = 0;


	}

	IEnumerator HideMenu ()
	{
		Time.timeScale = 1;
		isPaused = false;
		HOTween.Play ();

		TweenParms parms = new TweenParms ().Prop ("position", new Vector3 (_PausedBackground.transform.position.x, 16, -5)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_PausedBackground.transform, 0.6f, parms).WaitForCompletion ();

		parms = new TweenParms ().Prop ("position", new Vector3 (_PlayButton.transform.position.x, 16, -6)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_PlayButton.transform, 0.4f, parms).WaitForCompletion ();
		GetComponent<AudioSource> ().Play ();

		parms = new TweenParms ().Prop ("position", new Vector3 (_ReloadButton.transform.position.x, 16, -6)).Ease (EaseType.EaseOutQuart);
		HOTween.To (_ReloadButton.transform, 0.5f, parms).WaitForCompletion ();


		parms = new TweenParms ().Prop ("position", new Vector3 (_MenuButton.transform.position.x, 16, -6)).Ease (EaseType.EaseOutQuart);
		yield return StartCoroutine (HOTween.To (_MenuButton.transform, 0.2f, parms).WaitForCompletion ());

	}

	ArrayList ListOfMessages = new ArrayList ();

	IEnumerator  ShowMessage (GameObject go, string s)
	{
		if (ListOfMessages.Count == 0) {
			ListOfMessages.Add (s);
						
			var _currentGo = GameObject.Instantiate (go, new Vector3 (_gridWidth / 2, go.transform.position.y, go.transform.position.z), transform.rotation) as GameObject;
			(_currentGo.GetComponent (typeof(TextMesh))as TextMesh).text = s + "!!";
			foreach (Tweener tw in  HOTween.GetTweenersByTarget(_currentGo.transform,true))
				tw.Complete ();
					
			foreach (Transform t in _currentGo.transform.GetComponentsInChildren(typeof(Transform))) {
				Color oldColor = t.GetComponent<Renderer> ().material.color;
				t.GetComponent<Renderer> ().material.color = new Color (oldColor.r, oldColor.b, oldColor.g, 0f);
			}
			foreach (Transform t in _currentGo.transform.GetComponentsInChildren(typeof(Transform))) {
				Color oldColor = t.GetComponent<Renderer> ().material.color;
							
				TweenParms parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 1f)).Ease (EaseType.EaseInQuart);
				HOTween.To (t.GetComponent<Renderer> ().material, 0.5f, parms);
			}
			var _messageEffect = GameObject.Instantiate (_MessageEffectWhenShown, new Vector3 (_gridWidth / 2, go.transform.position.y, go.transform.position.z), transform.rotation) as GameObject;
			
			yield return new WaitForSeconds (1.5f); 
			foreach (Transform t in _currentGo.transform.GetComponentsInChildren(typeof(Transform))) {
				Color oldColor = t.GetComponent<Renderer> ().material.color;
								
				TweenParms parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseOutQuart);
				HOTween.To (t.GetComponent<Renderer> ().material, 0.8f, parms);
			}
			Destroy (_currentGo, 5);
			Destroy (_messageEffect, 6);
			yield return new WaitForSeconds (1f);
			ListOfMessages.Remove (s);
		}
		yield return true;
	}
	                                                             
	void SetScore (int _scoreTotal)
	{
		PlayerPrefs.SetInt ("LastScore", _scoreTotal);
		if (PlayerPrefs.GetInt ("HighScore") < _scoreTotal) {

			PlayerPrefs.SetInt ("HighScore", _scoreTotal);
			GetComponent<AudioSource> ().PlayOneShot (BestScoreSound);
		}
		if (PlayerPrefs.GetInt ("HighLevel") < _scoreTotal) {

			PlayerPrefs.SetInt ("HighLevel", int.Parse ((_LevelTextValue.GetComponent (typeof(TextMesh))as TextMesh).text));

		}
	}

	void CreateAFlappyBird (Vector3 position)
	{ 
		Destroy (_currentFlappyBirdCharacter);
		_currentFlappyBirdCharacter = GameObject.Instantiate (_theFlappyBirdCharacter, position, transform.rotation) as GameObject;
		Vector2 v = _currentFlappyBirdCharacter.GetComponent<Rigidbody2D> ().velocity;
		v.x = _flappyBirdCharacterXVelocity;
		_currentFlappyBirdCharacter.GetComponent<Rigidbody2D> ().velocity = v;
	}

	IEnumerator Randomize ()
	{
		for (int r = 0; r <= _theRandomizeNumberOfGems; r++) {
			GameObject firstTile = _arrayOfShapes [Random.Range (0, _arrayOfShapes.GetUpperBound (0) + 1), Random.Range (0, _arrayOfShapes.GetUpperBound (1) + 1)];
			GameObject secondTile = _arrayOfShapes [Random.Range (0, _arrayOfShapes.GetUpperBound (0) + 1), Random.Range (0, _arrayOfShapes.GetUpperBound (1) + 1)];

			if ((HOTween.GetTweenersByTarget (secondTile.transform, false).Count > 0) || 
				(HOTween.GetTweenersByTarget (firstTile.transform, false).Count > 0) || 
				firstTile.name == _theRandomizeGem.name + "(Clone)" || 
				secondTile.name == _theRandomizeGem.name + "(Clone)" ||
				firstTile.name == _emptyGameobject.name + "(Clone)" || 
				secondTile.name == _emptyGameobject.name + "(Clone)")
				continue;
			//Swap the tiles in the array
			DoSwapTile (firstTile, secondTile, ref _arrayOfShapes);
			//Animate the tiles
			DoSwapMotion (firstTile.transform, secondTile.transform);
			yield return new WaitForSeconds (0.035f);  
		}
		//Moving the tiles down to replace the empty ones
		DoEmptyDown (ref _arrayOfShapes);
	}

	void FreezeTime (GameObject go)
	{
		timing -= _theResetimeAmount;
		GameObject gameo = GameObject.Instantiate (_theResetimeGem as GameObject, go.transform.position, transform.rotation) as GameObject;
		TweenParms parms = new TweenParms ().Prop ("position", _Time.transform.position).Ease (EaseType.EaseOutQuart);
		HOTween.To (gameo.transform, 1.5f, parms);
		Color oldColor = gameo.GetComponent<Renderer> ().material.color;
		parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseOutQuart);
		HOTween.To (gameo.GetComponent<Renderer> ().material, 2f, parms);
		Destroy (gameo, 2);
	}

	IEnumerator ShowBlackHole (Vector3 position)
	{
		var blackHoleInstance = GameObject.Instantiate (_theBlackHoleToAnimate, new Vector3 (position.x, position.y, -1), transform.rotation) as GameObject;
		TweenParms parms = new TweenParms ().Prop ("localRotation", new Quaternion (0, 0, 80, 0)).Ease (EaseType.EaseOutQuart);
		HOTween.To (blackHoleInstance.transform, 4f, parms);
		Color oldColor = blackHoleInstance.GetComponent<Renderer> ().material.color;
		parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseOutQuart);
		HOTween.To (blackHoleInstance.GetComponent<Renderer> ().material, 8f, parms);
		parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
		HOTween.To (blackHoleInstance.transform, 14f, parms);
		Destroy (blackHoleInstance, 2);


		for (int r = 0; r <= _theBlackNumberOfGemsToAspire; r++) {
			GameObject firstTile = ((int)position.x - r < 0 || (int)position.y + r > _arrayOfShapes.GetUpperBound (1) ? null : _arrayOfShapes [(int)position.x - r, (int)position.y + r]);
			GameObject secondTile = ((int)position.x - r < 0 ? null : _arrayOfShapes [(int)position.x - r, (int)position.y]);
			GameObject thirdTile = ((int)position.x - r < 0 || (int)position.y - r < 0 ? null : _arrayOfShapes [(int)position.x - r, (int)position.y - r]);
			GameObject fourthTile = ((int)position.y + r > _arrayOfShapes.GetUpperBound (1) ? null : _arrayOfShapes [(int)position.x, (int)position.y + r]); 
			GameObject fifthTile = ((int)position.y - r < 0 ? null : _arrayOfShapes [(int)position.x, (int)position.y - r]);  
			GameObject sixTile = ((int)position.x + r > _arrayOfShapes.GetUpperBound (0) || (int)position.y + r > _arrayOfShapes.GetUpperBound (1) ? null : _arrayOfShapes [(int)position.x + r, (int)position.y + r]);  
			GameObject seventhTile = ((int)position.x + r > _arrayOfShapes.GetUpperBound (0) ? null : _arrayOfShapes [(int)position.x + r, (int)position.y]);
			GameObject eightTile = ((int)position.x + r > _arrayOfShapes.GetUpperBound (0) || (int)position.y - r < 0 ? null : _arrayOfShapes [(int)position.x + r, (int)position.y - r]);  

		    
			if (firstTile != null) {	
				//Replace the matching tile with an empty one
				_arrayOfShapes [(int)firstTile.transform.position.x, (int)firstTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)firstTile.transform.position.x, (int)firstTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (firstTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (firstTile.transform, 0.5f, parms);
	
				yield return new WaitForSeconds (0.035f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (secondTile != null) {
				_arrayOfShapes [(int)secondTile.transform.position.x, (int)secondTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)secondTile.transform.position.x, (int)secondTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (secondTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (secondTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f); 
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (thirdTile != null) {
				_arrayOfShapes [(int)thirdTile.transform.position.x, (int)thirdTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)thirdTile.transform.position.x, (int)thirdTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (thirdTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (thirdTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f); 
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (fourthTile != null) {	
				_arrayOfShapes [(int)fourthTile.transform.position.x, (int)fourthTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)fourthTile.transform.position.x, (int)fourthTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (fourthTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (fourthTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			}  
			if (fifthTile != null) {	
				_arrayOfShapes [(int)fifthTile.transform.position.x, (int)fifthTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)fifthTile.transform.position.x, (int)fifthTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (fifthTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (fifthTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			}  
			if (sixTile != null) { 
				_arrayOfShapes [(int)sixTile.transform.position.x, (int)sixTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)sixTile.transform.position.x, (int)sixTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (sixTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (sixTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			}   
			if (seventhTile != null) {  
				_arrayOfShapes [(int)seventhTile.transform.position.x, (int)seventhTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)seventhTile.transform.position.x, (int)seventhTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (seventhTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (seventhTile.transform, 0.5f, parms);
				yield return new WaitForSeconds (0.035f);  
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (eightTile != null) {	
				_arrayOfShapes [(int)eightTile.transform.position.x, (int)eightTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)eightTile.transform.position.x, (int)eightTile.transform.position.y, -1), transform.rotation) as GameObject;
				parms = new TweenParms ().Prop ("position", position).Ease (EaseType.EaseOutQuart);
				HOTween.To (eightTile.transform, 0.2f, parms);
				parms = new TweenParms ().Prop ("localScale", new Vector3 (0, 0, 0)).Ease (EaseType.EaseOutQuart);
				HOTween.To (eightTile.transform, 0.5f, parms);
				//Destroy(eightTile,1);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
				UpdateLevel (_scoreIncrement);
				UpdateLevel (_scoreIncrement);
			} 



			Destroy (firstTile, 1);
			Destroy (secondTile, 1);
			Destroy (thirdTile, 1);
			Destroy (fourthTile, 1);
			Destroy (fifthTile, 1);
			Destroy (sixTile, 1);
			Destroy (seventhTile, 1);
			Destroy (eightTile, 1);
	

		}





		//Moving the tiles down to replace the empty ones
		DoEmptyDown (ref _arrayOfShapes);
	}

	IEnumerator ShowBombermanBomb (Vector3 position)
	{
		
		for (int r = 0; r <= _theBombNumberOfGemsToExplode; r++) {
			GameObject firstTile = ((int)position.x - r < 0 ? null : _arrayOfShapes [(int)position.x - r, (int)position.y]);
			GameObject secondTile = ((int)position.y + r > _arrayOfShapes.GetUpperBound (1) ? null : _arrayOfShapes [(int)position.x, (int)position.y + r]); 
			GameObject thirdTile = ((int)position.y - r < 0 ? null : _arrayOfShapes [(int)position.x, (int)position.y - r]);  
			GameObject fourthTile = ((int)position.x + r > _arrayOfShapes.GetUpperBound (0) ? null : _arrayOfShapes [(int)position.x + r, (int)position.y]);
			
				
			if (firstTile != null) {	
				//Replace the matching tile with an empty one
				_arrayOfShapes [(int)firstTile.transform.position.x, (int)firstTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)firstTile.transform.position.x, (int)firstTile.transform.position.y, -1), transform.rotation) as GameObject;
			
				var destroyingParticle = GameObject.Instantiate (_theBombExplosion as GameObject, firstTile.transform.position, transform.rotation) as GameObject;
				Destroy (destroyingParticle, 2f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (secondTile != null) {
				_arrayOfShapes [(int)secondTile.transform.position.x, (int)secondTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)secondTile.transform.position.x, (int)secondTile.transform.position.y, -1), transform.rotation) as GameObject;

				var destroyingParticle = GameObject.Instantiate (_theBombExplosion as GameObject, secondTile.transform.position, transform.rotation) as GameObject;
				Destroy (destroyingParticle, 2f);
				_scoreTotal += _scoreIncrement;
			} 
			if (thirdTile != null) {
				_arrayOfShapes [(int)thirdTile.transform.position.x, (int)thirdTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)thirdTile.transform.position.x, (int)thirdTile.transform.position.y, -1), transform.rotation) as GameObject;

				var destroyingParticle = GameObject.Instantiate (_theBombExplosion as GameObject, thirdTile.transform.position, transform.rotation) as GameObject;
				Destroy (destroyingParticle, 2f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			} 
			if (fourthTile != null) {	
				_arrayOfShapes [(int)fourthTile.transform.position.x, (int)fourthTile.transform.position.y] = GameObject.Instantiate (_emptyGameobject, new Vector3 ((int)fourthTile.transform.position.x, (int)fourthTile.transform.position.y, -1), transform.rotation) as GameObject;
			
				var destroyingParticle = GameObject.Instantiate (_theBombExplosion as GameObject, fourthTile.transform.position, transform.rotation) as GameObject;
				Destroy (destroyingParticle, 2f);
				_scoreTotal += _scoreIncrement;
				UpdateLevel (_scoreIncrement);
			}  
		
			
			Destroy (firstTile, 0.5f);
			Destroy (secondTile, 0.5f);
			Destroy (thirdTile, 0.5f);
			Destroy (fourthTile, 0.5f);

			
			
		}
		//Moving the tiles down to replace the empty ones
		DoEmptyDown (ref _arrayOfShapes);
		yield return new WaitForSeconds (0f); 
	}

	IEnumerator FreezeGems (Vector3 position)
	{   	
		for (int r = 1; r <= _theFreezeNumberOfGems; r++) {
			GameObject firstTile = ((int)position.x - r < 0 ? null : _arrayOfShapes [(int)position.x - r, (int)position.y]);
			GameObject secondTile = ((int)position.y + r > _arrayOfShapes.GetUpperBound (1) ? null : _arrayOfShapes [(int)position.x, (int)position.y + r]); 
			GameObject thirdTile = ((int)position.y - r < 0 ? null : _arrayOfShapes [(int)position.x, (int)position.y - r]);  
			GameObject fourthTile = ((int)position.x + r > _arrayOfShapes.GetUpperBound (0) ? null : _arrayOfShapes [(int)position.x + r, (int)position.y]);
			
		
			
			TweenParms parms;
			if (firstTile != null) {	
				firstTile.tag = "Freeze";
				var destroyingParticle = GameObject.Instantiate (_theFreezeEffect as GameObject, new Vector3 (firstTile.transform.position.x, firstTile.transform.position.y, firstTile.transform.position.z - 1), transform.rotation) as GameObject;
				Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
				parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);
				HOTween.To (destroyingParticle.GetComponent<Renderer> ().material, _theFreezeTimeToStay, parms);
				Destroy (destroyingParticle, _theFreezeTimeToStay);
			} 
			if (secondTile != null) {

				secondTile.tag = "Freeze";
				var destroyingParticle = GameObject.Instantiate (_theFreezeEffect as GameObject, new Vector3 (secondTile.transform.position.x, secondTile.transform.position.y, secondTile.transform.position.z - 1), transform.rotation) as GameObject;
				Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
				parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);
				HOTween.To (destroyingParticle.GetComponent<Renderer> ().material, _theFreezeTimeToStay, parms);
				Destroy (destroyingParticle, _theFreezeTimeToStay);
				;
			} 
			if (thirdTile != null) {
			
				thirdTile.tag = "Freeze";
				var destroyingParticle = GameObject.Instantiate (_theFreezeEffect as GameObject, new Vector3 (thirdTile.transform.position.x, thirdTile.transform.position.y, thirdTile.transform.position.z - 1), transform.rotation) as GameObject;
				Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
				parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);
				HOTween.To (destroyingParticle.GetComponent<Renderer> ().material, _theFreezeTimeToStay, parms);
				Destroy (destroyingParticle, _theFreezeTimeToStay);
			} 
			if (fourthTile != null) {	

				fourthTile.tag = "Freeze";
				var destroyingParticle = GameObject.Instantiate (_theFreezeEffect as GameObject, new Vector3 (fourthTile.transform.position.x, fourthTile.transform.position.y, fourthTile.transform.position.z - 1), transform.rotation) as GameObject;
				Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
				parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);
				HOTween.To (destroyingParticle.GetComponent<Renderer> ().material, _theFreezeTimeToStay, parms);
				Destroy (destroyingParticle, _theFreezeTimeToStay);
			}  
			
			
			yield return new WaitForSeconds (_theFreezeTimeToStay); 
			
			if (firstTile != null) {	
				firstTile.tag = "Untagged";
			} 
			if (secondTile != null) {
				secondTile.tag = "Untagged";
			} 
			if (thirdTile != null) {
				thirdTile.tag = "Untagged";
			} 
			if (fourthTile != null) {	
				fourthTile.tag = "Untagged";
			}  
			
		}
		//Moving the tiles down to replace the empty ones
		DoEmptyDown (ref _arrayOfShapes);
	

	}
		
	void IncreaseTime (GameObject go)
	{
		timing += _theIncreasetimeAmount;

		GameObject gameo = GameObject.Instantiate (_theIncreasetimeGem as GameObject, go.transform.position, transform.rotation) as GameObject;
		TweenParms parms = new TweenParms ().Prop ("position", _Time.transform.position).Ease (EaseType.EaseOutQuart);
		HOTween.To (gameo.transform, 1.5f, parms);
		Color oldColor = gameo.GetComponent<Renderer> ().material.color;
		parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseOutQuart);
		HOTween.To (gameo.GetComponent<Renderer> ().material, 2f, parms);
		Destroy (gameo, 2);
	}

	IEnumerator ShowBlindness (Vector3 position)
	{   	
		GameObject firstTile = (_arrayOfShapes [(int)position.x, (int)position.y]);
						
					
		TweenParms parms;
		if (firstTile != null) {	
				
			var destroyingParticle = GameObject.Instantiate (_theBlindnessToAnimate as GameObject, new Vector3 (firstTile.transform.position.x, firstTile.transform.position.y, firstTile.transform.position.z - 1), transform.rotation) as GameObject;
			Color oldColor = destroyingParticle.GetComponent<Renderer> ().material.color;
			parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);
			HOTween.To (destroyingParticle.GetComponent<Renderer> ().material, _theBlindnessTimeToStay, parms);
			Destroy (destroyingParticle, _theBlindnessTimeToStay);
		} 
			
			
			
		yield return new WaitForSeconds (0); 
			
	
				
		
	}

	IEnumerator Init ()
	{
		isPaused = true;
		if (PlayerPrefs.GetInt ("Tutorial") != 1) {
			var isOkay = false;

			while (isOkay==false) {
             //   if (UnityEngine.Input.GetButtonDown ("Fire1")) {
                if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began) {
                    isOkay = true; 
					TweenParms parms = new TweenParms ().Prop ("localPosition", new Vector3 (100, 0, -10)).Ease (EaseType.EaseOutQuart);
					HOTween.To (_Tutorial.transform, 3f, parms);
					PlayerPrefs.SetInt ("Tutorial", 1);
			     	
				}
				yield return 0;	
			}
		
		} else {
			_Tutorial.transform.localPosition = new Vector3 (100, 0, -10);

		}
	
		AnimateBigSmall (_CountDown, new Vector3 (_gridWidth / 2 - 0.25f, _gridHeight / 2 + 0.5f, -5), "3");
		GetComponent<AudioSource> ().PlayOneShot (MatchSound);
		yield return new WaitForSeconds (0.7f);
		AnimateBigSmall (_CountDown, new Vector3 (_gridWidth / 2 - 0.25f, _gridHeight / 2 + 0.5f, -5), "2");
		GetComponent<AudioSource> ().PlayOneShot (MatchSound);
		yield return new WaitForSeconds (0.7f);
		AnimateBigSmall (_CountDown, new Vector3 (_gridWidth / 2 - 0.25f, _gridHeight / 2 + 0.5f, -5), "1");
		GetComponent<AudioSource> ().PlayOneShot (MatchSound);
		yield return new WaitForSeconds (0.7f);
		AnimateBigSmall (_CountDown, new Vector3 (_gridWidth / 2 - 0.25f, _gridHeight / 2 + 0.5f, -5), "Go!");
		GetComponent<AudioSource> ().PlayOneShot (MatchSound);
		yield return new WaitForSeconds (0.5f);
		isPaused = false;
		isCountingDown = false;
	}

	void UpdatePowers (float amount)
	{
		_theJokerGemPurcentToAppear *= amount;
		_theRandomizePurcentToAppear *= amount;
		_theChangeFatePurcentToAppear *= amount;
		_theFlappyBirdPurcentToAppear *= amount;
		_theResetimePurcentToAppear *= amount;
		_theBlackHolePurcentToAppear *= amount;
		_theBombPurcentToAppear *= amount;
		_theFreezePurcentToAppear *= (amount + 0.1f);
		_theIncreasetimePurcentToAppear *= (amount + 0.1f);
		_theBlindnessPurcentToAppear *= (amount + 0.1f);
			
	}

	void AnimateBigSmall (GameObject go, Vector3 position, string s)
	{ 
		var destroyingParticle = GameObject.Instantiate (go as GameObject, position, transform.rotation) as GameObject;
		(destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh).text = s;
		Color oldColor2 = destroyingParticle.GetComponent<Renderer> ().material.color;
		TweenParms parms2 = new TweenParms ().Prop ("color", new Color (oldColor2.r, oldColor2.b, oldColor2.g, 0f)).Ease (EaseType.EaseOutQuart);
		HOTween.To ((destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh), 4f, parms2);
		parms2 = new TweenParms ().Prop ("fontSize", 200).Ease (EaseType.EaseOutQuart);
		HOTween.To ((destroyingParticle.GetComponent (typeof(TextMesh))as TextMesh), 3f, parms2);
		Destroy (destroyingParticle, 4);
	}

}

