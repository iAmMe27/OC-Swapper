using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OC_Swapper.Swapper.Json;

public class JsonInterface
{
    public static async Task<T?> Read<T>(string filename)
    {
        await using FileStream openStream = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<T>(openStream);
    }

    public static async Task Write<T>(string filename, T obj)
    {
        await using FileStream openStream = new(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
        await JsonSerializer.SerializeAsync(openStream, obj);
    }
}