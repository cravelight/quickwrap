using Intuit.Ipp.DataService;

namespace quickwrap.connections
{
    public interface IQboConnection
    {
        DataService DataService { get; }
    }
}