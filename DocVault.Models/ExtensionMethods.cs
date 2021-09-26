using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocVault.Models
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string> SplitAndCleaned(this string text)
        {
            string[] phrases =
                text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            return phrases.Select(phrase => phrase.Replace(",", "").Trim());
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static byte[] ToChecksumByteArray(this string val)
        {
            return new UnicodeEncoding().GetBytes(val);
        }
    }
}