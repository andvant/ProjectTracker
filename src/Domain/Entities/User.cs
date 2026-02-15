namespace ProjectTracker.Domain.Entities;

public class User : Entity
{
    public string Name { get; }
    public string Email { get; }
    public DateTimeOffset RegistrationDate { get; }

    public User(string name, string email, DateTimeOffset registrationDate)
    {
        Name = name;
        Email = email;
        RegistrationDate = registrationDate;
    }
}
