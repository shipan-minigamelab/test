using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DissolveTest : MonoBehaviour
{
	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private ParticleSystem _dissolveParticle;

	[Button]
	private void Dissolve()
	{
		StartCoroutine(_Dissolve());
	}

	private IEnumerator _Dissolve()
	{
		_meshRenderer.enabled = true;
		_dissolveParticle.Play();
		yield return new WaitForSeconds(1);
		_meshRenderer.enabled = false;
	}
}