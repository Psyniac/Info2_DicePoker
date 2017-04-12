
// DICE POKER

using UnityEngine;
using System.Collections;

// Demo application script
public class DicePoker : MonoBehaviour
{

	// VARIABLES ARE PUBLIC TO TEST THEM IN THE UNITY EDITOR
	public int CURRENT_ROUND = 0;
	public int CURRENT_SUBROUND = 0;
	public int CURRENTLY_PLAYING = 0;
	public string CURRENT_PLAYER_NAME = "";
	public bool INGAME = false;
	public bool NEWGAME = true;
	public bool NEWSUBROUND = true;

	public int player_1_coins = 25;
	public int player_2_coins = 25;

	private int first_player = 0;
	private int second_player = 0;

	public string player_1_name = "Player 1";
	public string player_2_name = "Player 2";

	private string button1_label = "";
	private string button2_label = "";
	private string button3_label = "";
	private string button4_label = "";

	private string dice1_label = "";
	private string dice2_label = "";
	private string dice3_label = "";
	private string dice4_label = "";
	private string dice5_label = "";

	public int p1d1 = 0;
	public int p1d2 = 0;
	public int p1d3 = 0;
	public int p1d4 = 0;
	public int p1d5 = 0;

	public int p2d1 = 0;
	public int p2d2 = 0;
	public int p2d3 = 0;
	public int p2d4 = 0;
	public int p2d5 = 0;

	public string reDice1 = "";
	public string reDice2 = "";
	public string reDice3 = "";
	public string reDice4 = "";
	public string reDice5 = "";

	public int global_reroll = 0;
	public int global_pool = 0;
	private int follow_bet = 0;
	public string current_task;
	public int player_1_bet = 0;
	public int player_2_bet = 0;

	public string winner_ = "";
	public string player1_hand = "";
	public string player2_hand = "";

	public int[] p1dices;
	public int[] p2dices;
	public int[] p1count;
	public int[] p2count;

	public string[] winning_arch;
	private const int MODE_GALLERY = 1;
	private const int MODE_GAME = 2;
	private int mode = 0;

	public bool calculating_winner = false;

	// next camera position when moving the camera after switching mode
	private GameObject nextCameraPosition = null;
	// start camera position when moving the camera after switching mode
	private GameObject startCameraPosition = null;
	// store gameObject (empty) for mode MODE_GAME camera position
	private GameObject camP1 = null;
	private GameObject camP2 = null;
	// store gameObject (empty) for mode MODE_GALLERY camera position
	private GameObject camGallery = null;
	// speed of moving camera after switching mode
	private float cameraMovementSpeed = 0.8F;
	private float cameraMovement = 0;

	// initial/starting die in the gallery
	private string galleryDie = "d6-red";
	private GameObject galleryDieObject = null;

	// handle drag rotating the die in the gallery
	private bool dragging = false;
	private Vector2 dragStart;
	private Vector3 dragStartAngle;
	private Vector3 dragLastAngle;
	
	// rectangle GUI area's
	private Rect rectGallerySelectBox;
	private Rect rectGallerySelect;
	private Rect rectModeSelect;

	public bool settings_on = false;
	public bool credits_on = false;
	public bool ui_finalscreen_on = false;
	// GUI gallery die selector texture
	private Texture txSelector = null;
    
	public Font ui_font;
	public bool loadingGame = true;
	public GameObject confetti;
	// Use this for initialization

	void Start ()
	{		
		confetti.SetActive(false);
		// store/cache mode assiociated camera positions
		camP1 = GameObject.Find ("cameraPositionP1");
		camP2 = GameObject.Find ("cameraPositionP2");

		camGallery = GameObject.Find ("cameraPositionGallery");
		// set GUI rectangles of the (screen related) gallery selector
		rectGallerySelectBox = new Rect (Screen.width - 260, 10, 250, 170);
		rectGallerySelect = new Rect (Screen.width - 250, 35, 219, 109);
		rectModeSelect = new Rect (10, 10, 180, 190);
		// set (first) mode to gallery
		SetMode (MODE_GALLERY);	
		StartCoroutine (LoadDelay ());
	}

	IEnumerator LoadDelay ()
	{
		yield return new WaitForSeconds (2.5f);
		loadingGame = false;
	}

	private void SetMode (int pMode)
	{
		// camera is already moving - mode switching - no new mode will be set and we exit here
		if (nextCameraPosition != null || pMode == mode)
			return;

		switch (pMode) {
		case MODE_GALLERY:
			// switch to gallery mode
			calculating_winner = false;
			startCameraPosition = camP1;
			nextCameraPosition = camGallery;
			// create die that will be displayed in the gallery
			INGAME = false;
			SetGalleryDie (galleryDie);
			break;
		case MODE_GAME:
			// switch to rolling mode
			ResetForNewGame ();
			calculating_winner = false;
			startCameraPosition = camGallery;
			nextCameraPosition = camP1;
			INGAME = true;
			NEWGAME = true;
			UpdateGame ();
			break;
		}

		if (nextCameraPosition != null && mode == 0 && !INGAME) {
			// first time we set mode, so we do not move camera but set it at the right position
			//Camera.main.transform.position = nextCameraPosition.transform.position;
			//Camera.main.transform.rotation = nextCameraPosition.transform.rotation;
			// next camera position to null so camera will not start moving ( nextCameraPosition is camera moving indicator variable )
			//nextCameraPosition = null;
		}		

		mode = pMode;
		cameraMovement = 0;		
	}

	private void switchToP1 ()
	{

		startCameraPosition = camP2;
		nextCameraPosition = camP1;
		cameraMovement = 0;	
	}

	private void switchToP2 ()
	{

		startCameraPosition = camP1;
		nextCameraPosition = camP2;
		cameraMovement = 0;	

	}

	// determine a random color
	string randomColor {
		get {
			string _color = "blue";
			int c = System.Convert.ToInt32 (Random.value * 6);
			switch (c) {
			case 0:
				_color = "red";
				break;
			case 1:
				_color = "green";
				break;
			case 2:
				_color = "blue";
				break;
			case 3:
				_color = "yellow";
				break;
			case 4:
				_color = "white";
				break;
			case 5:
				_color = "black";
				break;				
			}
			return _color;
		}
	}
	
	// Update is called once per frame <<< (!)
	void Update ()
	{		
		// if next camera position is set we are , or have to start moving the camera.
		if (nextCameraPosition != null)
			MoveCamera ();
								
		switch (mode) {
		case MODE_GALLERY:
				// gallery mode to update the gallery
			UpdateGallery ();
			break;
		case MODE_GAME:
				// rolling mode to update the dice rolling
			UpdateGame ();
			break;
		}
	}

	// Moving the camera
	void MoveCamera ()
	{
		// increment total moving time
		cameraMovement += Time.deltaTime * 1;
		// if we surpass the speed we have to cap the movement because we are 'slerping'
		if (cameraMovement > cameraMovementSpeed)
			cameraMovement = cameraMovementSpeed;

		// slerp (circular interpolation) the position between start and next camera position
		Camera.main.transform.position = Vector3.Slerp (startCameraPosition.transform.position, nextCameraPosition.transform.position, cameraMovement / cameraMovementSpeed);
		// slerp (circular interpolation) the rotation between start and next camera rotation
		Camera.main.transform.rotation = Quaternion.Slerp (startCameraPosition.transform.rotation, nextCameraPosition.transform.rotation, cameraMovement / cameraMovementSpeed);

		// stop moving if we arrived at the desired next camera postion
		if (cameraMovement == cameraMovementSpeed)
			nextCameraPosition = null;	
	}

	// updating the gallery
	void UpdateGallery ()
	{
		if (!PointInRect (GuiMousePosition (), rectModeSelect) && !PointInRect (GuiMousePosition (), rectGallerySelectBox)) {
			// mouse position is not on GUI mode selector or gallery selector
			if (Input.GetMouseButton (Dice.MOUSE_LEFT_BUTTON)) {
				// mouse left button is held down
				if (!dragging) {
					// start dragging 
					dragging = true;
					// remember where (mouse coords) we started to drag and what the start angle of the die was
					dragStart = Input.mousePosition;
					dragStartAngle = galleryDieObject.transform.eulerAngles;
					// stop the gallery die rotation
					galleryDieObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
				} else {
					// we are dragging
					Vector2 delta = Input.mousePosition;
					// calculate delta vector of the mouse position related to our drag start point
					delta -= dragStart;
					// normalize this vector so we can use it to determine the desired rotation angle
					Vector2 dn = delta.normalized;
					// initialize the rotation of gallery die to its starting point
					galleryDieObject.transform.eulerAngles = dragStartAngle;
					// if we move the mouse horizontal we want to rotate around the Y axis (Vector3.up -> normalized vector)
					Vector3 mouseXRotationVector = Vector3.up;
					// if we move the mouse vertical we want to rotate towards the camera. we calculate this normalized rotation
					// vector by using the vector from the camera to the gallery die and rotating it 45 degrees around the Y-axis
					Vector3 mouseYRotationVector = Vector3.Lerp (camGallery.transform.position, galleryDieObject.transform.position, 1).normalized;
					mouseYRotationVector = Quaternion.Euler (0, 45, 0) * mouseYRotationVector;
					// we only want to rotate around the X and Z axis when moving the mouse vertical so we set y-axis to 0
					mouseYRotationVector.y = 0;
					// calculate the right rotation angle and rotate the die around its position using the mouse position delta vector
					Vector3 angle = (mouseYRotationVector * dn.y) + (mouseXRotationVector * dn.x * -1);
					galleryDieObject.transform.RotateAround (galleryDieObject.transform.position, angle, delta.magnitude * .6F);
					// store this angle as the 'last' angle so we can determine the right rotation when we release
					// the mouse button and stop dragging
					dragLastAngle = angle;
				}
			} else if (Input.GetMouseButtonUp (Dice.MOUSE_LEFT_BUTTON) && dragging) {
				// left mouse button was released while we were dragging
				float force = .4F;
				dragging = false;
				// add correct torque to spin the gallery die
				galleryDieObject.GetComponent<Rigidbody> ().AddTorque (dragLastAngle.normalized * force, ForceMode.Impulse);
				return;
			}
		}
	}

	// GAME MECHANICS

	private void ResetForNewGame ()
	{

		CURRENT_SUBROUND = 0;
		CURRENT_PLAYER_NAME = "";
		INGAME = true;
		NEWGAME = true;
		NEWSUBROUND = true;

		first_player = 0;
		second_player = 0;

		button1_label = "";
		button2_label = "";
		button3_label = "";
		button4_label = "";

		dice1_label = "";
		dice2_label = "";
		dice3_label = "";
		dice4_label = "";
		dice5_label = "";

		p1d1 = 0;
		p1d2 = 0;
		p1d3 = 0;
		p1d4 = 0;
		p1d5 = 0;

		p2d1 = 0;
		p2d2 = 0;
		p2d3 = 0;
		p2d4 = 0;
		p2d5 = 0;

		reDice1 = "";
		reDice2 = "";
		reDice3 = "";
		reDice4 = "";
		reDice5 = "";

		global_reroll = 0;
		global_pool = 0;
		follow_bet = 0;
		current_task = "";
		player_1_bet = 0;
		player_2_bet = 0;

		winner_ = "";
		player1_hand = "";
		player2_hand = "";

		NEWSUBROUND = true;
	}

	IEnumerator Calculate_Winner ()
	{
		int highest_count_p1 = 0;
		int highest_count_p2 = 0;
		int p1paircount = 0;
		int p2paircount = 0;
		int p1threecount = 0;
		int p2threecount = 0;
		int p1fourcount = 0;
		int p2fourcount = 0;
		int p1fifthcount = 0;
		int p2fifthcount = 0;
		int p1highest = 0;
		int p2highest = 0;

		player1_hand = "Nothing";
		player2_hand = "Nothing";
		p1dices [0] = p1d1;
		p1dices [1] = p1d2;
		p1dices [2] = p1d3;
		p1dices [3] = p1d4;
		p1dices [4] = p1d5;
		p2dices [0] = p2d1;
		p2dices [1] = p2d2;
		p2dices [2] = p2d3;
		p2dices [3] = p2d4;
		p2dices [4] = p2d5;
		p1count [0] = 0;
		p1count [1] = 0;
		p1count [2] = 0;
		p1count [3] = 0;
		p1count [4] = 0;
		p1count [5] = 0;
		p1count [6] = 0;
		p2count [0] = 0;
		p2count [1] = 0;
		p2count [2] = 0;
		p2count [3] = 0;
		p2count [4] = 0;
		p2count [5] = 0;
		p2count [6] = 0;
		//
		for (int i = 0; i < p1dices.Length; i += 1) {
			if(p1dices[i] == 0){
				p1dices[i] = Random.Range(1,6);
			}
			if(p2dices[i] == 0){
				p2dices[i] = Random.Range(1,6);
			}
		}
		//
		for (int i = 0; i < p1dices.Length; i += 1) {
			for (int l = 0; l < p1count.Length; l += 1) {
				if (p1dices [i] == l) {
					p1count [l] += 1;

					if (p1count [l] > highest_count_p1) {
						highest_count_p1 = p1count [l];
					}
				}

			}
		}

		for (int i = 0; i < p2dices.Length; i += 1) {
			for (int l = 0; l < p2count.Length; l += 1) {
				if (p2dices [i] == l) {
					p2count [l] += 1;

					if (p2count [l] > highest_count_p2) {
						highest_count_p2 = p2count [l];
					}
				}

			}
		}

		for (int i = 0; i < p1count.Length; i += 1) {
			if (p1count [i] == 2) {
				p1paircount += 1;

				if (i > p1highest) {
					p1highest = i;
				}
			}
			if (p1count [i] == 3) {
				p1threecount += 1;

				if (i > p1highest) {
					p1highest = i;
				}
			}
			if (p1count [i] == 4) {
				p1fourcount += 1;

				if (i > p1highest) {
					p1highest = i;
				}
			}
			if (p1count [i] == 5) {
				p1fifthcount += 1;

				if (i > p1highest) {
					p1highest = i;
				}
			}
			if (p2count [i] == 2) {
				p2paircount += 1;

				if (i > p2highest) {
					p2highest = i;
				}
			}
			if (p2count [i] == 3) {
				p2threecount += 1;

				if (i > p2highest) {
					p2highest = i;
				}
			}
			if (p2count [i] == 4) {
				p2fourcount += 1;

				if (i > p2highest) {
					p2highest = i;
				}
			}
			if (p2count [i] == 5) {
				p2fifthcount += 1;

				if (i > p2highest) {
					p2highest = i;
				}
			}
		}

		for (int i = 0; i < 6 + 1; i += 1) {
			if(p1count [i] >= 2){
				p1highest = i;
			}
			if(p2count [i] >= 2){
				p2highest = i;
			}
		}

		if (highest_count_p1 <= 1) {
			player1_hand = "Street ";
		}

		if (highest_count_p1 == 2) {
			if (p1paircount == 1) {
				player1_hand = "Pair of a Kind ";
			} else {
				player1_hand = "Double Pair of a Kind ";
			}
		}

		if (highest_count_p1 == 3) {
			if (p1paircount == 1) {
				player1_hand = "Full House ";
			} else {
				player1_hand = "Three of a Kind ";
			}
		}

		if (highest_count_p1 == 4) {
			player1_hand = "Four of a Kind ";
		}

		if (highest_count_p1 == 5) {
			player1_hand = "Five of a Kind ";
		}

		// PLAYER 2

		if (highest_count_p2 <= 1) {
			player2_hand = "Street ";
		}

		if (highest_count_p2 == 2) {
			if (p2paircount == 1) {
				player2_hand = "Pair of a Kind ";
			} else {
				player2_hand = "Double Pair of a Kind ";
			}
		}

		if (highest_count_p2 == 3) {
			if (p2paircount == 1) {
				player2_hand = "Full House ";
			} else {
				player2_hand = "Three of a Kind ";
			}
		}

		if (highest_count_p2 == 4) {
			player2_hand = "Four of a Kind ";
			confetti.SetActive(true);
		}

		if (highest_count_p2 == 5) {
			player2_hand = "Five of a Kind ";
			confetti.SetActive(true);
		}

		//STREET 
		int winner_index = 0;

		int player1winningscore = System.Array.IndexOf (winning_arch, player1_hand);
		int player2winningscore = System.Array.IndexOf (winning_arch, player2_hand);
		if (player1winningscore == -1) {
			winner_index = 2;
		}
		if (player2winningscore == -1) {
			winner_index = 1;
		}

		if (player1winningscore == player2winningscore) {

			if (p1highest >= p2highest) {
				winner_index = 1;
			} else {
				winner_index = 2;
			}
		} else {

			if (player1winningscore > player2winningscore) {
				winner_index = 1;
			} else {
				winner_index = 2;
			}

		}

		if (winner_index == 1) {
			winner_ = player_1_name;
			player_1_coins += global_pool;

		} else {
			winner_ = player_2_name;
			player_2_coins += global_pool;
		}

		player1_hand = player1_hand + " ( " + p1highest + " highest.) ";
		player2_hand = player2_hand + " ( " + p2highest + " highest.) ";

		yield return new WaitForSeconds (1);
		calculating_winner = true;

	}

	// dertermine random rolling force
	private GameObject spawnPoint = null;

	private Vector3 Force ()
	{
		Vector3 rollTarget = Vector3.zero + new Vector3 (1 + 2 * Random.value, .5F + 5 * Random.value, -1 - 2 * Random.value);
		return Vector3.Lerp (spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
	}

	private void SetCurrentPlayerName ()
	{
		if (CURRENTLY_PLAYING == 2) {
			CURRENT_PLAYER_NAME = player_2_name;
			switchToP2 ();
		} else {
			CURRENT_PLAYER_NAME = player_1_name;
			switchToP1 ();
		}

	}

	void UpdateGame ()
	{



		if (INGAME) {

			if (NEWGAME) {
				NEWGAME = false;

				global_pool = 0;
				spawnPoint = GameObject.Find ("spawnPointP1");
				CURRENT_ROUND += 1;
				CURRENT_SUBROUND = 0;

				if (CURRENTLY_PLAYING == 0 || CURRENTLY_PLAYING == 2) {
					CURRENTLY_PLAYING = 1;
					first_player = 1;
					second_player = 2;

				} else {
					CURRENTLY_PLAYING = 2;
					first_player = 2;
					second_player = 1;

				}
				INGAME = true;
				NEWGAME = false;
				NEWSUBROUND = true;

			}

			if (NEWSUBROUND) {
				NEWSUBROUND = false;

				button1_label = "";
				button2_label = "";
				button3_label = "";
				button4_label = "";
				dice1_label = "";
				dice2_label = "";
				dice3_label = "";
				dice4_label = "";
				dice5_label = "";

				switch (CURRENT_SUBROUND) {
				case 0:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": please bet...";
					button1_label = "Small Blind";


					break;
				case 1:
					CURRENTLY_PLAYING = second_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Follow / Raise?";
					button1_label = "Follow";
					button2_label = "Raise";

					break;
				case 2:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName (); 
					current_task = CURRENT_PLAYER_NAME + ":Follow / Quit";
					button1_label = "Follow";
					button2_label = "Quit";

					break;
				case 3:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": please roll...";
					button1_label = "Roll";

					break;
				case 4:
					CURRENTLY_PLAYING = second_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": please roll...";
					button1_label = "Roll";

					break;
				case 5:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Bet small / big / Quit";
					if (CURRENTLY_PLAYING == 1) {
						dice1_label = "Dice1: " + p1d1;
						dice2_label = "Dice2: " + p1d2;
						dice3_label = "Dice3: " + p1d3;
						dice4_label = "Dice4: " + p1d4;
						dice5_label = "Dice5: " + p1d5;
					} else {
						dice1_label = "Dice1: " + p2d1;
						dice2_label = "Dice2: " + p2d2;
						dice3_label = "Dice3: " + p2d3;
						dice4_label = "Dice4: " + p2d4;
						dice5_label = "Dice5: " + p2d5;
					}
					button1_label = "Small";
					button2_label = "Big";
					button3_label = "Quit";

					break;
				case 6:
					CURRENTLY_PLAYING = second_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Follow / Raise / Quit";
					if (CURRENTLY_PLAYING == 1) {
						dice1_label = "Dice1: " + p1d1;
						dice2_label = "Dice2: " + p1d2;
						dice3_label = "Dice3: " + p1d3;
						dice4_label = "Dice4: " + p1d4;
						dice5_label = "Dice5: " + p1d5;
					} else {
						dice1_label = "Dice1: " + p2d1;
						dice2_label = "Dice2: " + p2d2;
						dice3_label = "Dice3: " + p2d3;
						dice4_label = "Dice4: " + p2d4;
						dice5_label = "Dice5: " + p2d5;
					}
					button1_label = "Follow";
					button2_label = "Raise";
					button3_label = "Quit";

					break;
				case 7:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Follow / Quit";
					if (CURRENTLY_PLAYING == 1) {
						dice1_label = "Dice1: " + p1d1;
						dice2_label = "Dice2: " + p1d2;
						dice3_label = "Dice3: " + p1d3;
						dice4_label = "Dice4: " + p1d4;
						dice5_label = "Dice5: " + p1d5;
					} else {
						dice1_label = "Dice1: " + p2d1;
						dice2_label = "Dice2: " + p2d2;
						dice3_label = "Dice3: " + p2d3;
						dice4_label = "Dice4: " + p2d4;
						dice5_label = "Dice5: " + p2d5;
					}
					button1_label = "Follow";
					button2_label = "Quit";
		
					break;
				case 8:
					CURRENTLY_PLAYING = first_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Select dices";
					button1_label = "Reroll";
					dice1_label = "" + p1d1;
					dice2_label = "" + p1d2;
					dice3_label = "" + p1d3;
					dice4_label = "" + p1d4;
					dice5_label = "" + p1d5;

					break;
				case 9:
					CURRENTLY_PLAYING = second_player;
					SetCurrentPlayerName ();
					current_task = CURRENT_PLAYER_NAME + ": Select dices";
					button1_label = "Reroll";
					dice1_label = "" + p2d1;
					dice2_label = "" + p2d2;
					dice3_label = "" + p2d3;
					dice4_label = "" + p2d4;
					dice5_label = "" + p2d5;
					break;
				case 10:
					StartCoroutine (Calculate_Winner ());
					break;
				}

				if (CURRENTLY_PLAYING == 2) {
					CURRENT_PLAYER_NAME = player_2_name;
					spawnPoint = GameObject.Find ("spawnPointP2");
				} else {
					CURRENT_PLAYER_NAME = player_1_name;
					spawnPoint = GameObject.Find ("spawnPointP1");
				}

			}

		}
	}

	public void DiceReRoll (int rerollsize)
	{

		string[] a = galleryDie.Split ('-');
		Dice.Roll ("" + rerollsize + a [0], galleryDie, spawnPoint.transform.position, Force ());

		StartCoroutine (DiceReRollWait ());
	}

	IEnumerator DiceReRollWait ()
	{
		yield return new WaitForSeconds (4);

		if (CURRENTLY_PLAYING == 2) {
			int current_counter = 0;
			if (p2d1 == -1) {
				p2d1 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p2d2 == -1) {
				p2d2 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p2d3 == -1) {
				p2d3 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p2d4 == -1) {
				p2d4 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p2d5 == -1) {
				p2d5 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
		} else {
			int current_counter = 0;
			if (p1d1 == -1) {
				p1d1 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p1d2 == -1) {
				p1d2 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p1d3 == -1) {
				p1d3 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p1d4 == -1) {
				p1d4 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
			if (p1d5 == -1) {
				p1d5 = Dice.ValueOf (current_counter);
				current_counter += 1;
			}
		}

		Dice.Clear ();

		if (CURRENT_SUBROUND == 9) {
			CURRENT_SUBROUND = 10;
			NEWSUBROUND = true;
		}

		if (CURRENT_SUBROUND == 8) {
			CURRENT_SUBROUND = 9;
			NEWSUBROUND = true;
		}
			
	}

	public void DiceRoll ()
	{

		string[] a = galleryDie.Split ('-');
		Dice.Roll ("5" + a [0], galleryDie, spawnPoint.transform.position, Force ());
		StartCoroutine (DiceRollWait ());
	}


	IEnumerator DiceRollWait ()
	{
		yield return new WaitForSeconds (4);

		if (CURRENTLY_PLAYING == 2) {
			p2d1 = Dice.ValueOf (0);
			p2d2 = Dice.ValueOf (1);
			p2d3 = Dice.ValueOf (2);
			p2d4 = Dice.ValueOf (3);
			p2d5 = Dice.ValueOf (4);

			if (p2d1 == 0) {
				p2d1 = Random.Range (1, 6);
			}
			if (p2d2 == 0) {
				p2d2 = Random.Range (1, 6);
			}
			if (p2d3 == 0) {
				p2d3 = Random.Range (1, 6);
			}
			if (p2d4 == 0) {
				p2d4 = Random.Range (1, 6);
			}
			if (p2d5 == 0) {
				p2d5 = Random.Range (1, 6);
			}

		} else {

			p1d1 = Dice.ValueOf (0);
			p1d2 = Dice.ValueOf (1);
			p1d3 = Dice.ValueOf (2);
			p1d4 = Dice.ValueOf (3);
			p1d5 = Dice.ValueOf (4);

			if (p1d1 == 0) {
				p1d1 = Random.Range (1, 6);
			}
			if (p1d2 == 0) {
				p1d2 = Random.Range (1, 6);
			}
			if (p1d3 == 0) {
				p1d3 = Random.Range (1, 6);
			}
			if (p1d4 == 0) {
				p1d4 = Random.Range (1, 6);
			}
			if (p1d5 == 0) {
				p1d5 = Random.Range (1, 6);
			}
		}
		Dice.Clear ();
		if (CURRENT_SUBROUND == 4) {
			CURRENT_SUBROUND = 5;
			NEWSUBROUND = true;
		}
		if (CURRENT_SUBROUND == 3) {
			CURRENT_SUBROUND = 4;
			NEWSUBROUND = true;
		}

	}

	public void clicked (string label)
	{

		if (label.Equals ("Small Blind") || label.Equals ("Small")) {

			if (CURRENTLY_PLAYING == 2) {
				player_2_coins -= 2;
				player_2_bet += 2;
			} else {
				player_1_coins -= 2;
				player_1_bet += 2;
			}
	
			global_pool += 2;

			if (CURRENT_SUBROUND == 0) {
				CURRENT_SUBROUND = 1;
				NEWSUBROUND = true;
			}

			if (CURRENT_SUBROUND == 5) {
				CURRENT_SUBROUND = 6;
				NEWSUBROUND = true;
			}
		}

		if (label.Equals ("Raise")) {
			follow_bet = 0;

			if (CURRENTLY_PLAYING == 2) {
				follow_bet = player_1_bet - player_2_bet;
				player_2_coins -= follow_bet;
				player_2_bet += follow_bet;
				player_2_coins -= 2;
				player_2_bet += 2;
			} else {
				follow_bet = player_2_bet - player_1_bet;
				player_1_coins -= follow_bet;
				player_1_bet += follow_bet;
				player_1_coins -= 2;
				player_1_bet += 2;
			}

			global_pool += follow_bet + 2;

			if (CURRENT_SUBROUND == 1) {
				CURRENT_SUBROUND = 2;
				NEWSUBROUND = true;
			}

			if (CURRENT_SUBROUND == 6) {
				CURRENT_SUBROUND = 7;
				NEWSUBROUND = true;
			}
		}

		if (label.Equals ("Follow")) {
			follow_bet = 0;

			if (CURRENTLY_PLAYING == 2) {
				follow_bet = player_1_bet - player_2_bet;
				player_2_coins -= follow_bet;
				player_2_bet += follow_bet;
			} else {
				follow_bet = player_2_bet - player_1_bet;
				player_1_coins -= follow_bet;
				player_1_bet += follow_bet;
			}
		
			global_pool += follow_bet;

			if (CURRENT_SUBROUND == 1) { 
				CURRENT_SUBROUND = 3;
				NEWSUBROUND = true;
			}

			if (CURRENT_SUBROUND == 2) {
				CURRENT_SUBROUND = 3;
				NEWSUBROUND = true;
			}

			if (CURRENT_SUBROUND == 6) {
				CURRENT_SUBROUND = 8;
				NEWSUBROUND = true;
			}
			if (CURRENT_SUBROUND == 7) {
				CURRENT_SUBROUND = 8;
				NEWSUBROUND = true;
			}
		}

		if (label.Equals ("Quit")) {
			QuitIngame ();
		}

		if (label.Equals ("Roll")) {
			global_reroll = 0;
			DiceRoll ();
			button1_label = "";

		}

		if (label.Equals ("Reroll")) {

			dice1_label = "";
			dice2_label = "";
			dice3_label = "";
			dice4_label = "";
			dice5_label = "";

			DiceReRoll (global_reroll);
			button1_label = "";
			global_reroll = 0;

		}


		if (label.Equals ("Big Blind") || label.Equals ("Big")) {

			if (CURRENTLY_PLAYING == 2) {
				player_2_coins -= 4;
				player_2_bet += 4;
			} else {
				player_1_coins -= 4;
				player_1_bet += 4;
			}
	
			global_pool += 4;

			if (CURRENT_SUBROUND == 5) {
				CURRENT_SUBROUND = 6;
				NEWSUBROUND = true;
			}
		}


		// NEXT SUB ROUND
		if (!label.Equals ("Roll")) {

		} else {
			//DiceWait();
		}

	}

	void ResetAll ()
	{
		player_1_name = "Player 1";
		player_2_name = "Player 2";
		player_1_coins = 25;
		player_2_coins = 25;
		global_pool = 0;
		CURRENT_ROUND = 0;
		CURRENT_SUBROUND = 0;
		CURRENTLY_PLAYING = 0;
		INGAME = false;
		NEWGAME = true;
		SetMode (MODE_GALLERY);
	}

	public int IntParseFast (string value)
	{
		int result = 0;
		for (int i = 0; i < value.Length; i++) {
			char letter = value [i];
			result = 10 * result + (letter - 48);
		}
		return result;
	}

	void dice1select (string label)
	{

		if (label.Equals ("- REROLL -")) {
			dice1_label = "" + reDice1;
			global_reroll -= 1;
			if (CURRENTLY_PLAYING == 1) {
				p1d1 = IntParseFast (dice1_label);
			} else {
				p2d1 = IntParseFast (dice1_label);
			}
		}

		if (label.Equals ("1") || label.Equals ("2") || label.Equals ("3") || label.Equals ("4") || label.Equals ("5") || label.Equals ("6")) {
			reDice1 = label;
			label = "- REROLL -";
			dice1_label = "- REROLL -";
			if (CURRENTLY_PLAYING == 2) {
				p2d1 = -1;
			} else {
				p1d1 = -1;
			}
			global_reroll += 1;
		}



			
	}

	void dice2select (string label)
	{

		if (label.Equals ("- REROLL -")) {
			dice2_label = "" + reDice2;
			global_reroll -= 1;
			if (CURRENTLY_PLAYING == 1) {
				p1d2 = IntParseFast (dice2_label);
			} else {
				p2d2 = IntParseFast (dice2_label);
			}
		}

		if (label.Equals ("1") || label.Equals ("2") || label.Equals ("3") || label.Equals ("4") || label.Equals ("5") || label.Equals ("6")) {
			reDice2 = label;

			label = "- REROLL -";
			dice2_label = "- REROLL -";
			if (CURRENTLY_PLAYING == 2) {
				p2d2 = -1;
			} else {
				p1d2 = -1;
			}
			global_reroll += 1;
		}
						
	}

	void dice3select (string label)
	{

		if (label.Equals ("- REROLL -")) {
			dice3_label = "" + reDice3;
			global_reroll -= 1;
			if (CURRENTLY_PLAYING == 1) {
				p1d3 = IntParseFast (dice3_label);
			} else {
				p2d3 = IntParseFast (dice3_label);
			}
		}

		if (label.Equals ("1") || label.Equals ("2") || label.Equals ("3") || label.Equals ("4") || label.Equals ("5") || label.Equals ("6")) {
			
			reDice3 = label;

			label = "- REROLL -";
			dice3_label = "- REROLL -";
			if (CURRENTLY_PLAYING == 2) {
				p2d3 = -1;
			} else {
				p1d3 = -1;
			}
			global_reroll += 1;
		}

	}

	void dice4select (string label)
	{

		if (label.Equals ("- REROLL -")) {
			dice4_label = "" + reDice4;
			global_reroll -= 1;
			if (CURRENTLY_PLAYING == 1) {
				p1d4 = IntParseFast (dice4_label);
			} else {
				p2d4 = IntParseFast (dice4_label);
			}
		}

		if (label.Equals ("1") || label.Equals ("2") || label.Equals ("3") || label.Equals ("4") || label.Equals ("5") || label.Equals ("6")) {
			reDice4 = label;

			label = "- REROLL -";
			dice4_label = "- REROLL -";
			if (CURRENTLY_PLAYING == 2) {
				p2d4 = -1;
			} else {
				p1d4 = -1;
			}
			global_reroll += 1;
		}

	}

	void dice5select (string label)
	{

		if (label.Equals ("- REROLL -")) {
			dice5_label = "" + reDice5;
			global_reroll -= 1;
			if (CURRENTLY_PLAYING == 1) {
				p1d5 = IntParseFast (dice5_label);
			} else {
				p2d5 = IntParseFast (dice5_label);
			}
		}

		if (label.Equals ("1") || label.Equals ("2") || label.Equals ("3") || label.Equals ("4") || label.Equals ("5") || label.Equals ("6")) {
			reDice5 = label;

			label = "- REROLL -";
			dice5_label = "- REROLL -";
			if (CURRENTLY_PLAYING == 2) {
				p2d5 = -1;
			} else {
				p1d5 = -1;
			}
			global_reroll += 1;
		}

	}
	// handle GUI
	void OnGUI ()
	{
		GUI.skin.font = ui_font;
		GUI.skin.box.font = ui_font;
		GUI.skin.label.font = ui_font;
		GUI.skin.button.font = ui_font;

		if( ui_finalscreen_on )
		{
			if(!confetti.activeInHierarchy){
			confetti.SetActive(true);
			}

			GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "FINAL WINNER");
			int fs = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 23;
			GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 150, 440, 60), winner_ + " is the final winner!");
			GUI.skin.label.fontSize = fs;




			if (GUI.Button (new Rect (Screen.width / 2 - 220, Screen.height / 2 + 60, 440, 40), "NEXT GAME")) {

				confetti.SetActive(false);

					ResetAll ();
					calculating_winner = false;
					ResetForNewGame ();
					SetMode (MODE_GAME);
				ui_finalscreen_on = false;



			}


		}else{
		if (credits_on) {

			GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "");
			GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "");
			GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "CREDITS");

			GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 150, 440, 440), "Created by Leonard Stemmildt \nCreated with Unity 5.5.0b11\n\n\n Used Unity 3d Packs:\nSimple Sky\nHitori Yubin Soundpack\nSimple Dice by WyrmTale\nAnimated Knight by masatomo\nLowPoly Trees by Area730");

			if (GUI.Button (new Rect (Screen.width / 2 - 220, Screen.height / 2 + 40, 440, 40), "BACK")) {
				credits_on = false;


			}


		} else {
			if (loadingGame) {
				GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");
				GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");
				GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");
				GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");
				GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");GUI.Box (new Rect (-50, -50, Screen.width + 100, Screen.height + 100), "");
				int fs = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = 62;
				GUI.Label (new Rect (40, Screen.height - 100, 400, 100), "LOADING");
				GUI.skin.label.fontSize = fs;
			} else {

				if (settings_on) {

					GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "");
					GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "");
					GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "SETTINGS");

					//GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 150, 440, 20), winner_ + " is the winner!");
					GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 80, 440, 20), "PLAYER1 NAME:");
					player_1_name = GUI.TextField (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 60, 440, 20), player_1_name);


					GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 40, 440, 20), "PLAYER2 NAME:");	
					player_2_name = GUI.TextField (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 20, 440, 20), player_2_name);

					if (GUI.Button (new Rect (Screen.width / 2 - 220, Screen.height / 2 + 40, 440, 40), "Set Player Names")) {
						settings_on = false;


					}


				} else {

					if (!calculating_winner) {
						// Make mode selection box
						GUI.Box (rectModeSelect, "");

						if (GUI.Button (new Rect (20, 20, 160, 30), "NEW PLAYERS")) {
							ResetAll ();
							settings_on = true;
						}

						if (GUI.Button (new Rect (20, 50, 160, 25), "NEXT ROUND")) {
							SetMode (MODE_GAME);
						}
				
						if (GUI.Button (new Rect (20, 80, 160, 25), "MAIN MENU")) {
							SetMode (MODE_GALLERY);
						}
		
		
						if (GUI.Button (new Rect (20, 105, 160, 25), "SETTINGS")) {
							settings_on = true;

						}

						if (GUI.Button (new Rect (20, 135, 160, 25), "CREDITS")) {
							credits_on = true;

						}

						if (GUI.Button (new Rect (20, 160, 160, 30), "EXIT APP")) {
							Application.Quit ();

						}

						switch (mode) {
						case MODE_GALLERY:
							if (nextCameraPosition == null) {
								// camera is not moving so display dice selector
								GUI.Box (rectGallerySelectBox, "v 1.0");						
								if (txSelector == null) {
									// determine dieType dependent selection texture
									string add = "";
									if (galleryDie.IndexOf ("-dots") >= 0)
                            // dice with dots found so we have to append -dots when loading material
                            add = "-dots";
									// we have to load our selector texture
									txSelector = (Texture)Resources.Load ("Textures/GUI-selector/select-" + galleryDie.Split ('-') [0] + add);
								}
                    
								if (txSelector != null) {
									// draw our selector texture
									GUI.DrawTexture (rectGallerySelect, txSelector, ScaleMode.ScaleToFit, true, 0f);
								}

								// check current mouseposition against selector
								string status = CheckSelection (rectGallerySelect);
								if (status == "")
									status = "[select color]";

								// display status label
								GUI.Label (new Rect (Screen.width - 245, 145, 230, 20), status);					
							}


							GUI.Box (new Rect (Screen.width - 260, 190, 250, 30), "");
							GUI.Label (new Rect (Screen.width - 254, 193, 246, 26), "Rotate the die by dragging.");
							break;

						case MODE_GAME:
                // display rolling message on bottom

				//GUI.Box(new Rect(10, 200, 180, 500), "");
							GUI.Box (rectGallerySelectBox, "Scoreboard");
							GUI.Label (new Rect (Screen.width - 245, 45, 230, 20), player_1_name);	
							GUI.Label (new Rect (Screen.width - 145, 45, 230, 20), player_2_name);
		
							GUI.Label (new Rect (Screen.width - 245, 65, 230, 20), player_1_coins + " Coins");	
							GUI.Label (new Rect (Screen.width - 145, 65, 230, 20), player_2_coins + " Coins");	

							GUI.Label (new Rect (Screen.width - 245, 110, 230, 20), "Current Pool:" + global_pool);

							int fs = GUI.skin.label.fontSize;
							GUI.skin.label.fontSize = 18;
							GUI.Label (new Rect (20, Screen.height - 120, 400, 30), "now playing");
							GUI.skin.label.fontSize = 62;
							GUI.Label (new Rect (20, Screen.height - 100, 400, 100), CURRENT_PLAYER_NAME);

							GUI.skin.label.fontSize = 22;
							if (CURRENTLY_PLAYING == 1) {
								GUI.Label (new Rect (20, Screen.height - 40, 400, 30), "Coins: " + player_1_coins);
							} else {
								GUI.Label (new Rect (20, Screen.height - 40, 400, 30), "Coins: " + player_2_coins);
							}
							GUI.skin.label.fontSize = fs;
			//button1_label = "test";button2_label = "test";
							if (!button1_label.Equals ("")) {
								if (GUI.Button (new Rect (Screen.width - 260, 180, 250, 40), button1_label)) {
									clicked (button1_label);
								}
							}
							if (!button2_label.Equals ("")) {
								if (GUI.Button (new Rect (Screen.width - 260, 220, 250, 40), button2_label)) {
									clicked (button2_label);
								}
							}
							if (!button3_label.Equals ("")) {
								if (GUI.Button (new Rect (Screen.width - 260, 260, 250, 40), button3_label)) {
									clicked (button3_label);
								}
							}
							if (!button4_label.Equals ("")) {
								if (GUI.Button (new Rect (Screen.width - 260, 300, 250, 40), button4_label)) {
									clicked (button4_label);
								}
							}

							if (!dice1_label.Equals ("")) {
								if (GUI.Button (new Rect (20, 240, 160, 40), dice1_label)) {
									dice1select (dice1_label);

								}
							}
							if (!dice2_label.Equals ("")) {
								if (GUI.Button (new Rect (20, 280, 160, 40), dice2_label)) {
									dice2select (dice2_label);
								}
							}
							if (!dice3_label.Equals ("")) {
								if (GUI.Button (new Rect (20, 320, 160, 40), dice3_label)) {
									dice3select (dice3_label);
								}
							}
							if (!dice4_label.Equals ("")) {
								if (GUI.Button (new Rect (20, 360, 160, 40), dice4_label)) {
									dice4select (dice4_label);
								}
							}
							if (!dice5_label.Equals ("")) {
								if (GUI.Button (new Rect (20, 400, 160, 40), dice5_label)) {
									dice5select (dice5_label);
								}
							}



							GUI.Label (new Rect (Screen.width - 245, 150, 230, 20), current_task);
               //  GUI.Box(new Rect(Screen.width-538, Screen.height-40, 540, 40), "");

               // GUI.Label(new Rect((Screen.width - 530), Screen.height - 32, 520, 22), "Click with the left (all die types) or right (gallery die) mouse button in the center to roll.");
							if (Dice.Count ("") > 0) {
								// we have rolling dice so display rolling status
								//GUI.Box(new Rect( 10 , Screen.height - 75 , Screen.width - 20 , 30), "");
								// GUI.Label(new Rect(20, Screen.height - 70, Screen.width, 20), Dice.AsString(""));
							}

							break;
						}				
					} else {

						GUI.Box (new Rect (Screen.width / 2-250, Screen.height / 4, 500, Screen.height / 2), "");
						GUI.Box (new Rect (Screen.width / 2-250, Screen.height / 4, 500, Screen.height / 2), "");
						GUI.Box (new Rect (Screen.width / 2-250, Screen.height / 4, 500, Screen.height / 2), "SCORE");
						int fs = GUI.skin.label.fontSize;
						GUI.skin.label.fontSize = 23;
						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 150, 440, 60), winner_ + " is the winner!");
						GUI.skin.label.fontSize = fs;

						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 80, 440, 20), player_1_name + "'s hand: " + player1_hand);
						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 60, 440, 20), player_2_name + "'s hand: " + player2_hand);

						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 40, 440, 20), player_1_name + ": ");	
						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 0, 440, 20), player_2_name + ": ");
						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 - 20, 440, 20), player_1_coins + " Coins");	
						GUI.Label (new Rect (Screen.width / 2 - 220, Screen.height / 2 + 20, 440, 20), player_2_coins + " Coins");	

							string buttonSTR = "NEXT ROUND";
							if (player_1_coins <= 0 || player_2_coins <= 0) {
								buttonSTR = "FINAL WINNER! >>";
							}
				
							if (GUI.Button (new Rect (Screen.width / 2 - 220, Screen.height / 2 + 60, 440, 40), buttonSTR)) {

							confetti.SetActive(false);
							if (player_1_coins <= 0 || player_2_coins <= 0) {
								ui_finalscreen_on = true;
							} else {
								calculating_winner = false;
								ResetForNewGame ();
								SetMode (MODE_GAME);
							}
				


						}



					}

				}
			}

		}

		}
	}

	// check if a point is within a rectangle
	private bool PointInRect (Vector2 p, Rect r)
	{
		return  (p.x >= r.xMin && p.x <= r.xMax && p.y >= r.yMin && p.y <= r.yMax);
	}

	private string CheckSelection (Rect r)
	{
		string status = "";
		// mlb is true when left mouse button is clicked
		bool mlb = Input.GetMouseButtonDown (Dice.MOUSE_LEFT_BUTTON);
		// determine current GUI mouse position
		Vector2 mp = GuiMousePosition ();
		// check current GUI mouse position 
		txSelector = null;
		SetGalleryDie ("d6-" + galleryDie.Split ('-') [1] + "-dots");
        
		if (PointInRect (mp, new Rect (r.xMin + 12, r.yMin + 70, 200, 28))) {
			// we are in dice color selection so set active color if mouse button was clicked

			// check if we had a d6 with dots
			string add = "";
			if (galleryDie.IndexOf ("-dots") >= 0)
                    // dice with dots found so we have to append -dots when loading material
                    add = "-dots";               
                    
			if (mp.x - r.xMin < 45) {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-red" + add);
				status = "red";
			} else if (mp.x - r.xMin < 75) {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-black" + add);
				status = "black";
			} else if (mp.x - r.xMin < 115) {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-white" + add);
				status = "white";
			} else if (mp.x - r.xMin < 147) {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-yellow" + add);
				status = "yellow";
			} else if (mp.x - r.xMin < 180) {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-green" + add);
				status = "green";
			} else {
				if (mlb)
					SetGalleryDie (galleryDie.Split ('-') [0] + "-blue" + add);
				status = "blue";
			}
		}
		return status;	
	}

	public void QuitIngame ()
	{
		calculating_winner = false;

		if(CURRENTLY_PLAYING == 1){

			player_2_coins += global_pool;
		}else{

			player_1_coins += global_pool;
		}


		ResetForNewGame ();
		SetMode (MODE_GALLERY);
	}

	// translate Input mouseposition to GUI coordinates using camera viewport
	private Vector2 GuiMousePosition ()
	{
		Vector2 mp = Input.mousePosition;
		Vector3 vp = Camera.main.ScreenToViewportPoint (new Vector3 (mp.x, mp.y, 0));
		mp = new Vector2 (vp.x * Camera.main.pixelWidth, (1 - vp.y) * Camera.main.pixelHeight);
		return mp;
	}

	// set spcific gallery die type
	void SetGalleryDie (string die)
	{
		Vector3 newRotation = new Vector3 (-90, -65, 0);
		Vector4 angleVelocity = Vector3.zero;
		// destroy current gallery die if we have one
		if (galleryDie != "" && galleryDieObject != null) {
			// save rotation and angle velocity so we can set it on the new die later
			newRotation = galleryDieObject.transform.eulerAngles;
			angleVelocity = galleryDieObject.GetComponent<Rigidbody> ().angularVelocity;
			galleryDieObject.SetActive (false);
			// destroy die gameObject
			GameObject.Destroy (galleryDieObject);
		}		
		galleryDie = die;
		string[] a = galleryDie.Split ('-');						
		GameObject g = GameObject.Find ("platform-2");
		if (g != null) {
			// create the new die
			galleryDieObject = Dice.prefab (a [0], g.transform.position + new Vector3 (0, 3.8F, 0), newRotation, new Vector3 (1.4f, 1.4f, 1.4f), die);
			// disable rigidBody gravity
			galleryDieObject.GetComponent<Rigidbody> ().useGravity = false;
			// add saved angle and angle velocity or torque impulse
			if (angleVelocity.x == 0 && angleVelocity.y == 0 && angleVelocity.z == 0)
				galleryDieObject.GetComponent<Rigidbody> ().AddTorque (new Vector3 (0, -.4F, 0), ForceMode.Impulse);
			else
				galleryDieObject.GetComponent<Rigidbody> ().angularVelocity = angleVelocity;
		}
	}
		
}


// USING SIMPLE DICE

/**
 * Copyright (c) 2010-2015, WyrmTale Games and Game Components
 * All rights reserved.
 * http://www.wyrmtale.com
 *
 * THIS SOFTWARE IS PROVIDED BY WYRMTALE GAMES AND GAME COMPONENTS 'AS IS' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL WYRMTALE GAMES AND GAME COMPONENTS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */ 
