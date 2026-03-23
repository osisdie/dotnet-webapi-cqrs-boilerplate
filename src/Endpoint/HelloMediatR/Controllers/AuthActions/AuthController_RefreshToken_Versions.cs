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
        [ApiVersion("202603")]
        [ApiExplorerSettings(GroupName = "v202603")]
        [Route("api/v202603/auth/refresh-token")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> RefreshToken_v202603([FromBody] AuthRefreshToken_RequestDto requestDto) => await RefreshToken_v202303(requestDto);

        /// <summary>
        /// Refresh token key
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [ApiVersion("202303")]
        [ApiExplorerSettings(GroupName = "v202303")]
        [Route("api/v202303/auth/refresh-token")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> RefreshToken_v202303([FromBody] AuthRefreshToken_RequestDto requestDto) => await RefreshToken_v202203(requestDto);

        /// <summary>
        /// Refresh token key
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [ApiVersion("202203")]
        [ApiExplorerSettings(GroupName = "v202203")]
        [Route("api/v202203/auth/refresh-token")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> RefreshToken_v202203([FromBody] AuthRefreshToken_RequestDto requestDto) => await RefreshToken_v202103(requestDto);

        /// <summary>
        /// Refresh token key
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [ApiVersion("202103")]
        [ApiExplorerSettings(GroupName = "v202103")]
        [Route("api/v202103/auth/refresh-token")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> RefreshToken_v202103([FromBody] AuthRefreshToken_RequestDto requestDto)
        {
            var res = _sessionAdmin.RefeshToken(requestDto.RefreshToken);
            await Task.CompletedTask;
            return Ok(res);
        }
    }
}
