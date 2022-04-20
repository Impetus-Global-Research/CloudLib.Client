using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace CloudLib.Client.WinUI.Core.Helpers
{
    public static class Json
    {
        public static async Task<T> ToObjectAsync<T>(Stream value)
            => await JsonSerializer.DeserializeAsync<T>(value);

        public static async Task<string> StringifyAsync(object value)
            => await Task.Run(()
                => JsonSerializer.Deserialize<string>(value.ToString()));
    }
}
