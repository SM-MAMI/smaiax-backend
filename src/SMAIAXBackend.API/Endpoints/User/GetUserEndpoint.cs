using Microsoft.AspNetCore.Http.HttpResults;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.User;

public static class GetUserEndpoint
{
    public static async  Task<Ok<UserDto>> Handle(IUserService userService,HttpContext context)
    {
        var userId = context.Items["UserId"] as string;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException();
        }

        var userDto = await userService.GetUserByIdAsync(new Guid(userId));
        
        return TypedResults.Ok(userDto);
    }
}