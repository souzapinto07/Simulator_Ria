using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Ria.Models
{
    public record ErrorResponse
    {
        public string Message { get; private set; }
        public List<string> Errors { get; private set; }

        public ErrorResponse(string message, List<string> errors)
        {
            Message = message;
            Errors = errors;
        }   
    }
}
