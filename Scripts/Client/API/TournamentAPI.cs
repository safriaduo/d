using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class TournamentAPI
{
    public static string weekendTournamentId="";
    public static bool isWeekendTournamentActive=false;
    public static int playerWins=0;
    public static int playerLoss=0;
    public const int MaxScore = 18;
    public static async Task JoinTournament()
    {
        if(string.IsNullOrEmpty(weekendTournamentId))
            return;
        await GameController.Instance.Client.JoinTournamentAsync(GameController.Instance.Session, weekendTournamentId);
    }

    public static async Task SearchWeekendTournament()
    {
        var categoryStart = 1;
        var categoryEnd = 2;
        var limit = 10;
        //omitted start time because omitting the start and end time parameters returns the ongoing and future tournaments so that it will only show ongoing
        var result = await GameController.Instance.Client.ListTournamentsAsync(GameController.Instance.Session, categoryStart, categoryEnd, endTime:null, limit:limit);
        if (result.Tournaments.ToList().ConvertAll(tournament => tournament.Title)
            .Contains(Constants.WeekendTournament))
        {
            var weekendTournament = result.Tournaments
                .First((tournament) => tournament.Title == Constants.WeekendTournament);
            var unixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            isWeekendTournamentActive = weekendTournament.StartActive < unixTime && unixTime < weekendTournament.EndActive;
            weekendTournamentId = result.Tournaments
                .First((tournament) => tournament.Title == Constants.WeekendTournament).Id;
        }
    }
    
    public static async Task HasPlayerAlreadyJoined()
    {
        if(string.IsNullOrEmpty(weekendTournamentId))
            return;
        await GameController.Instance.Client.JoinTournamentAsync(GameController.Instance.Session, weekendTournamentId);
    }

    public static async Task LoadPlayerRecord()
    {
        if(!isWeekendTournamentActive)
            return;
        var records = await GameController.Instance.GetOwnerLeaderboardAsync(10,weekendTournamentId);
        foreach (var record in records.Records)
        {
            if(record.Username != GameController.Username)
                continue;
            var score = record.Score == null ? 0 : int.Parse(record.Score);
            playerWins = score;
            playerLoss = record.NumScore - score;
        }
    }
}
