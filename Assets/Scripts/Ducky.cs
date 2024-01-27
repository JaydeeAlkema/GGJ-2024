using NaughtyAttributes;
using UnityEngine;

namespace Assets.Scripts
{
	public class Ducky : MonoBehaviour
	{
		[SerializeField, BoxGroup("References")] private Animator animator = null;

		public Animator GetAnimator() => animator;

		public void SetAndResetTriggers(string triggerToSet)
		{
			foreach (AnimatorControllerParameter parameter in animator.parameters)
			{
				if (parameter.type != AnimatorControllerParameterType.Trigger) return;
				animator.ResetTrigger(parameter.name);
			}
			animator.SetTrigger(triggerToSet);
		}
	}
}