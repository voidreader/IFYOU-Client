// Copyright (c) 2015 - 2020 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Doozy.AnimatedIcons.MinimalistTwo
{
	public class AnimatedIconsController : MonoBehaviour
	{
		[Serializable]
		public enum TriggerName
		{
			A,
			B,
			Click,
			Loop,
			Disabled
		}

		private List<Animator> m_animator = new List<Animator>();

        private void Awake() => UpdateAnimatorsList();
		private void Update() => ListenForKeys();

        private void UpdateAnimatorsList() => m_animator = FindObjectsOfType<Animator>().ToList();

        public void TriggerAnimation(TriggerName triggerName) => TriggerAnimation(triggerName.ToString());

		public void TriggerAnimation(string trigger)
		{
            if (m_animator == null) m_animator = new List<Animator>();
            else m_animator.Clear();
            UpdateAnimatorsList();
            var nullReferenceFound = false;
			foreach (Animator animator in m_animator)
			{
				if (animator == null)
				{
					nullReferenceFound = true;
					continue;
				}


				if (!animator.parameters.Any(parameter => parameter.name.Equals(trigger))) continue;
				animator.SetTrigger(trigger);
			}

			if (nullReferenceFound) m_animator = m_animator.Where(a => a != null).ToList();
		}

		private void ListenForKeys()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				TriggerAnimation(TriggerName.A);
				return;
			}

			if (Input.GetKeyDown(KeyCode.B))
			{
				TriggerAnimation(TriggerName.B);
				return;
			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				TriggerAnimation(TriggerName.Click);
				return;
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				TriggerAnimation(TriggerName.Loop);
				return;
			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				TriggerAnimation(TriggerName.Disabled);
				return;
			}
		}
	}
}