namespace SapSecurity.Model;

/// <summary>
/// http request body
/// </summary>
public class HttpMessageInfo
{
    /// <summary>
    /// base url
    /// </summary>
    public string RawUrl { get; set; }
    /// <summary>
    /// requested route
    /// </summary>
    public string Route { get; set; }
    /// <summary>
    /// POST or GET
    /// </summary>
    public string RequestType { get; set; }
    /// <summary>
    /// Authentication header
    /// </summary>
    public string? AuthHeader { get; set; }
    /// <summary>
    /// request body
    /// </summary>
    public string BodyMessage { get; set; }
    /// <summary>
    /// values in query string
    /// </summary>
    public Dictionary<string, string> QueryString { get; set; }
}