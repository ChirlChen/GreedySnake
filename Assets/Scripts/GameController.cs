using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum EGameElement
{
	EGE_Snake0 = 0,
	EGE_Snake1,
	EGE_Food,
	EGE_Block,

	EGE_Cnt,
}

public class GameController : MonoBehaviour
{
	public GameObject[] prefabs = new GameObject[(int)EGameElement.EGE_Cnt];

	private static int playerCnt = 2;
	private static int AiCnt = 1;
	private int maxFoodCnt = 10;
	private int foodInterval = 2;
	// S生成食物的间隔s。
	private Text[] scores = new Text[playerCnt];
	private Text[] lifeInfos = new Text[playerCnt];
	private GameObject[] players = new GameObject[playerCnt];
	private GameObject[] AiPlayers = new GameObject[AiCnt];

	private Snake[] snakeObjs = new Snake[playerCnt];
	private GameObject[] foods;
	private bool[] isInQueue;
	private List<GameObject> blocks = new List<GameObject> ();
	//记录食物是否已显示；
	private Queue<int> disactiveFoods = new Queue<int> ();
	private Button pause;
	// Use this for initialization
	void Start ()
	{
		//绑定按钮事件;
		pause = GlobalControl.Instance.FindButton ("Pause");
		//添加点击侦听
		pause.onClick.AddListener (Pause);

		Button btn = GlobalControl.Instance.FindButton ("Back");
		btn.onClick.AddListener (Back);

		//创建玩家； 
		for (int i = 0; i < playerCnt; ++i) {
			
			players [i] = (GameObject)Instantiate (prefabs [i], new Vector3 (-40.0f, 0.0f, 0.0f), Quaternion.identity);

		}
			
		//创建Ai
		for (int i = 0; i < AiCnt; ++i) {
			AiPlayers [i] = (GameObject)Instantiate (prefabs [i % 2], new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity);
			Snake sn = AiPlayers [i].gameObject.GetComponent<Snake> ();
			sn.SetColor (Color.red);
			sn.SetAI (true);
		}


		//获取地图size
		MeshRenderer mesh = GameObject.Find ("Background").GetComponent<MeshRenderer> ();
		Vector3 ext = mesh.bounds.extents;
		Vector3 ctr = mesh.bounds.center;
		GlobalControl.Instance.mapWidth = ext.x;
		GlobalControl.Instance.mapHeight = ext.y;
		GlobalControl.Instance.mapCenter = ctr;

		//创建地图围墙
		PlotWall ();

		//设置提示信息;
		for (int i = 0; i < playerCnt; i++) {
			scores [i] = GameObject.Find ("PlayerScore" + i.ToString ()).GetComponent<Text> ();   
			snakeObjs [i] = players [i].gameObject.GetComponent<Snake> ();
			lifeInfos [i] = GameObject.Find ("Revive" + i.ToString ()).GetComponent<Text> ();   
			lifeInfos [i].text = "life :";
		}
      
		//Debug.Log ("--------------------------scores.text:" + scores [0]);
		//产生食物；
		foods = new GameObject[maxFoodCnt];
		isInQueue = new bool[maxFoodCnt];
		for (int i = 0; i < maxFoodCnt; ++i) {
			GameObject obj = (GameObject)Instantiate (prefabs [2], new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity);
			obj.SetActive (false);
			foods [i] = obj;
			isInQueue [i] = true;

			disactiveFoods.Enqueue (i);
		}
		InvokeRepeating ("CreateFoodRandomly", 0, foodInterval);
	}

	void PlotWall ()
	{
		GameObject gobj = prefabs [(int)EGameElement.EGE_Block];
		Vector3 blockSize = gobj.GetComponent<Transform> ().localScale * 1.2f;
		float mapWidth = GlobalControl.Instance.mapWidth;
		float mapHeight = GlobalControl.Instance.mapHeight;

		int vLoopCnt = (int)(mapHeight / blockSize.y);
		int hLoopCnt = (int)(mapWidth / blockSize.x);
		float vBoardSide = mapWidth - blockSize.x;
		float hBoardSide = mapHeight - blockSize.y;

		for (int i = 1; i <= vLoopCnt; ++i) {
			Vector3[] pos = {new Vector3 (vBoardSide, i * blockSize.y, 20.0f), new Vector3 (vBoardSide, -(i * blockSize.y), 20.0f),
				new Vector3 (-vBoardSide, i * blockSize.y, 20.0f), new Vector3 (-vBoardSide, -(i * blockSize.y), 20.0f)
			};
			for (int j = 0; j < 4; ++j) {
				GameObject obj = (GameObject)Instantiate (gobj, pos [j], Quaternion.identity);					
				blocks.Add (obj);
			}
		}

		for (int i = 1; i <= hLoopCnt; ++i) {
			Vector3[] pos = {new Vector3 (i * blockSize.x, hBoardSide, 20.0f), new Vector3 (-(i * blockSize.x), hBoardSide, 20.0f),
				new Vector3 (i * blockSize.x, -hBoardSide, 20.0f), new Vector3 (-(i * blockSize.x), -hBoardSide, 20.0f)
			};
			for (int j = 0; j < 4; ++j) {
				GameObject obj = (GameObject)Instantiate (gobj, pos [j], Quaternion.identity);					
				blocks.Add (obj);
			}
		}

	}

	void CreateFoodRandomly ()
	{
		//print ("There are " + (maxFoodCnt - disactiveFoods.Count).ToString () + " stars \n");
		for (int i = 0; i < maxFoodCnt; ++i) {
			if (!isInQueue [i] && !foods [i].activeSelf) {
				disactiveFoods.Enqueue (i);
				isInQueue [i] = true;
			}
		}
		if (disactiveFoods.Count != 0) {
			float x = GlobalControl.Instance.GetRandomX ();
			float y = GlobalControl.Instance.GetRandomY ();

			int top = disactiveFoods.Dequeue ();
			foods [top].transform.position = new Vector3 (x, y, 0.0f) + GlobalControl.Instance.mapCenter;
			foods [top].SetActive (true);
			isInQueue [top] = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		SetText ();
	}

	void SetText ()
	{
		for (int i = 0; i < playerCnt; ++i) {
			Snake sn = snakeObjs [i];
			scores [i].text = "Player " + i.ToString () + ": " + sn.GetScore ().ToString ();
			//print ("Current Score = " + scores [i].text);
			if (!sn.IsDead ())
				lifeInfos [i].text = "life: " + sn.life.ToString ();
			else if (sn.life > 0)
				lifeInfos [i].text = "Waiting for revive...";
			else
				lifeInfos [i].text = "Your game is over...";
		}
	}

	void OnGUI ()  //按键分发；
	{
		Event e = Event.current;
		if (e.isKey) {
			KeyCode keyCode = e.keyCode;
			if (keyCode == KeyCode.W || keyCode == KeyCode.S || keyCode == KeyCode.A || keyCode == KeyCode.D) {
				snakeObjs [0].GetKeyCode (keyCode);
			} else if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow
			           || keyCode == KeyCode.LeftArrow || keyCode == KeyCode.RightArrow) {
				snakeObjs [1].GetKeyCode (keyCode);
			}
		}
	}


	void Pause ()
	{
		Text text = pause.transform.Find ("Text").GetComponent<Text> ();
		if (text.text == "Pause") {
			Time.timeScale = 0;
			text.text = "Continue";
		} else {
			Time.timeScale = 1.0f;
			text.text = "Pause";
		}

	}



	void Back ()
	{
		SceneManager.LoadScene (0);
		//Application.LoadLevel (0);
	}
}
