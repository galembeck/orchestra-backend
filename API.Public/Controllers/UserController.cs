using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    IUserService userService,
    ICompanyService companyService,
    IHttpContextAccessor httpContextAccessor)
    : _BaseController(httpContextAccessor)
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ICompanyService _companyService = companyService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO body)
    {
        var securityInfo = GetSecurityInfo(Request);

        await new UserCreationValidator().ValidateAndThrowAsync(body);

        var model = await _userService.CreateAsync(CreateUserDTO.DTOToModel(body), securityInfo);

        return Ok(PublicUserDTO.ModelToDTO(model));
    }

    [HttpPost("check-availability")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckAvailability(
    [FromBody] CheckAvailabilityDTO body,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(body.Email) && string.IsNullOrWhiteSpace(body.Document))
            return BadRequest(new { message = "Provide email and/or document." });

        bool emailAvailable = true;
        bool documentAvailable = true;

        if (!string.IsNullOrWhiteSpace(body.Email))
        {
            var byEmail = await _userService.GetUsersByEmail(body.Email, cancellationToken);
            emailAvailable = byEmail is null || byEmail.Count == 0;
        }

        if (!string.IsNullOrWhiteSpace(body.Document))
        {
            var clean = new string(body.Document.Where(char.IsDigit).ToArray());
            var byDoc = await _userService.GetByDocumentAsync(clean, cancellationToken);
            documentAvailable = byDoc is null;
        }

        return Ok(new { emailAvailable, documentAvailable });
    }

    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN, ProfileType.PLATFORM_DEVELOPER)]
    [HttpGet]
    public async Task<IActionResult> Me(CancellationToken cancellationToken = default)
    {
        var securityInfo = GetSecurityInfo(Request);

        User response = await _userService.GetUserAsync(Authenticated.User.Id, securityInfo, cancellationToken);

        var dto = PublicUserDTO.ModelToDTO(response);

        if (response?.AccountType == Domain.Enumerators.AccountType.WORKER)
        {
            var ctx = await _companyService.GetWorkerContextAsync(response.Id, cancellationToken);
            if (ctx is not null)
                dto.Worker = WorkerContextDTO.Build(ctx.Value.member, ctx.Value.company, ctx.Value.role);
        }

        if (response?.AccountType == Domain.Enumerators.AccountType.COMPANY)
        {
            var companies = await _companyService.GetForUserAsync(response.Id, cancellationToken);
            var owned = companies.FirstOrDefault(c => c.OwnerUserId == response.Id) ?? companies.FirstOrDefault();
            if (owned is not null)
                dto.Company = CompanyContextDTO.Build(owned, owned.OwnerUserId == response.Id);
        }

        return Ok(dto);
    }

    [HttpPut("me")]
    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN, ProfileType.PLATFORM_DEVELOPER)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe([FromForm] UpdateProfileDTO dto, CancellationToken cancellationToken = default)
    {
        var userId = Authenticated?.User?.Id;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (dto.Password != dto.PasswordConfirmation)
                return BadRequest("Passwords do not match.");

            if (dto.Password.Length < 8)
                return BadRequest("Password must be at least 8 characters.");
        }

        var user = await _userService.UpdateProfileAsync(
            userId,
            dto.Name,
            dto.Email,
            dto.Cellphone,
            dto.Document,
            dto.Password,
            dto.ReceiveEmailOffers,
            dto.ReceiveWhatsappOffers,
            dto.Avatar,
            dto.Zipcode,
            dto.Address,
            dto.Number,
            dto.Complement,
            dto.Neighborhood,
            dto.City,
            dto.State,
            cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(user));
    }
}
