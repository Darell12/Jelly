using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Api.Attributes;
using Jellyfin.Api.Constants;
using Jellyfin.Api.Extensions;
using Jellyfin.Api.Helpers;
using Jellyfin.Api.ModelBinders;
using Jellyfin.Api.Models.StreamingDtos;
using MediaBrowser.Common.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Api.Controllers;

/// <summary>
/// The Content controller for dinamic redirect with token information.
/// </summary>
[ApiController]
[Route("Api")]
public class ContentController : BaseJellyfinApiController
{
    private readonly ILibraryManager _libraryManager;
    private readonly IUserManager _userManager;
    private readonly IDtoService _dtoService;
    private readonly IMediaSourceManager _mediaSourceManager;
    private readonly IServerConfigurationManager _serverConfigurationManager;
    private readonly IMediaEncoder _mediaEncoder;
    private readonly TranscodingJobHelper _transcodingJobHelper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EncodingHelper _encodingHelper;
    // private readonly TranscodingJobType _transcodingJobType = TranscodingJobType.Progressive;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentController"/> class.
    /// </summary>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    /// <param name="userManager">Instance of the <see cref="IUserManager"/> interface.</param>
    /// <param name="dtoService">Instance of the <see cref="IDtoService"/> interface.</param>
    /// <param name="mediaSourceManager">Instance of the <see cref="IMediaSourceManager"/> interface.</param>
    /// <param name="serverConfigurationManager">Instance of the <see cref="IServerConfigurationManager"/> interface.</param>
    /// <param name="mediaEncoder">Instance of the <see cref="IMediaEncoder"/> interface.</param>
    /// <param name="transcodingJobHelper">Instance of the <see cref="TranscodingJobHelper"/> class.</param>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="encodingHelper">Instance of <see cref="EncodingHelper"/>.</param>
    public ContentController(
        ILibraryManager libraryManager,
        IUserManager userManager,
        IDtoService dtoService,
        IMediaSourceManager mediaSourceManager,
        IServerConfigurationManager serverConfigurationManager,
        IMediaEncoder mediaEncoder,
        TranscodingJobHelper transcodingJobHelper,
        IHttpClientFactory httpClientFactory,
        EncodingHelper encodingHelper)
        {
        _libraryManager = libraryManager;
        _userManager = userManager;
        _dtoService = dtoService;
        _mediaSourceManager = mediaSourceManager;
        _serverConfigurationManager = serverConfigurationManager;
        _mediaEncoder = mediaEncoder;
        _transcodingJobHelper = transcodingJobHelper;
        _httpClientFactory = httpClientFactory;
        _encodingHelper = encodingHelper;
        }

    /// <summary>
    /// Method/Endpoint to define the response.
    /// </summary>
    /// <returns>Return the method defined by token.</returns>
    [HttpGet("base")]
    public ActionResult<string> Get()
    {
        return Ok("Respuesta Base 😎");
        // if (HttpContext.Items["method"] != null &&
        //     HttpContext.Items["method"]?.ToString() == "special")
        // {
        //     return SpecialMethod();
        // }

        // return DefaultMethod();
    }

    /// <summary>
    /// Method/Endpoint to define the response.
    /// </summary>
    /// <returns>Return the method defined by token.</returns>
    [HttpGet("")]
    public ActionResult<string> Get2()
    {
        return Ok("Respuesta Base 😎2");
    }
}
