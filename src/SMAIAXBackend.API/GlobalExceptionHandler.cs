using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SMAIAXBackend.Application.Exceptions;

namespace SMAIAXBackend.API;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = exception.Message
        };

        switch (exception)
        {
            case RegistrationException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Registration Error";
                break;
            default:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                // Override details to not expose internal error details
                problemDetails.Detail = "Something went wrong.";
                break;
        }
        
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}