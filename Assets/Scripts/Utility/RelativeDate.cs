using System;
public class RelativeDate
{
    const int SECOND = 1;
    const int MINUTE = 60 * SECOND;
    const int HOUR = 60 * MINUTE;
    const int DAY = 24 * HOUR;
    const int MONTH = 30 * DAY;


    public static string GetRelativeDate(long timestamp)
    {
        var ts = new TimeSpan(DateTime.UtcNow.Ticks - DateTime.FromFileTime(timestamp).Ticks);
        double delta = Math.Abs(ts.TotalSeconds);

        if (delta < 1 * MINUTE)
            return ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds";

        if (delta < 2 * MINUTE)
            return "minute";

        if (delta < 45 * MINUTE)
            return ts.Minutes + " minutes";

        if (delta < 90 * MINUTE)
            return "hour";

        if (delta < 24 * HOUR)
            return ts.Hours + " hours";

        if (delta < 48 * HOUR || delta < 30 * DAY)
            return ts.Days + " days";

        if (delta < 12 * MONTH)
        {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "one month" : months + " months";
        }
        else
        {
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year" : years + " years";
        }
    }

}