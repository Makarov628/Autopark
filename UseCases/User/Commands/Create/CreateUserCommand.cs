using MediatR;

namespace Autopark.UseCases.User.Commands.Create;

public class CreateUserCommand : IRequest<int> // возвращает Id созданного пользователя
{
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}