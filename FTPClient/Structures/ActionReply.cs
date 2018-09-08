using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTPClient.Structures
{
    public class ActionReply
    {
        public ActionReply()
        {
            Code = "";
            Message = "";
            InfoMessages = "";
        }

        public string Code { get; set; }
        public string Message { get; set; }
        public string InfoMessages { get; set; }

        public string ErrorMessage
        {
            get
            {
                if (!Success && InfoMessages?.Length > 0)
                {
                    var m = InfoMessages.Split('\n').Aggregate("", (current, s) => current + (Regex.Replace(s, "^[0-9]{3}-", "").Trim() + "; "));
                    return m + Message;
                }

                return "";
            }
        }
        public bool Success => (Type != -1 && Type >= 1 | Type <= 3);
        public int Type => (Code?.Length > 0 && Utilities.IsInteger(Code[0])) ? int.Parse(Code[0].ToString()) : -1;

        public override string ToString()
        {
            return string.Format(
                "[Code] {0}{1}[Message] {2}{1}[InfoMessages] {3}{1}[ErrorMessage] {4}{1}[Success] {5}{1}[Type] {6}",
                Code, Environment.NewLine,
                Message,
                InfoMessages, ErrorMessage,
                Success ? "Yes" : "No", Type.ToString());
        }
    }
}
