using Healthcare_Hospital_Management_System.Infrastructure;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xkcd
{
    public class XkcdClient : IXkcdClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public XkcdClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async IAsyncEnumerable<(string? Name, byte[] Content)> GetComicImageAsync(int number1, int number2, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();

            for (var i = number1; i <= number2; i++)
            {
                var comicInfoUrl = $"https://xkcd.com/{i}/info.0.json";
                var comicInfo = default(ComicInfo);

                var response = await httpClient.GetAsync(comicInfoUrl, cancellationToken);
                byte[] content;

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using (var streamReader = new StreamReader(stream))
                {
                    comicInfo = await JsonSerializer.DeserializeAsync<ComicInfo>(stream, JsonSerializerOptions.Default, cancellationToken);
                }

                var comicImageName = Path.GetFileName(comicInfo!.ImageUrl);

                var imageResponse = await httpClient.GetAsync(comicInfo!.ImageUrl, cancellationToken);

                var imageStream = await imageResponse.Content.ReadAsStreamAsync(cancellationToken);
                using (var streamReader = new StreamReader(imageStream))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms, cancellationToken);
                        content = ms.ToArray();
                    }
                }

                yield return (comicImageName, content);
            }
        }
    }

    internal class ComicInfo
    {
        [JsonPropertyName("img")]
        public string? ImageUrl { get; set; }
    }
}
