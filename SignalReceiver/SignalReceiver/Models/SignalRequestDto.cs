namespace SignalReceiver.Models
{
    public class SignalRequestDto
    {
        public List<double> SignalData { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
