using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class ActionsQueue : MonoBehaviour
	{
		[SerializeField, BoxGroup("References"), Expandable] private PlayerAction[] actions = null;
		[SerializeField, BoxGroup("References")] private CrowdMember[] crowdMembers = null;
		[Space]
		[SerializeField, BoxGroup("Runtime"), Expandable] private List<PlayerAction> actionsQueue = new();
		[SerializeField, BoxGroup("Runtime")] private int currentActionQueueIndex = 0;
		[Space]
		[SerializeField, BoxGroup("Settings")] private int maxActionsInQueue = 4;
		[Space]
		[SerializeField, BoxGroup("Debugging")] private bool printCrowdMemberFinalScores = false;

		private ActionsQueue instance = null;
		private event Action OnAction;
		private Controls controls;

		public ActionsQueue Instance { get => instance; }

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}

			controls = new Controls();
			controls.Enable();
		}

		private void OnEnable()
		{
			controls.Player.Action1.performed += ctx => AddActionToQueue(actions[0]);
			controls.Player.Action2.performed += ctx => AddActionToQueue(actions[1]);
			controls.Player.Action3.performed += ctx => AddActionToQueue(actions[2]);
			controls.Player.Action4.performed += ctx => AddActionToQueue(actions[3]);
		}

		private void OnDisable()
		{
			controls.Disable();
			controls.Player.Action1.performed -= ctx => AddActionToQueue(actions[0]);
			controls.Player.Action2.performed -= ctx => AddActionToQueue(actions[1]);
			controls.Player.Action3.performed -= ctx => AddActionToQueue(actions[2]);
			controls.Player.Action4.performed -= ctx => AddActionToQueue(actions[3]);
		}

		private void AddActionToQueue(PlayerAction playerAction)
		{
			actionsQueue.Add(playerAction);
			currentActionQueueIndex++;
			TestSequence();
			if (currentActionQueueIndex == maxActionsInQueue)
			{
				PrintActionsQueue();
				currentActionQueueIndex = 0;
				actionsQueue.Clear();
			}
		}

		[Button]
		private void TestSequence()
		{
			SetupCrowdMembers();
			OnAction?.Invoke();
			OnAction = null;
			if (printCrowdMemberFinalScores) GetFinalStatesOfCrowdMembers();
			ResetCrowdMembers();
		}
		private void SetupCrowdMembers()
		{
			foreach (PlayerAction playerAction in actionsQueue)
			{
				foreach (CrowdMember crowdMember in crowdMembers)
				{
					OnAction += () => crowdMember.CheckPossitivesAndNegatives(playerAction.PlayerActionTypes);
				}
			}
		}
		private void ResetCrowdMembers()
		{
			foreach (CrowdMember crowdMember in crowdMembers)
			{
				crowdMember.SetScore(0);
			}
		}
		private void GetFinalStatesOfCrowdMembers()
		{
			bool allGood = true;
			string text = "";
			foreach (CrowdMember crowdMember in crowdMembers)
			{
				text += $"[ {crowdMember.gameObject.name}: {crowdMember.GetScore()} ]";
				if (crowdMember.GetScore() <= 0) allGood = false;
			}
			string textColor = allGood ? "green" : "red";
			Debug.Log($"<color={textColor}>Final states of crowd members: {text}</color>", this);
		}
		private void PrintActionsQueue()
		{
			string text = "";
			foreach (PlayerAction playerAction in actionsQueue)
			{
				text += $"| {playerAction.name} |";
			}
			Debug.Log($"Actions queue: {text}", this);
		}
	}
}
