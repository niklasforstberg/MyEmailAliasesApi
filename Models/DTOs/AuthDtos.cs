namespace EmailAliasApi.Models.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password); 