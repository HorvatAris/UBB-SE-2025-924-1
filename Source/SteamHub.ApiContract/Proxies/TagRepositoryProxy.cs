// <copyright file="ITagServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Repositories;

    public class TagRepositoryProxy : ITagRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public TagRepositoryProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");

        }


        public async Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Tags", request);
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<CreateTagResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from CreateTagAsync");
        }

        public async Task DeleteTagAsync(int tagId)
        {
            var response = await _httpClient.DeleteAsync($"/api/Tags/{tagId}");
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task<GetTagsResponse> GetAllTagsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Tags");
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<GetTagsResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetAllTagsAsync");
        }

        public async Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId)
        {
            var response = await _httpClient.GetAsync($"/api/Tags/{tagId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<TagNameOnlyResponse>(_options);
            return result;
        }

        public async Task UpdateTagAsync(int tagId, UpdateTagRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/Tags/{tagId}", content);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }
    }
}