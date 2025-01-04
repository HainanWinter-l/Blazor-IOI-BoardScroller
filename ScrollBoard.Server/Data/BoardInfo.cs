namespace ScrollBoard.Server.Data
{
    public class BoardInfo
    {
        public DateTime BeginAt { get; set; }
        public DateTime BlockAt { get; set; }
        public List<int> Pids { get; set; } = [];
        public Dictionary<int, int> MaxScoreMap { get; set; } = [];
        public double GoldRate { get; set; }
        public double SilverRate { get; set; }
        public double BronzeRate { get; set; }
    }
}
