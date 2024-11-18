namespace {{ ProjectName}}.Server;

public interface IAppStartup
{
    public void ConfigureServices(IServiceCollection services);
}