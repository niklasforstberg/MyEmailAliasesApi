using System.ComponentModel.DataAnnotations;

namespace EmailAliasApi.Models.DTOs;

public record LoginRequest(
    [property: Required][property: EmailAddress][property: MaxLength(256)] string Email,
    [property: Required][property: MaxLength(128)] string Password
);

public record RegisterRequest(
    [property: Required][property: EmailAddress][property: MaxLength(256)] string Email,
    [property: Required][property: MinLength(6)][property: MaxLength(128)] string Password
);

public record ForgotPasswordRequest(
    [property: Required][property: EmailAddress][property: MaxLength(256)] string Email
);

public record ResetPasswordRequest(
    [property: Required][property: MaxLength(512)] string Token,
    [property: Required][property: MinLength(6)][property: MaxLength(128)] string NewPassword
);