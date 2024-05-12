using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain
{
    public class EmailHelperCommon
    {
        public string? FromFullName { get; set; }
        public string? FromEmailAddress { get; set; }
        public string? ToFullName { get; set; }
        public string? ToEmailAddress { get; set; }
        public string? CCFullName { get; set; }
        public string? CCEmail { get; set; }
        public string? Message { get; set; }
        public string? SmtpPort { get; set; }
        public string? SmtpServer { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public string? EnableSsl { get; set; }
        public string? EmailAddress { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public string? UseDefaultCredentials { get; set; }
    }
}
