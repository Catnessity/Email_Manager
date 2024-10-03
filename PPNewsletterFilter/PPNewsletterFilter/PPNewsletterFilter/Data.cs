using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit;

namespace PPNewsletterFilter
{
    public static class Data
    {
         public static ImapClient? Client { get; set; }
         public static Dictionary<string, int>? map;
    }
}
