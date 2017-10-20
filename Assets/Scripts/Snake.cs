using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESnakeComponent
{
	ESC_Head = 0,
	ESC_Body = 1,
	ESC_Corner = 2,
	//拐角
	ESC_Tail = 3,

	ESC_Cnt,
}

public class Snake : MonoBehaviour
{
	public GameObject[] prefabs = new GameObject[(int)ESnakeComponent.ESC_Cnt];

	public Vector2 dirctHead;
	public Vector2 dirctTail;
	public int life;

	private List<GameObject> bodys = new List<GameObject> ();
	private SnakeHead sh;

	private bool isDead;
	private bool isAI;
	private float speed;
	private int totalScore = 0;
	private Color color = Color.white;

	public int GetScore ()
	{
		return totalScore;
		//head.score;
	}

	public bool IsDead ()
	{
		return isDead;
	}

	public void SetAI (bool ai)
	{
		if (ai) {
			isAI = ai;
			speed *= 1.0f;

			InvokeRepeating ("AiRun", 0.0f, Random.Range (2.0f, 3.0f));
			InvokeRepeating ("CheckAiIsCloseToWall", 0.0f, 0.1f);
		}
	}

	public void SetColor (Color c)
	{
		color = c;
	}

	public void GetNewLife ()
	{
		isDead = false;
		dirctHead = Vector2.up;
		dirctTail = Vector2.up;
		foreach (GameObject tmp in bodys) {
			if (tmp != null) {
				Destroy (tmp.gameObject);	
			}
		}
		bodys.Clear ();

		//创建初始的三节点蛇，头 身 尾
		InitOneSnake ();

		if (!isAI)
			life -= 1;

		InvokeRepeating ("SnakeMove", 0.0f, speed);
	}

	void AiRun ()
	{
		if (!isDead) {
			/*string prev = "Body";
			foreach (GameObject obj in bodys) {
				obj.tag = "Self";
			}*/
			Vector2 dir = sh.CalcDirction (dirctHead);
			RoateHead (dir);	

			/*foreach (GameObject obj in bodys) {
				obj.tag = prev;
			}*/
		}
			
	}

	void CheckAiIsCloseToWall ()
	{
		if (isAI && !isDead && GlobalControl.Instance.IsCloseToWall (sh.transform.position)) { //如果AITail靠近墙体，则调用Ai算法，求方向;
			//Debug.Log ("-------------------Too close to wall--------------------");
			AiRun ();
		}
	}

	void InitOneSnake ()
	{
		float x = GlobalControl.Instance.GetRandomX ();
		float y = 0.0f;//GlobalControl.Instance.GetRandomY ();
		float gridSize = GlobalControl.Instance.gridSize;

		GameObject obj = (GameObject)Instantiate (prefabs [(int)ESnakeComponent.ESC_Head], new Vector3 (x, y + gridSize, 0.0f), Quaternion.identity);
		sh = obj.gameObject.GetComponent<SnakeHead> ();

		obj = (GameObject)Instantiate (prefabs [(int)ESnakeComponent.ESC_Body], new Vector3 (x, y + 0.0f, 0.0f), Quaternion.identity);
		RenderColor (obj);
		bodys.Add (obj);

		obj = (GameObject)Instantiate (prefabs [(int)ESnakeComponent.ESC_Tail], new Vector3 (x, y - gridSize, 0.0f), Quaternion.identity);
		bodys.Add (obj);
		RenderColor (obj);
	}

	public void GetKeyCode (KeyCode keycode)  //从上层获取按键分发;
	{
		if (!isDead) {
			Vector2 dirct = ChangeDirction (keycode);
			RoateHead (dirct);   //蛇头带动蛇身动，此处为整个蛇的移动入口；
		} else {
			print ("Sorry, you are dead!\n");
		}
	}

	void RenderColor (GameObject obj)
	{
		if (obj != null) {
			SpriteRenderer sr = obj.gameObject.GetComponent<SpriteRenderer> ();
			sr.color = color;
		}
	}
		
	// Update is called once per frame
	void Update ()
	{
		
	}

	void Start ()
	{
		speed = 0.125f;
		life = 5;
		GetNewLife ();
	}

	void FixedUpdate ()
	{
		
	}


	void SnakeMove ()
	{
		if (CheckAlive ()) {  //是否活着；
			Vector3 pos = sh.transform.position; //头结点移动前;
			sh.transform.position += new Vector3 (dirctHead.x, dirctHead.y, 0.0f) * GlobalControl.Instance.gridSize;
			BodyMove (pos);	

			//Debug.Log ("-----------x,y--------------" + sh.transform.position);
		} else if (life > 0) { //生命大于0，则等待重生
			Invoke ("GetNewLife", 5.0f);
			isDead = true;
		} else
			isDead = true;
	}

	void RoateHead (Vector2 newDir)
	{
		float deltaAng = GlobalControl.Instance.CalcAngle (dirctHead, newDir);
		dirctHead = newDir;

		sh.transform.Rotate (0.0f, 0.0f, deltaAng);
		SnakeMove ();
	}

	Vector2 ChangeDirction (KeyCode keyCode)
	{
		Vector2 dirct = dirctHead;
		if ((keyCode == KeyCode.W || keyCode == KeyCode.UpArrow) && dirct != Vector2.down) {
			dirct = Vector2.up;
		}
		if ((keyCode == KeyCode.S || keyCode == KeyCode.DownArrow) && dirct != Vector2.up) {

			dirct = Vector2.down;
		}
		if ((keyCode == KeyCode.A || keyCode == KeyCode.LeftArrow) && dirct != Vector2.right) {

			dirct = Vector2.left;
		}
		if ((keyCode == KeyCode.D || keyCode == KeyCode.RightArrow) && dirct != Vector2.left) {
			dirct = Vector2.right;
		}

		return dirct;
	}

	void RoateTail ()  //通过与前一个尾部的节点的位置向量与尾部自身的向量比较求转角;
	{
		GameObject obj = bodys [bodys.Count - 2];
		GameObject tailObj = bodys [bodys.Count - 1];

		Vector3 moveVec = obj.transform.position - tailObj.transform.position;
		Vector2 dirc = new Vector2 (moveVec.x, moveVec.y);
		dirc.x = Mathf.Abs (dirc.x) > Mathf.Abs (dirc.y) ? dirc.x / Mathf.Abs (dirc.x) : 0.0f;
		dirc.y = Mathf.Abs (dirc.x) < Mathf.Abs (dirc.y) ? dirc.y / Mathf.Abs (dirc.y) : 0.0f;

		//print ("------------------Tail dirc " + dirc.x.ToString () + ", " + dirc.y.ToString ());
		float deltaAng = GlobalControl.Instance.CalcAngle (dirctTail, dirc);

		bodys [bodys.Count - 1].transform.Rotate (0.0f, 0.0f, deltaAng);

		dirctTail = dirc;
	}

	bool CheckAlive ()
	{
		if (sh.isDead) {
			Destroy (sh.gameObject);
			foreach (GameObject obj in bodys) {
				obj.gameObject.tag = "Food";
				obj.transform.localScale.Scale (new Vector3 (0.6f, 0.6f, 1.0f));
			}

			CancelInvoke ("SnakeMove");
		}

		return !sh.isDead;
	}

	void BodyMove (Vector3 pos)
	{
		if (sh.eat) { //吃到食物；
			//print ("Eat food ---------------pos = " + pos.x.ToString () + "," + pos.y.ToString () + "\n");
			GameObject obj = (GameObject)Instantiate (prefabs [(int)ESnakeComponent.ESC_Body], pos, Quaternion.identity);
			if (bodys.Count % 3 == 0)
				RenderColor (obj);
			
			bodys.Insert (0, obj);

			sh.eat = false;
			totalScore += 5;
		} else { //普通移动；
			GameObject obj = bodys [bodys.Count - 2];
			RoateTail (); //更新蛇尾的方向；
			bodys [bodys.Count - 1].transform.position = obj.transform.position;  //更新蛇尾的位置；

			obj.transform.position = pos;
			bodys.Insert (0, obj);
			bodys.RemoveAt (bodys.Count - 2);
		}
	}
}

