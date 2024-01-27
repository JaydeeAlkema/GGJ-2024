using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsPAge : MonoBehaviour
{
	private Controls controls;

	private void Awake()
	{
		controls = new Controls();
		controls.Enable();
	}
}
