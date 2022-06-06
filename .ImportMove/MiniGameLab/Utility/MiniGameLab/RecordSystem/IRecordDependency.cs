public interface IRecordDependency
{
    public int GetCurrentAnimation => GetCurrentAnimIndex();
    public bool PauseRecorder => GetPauseValue();
    public int GetCurrentAnimIndex();
    public void PlayAnimation(int index);

    public bool GetPauseValue();
}