using System.Security.Cryptography;

namespace TechLabManagement.Services.Security;

public static class PasswordHasher
{
	public static void CreateHash(string password, out byte[] salt, out byte[] hash)
	{
		salt = RandomNumberGenerator.GetBytes(16);
		hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
	}

	public static bool Verify(string password, byte[] salt, byte[] hash)
	{
		var computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
		return CryptographicOperations.FixedTimeEquals(computed, hash);
	}
}


