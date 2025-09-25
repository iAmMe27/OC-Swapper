using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OC_Swapper.Swapper.Json;

public static class JsonInterface
{
    public static async Task<T?> ReadAsync<T>(string filename)
    {
        await using FileStream openStream = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<T>(openStream);
    }
    
    public static T? Read<T>(string filename)
    {
        using FileStream openStream = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        return JsonSerializer.Deserialize<T>(openStream) ?? default;
    }

    public static async Task WriteAsync<T>(string filename, T obj)
    {
        await using FileStream openStream = new(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
        await JsonSerializer.SerializeAsync(openStream, obj);
    }

    public static void Write<T>(string filename, T obj)
    {
        using FileStream openStream = new(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
        JsonSerializer.Serialize(openStream, obj);
    }
}