public class GreetingService : IGreetingService
{
    private readonly IConfiguration _config;

    public GreetingService (IConfiguration config)
    {
        _config = config;
    }
    
    public string GetGreeting(string username) 
    {
        var baseMessage = _config["GreetingService:DefaultMessage"];
        return $"{baseMessage}, {username}!";
    }

    public string GetGreetingMessage(string username)
    {
        throw new NotImplementedException();
    }
}