﻿using System.Text.Json;
using API.data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
namespace API;

public static class HttpExtension
{
 public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header){
    var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));

    response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
 }
}