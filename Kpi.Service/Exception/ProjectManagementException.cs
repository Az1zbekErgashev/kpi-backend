namespace Kpi.Service.Exception;

public class KpiException : System.Exception
{
    public int Code { get; set; }
    public bool? Global { get; set; }

    public KpiException(int code, string message, bool? global = true) : base(message)
    {
        Code = code;
        Global = global;
    }
}
