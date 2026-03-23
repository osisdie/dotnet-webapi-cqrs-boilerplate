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
        [ApiVersion("202603")]
        [ApiExplorerSettings(GroupName = "v202603")]
        [Route("api/v202603/auth/login")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> Login_v202603([FromBody] HelloLogin_RequestDto requestDto) => await Login_v202303(requestDto);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="requestDto">Username and password</param>
        /// <returns></returns>
        [ApiVersion("202303")]
        [ApiExplorerSettings(GroupName = "v202303")]
        [Route("api/v202303/auth/login")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> Login_v202303([FromBody] HelloLogin_RequestDto requestDto) => await Login_v202203(requestDto);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="requestDto">Username and password</param>
        /// <returns></returns>
        [ApiVersion("202203")]
        [ApiExplorerSettings(GroupName = "v202203")]
        [Route("api/v202203/auth/login")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> Login_v202203([FromBody] HelloLogin_RequestDto requestDto) => await Login_v202103(requestDto);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="requestDto">Username and password</param>
        /// <returns></returns>
        [ApiVersion("202103")]
        [ApiExplorerSettings(GroupName = "v202103")]
        [Route("api/v202103/auth/login")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ISvcResponseBaseDto<JwtTokenDto>))]
        public async Task<IActionResult> Login_v202103([FromBody] HelloLogin_RequestDto requestDto)
        {
            var svcRes = await _svcLogin.ExecuteAsync(requestDto);
            if (!svcRes.Any())
            {
                svcRes.SetSubCode(HelloMediatRSvcCodeEnum.UnAuthenticated);

                _mediator?.Publish(new SvcEvent_MetadataDto
                {
                    From = HttpContext.Request.GetDisplayUrl(),
                    Category = "user-login",
                    IsSuccess = svcRes.Any(),
                    User = requestDto.UserName,
                    RequestDto = requestDto,
                    ResponseDto = svcRes,
                    Context = HttpContext
                });

                return new UnauthorizedObjectResult(svcRes);
            }

            var jwtRes = await _sessionAdmin.Authentication(requestDto.UserName, requestDto.Password);
            if (!jwtRes.Any())
            {
                svcRes.SetSubCode(SvcCodeEnum.TokenCreateFailed);
                return new UnauthorizedObjectResult(jwtRes);
            }

            var res = new SvcResponseDto<HelloLogin_ResponseDto>
            {
                Data = new HelloLogin_ResponseDto
                {
                    UserId = jwtRes.Data.UserId,
                    UserName = jwtRes.Data.UserName,
                    Token = jwtRes.Data.Token,
                    RefreshToken = jwtRes.Data.RefreshToken,
                    Exp = jwtRes.Data.Exp,
                }
            }.Success();

            return Ok(res);
        }
    }
}
