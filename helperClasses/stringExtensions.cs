using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{

    internal static class StringExtensions
    {

        internal static string Link(this string s, string url)
        {

            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, s);
        }

        internal static string ParseURL(this string s)
        {

            return Regex.Replace(s,
                @"(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", new MatchEvaluator(StringExtensions.URL));
        }

        internal static string ParseUsername(this string s)
        {

            return Regex.Replace(s,
                "(@)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(StringExtensions.Username));
        }
        internal static string TrimWhitespace(this string s)
        {
            return Regex.Replace(s, @"\s+", " ");
        }
        public static string ParseHashtag(this string s)
        {

            return Regex.Replace(s,
                "(#)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(StringExtensions.Hashtag));
        }

        private static string Hashtag(Match m)
        {
            string x = m.ToString();
            string tag = x.Replace("#", "%23");
            return x.Link("https://twitter.com/hashtag/" + tag);
        }

        private static string Username(Match m)
        {
            string x = m.ToString();
            string username = x.Replace("@", "");
            return x.Link("http://twitter.com/" + username);
        }

        private static string URL(Match m)
        {

            string x = m.ToString();
            return x.Link(x);
        }
        private static int WordCount(this string s)
        {
            return s.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        private static bool IsLike(this string s, string wildcardPattern)
        {
            if (s == null || String.IsNullOrEmpty(wildcardPattern)) return false;
            // turn into regex pattern, and match the whole string with ^$
            var regexPattern = "^" + Regex.Escape(wildcardPattern) + "$";

            // add support for ?, #, *, [], and [!]
            regexPattern = regexPattern.Replace(@"\[!", "[^")
                                       .Replace(@"\[", "[")
                                       .Replace(@"\]", "]")
                                       .Replace(@"\?", ".")
                                       .Replace(@"\*", ".*")
                                       .Replace(@"\#", @"\d");

            var result = false;
            try
            {
                result = Regex.IsMatch(s, regexPattern);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(String.Format("Invalid pattern: {0}", wildcardPattern), ex);
            }
            return result;
        }
    }
}