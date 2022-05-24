using NaughtyAttributes;
using UnityEngine;

public class NewGhost : MonoBehaviour
{
    [SerializeField] private Transform RecordedObj;
    [SerializeField] private GameObject GhostPrefab;

    private Replay_System ReplaySystem;

    private void Awake()
    {
        ReplaySystem = new Replay_System(this);
    }

    [Button]
    public void StartRun()
    {
        ReplaySystem.StartRun(RecordedObj, 1);
    }
    
    [Button]
    public void FinishRun()
    {
        ReplaySystem.FinishRun();
        ReplaySystem.StopReplay();
    }

    [Button]
    public void PlayRecord()
    {
        ReplaySystem.PlayRecording(RecordingType.Best, GhostPrefab, false);
    }

}
