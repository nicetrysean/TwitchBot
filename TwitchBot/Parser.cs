using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TwitchBot
{
    internal static class Parser
    {
        public static Dictionary<string, ShuffleBag<string>> UserDefinedGenerations = new Dictionary<string, ShuffleBag<string>>();
        public static Dictionary<string, string> ApplicationDefined = new Dictionary<string, string>();

        private static readonly Regex VariableRegex = new Regex(@"^(#)([a-zA-Z\d\-]+)(#)");

        private static readonly Regex TextRegex = new Regex(@"^[\s\S]+?(?=[#]|$)");

        public static string Parse(string src)
        {
            string result = "";
            while (src.Length > 0)
            {
                Match cap;

                //Search Variables
                if ((cap = VariableRegex.Match(src)).Success)
                {
                    ShuffleBag<string> captureResult;
                    if (UserDefinedGenerations.TryGetValue(cap.Groups[2].Value, out captureResult))
                    {
                        result += Parse(captureResult.Next());
                    }
                    else
                    {
                        string value;
                        if (ApplicationDefined.TryGetValue(cap.Groups[2].Value, out value))
                        {
                            result += value;
                        }
                    }

                    src = src.Substring(cap.Groups[0].Length);
                }

                //Search Text
                if (!(cap = TextRegex.Match(src)).Success) continue;
                result += cap.Groups[0].Value;
                src = src.Substring(cap.Groups[0].Length);
            }

            return result;
        }
    }
}
