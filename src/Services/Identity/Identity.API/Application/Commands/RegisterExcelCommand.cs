using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Application.Commands
{
    public class RegisterExcelCommand : IRequest<bool>
    {
        public IFormFile formFile;
    }
}
