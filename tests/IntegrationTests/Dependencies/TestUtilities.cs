using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;
using System.Collections.Generic;
using GymDBAccess.Models;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// Utility methods for integration tests, such as logging in to obtain a JWT.
	/// </summary>
	public static class TestUtilities
	{
		/// <summary>
		/// Logs in with environment-based credentials (LOGIN_USERNAME & LOGIN_PASSWORD)
		/// and returns the JWT token as a string.
		/// </summary>
		public static async Task<string> LoginAndGetTokenAsync(HttpClient client)
		{
			var username = System.Environment.GetEnvironmentVariable("LOGIN_USERNAME") ?? "testUser";
			var password = System.Environment.GetEnvironmentVariable("LOGIN_PASSWORD") ?? "testPass";

			var loginModel = new LoginModel
			{
				Username = username,
				Password = password
			};

			var response = await client.PostAsJsonAsync("/api/auth/login", loginModel);
			response.StatusCode.Should().Be(HttpStatusCode.OK,
				"login should succeed if credentials match environment variables");

			var jsonDict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
			jsonDict.Should().NotBeNull("the response must be valid JSON");

			jsonDict.Should().ContainKey("token");
			var token = jsonDict["token"];
			token.Should().NotBeNullOrEmpty("the Token property should not be null");

			return token;
		}

		/// <summary>
		/// Sets the 'Authorization' header to use the provided bearer token on subsequent requests.
		/// </summary>
		public static void AddAuthToken(this HttpClient client, string token)
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}
	}
}
