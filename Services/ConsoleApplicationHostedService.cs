namespace Console.Application.Services;

public abstract class ConsoleApplicationHostedService
{
    public abstract Task OnStart();
    public abstract Task OnExit();
}