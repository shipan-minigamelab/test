using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
	[HorizontalLine(2, EColor.Green)]
	[Tag, SerializeField] private string _checkTag;

	[OnValueChanged(nameof(OnBoundChange)),SerializeField] private Vector3 _bound;
	[SerializeField] private BoxCollider _boxCollider;

	[HorizontalLine(2, EColor.Green)]
	public Action OnTriggerEnterEvent;

	public Action OnTriggerExitEvent;

	[ReadOnly] [SerializeField] private bool isInside = false;


	private void OnBoundChange()
	{
		if (_boxCollider) _boxCollider.size = _bound;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(_checkTag) || other.CompareTag(_checkTag))
		{
			if (!isInside)
			{
				OnEnter();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(_checkTag) || other.CompareTag(_checkTag))
		{
			if (isInside)
			{
				OnExit();
			}
		}
	}

	public void OnEnter()
	{
		isInside = true;
		OnTriggerEnterEvent?.Invoke();
	}

	public void OnExit()
	{
		isInside = false;
		OnTriggerExitEvent?.Invoke();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, _bound);
	}
}