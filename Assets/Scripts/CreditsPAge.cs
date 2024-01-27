using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class CreditsPage : MonoBehaviour
	{
		private Controls controls;

		private void Awake()
		{
			controls = new Controls();
			controls.Enable();

			controls.UI.Escape.performed += ctx => Back();
		}

		private void Back()
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
