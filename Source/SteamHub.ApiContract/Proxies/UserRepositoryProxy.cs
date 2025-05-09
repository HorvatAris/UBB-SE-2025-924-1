// <copyright file="IUserServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;

    public class UserRepositoryProxy : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Ensures proper enum parsing
        };

        public UserRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Adjust to your actual backend URL
            };
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Users", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from CreateUserAsync");
        }

        public async Task UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Users/{id}", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }

        public async Task<GetUsersResponse?> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/Users");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>(_options);
            return result;
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Users/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
            var result = await response.Content.ReadFromJsonAsync<UserResponse>(_options);
            return result;
        }
    }
}
