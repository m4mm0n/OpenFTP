using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient.Helpers
{
    static class SendCommand
    {

    }
    /*
          private string readLine()
     {

       while(true)
       {
           if (useStream)
               bytes = stream.Read(buffer, buffer.Length, 0);
           else
             bytes = clientSocket.Receive(buffer, buffer.Length, 0);


         mes += ASCII.GetString(buffer, 0, bytes);
         if(bytes < buffer.Length)
         {
           break;
         }
       }

       char[] seperator = {'\n'};
       string[] mess = mes.Split(seperator);

       if(mes.Length > 2)
       {
         mes = mess[mess.Length-2];
       }
       else
       {
         mes = mess[0];
       }

       if(!mes.Substring(3,1).Equals(" "))
       {
         return readLine();
       }

       return mes;
     }

     private void readReply()
     {
         if (useStream)
             reply=ResponseMsg();
         else
         {

             mes = "";
             reply = readLine();
             retValue = Int32.Parse(reply.Substring(0, 3));
         }
     }

         private string ResponseMsg()
     {
         System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
         byte[] serverbuff = new Byte[1024];
         int count = 0;
         while (true)
         {
             byte[] buff = new Byte[2];
             int bytes = stream.Read(buff, 0, 1);
             if (bytes == 1)
             {
                 serverbuff[count] = buff[0];
                 count++;

                 if (buff[0] == '\n')
                 {
                     break;
                 }
             }
             else
             {
                 break;
             };
         };
         string retval = enc.GetString(serverbuff, 0, count);
         Console.WriteLine(" READ:" + retval);
         retValue = Int32.Parse(retval.Substring(0, 3));
         return retval;
     }
     */
}
