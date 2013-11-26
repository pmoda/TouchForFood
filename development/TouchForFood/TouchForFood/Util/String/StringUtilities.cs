using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace TouchForFood.Util
{
    public class StringUtilities
    {
        public static string ExceptBlanks(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        continue;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}