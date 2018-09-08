namespace FTPClient.Structures
{
    public struct Reply
    {
        public string Response;
        public string Message;
        public int Code;

        public Reply(string reply, string mes, int code)
        {
            Response = reply.Substring(3);
            Message = mes;
            Code = code;
        }
    }
}
