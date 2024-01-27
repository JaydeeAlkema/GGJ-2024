using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class ActionsQueue : MonoBehaviour
	{
		[SerializeField, BoxGroup("References"), Expandable] private PlayerAction[] actions = null;
		[SerializeField, BoxGroup("References")] private CrowdMember[] crowdMembers = null;
		[SerializeField, BoxGroup("References")] private Image[] iconHolders = null;
		[Space]
		[SerializeField, BoxGroup("Runtime"), Expandable] private List<PlayerAction> actionsQueue = new();
		[SerializeField, BoxGroup("Runtime")] private int currentActionQueueIndex = 0;
		[SerializeField, BoxGroup("Runtime")] private bool canInputActions = true;
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
			controls.Player.Action1.performed += ctx => StartCoroutine(AddActionToQueue(actions[0]));
			controls.Player.Action2.performed += ctx => StartCoroutine(AddActionToQueue(actions[1]));
			controls.Player.Action3.performed += ctx => StartCoroutine(AddActionToQueue(actions[2]));
			controls.Player.Action4.performed += ctx => StartCoroutine(AddActionToQueue(actions[3]));
		}

		private void OnDisable()
		{
			controls.Disable();
			controls.Player.Action1.performed -= ctx => StartCoroutine(AddActionToQueue(actions[0]));
			controls.Player.Action2.performed -= ctx => StartCoroutine(AddActionToQueue(actions[1]));
			controls.Player.Action3.performed -= ctx => StartCoroutine(AddActionToQueue(actions[2]));
			controls.Player.Action4.performed -= ctx => StartCoroutine(AddActionToQueue(actions[3]));
		}

		private IEnumerator AddActionToQueue(PlayerAction playerAction)
		{
			if (!canInputActions) yield break;
			canInputActions = false;

			actionsQueue.Add(playerAction);
			AddIconToCurrentIconHolder();
			currentActionQueueIndex++;
			TestSequence();
			if (currentActionQueueIndex >= maxActionsInQueue)
			{
				PrintActionsQueue();
				yield return new WaitForSeconds(1f);
				currentActionQueueIndex = 0;
				actionsQueue.Clear();
				ResetCrowdMembers();
				ResetIcons();
			}
			canInputActions = true;
		}
		private void AddIconToCurrentIconHolder()
		{
			iconHolders[currentActionQueueIndex].sprite = actionsQueue[^1].GetIcon();
		}

		private void ResetIcons()
		{
			foreach (Image iconHolder in iconHolders)
			{
				iconHolder.sprite = null;
			}
		}

		[Button]
		private void TestSequence()
		{
			SetupCrowdMembers();
			OnAction?.Invoke();
			OnAction = null;
			if (printCrowdMemberFinalScores) GetFinalStatesOfCrowdMembers();
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
			Debug.Log("Crowd members reseted", this);
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
