using ApiMoniEquipamentosPesados.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiMoniEquipamentosPesados.GlobalExceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is RegraDeNegocioException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Erro de Validação";
            problemDetails.Detail = exception.Message;
        }
        else if (exception is NaoEncontradoException)
        {
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = "Recurso Não Encontrado";
            problemDetails.Detail = exception.Message;
        }
        else
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Erro Interno no Servidor";
            problemDetails.Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; 
    }
}