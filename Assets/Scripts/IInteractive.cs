using UnityEngine;
using System.Collections;

public enum Instruments {Axe, Pickaxe, BuildMode};
public interface IInteractive {

	bool isInteractable(Instruments item);
	bool interact();
	float maxInteractDistance();
	string getName();
}
