using System.Net;
using System.Threading.Tasks;
using CoreFX.Abstractions.Bases.Interfaces;
using CoreFX.Abstractions.Contracts;
using CoreFX.Abstractions.Enums;
using CoreFX.Abstractions.Extensions;
using CoreFX.Auth.Models;
using CoreFX.Notification.Models;
using Hello.MediatR.Domain.Contract;
using Hello.MediatR.Domain.Contract.AuthServices.Login;
using Hello.MediatR.Endpoint.Controllers.Bases;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hello.MediatR.Endpoint.Controllers.AuthActions
{
    public partial class AuthController : DomainContollerBase
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="requestDto">Username and password</param>
        /// <returns></returns>
        [ApiVersionNeutral]
        [ApiExplorerSettings(GroupName = "latest")]
        [Route("api/auth/login")] // latest
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> Login_latest([FromBody] HelloLogin_RequestDto requestDto) => await Login_v202603(requestDto);
    }
}
