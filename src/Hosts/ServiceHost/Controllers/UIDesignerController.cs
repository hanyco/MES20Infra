//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Web.Http.Description;
//using HanyCo.Infra.Designers.UI.Queries;
//using HanyCo.Infra.Dto;
//using HanyCo.Infra.Web;
//using HanyCo.Infra.Web.Controllers;

//using Microsoft.AspNetCore.Mvc;

//namespace HanyCo.Infra.ServiceHost.Areas.FormDesigner.Controllers
//{
//    /// <summary>
//    /// Form Generator REST API
//    /// </summary>
//    /// <seealso cref="HanyCo.Infra.Web.Controllers.MesApiControllerBase" />
//    [ApiController]
//    [Route("api/[controller]")]

//    //! [Authorize(Roles = "Admin,Designer")] //! or

//    //! [Authorize(Roles = "Admin")] //! and
//    //! [Authorize(Roles = "Designer")]

//    //[AllowAnonymous]
//    public class UIDesignerController : MesApiControllerBase
//    {
//        /// <summary>
//        /// Gets the modules.
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("modules")]
//        [ResponseType(typeof(IApiResult<IEnumerable<ModuleDto>?>))]
//        public async Task<IApiResult<IEnumerable<ModuleDto>>> GetModules() =>
//            await this.EnqueryAsync<GetModulesQueryParameter, GetModulesQueryResult, IEnumerable<ModuleDto>>();

//        /// <summary>
//        /// 📦 Gets the module by module identifier.
//        /// </summary>
//        /// <param name="id">🆔 The identifier.</param>
//        /// <returns>Works nicely ❤</returns>
//        /// <response code="200" link="www.google.com">😊 Module found and returned.. </response>
//        /// <response code="400">😢 Module not found.</response>
//        /// <response code="500">💥 Somthnig wrong happened. Is database available?</response>
//        /// <remarks>Gets the module by module identifier and presents to you.🎁</remarks>
//        [HttpGet("modules/{id}")]
//        [HttpGet("module/{id}")]
//        [ResponseType(typeof(IApiResult<ModuleDto>))]
//        [ProducesResponseType(500)]
//        public async Task<IApiResult<ModuleDto>> GetModuleById(Guid id) =>
//            await this.EnqueryAsync<GetModuleByIdQueryResult, ModuleDto>(new GetModuleByIdQueryParameter(id));

//        /// <summary>
//        /// Gets the DTOs by module identifier.
//        /// </summary>
//        /// <param name="id">The identifier.</param>
//        /// <returns></returns>
//        [HttpGet("modules/{id}/dtos")]
//        [HttpGet("module/{id}/dtos")]
//        [HttpGet("module/{id}/dto")]
//        [HttpGet("modules/{id}/dto")]
//        public async Task<IApiResult<IEnumerable<DTODto>?>> GetDtosByModuleId(Guid id) =>
//            await this.EnqueryAsync<GetDtosByModuleIdQueryResult, IEnumerable<DTODto>?>(new GetDtosByModuleIdQueryParameter(id));

//        /// <summary>
//        /// Gets the DTO by identifier.
//        /// </summary>
//        /// <param name="id">The identifier.</param>
//        /// <returns></returns>
//        [HttpGet("dtos/{id}")]
//        [HttpGet("dto/{id}")]
//        public async Task<IApiResult<DTODto?>> GetDtoById(Guid id)
//        {
//            var cqResult = await this.QueryProcessor.ExecuteAsync(new GetDtoByIdQueryParameter(id));
//            return this.Succees(cqResult.Result);
//        }

//        /// <summary>
//        /// Does nothing . Just for testing Controller health.
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("five")]
//        public async Task<IApiResult> Five() => await Task.FromResult(ApiHelper.New("This is Five."));
//    }
//}