﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit;
using System.Text.Json;
using System.IO;
using System.Text.Json.Nodes;
using Org.BouncyCastle.Tls;
using System.Reflection.Metadata;

namespace PPNewsletterFilter
{
    public static class Data
    {
        private static readonly object lockObject = new object();

        public static ImapClient? Client { get; set; }
        public static List<Tuple<string, int, bool, string, List<UniqueId>>>? map;

        public static JsonNode? unsubscribedSenders;
        public static string unsubscribedSendersFilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\unsubscribedMails.json";


        //load existing data or create new file to store unsubscribed senders
        public static void InitializeUnsubscribedSenders()
        {
            if (!File.Exists(unsubscribedSendersFilepath))
            {
                //create file with empty array content
                File.WriteAllText(unsubscribedSendersFilepath, "[]");
            }
            unsubscribedSenders = JsonNode.Parse(File.ReadAllText(unsubscribedSendersFilepath));
        }

        public static void AddDataToUnsubscribedSenders(string mailsender, string link)
        {
            if (unsubscribedSenders != null && unsubscribedSenders is JsonArray)
            {
                DateTime currentDate = DateTime.Now;
                var newEntry = new JsonObject
                {
                    ["sender"] = mailsender,
                    ["link"] = link,
                    ["unsubscribedOn"] = currentDate.ToString()
                };

                lock (lockObject)
                {
                    unsubscribedSenders.AsArray().Add(newEntry);
                    File.WriteAllText(unsubscribedSendersFilepath, unsubscribedSenders.ToJsonString());
                }
            }
            else
            {
                Console.WriteLine("Invalid JSON structure.");
            }
        }

        //public bool SenderIgnoredUnsubscribe(string sender)
        //{

        //}

    }
}
    

