using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	private float angle;
	public float radius = 50.0f;
	private Light sun;
	public float dayCycleInSeconds = 5f;
	private float start = 0;
	public Color red;
	public Color yellow;
	// Use this for initialization
	void Start () {
		angle = 0;
		start = 0;
		sun = GetComponent<Light> ();
		sun.color = red;
		sun.intensity = 0.1f;
	}

	float getTime() {
		return Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		angle += Time.deltaTime * (6.28318531f/dayCycleInSeconds);
		if (angle >= 6.28318531f) {
			angle = 0f;
			start = getTime();
		}

		//var yellow = new Color (255f/255f, 250f/255f, 229f/255f);
		//var red = new Color (197f/255f, 30f/255f, 0f);

		float dt = getTime() - start;
		if (getTime() - start < 0.03f * dayCycleInSeconds) {
			//rasvet
			sun.intensity = Mathf.Lerp(0.1f, 1.5f, dt/(0.03f * dayCycleInSeconds));
			gameObject.GetComponent<Renderer>().material.color = sun.color;
		} else if (getTime() - start < 0.1f * dayCycleInSeconds) {
			//rasvet
			dt = dt - 0.03f * dayCycleInSeconds;
			sun.intensity = Mathf.Lerp(1.5f, 2.8f, dt/(0.07f * dayCycleInSeconds));
			sun.color = Color.Lerp(red, yellow, dt/(0.07f * dayCycleInSeconds));
			gameObject.GetComponent<Renderer>().material.color = sun.color;
		} else if(dt > 0.35f * dayCycleInSeconds && dt < 0.5f * dayCycleInSeconds) {
			//zakat
			dt = dt - 0.35f*dayCycleInSeconds;
			sun.intensity = Mathf.Lerp(2.8f, 0.1f, dt/(0.15f * dayCycleInSeconds));
			sun.color = Color.Lerp(yellow, red, dt/(0.15f * dayCycleInSeconds));
			gameObject.GetComponent<Renderer>().material.color = sun.color;
		}

		transform.position = new Vector3 (Mathf.Cos(angle)*radius*1.5f,Mathf.Sin(angle)*radius,0f);
	}
}
