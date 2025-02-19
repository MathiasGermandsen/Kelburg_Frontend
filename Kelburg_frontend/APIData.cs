namespace Kelburg_frontend;

public class APIData
{
    protected readonly string _urlPath;

    public APIData(string name, string urlPath)
    {
        Name = name;
        _urlPath = urlPath;
    }

    public string Name { get; }

    public string Create => $"{_urlPath}/create";
    public string Read => $"{_urlPath}/read";
    public string Delete => $"{_urlPath}/delete";
}