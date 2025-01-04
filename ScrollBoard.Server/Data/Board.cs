

using System;
using System.Collections.Generic;

namespace ScrollBoard.Server.Data;

public class Board
{
    public DateTime BeginTime { get; }
    public int Gold { get; }
    public int Silver { get; }
    public int Bronze { get; }
    public IEnumerable<int> Pids { get; }
    public List<Team> Teams { get; set; }
    public IReadOnlyDictionary<int, int> MaxScoreMap { get; }
    public int Current;
    public Board(BoardInfo info, List<Team> teams)
    {

        BeginTime = info.BeginAt;
        Gold = (int)Math.Ceiling(info.GoldRate * teams.Count);
        Silver = (int)Math.Ceiling(info.SilverRate * teams.Count);
        Bronze = (int)Math.Ceiling(info.BronzeRate * teams.Count);
        Pids = info.Pids;
        MaxScoreMap = info.MaxScoreMap;
        Teams = teams;
        Current = teams.Count - 1;
        ScrollTo(info.BlockAt);
    }

    private static int DiffSeconds(DateTime startTime, DateTime endTime)
    {
        TimeSpan timeSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return (int)timeSpan.TotalSeconds;
    }

    public void ScrollToEnd()
    {
        foreach (var team in Teams)
        {
            team.TotalScore = 0;
            team.Solved = 0;
            team.TotalUsedTime = 0;
            foreach (var (pid, submissions) in team.SubmissionsMap)
            {
                // 找出得分这道题最高的评测分数和评测时间
                int score = 0;
                DateTime judgeAt = BeginTime;
                bool isAccept = false;
                foreach (var submission in submissions.Where(submission => submission.Score > score))
                {
                    score = submission.Score;
                    judgeAt = submission.JudgeAt;
                    if (submission.Status != SubmissionStatus.Accepted)

                        continue;
                    isAccept = true;
                    break;
                }

                const int left = 0;
                // 完成这道题的用时
                var usedTime = score == 0 ? 0 : DiffSeconds(BeginTime, judgeAt);
                score = (int)Math.Ceiling(score / 100.0 * MaxScoreMap[pid]);
                team.ScoreMap[pid] = score;
                team.UsedTimeMap[pid] = usedTime;
                team.TotalScore += score;
                team.TotalUsedTime += usedTime;
                if (!isAccept)
                {
                    team.LeftMap[pid] = left;
                    continue;
                }

                team.LeftMap[pid] = 0;
                team.Solved++;
                team.IsAcceptedMap[pid] = true;
            }
        }

        UpdateTeamRank();
        // 初始化绘制榜单
        for (int i = Teams.Count - 1; i >= 0; --i)
        {
            if (Teams[i].LeftMap.Values.All(left => left == 0))
                continue;
            Current = i;
            return;

        }
    }

    public void ScrollTo(DateTime blockTime)
    {
        foreach (var team in Teams)
        {
            foreach (var (pid, submissions) in team.SubmissionsMap)
            {
                // 找出得分这道题最高的评测分数和评测时间
                int score = 0;
                DateTime judgeAt = BeginTime;
                bool isAccept = false;
                // 要记得找封榜前的
                foreach (var submission in submissions.Where(x => x.JudgeAt <= blockTime))
                {
                    if (submission.Score <= score)
                        continue;
                    score = submission.Score;
                    judgeAt = submission.JudgeAt;
                    if (submission.Status == SubmissionStatus.Accepted)
                        isAccept = true;
                }

                // 计算出封榜后这道题有多少次提交
                var left = submissions.Count(x => x.JudgeAt > blockTime);
                // 完成这道题的用时
                var usedTime = score == 0 ? 0 : DiffSeconds(BeginTime, judgeAt);
                score = (int)Math.Ceiling(score / 100.0 * MaxScoreMap[pid]);
                team.ScoreMap.Add(pid, score);
                team.UsedTimeMap.Add(pid, usedTime);
                team.TotalScore += score;
                team.TotalUsedTime += usedTime;
                if (!isAccept)
                {
                    team.LeftMap.Add(pid, left);
                    continue;
                }

                team.LeftMap.Add(pid, 0);
                team.Solved++;
                team.IsAcceptedMap.Add(pid, true);
            }
        }

        UpdateTeamRank();
        // 初始化绘制榜单
        for (int i = Teams.Count - 1; i >= 0; --i)
        {
            if (Teams[i].LeftMap.Values.All(left => left == 0))
                continue;
            Current = i;
            return;
        }
    }

    public void UpdateTeamRank()
    {
        // 先比较分数 再比较过题数 再比较总用时
        Teams.Sort((a, b) => b.CompareTo(a));
        for (int i = 0; i < Teams.Count; ++i)
            Teams[i].CurrentRank = i + 1;
    }

    public void JudgeCurrentTeam(int pid)
    {
        var submission = Teams[Current].SubmissionsMap[pid].MaxBy(x => x.Score);
        var isAccepted = submission!.Status == SubmissionStatus.Accepted;
        var score = (int)Math.Ceiling(submission.Score / 100.0 * MaxScoreMap[pid]);
        Teams[Current].ScoreMap[pid] = score;
        Teams[Current].LeftMap[pid] = 0;
        Teams[Current].UsedTimeMap[pid] = submission.Score == 0 ? 0 : DiffSeconds(BeginTime, submission.JudgeAt);
        Teams[Current].IsAcceptedMap[pid] = isAccepted;
        Teams[Current].Solved = Teams[Current].IsAcceptedMap.Count(x => x.Value);
        Teams[Current].TotalScore = Teams[Current].ScoreMap.Sum(x => x.Value);
        Teams[Current].TotalUsedTime = Teams[Current].UsedTimeMap.Sum(x => x.Value);
    }

    public KeyValuePair<int, int> FindCurrentWaitForJudge()
    {
        var waitForJudge = Teams[Current].LeftMap.FirstOrDefault(kv => kv.Value > 0);
        while (waitForJudge.Value == 0 && Current >= Gold + Silver + Bronze)
        {
            Current--;
            waitForJudge = Teams[Current].LeftMap.FirstOrDefault(kv => kv.Value > 0);
        }

        return waitForJudge;
    }
}
