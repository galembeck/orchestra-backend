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
public class UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
    : _BaseController(httpContextAccessor)
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO body)
    {
        var securityInfo = GetSecurityInfo(Request);

        await new UserCreationValidator().ValidateAndThrowAsync(body);

        var model = await _userService.CreateAsync(CreateUserDTO.DTOToModel(body), securityInfo);

        return Ok(PublicUserDTO.ModelToDTO(model));
    }

    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN, ProfileType.PLATFORM_DEVELOPER)]
    [HttpGet]
    public async Task<IActionResult> Me(CancellationToken cancellationToken = default)
    {
        var securityInfo = GetSecurityInfo(Request);

        User response = await _userService.GetUserAsync(Authenticated.User.Id, securityInfo, cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(response));
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
            cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(user));
    }
}
