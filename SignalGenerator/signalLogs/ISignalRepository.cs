namespace SignalGenerator.signalLogs
{
    public interface ISignalRepository
    {
        Task SaveSignalLogAsync(SignalLog signalLog);
    }

}