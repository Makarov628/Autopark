namespace Autopark.UseCases.System.Commands.InitialSetup;

public record InitialSetupResponse(
    bool Success,
    string Message,
    int? AdminUserId = null
);