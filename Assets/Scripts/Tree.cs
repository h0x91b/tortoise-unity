using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour, IInteractive {
	private float lastGrowTime = 0f;
	float targetTime;
	//public Vector3 minHeight = new Vector3(0.1f,0.1f,0.1f);
	public Vector3 maxHeight = new Vector3(0.8f,0.8f,0.8f);
	public Vector3 growStep = new Vector3(0.1f,0.1f,0.1f);
	public float growTimeIntervalInSeconds = 3f;
	public float animateGrowTime = 1f;
	public GameObject dropableLogPrefab;
	Vector3 initScale;
	Vector3 targetScale;
	// Use this for initialization
	void Start () {
		lastGrowTime = Time.time + (float)Random.Range(0, growTimeIntervalInSeconds);
		float size = (float)Random.Range (5, 75)/100f;
		transform.localScale = new Vector3 (size, size, size);
		initScale = transform.localScale;
		targetScale = transform.localScale;
		targetTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - lastGrowTime >= growTimeIntervalInSeconds)
			grow();

		if (targetTime > Time.time) {
			var remain = targetTime - Time.time;
			var progress = 1f-remain/animateGrowTime;
			transform.localScale = Vector3.Lerp (initScale, targetScale, progress);
		}
	}

	void grow() {
		initScale = transform.localScale;
		targetScale = new Vector3 (transform.localScale.x+growStep.x,transform.localScale.y+growStep.y,transform.localScale.z+growStep.z);
		if (targetScale.x > maxHeight.x)
			targetScale = maxHeight;
		targetTime = Time.time + animateGrowTime;
		lastGrowTime = Time.time;
	}

	public bool isInteractable(Instruments item) {
		return item == Instruments.Axe;
	}

	void spawnLogs() {
		Instantiate(dropableLogPrefab, new Vector3(transform.position.x, 7, transform.position.z), Quaternion.identity);
	}

	public string getName() {
		return "Tree";
	}


	public bool interact() {

		initScale = transform.localScale;
		targetScale = new Vector3 (transform.localScale.x-0.05f,transform.localScale.y-0.05f,transform.localScale.z-0.05f);
		if (targetScale.x < 0) {
			targetScale = new Vector3(0,0,0);
		}
		targetTime = Time.time + 0.5f;
		lastGrowTime = Time.time;
		spawnLogs ();

		return false; //return true when Destroy
	}

	public float maxInteractDistance() {
		return 3f;
	}
}
