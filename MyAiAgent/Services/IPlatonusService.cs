using MyAiAgent.Controllers;
using Refit;

namespace MyAiAgent.Services;

public interface IPlatonusService
{
    [Post("/rest/api/login")]
    Task<LoginResponse> Login([Body] LoginRequest request);

    [Post("/rest/schedule/userSchedule/student/initial/0/ru")]
    Task<HttpContent> GetSchedule([Header("Sid")] string sid, [Header("Token")] string token);

    [Get("/rest/assignments/subjects/2025/1/ru")]
    Task<HttpContent> GetAssignments([Header("Sid")] string sid, [Header("Token")] string token);

    [Post("/rest/transcript/load/ru/0")]
    Task<HttpContent> GetTranscript([Header("Sid")] string sid, [Header("Token")] string token);
}

public class LoginRequest
{
    public bool AuthForDeductedStudentsAndGraduates { get; set; }
    public string IcNumber { get; set; }
    public string Iin { get; set; }
    public string Login { get; set; } = null;
    public string Password { get; set; }
}