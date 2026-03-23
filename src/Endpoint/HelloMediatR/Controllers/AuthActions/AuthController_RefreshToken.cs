using System.Net;
using System.Threading.Tasks;
using CoreFX.Abstractions.Bases.Interfaces;
using CoreFX.Auth.Contracts.RefreshToken;
using CoreFX.Auth.Models;
using Hello.MediatR.Endpoint.Controllers.Bases;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hello.MediatR.Endpoint.Controllers.AuthActions
{
    public partial class AuthController : DomainContollerBase
    {
        /// <summary>
        /// Refresh token key
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [ApiVersionNeutral]
        [ApiExplorerSettings(GroupName = "latest")]
        [Route("api/auth/refresh-token")] // latest
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> RefreshToken_latest([FromBody] AuthRefreshToken_RequestDto requestDto) => await RefreshToken_v202603(requestDto);
    }
}
