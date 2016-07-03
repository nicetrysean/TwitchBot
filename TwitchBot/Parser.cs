using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitchBot
{
    internal static class Parser
    {
        public static Dictionary<string, ShuffleBag<string>> Generations = new Dictionary<string, ShuffleBag<string>>();

        private static readonly Regex VariableRegex = new Regex(@"^(#)([a-zA-Z\d\-]+)(#)");

        private static readonly Regex TextRegex = new Regex(@"^[\s\S]+?(?=[#]|$)");

        public static string Parse(string src, params KeyValuePair<string, string>[] values)
        {
            string result = "";
            while (src.Length > 0)
            {
                Match cap;

                //Search Variables
                if ((cap = VariableRegex.Match(src)).Success)
                {
                    ShuffleBag<string> captureResult;
                    if (Generations.TryGetValue(cap.Groups[2].Value, out captureResult))
                    {
                        result += Parse(captureResult.Next());
                    }
                    else
                    {
                        //Insert variables
                        for (int i = 0; i < values.Length; i++)
                        {
                            var value = values[i];
                            if (value.Key == cap.Groups[2].Value)
                            {
                                result += value.Value;
                            }
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
