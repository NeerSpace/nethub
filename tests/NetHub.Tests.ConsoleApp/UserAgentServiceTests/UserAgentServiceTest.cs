using Microsoft.Extensions.DependencyInjection;
using Ng.Services;

namespace NetHub.Tests.ConsoleApp.UserAgentServiceTests;

public static class UserAgentServiceTest
{
    public static void Run()
    {
        var provider = BuildServiceProvider();
        var uaService = provider.GetRequiredService<IUserAgentService>();
        var uaList = Assets.GetTextAsset("1000-user-agent-samples.txt");
        var fina = "";

        foreach (var userAgent in uaList.Split('\n'))
        {
            // Console.WriteLine(userAgent);
            var ua = uaService.Parse(userAgent);
            fina += $"OS: {ua.Platform} \tBrowser: {ua.Browser} {ua.BrowserVersion} \tMobile: {ua.IsMobile} \tRobot: {ua.Robot}\n";
            // Console.WriteLine(result);
        }

        File.WriteAllText(Assets.GetAssetPath("output.txt"), fina);
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddUserAgentService();
        return services.BuildServiceProvider();
    }
}