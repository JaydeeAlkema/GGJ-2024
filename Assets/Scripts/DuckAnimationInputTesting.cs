using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public class DuckAnimationInputTesting : MonoBehaviour
{
	private Animator animator;

	void Awake()
	{
		if (animator == null)
			try
			{
				animator = GetComponentInChildren<Animator>();
			}
			catch (Exception ue)
			{
				Debug.LogError(ue.Message);
			}
	}

	void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				SetAndResetTriggers("doDance");
			}
			if (Input.GetKeyDown(KeyCode.W))
			{
				SetAndResetTriggers("doRoll");
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				SetAndResetTriggers("doFlip");
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				SetAndResetTriggers("doTeabag");
			}
		}
	}

	void SetAndResetTriggers(string triggerToSet)
	{
		foreach (AnimatorControllerParameter parameter in animator.parameters)
		{
			if (parameter.type != AnimatorControllerParameterType.Trigger) return;
			animator.ResetTrigger(parameter.name);
		}
		animator.SetTrigger(triggerToSet);
	}
}
