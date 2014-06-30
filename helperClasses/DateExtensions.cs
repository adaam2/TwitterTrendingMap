namespace System
{
    internal static class DateExtensions
    {
        internal static bool IsFuture(this DateTime date, DateTime from)
        {
            return date.Date > from.Date;
        }

        internal static bool IsFuture(this DateTime date)
        {
            return date.IsFuture(DateTime.Now);
        }

        internal static bool IsPast(this DateTime date, DateTime from)
        {
            return date.Date < from.Date;
        }

        internal static bool IsPast(this DateTime date)
        {
            return date.IsPast(DateTime.Now);
        }
    }
}