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
        using (var client = new ImapClient())
        {
            Console.WriteLine("Use private password?");
            string choice = Console.ReadLine();
            string password = "";

            if (choice.ToLower() == "y" || choice.ToLower() == "yes")
            {
                System.Console.Write("password: ");
                while (true)
                {
                    var key = System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }
            }
            else
            {
                Console.WriteLine("Please choose the email domain you want to use:");
                Console.WriteLine("1) GMX");
                Console.WriteLine("2) Gmail");
                var key = Console.ReadKey(true);

                switch (key.KeyChar)
                {
                    case '1':
                        password = "C-mailT0123!";
                        client.Connect("imap.gmx.net", 993, true);
                        client.Authenticate("c-mail.test@gmx.at", password);
                        break;
                    case '2':
                        password = "anfanjlunavnzctj";
                        client.Connect("imap.gmail.com", 993, true);
                        client.Authenticate("c.mail.testmail@gmail.com", password);
                        break;

                    default:
                        throw new Exception("Please enter valid character");
                        break;
                }

            }




            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            Console.WriteLine("Total messages: {0}", inbox.Count);
            for (int i = 0; i < inbox.Count; i++)
            {
                var message = inbox.GetMessage(i);
                if (message.From.ToString().Contains("fredi.f5000@gmail.com"))
                {
                    inbox.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
                    inbox.Expunge();
                    Console.WriteLine("it worked... or did it?");
                }
                Console.WriteLine(message.Subject);
            }



            client.Disconnect(true);
        }

    }
}