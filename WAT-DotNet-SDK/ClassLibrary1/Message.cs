using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace usace.wat.plugin
{
    public class Message
    {
        public Status status { get; private set; } 
        public short progress { get; private set; }
        public Level level { get; private set; }
        public string message { get; private set; }
        public string sender { get; private set; }
        public string payload_id { get; private set; }
        public DateTime date { get; private set; }
        public Message()
        {
            message = string.Empty;
            level = Level.INFO;
            payload_id = string.Empty;
            progress = 0;
            status = Status.COMPUTING;
            date = DateTime.Now;
            sender = string.Empty;
        }
        //this functions like an override for ToString(), but it's technically not.
    public string ToString([CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNo = 0)//, [CallerArgumentExpression] string methodName ="")
        {
            string s = "";
            if (sender == "")
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


        }
        public static MessageBuilder BuildMessage()
        {
            MessageBuilder builder = new MessageBuilder();
            return builder;
        }
        public class MessageBuilder
        {
            private Message _message;
            public MessageBuilder()
            {
                _message = new Message();
            }
            public MessageBuilder withMessage(String message)
            {
                _message.message = message;
                return this;
            }
            public MessageBuilder fromSender(String payloadId)
            {
                _message.payload_id = payloadId;
                return this;
            }
            public MessageBuilder withErrorLevel(Level level)
            {
                _message.level = level;
                return this;
            }
            public MessageBuilder withProgress(short progress)
            {
                _message.progress = progress;
                return this;
            }
            public MessageBuilder withStatus(Status status)
            {
                _message.status = status;
                return this;
            }
            public Message build()
            {
                _message.date = DateTime.Now;
                return _message;
            }
        }
        public enum Status
        {
            COMPUTING, //Status = "Computing"
            FAILED,    //Status = "Failed"
            SUCCEEDED, //Status = "Succeeded"
        }
        public enum Level
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            PANIC,
            DISABLED,
        }

    }
}
