using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain
{
    public class ResponseCommon
    {
        public BaseResponseModel? Result { get; set; }
        public object? ResultCommon { get; set; }
    }
}
