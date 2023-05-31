public class Metrics
{
    // Engagement metrics
    public int DaysLoggedIn { get; set; }
    public float SessionTime { get; set; }
    public int LevelPassed { get; set; }

    // Purchase metrics
    public float ValueOfPurchase { get; set; }
    public string IdOfPurchasedItem { get; set; }

    // Ad metrics
    public string IdOfAdWatched { get; set; }
    public int WatchedSeconds { get; set; }
}