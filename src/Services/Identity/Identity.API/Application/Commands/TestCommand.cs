using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace Identity.API.Application.Commands
{
    public class TestCommand : IRequest<bool>
    {
        public TestCommand(string email)
        {
            Email = email;
        }

        [DataMember] public string Email { get; set; }
    }
}
