using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class MoveFrame : MonoBehaviour
	{
		[SerializeField, BoxGroup("References")] private Animator animator = null;
		[SerializeField, BoxGroup("References")] private SpriteRenderer icon = null;

		public void SetIcon(Sprite sprite)
		{
			icon.sprite = sprite;
		}

		public void SetAsSelected()
		{
			animator.SetBool("isSelected", true);
		}

		public void SetAsUnselected()
		{
			animator.SetBool("isSelected", false);
		}
	}
}