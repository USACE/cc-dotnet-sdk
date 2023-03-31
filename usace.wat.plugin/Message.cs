using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
    public class Message
    {
        public string message { get; private set; }
        public Message()
        {
            message = string.Empty;
        }
        //this functions like an override for ToString(), but it's technically not.
    public string ToString([CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNo = 0)//, [CallerArgumentExpression] string methodName ="")
        {
            string s = "";
      return s;
/*            if (sender == "")
            {
                s = "Unknown Sender";
            }
            else
            {
                s = sender;
            }
            if (level.CompareTo(Level.DEBUG) == 0)
            {
                return s + " issues " + level.ToString() + " at " + date.ToString() + " from file " + filePath + " on line " + lineNo + " in method name " + memberName + "\n\t" + message + "\n";

            }
            else
            {
                if (level.CompareTo(Level.ERROR) >= 0)
                {
                    return s + " issues " + level.ToString() + " at " + date.ToString() + " from file " + filePath + " on line " + lineNo + " in method name " + memberName + "\n\t" + message + "\n";

                }
                return s + " issues " + level.ToString() + " at " + date.ToString() + "\n\t" + message + "\n";
            }
*/


        }
         
      

    }
}
