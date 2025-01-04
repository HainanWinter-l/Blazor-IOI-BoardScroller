namespace ScrollBoard.Server.Data
{
    public class Team
    {
        public int Uid { get; set; }
        public required string Uname { get; set; }
        public int Solved { get; set; }
        public int TotalScore { get; set; }
        public int TotalUsedTime { get; set; }
        public Dictionary<int, bool> IsAcceptedMap { get; set; } = [];
        public Dictionary<int, int> ScoreMap { get; set; } = [];
        public Dictionary<int, List<Submission>> SubmissionsMap { get; set; } = [];
        public Dictionary<int, int> LeftMap { get; set; } = [];
        public Dictionary<int, int> UsedTimeMap { get; set; } = [];
        public int CurrentRank { get; set; }
        // 先比较分数 再比较过题数 再比较总用时
        public int CompareTo(Team other)
        {
            var result = TotalScore.CompareTo(other.TotalScore);
            if (result != 0)
                return result;
            result = Solved.CompareTo(other.Solved);
            if (result != 0)
                return result;
            result = other.TotalUsedTime.CompareTo(TotalUsedTime);
            return result != 0 ? result : Uid.CompareTo(other.Uid);
        }
    }
}



