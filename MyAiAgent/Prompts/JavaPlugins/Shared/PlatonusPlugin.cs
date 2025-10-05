using System.ComponentModel;
using Microsoft.SemanticKernel;
using MyAiAgent.Services;
using Refit;

namespace MyAiAgent.Prompts.JavaPlugins.Shared;

public class PlatonusPlugin
{
    private readonly IPlatonusService _platonusService = RestService.For<IPlatonusService>("https://aiu.c-platonus.kz");

    [KernelFunction("login")]
    [Description("Login to Platonus with credentials")]
    public async Task<string> Login(string password, string iin = "")
    {
        var request = new LoginRequest
        {
            AuthForDeductedStudentsAndGraduates = false,
            IcNumber = iin,
            Iin = iin,
            Login = null,
            Password = password
        };
        
        var response = await _platonusService.Login(request);
        return await response.ReadAsStringAsync();
    }
    
    [KernelFunction("get_schedule")]
    [Description("Fetch user schedule")]
    public async Task<string> GetSchedule(string sid, string token)
    {
        var response = await _platonusService.GetSchedule(sid, token);
        return await response.ReadAsStringAsync();
    }

    [KernelFunction("get_assignments")]
    [Description("Fetch assignments for subjects")]
    public async Task<string> GetAssignments(string sid, string token)
    {
        var response = await _platonusService.GetAssignments(sid, token);
        return await response.ReadAsStringAsync();
    }

    [KernelFunction("get_transcript")]
    [Description("Fetch transcript data")]
    public async Task<string> GetTranscript(string sid, string token)
    {
        var response = await _platonusService.GetTranscript(sid, token);
        return await response.ReadAsStringAsync();
    }
}