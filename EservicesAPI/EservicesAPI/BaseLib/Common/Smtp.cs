using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Common
{
    public class Smtp
    {
        public string SmtpUrl { get; set; }
        public int Port { get; set; }
        public string FromMail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Text { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentName { get; set; }
        public string ToMail { get; set; }
    }
}
