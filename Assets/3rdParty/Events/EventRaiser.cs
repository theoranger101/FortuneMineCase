using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Events
{
	public class EventRaiser : MonoBehaviour
	{

		public EventContext eventContext;

		public float delay;

		[Button]
		public void Raise()
		{
			if (delay < Mathf.Epsilon)
			{
				RaiseNow();
			}
			else
			{
				StartCoroutine(WaitAndRaise());
			}
		}
		
		[Button]
		public void RaiseNow()
		{
			eventContext.SendEvent();
		}


		private IEnumerator WaitAndRaise()
		{
			yield return new WaitForSecondsRealtime(delay);

			RaiseNow();
		}
	}
}
