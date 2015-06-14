using UnityEngine;
using System.Collections;

public class PickableItem : MonoBehaviour {

	public enum Type {Logs, SomethingElse};
	public Type type;
	public int count = 1;

	public string getName() {
		return type.ToString();
	}
}
