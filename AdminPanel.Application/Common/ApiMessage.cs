using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Common
{
    public class ApiMessage<T>
    {
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Value { get; set; }

    }
}
