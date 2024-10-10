using System;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Net.Imap;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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

            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            var summaries = inbox.Fetch(0, -1, MessageSummaryItems.UniqueId);
            int messageCount = inbox.Count;

            for (int i = 0; i < messageCount; i++)
            {
                Console.WriteLine(summaries[i].UniqueId);
            }




        }


    }
    public static List<string> ExtractLinksFromMailBody(string text)
    {
        // Regex pattern to match HTTP/HTTPS URLs
        string pattern = @"(http[s]?://[^\s]+)";

        // Create a regex object
        Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Find all matches in the input text
        MatchCollection matches = regex.Matches(text);

        // Initialize a list to store the links
        List<string> links = new List<string>();

        // Add each match (link) to the list
        foreach (Match match in matches)
        {
            links.Add(match.Value);
        }

        return links;
    }
}
