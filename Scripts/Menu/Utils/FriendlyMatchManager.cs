namespace Dawnshard.Menu
{
    /// <summary>
    /// Holds data for a pending friendly match invitation.
    /// </summary>
    public static class FriendlyMatchManager
    {
        public static string MatchId { get; private set; }
        public static string OpponentName { get; private set; }

        /// <summary>
        /// Returns true if there is a pending friendly match request.
        /// </summary>
        public static bool HasPendingMatch => !string.IsNullOrEmpty(MatchId);

        /// <summary>
        /// Called when a friendly match invitation is received.
        /// </summary>
        public static void StartFriendlyMatch(string opponentName, string matchId)
        {
            OpponentName = opponentName;
            MatchId = matchId;
        }

        /// <summary>
        /// Clear current invitation data.
        /// </summary>
        public static void Clear()
        {
            OpponentName = null;
            MatchId = null;
        }
    }
}
