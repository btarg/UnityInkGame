using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UBSPEntities
{
	public class UBSPTrigger : UBSPBaseActivator
	{
		public UBSPBaseActivator exitTarget;
		public bool once = false;
		public bool partialMatch = false;
		public string restrictName;
		private bool restrict = false;

		private int activations = 0;
		
		void Start ()
		{
			restrict = (!string.IsNullOrEmpty(restrictName));
		}
		
		void OnTriggerEnter (Collider c1)
		{
			HandleTrigger(c1, target);
		}
		void OnTriggerExit(Collider c1)
		{
			HandleTrigger(c1, exitTarget);
		}

		void HandleTrigger(Collider c1, UBSPBaseActivator target) {
			
			// only trigger once if set
			if (once && activations > 0) {
				return;
			}
			activations++;

			if (restrict)
			{
				if (partialMatch)
				{
					if (c1.gameObject.name.Contains(restrictName))
					{
						if (target != null) target.trigger();
					}
				}
				else
				{
					if (c1.gameObject.name == restrictName)
					{
						if (target != null) target.trigger();
					}
				}				
			}
			else
			{
				if (target != null) target.trigger();
			}
		}
	}
}