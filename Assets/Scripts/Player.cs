using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	Camera camera;
	public UnityEngine.UI.Text actionItemGUI;
	public UnityEngine.UI.Text crosschair;
	public UnityEngine.UI.Text lastLogEntryGUI;
	public UnityEngine.UI.Text selectedInstrumentGUI;
	private float coldDown = 0f;
	private Instruments selectedInstrument;
	Dictionary<PickableItem.Type, int> invertory;
	List<Instruments> availableInstruments;
	int instrIndex = 0;
	public Animator animAxe;
	AudioSource audioSource;
	public AudioClip missSound;
	public AudioClip axeHit; 
	public GameObject axe;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		camera = GetComponentInChildren<Camera> ();
		Cursor.visible = false; 
		invertory = new Dictionary<PickableItem.Type, int> ();
		availableInstruments = new List<Instruments> ();
		availableInstruments.Add (Instruments.Axe);
		availableInstruments.Add (Instruments.Pickaxe);
		selectedInstrument = Instruments.Axe;
		selectedInstrumentGUI.text = String.Format("In hand: {0}", selectedInstrument.ToString ());
	}

	void attackAir() {
		if (coldDown > Time.time)
			return;
		//missSound.Play();
		audioSource.clip = missSound;
		audioSource.Play ();
		animAxe.SetTrigger("Attack");;
		coldDown = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("escape"))
			Application.Quit();

		if (Input.GetAxisRaw ("Mouse ScrollWheel") > 0) {
			selectedInstrument = availableInstruments[instrIndex++ % availableInstruments.Count];
			selectedInstrumentGUI.text = String.Format("In hand: {0}", selectedInstrument.ToString ());
		} else if (Input.GetAxisRaw ("Mouse ScrollWheel") < 0) {
			if(--instrIndex<0)
				instrIndex = availableInstruments.Count - 1;
			selectedInstrument = availableInstruments[instrIndex % availableInstruments.Count];
			selectedInstrumentGUI.text = String.Format("In hand: {0}", selectedInstrument.ToString ());
		}

		var renderers = axe.GetComponentsInChildren<Renderer>();
		foreach(var r in renderers) {
			r.enabled = selectedInstrument == Instruments.Axe;;
		}

		if (transform.position.y < -100f) {
			transform.position = new Vector3(0,7f,0);
		}

		Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;

		if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
			if(hit.transform.tag == "Terrain") {
				float height = Terrain.activeTerrain.SampleHeight(hit.point);
				actionItemGUI.text = String.Format("Terrain height {0}", height);
				actionItemGUI.enabled = true;
				actionItemGUI.color = Color.blue;
			} else if (hit.transform.tag != "Interactive" && hit.transform.tag != "Pickable") {
				Debug.DrawRay (ray.origin, ray.direction * 100, Color.red);
				actionItemGUI.enabled = false;
				crosschair.color = Color.red;
				if(Input.GetMouseButtonDown(0)) {
					attackAir();
				}
				return;
			}

			if(hit.transform.tag == "Pickable") {
				PickableItem item = hit.transform.GetComponent<PickableItem>();
				actionItemGUI.enabled = true;
				actionItemGUI.text = item.getName();

				if(hit.distance < 5f) {
					actionItemGUI.color = crosschair.color = Color.green;
					if(Input.GetMouseButtonDown(0)) {
						if(!invertory.ContainsKey(item.type)) {
							invertory[item.type] = 0;
						}
						invertory[item.type] += item.count;
						Debug.LogFormat("Picked up {0} {1}, now totally I have {2}", item.count, item.type.ToString(), invertory[item.type]);
						lastLogEntryGUI.text = String.Format("Picked up {0} {1}, now totally I have {2} {3}", item.count, item.type.ToString(), invertory[item.type], item.type.ToString());
						Destroy(hit.transform.gameObject);
					}
				} else {
					actionItemGUI.color = crosschair.color = Color.yellow;
					if(Input.GetMouseButtonDown(0)) {
						attackAir();
					}
				}
				return;
			}

			IInteractive elm = hit.transform.GetComponent<IInteractive>();
			if(elm == null || !elm.isInteractable(selectedInstrument)) {
				crosschair.color = Color.red;
				if(Input.GetMouseButtonDown(0)) {
					attackAir();
				}
				return;
			}
			if(hit.distance < elm.maxInteractDistance()) {
				actionItemGUI.enabled = true;
				actionItemGUI.text = elm.getName();
				actionItemGUI.color = crosschair.color = Color.green;
				Debug.DrawRay (ray.origin, ray.direction * 100, Color.green);

				if(Time.time - coldDown > 1.5f && Input.GetMouseButtonDown(0)) {
					coldDown = Time.time;
					//Destroy(hit.transform.gameObject);
					//print(hit.transform.gameObject.transform.localScale);// -= 0.9f;
					//var scale = hit.transform.gameObject.transform.localScale;
					//hit.transform.gameObject.transform.localScale = new Vector3(scale.x-0.1f, scale.y-0.1f, scale.z-0.1f);
					elm.interact();
					animAxe.SetTrigger("Attack");
					audioSource.clip = axeHit;
					audioSource.Play ();
				}
			}
			else if(hit.distance < 25) {
				if(Input.GetMouseButtonDown(0)) {
					attackAir();
				}
				actionItemGUI.enabled = true;
				actionItemGUI.text = elm.getName();
				actionItemGUI.color = crosschair.color = Color.yellow;
				Debug.DrawRay (ray.origin, ray.direction * 100, Color.yellow);
			}else {
				if(Input.GetMouseButtonDown(0)) {
					attackAir();
				}
				Debug.DrawRay (ray.origin, ray.direction * 100, Color.magenta);
				actionItemGUI.enabled = false;
				crosschair.color = Color.red;
				if(Input.GetMouseButtonDown(0)) {
					attackAir();
				}
			}
		}
		else {
			Debug.DrawRay (ray.origin, ray.direction * 100, Color.red);
			actionItemGUI.enabled = false;
			crosschair.color = Color.red;
			if(Input.GetMouseButtonDown(0)) {
				attackAir();
			}
		}
	}
}
