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






            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            Console.WriteLine("Total messages: {0}", inbox.Count);


            var NoFilterUids = inbox.Search(SearchQuery.All);
            var searchQuery = SearchQuery.Not(
    SearchQuery.Or(
        SearchQuery.BodyContains("unsubscribe"),
        SearchQuery.Or(
            SearchQuery.BodyContains("abbestellen"),
            SearchQuery.Or(
                SearchQuery.BodyContains("abmelden"),
                SearchQuery.Or(
                    SearchQuery.BodyContains("deabonnieren"),
                    SearchQuery.BodyContains("optout")
                )
            )
        )
    )
);
            //var FilteredUids = inbox.Search(searchQuery);

            //// Subtract the filtered UIDs from the original UIDs using Except
            //var uids = NoFilterUids.Except(FilteredUids).ToList();

            //foreach (var uid in uids)
            //{
            var items = inbox.Fetch(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.Headers);
            List<string> FoundFromList = new List<string>();
            foreach (var item in items)
            {
                var fromAddress = item.Envelope.From.Mailboxes.FirstOrDefault()?.Address;
                var listUnsubscribeHeader = item.Headers["List-Unsubscribe"];


                if (FoundFromList.Contains(fromAddress))
                {

                }
                else if (listUnsubscribeHeader != null)
                {
                    Console.WriteLine(listUnsubscribeHeader);
                    FoundFromList.Add(fromAddress);
                }

            }

            //Console.WriteLine(inbox.GetMessage(uid).Headers["List-Unsubscribe"].ToString());
            //Console.WriteLine("Email Number: " + uid.ToString());
            //List<string> links = ExtractLinksFromMailBody(inbox.GetMessage(uid).HtmlBody.ToString());
            //foreach (var link in links)
            //{
            //    Console.WriteLine(link);
            //}
            Console.WriteLine("\n\n\n");
            //}

            //for (int i = 0; i < inbox.Count; i++)
            //{
            //    var message = inbox.GetMessage(i);

            //    if (message.From.ToString().Contains("fredi.f5000@gmail.com"))
            //    {
            //        inbox.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
            //        inbox.Expunge();
            //        Console.WriteLine("it worked... or did it?");
            //    }
            //    Console.WriteLine(message.Subject);
            //}



            client.Disconnect(true);
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
