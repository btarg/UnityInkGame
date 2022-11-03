//#define DBG_ON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UBSPEntities
{
	public class UBSPSound : UBSPBaseActivator
	{
		public AudioClip clip;
		public float volume;
		public float spatialBlend;
		public bool playOnAwake;
		public bool loop;
		public float maxDistance;
		public float minDistance;

		AudioSource source;

		private void Start() {

			source = gameObject.GetComponent<AudioSource>();

			if (source == null) {
				source = gameObject.AddComponent<AudioSource>();
			}

			source.clip = clip;
			source.volume = volume;
			source.spatialBlend = spatialBlend;
			source.playOnAwake = playOnAwake;
			source.loop = loop;
			source.maxDistance = maxDistance;
			source.minDistance = minDistance;

		}

		public override void trigger () {
			
			source.Play();

		}
	}
}
