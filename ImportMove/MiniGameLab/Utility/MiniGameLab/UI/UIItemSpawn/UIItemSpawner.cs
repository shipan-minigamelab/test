using System;
using System.Collections;
using System.Collections.Generic;
using CommonStuff;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class UIItemSpawner : MonoBehaviour
{
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Transform holder;

	private float xAdd = 150;
	private float yAdd = 0;
	private int _count = 0;

	public void SpawnItems(int amount, float radius, float delay, Transform spawnPoint, Transform target, float moveTimeMultiplier = 1, Action OnComplete = null)
	{
		StartCoroutine(_SpawnItems(amount, radius, delay, spawnPoint, target, moveTimeMultiplier, OnComplete));
	}

	private IEnumerator _SpawnItems(int amount, float radius, float delay, Transform spawnPoint, Transform target, float moveTimeMultiplier = 1, Action OnComplete = null)
	{
		yield return new WaitForSeconds(delay);
		_count = 0;
		// Vector3 spawnPoint = CameraController.MainCamera.WorldToScreenPoint(PlayerManager.instance.GetCurrentPlayerTransform().position);
		for (int i = 0; i < amount; i++)
		{
			Transform _spawnPoint = spawnPoint == null ? transform : spawnPoint;
			GameObject xpObj = Instantiate(itemPrefab, _spawnPoint.position, Quaternion.identity, holder);
			xpObj.transform.localScale = Vector3.zero;
			xpObj.transform.localEulerAngles = Vector3.zero;
			// int index = i;
			Vector3 randomPos = (_spawnPoint.position + ((Vector3) Random.insideUnitCircle * radius));
			float moveTime = 1f * moveTimeMultiplier * Random.Range(1, 1.2f);
			// xpObj.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 1).SetEase(Ease.OutBack);
			float randomX = xAdd; // Random.Range(-xAdd, xAdd); //Random.value > 0.5f ? xAdd : -xAdd;
			float randomY = Random.Range(-yAdd, yAdd); //Random.value > 0.5f ? yAdd : -yAdd;
			Vector3[] path = new Vector3[3];

			xpObj.transform.DOMove(randomPos, Random.Range(0.2f, 0.3f)).SetDelay(Random.Range(0f, 0.4f)).SetEase(Ease.OutBack, 3f)
				.OnStart(() => { xpObj.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack, Random.Range(2, 4)); })
				.OnComplete(() =>
				{
					path[0] = xpObj.transform.position;
					path[1] = new Vector3(
						(path[0].x + target.position.x) / 2f + randomX,
						(path[0].y + target.position.y) / 2f + randomY,
						target.position.z);
					path[2] = target.position;
					xpObj.transform.DOPath(path, moveTime, PathType.CatmullRom, PathMode.Sidescroller2D)
						// .SetDelay(0.1f * index)
						// .SetDelay(Random.Range(0f, 0.2f))
						.SetEase(Ease.InBack, Random.Range(0f, 3f))
						// .SetLookAt(0.1f)
						.OnStart(() =>
						{
							xpObj.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 1);
							// xpObj.transform.DOPunchScale(Vector3.one * 1.3f, 0.1f, 3).SetDelay(moveTime / 3);
						})
						.OnComplete(() =>
						{
							xpObj.transform.DOPunchScale(Vector3.one * 0.3f, 0.1f, 1).OnComplete(() => { xpObj.transform.DOScale(0, 0.2f).OnComplete(() => { Destroy(xpObj); }); });
							// AudioManager.Instance.Play(AudioIDs.GoldCollect);
							_count++;
							if (_count == amount)
							{
								OnComplete?.Invoke();
							}
						});
				});
		}
	}
	
	[Button]
	private void Test()
	{
		SpawnItems(10, 200, 0, transform, AudioManager.Instance.transform);
	}
}