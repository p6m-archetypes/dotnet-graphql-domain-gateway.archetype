using Serilog;

namespace {{ ProjectName }}.Server.GraphQL;

public class GraphQlErrorFilter : IErrorFilter
{

    public IError OnError(IError error)
    {
        // Log the exception if there is one
        if (error.Exception is not null)
        {
            Log.Error(error.Exception, "An unhandled exception occurred in GraphQL execution.");
        }
        else
        {
            Log.Error("A GraphQL error occurred: {ErrorMessage}", error.Message);
        }

        // Return the error as is, or modify it for clients
        return error.WithMessage("An unexpected error occurred."); // Custom message
    }
}