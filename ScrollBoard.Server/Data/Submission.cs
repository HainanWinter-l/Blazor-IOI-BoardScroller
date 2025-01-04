namespace ScrollBoard.Server.Data
{
    public enum SubmissionStatus : byte
    {
        Accepted = 1,
        WrongAnswer = 2,
        TimeLimitExceeded = 3,
        MemoryLimitExceeded = 4,
        RuntimeError = 6,
        CompileError = 7,
        SystemError = 8,
        Judging = 20,
        Compiling = 21,
        Ignored = 30
    }

    public class Submission
    {
        public DateTime JudgeAt { get; set; }
        public int Pid { get; set; }
        public int Score { get; set; }
        public SubmissionStatus Status { get; set; }
        public int Uid { get; set; }
        public string Uname { get; set; } = string.Empty;
    }
}
