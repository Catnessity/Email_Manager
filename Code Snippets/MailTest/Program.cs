using System;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Net.Imap;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography;
internal class Program
{
    private static void Main(string[] args)
    {
        System.Console.Write("password: ");
        string password = null;
        while (true)
        {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
                break;
            password += key.KeyChar;
        }
        using (var client = new ImapClient())
        {
            client.Connect("imap.gmx.net", 993, true);

            client.Authenticate("peters.jasmin@gmx.at", password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            Console.WriteLine("Total messages: {0}", inbox.Count);
            for (int i = 0; i < inbox.Count; i++)
            {
                var message = inbox.GetMessage(i);
                if(message.From.ToString().Contains("fredi.f5000@gmail.com"))
                {
                    inbox.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
                    inbox.Expunge();
                    Console.WriteLine("it worked... or did it?");
                }
            }



            client.Disconnect(true);
        }
    }
}