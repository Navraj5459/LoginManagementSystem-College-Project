using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain
{
    public class BaseResponseModel
    {
        public int ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? Id { get; set; }
        public object? Extra { get; set; }
        public int? StatusCode { get; set; }
        public void SetResponse(int errorcode, string msg, string id)
        {
            ErrorCode = errorcode;
            Message = msg;
            Id = id;
        }
    }
}
