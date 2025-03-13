namespace SignalGenerator.Models
{
    public class SignalConfigGeneration
    {
        public SignalConfigGeneration()
        {
            NumberOfSignals = 1;
            SignalCount = 1;
            MinFrequency = 58.5;
            MaxFrequency = 60.5;
            Interval = 1000;  // Interval in milliseconds - 1 second per signal
            Signals = new List<double>();
            TimeRemaining = 0;
        }

        public int SignalCount { get; set; }
        public double MinFrequency { get; set; }
        public double MaxFrequency { get; set; }
        public int Interval { get; set; }
        public List<double> Signals { get; set; }
        public double TimeRemaining { get; set; }
        public int NumberOfSignals { get; set; }
    }
}