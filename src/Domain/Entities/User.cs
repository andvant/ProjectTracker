namespace ProjectTracker.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    public User(string name, string email, DateTime registrationDate)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        RegistrationDate = registrationDate;
    }
}
