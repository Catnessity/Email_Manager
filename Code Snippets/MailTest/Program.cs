using System;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Net.Imap;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
internal class Program
{
    
    private static void Main(string[] args)
    {
        
        using (var client = new ImapClient())
        {
            //Console.WriteLine("Use private password?");
            //string choice = Console.ReadLine();
            //string password = "";

            //if (choice.ToLower() == "y" || choice.ToLower() == "yes")
            //{
            //    System.Console.Write("password: ");
            //    while (true)
            //    {
            //        ConsoleKeyInfo consoleKeyInfo = System.Console.ReadKey();
            //        if (consoleKeyInfo.Key == ConsoleKey.Enter)
            //        {
            //            break;
            //        }
            //        password += consoleKeyInfo.KeyChar;
            //    }
            //}
            //else
            //{
                var password = "C-mailT0123!";
            //}
            Console.WriteLine("Please choose the email domain you want to use:");
            Console.WriteLine("1) GMX");
            Console.WriteLine("2) Gmail");
            Console.WriteLine("3) Outlook");
            //Console.WriteLine("4) Protonmail"); //proton would need pro version to allow imap
            var key = Console.ReadKey(true);
            var imap_address = "";
            var email = "";

            switch (key.KeyChar)
            {
                case '1':
                    imap_address = "imap.gmx.net";
                    email = "c-mail.test@gmx.at";
                    break;
                case '2':
                    imap_address = "imap.gmail.com";
                    email = "c.mail.testmail@gmail.com";
                    break;
                case '3':
                    imap_address = "imap-mail.outlook.com";
                    email = "ctestlinz@outlook.com";
                    break;
                //case '4':
                //    imap_address = "imap.protonmail.ch";
                //    email = "c-mail.test@proton.me";
                //    break;

                default:
                    throw new Exception("Please enter valid character");
                    break;
            }
            client.Connect(imap_address, 993, true);
            client.Authenticate(email, password);






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