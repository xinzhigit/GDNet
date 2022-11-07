using Net.Share;

namespace Client;

public class Example
{
    [Rpc]
    void Test(string str)
    {
        Console.WriteLine(str);
    }
}