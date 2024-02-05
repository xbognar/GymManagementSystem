namespace GymDBAccess.Services.Interfaces
{
    public interface IJwtService
    {

		string GenerateJwtToken(string username);

	}
}