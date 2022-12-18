namespace WeatherForecast.Api.Models
{
    public class ResponseBase<T>
    {
        public int Status { get; set; } = 200;
        public T? Item { get; set; }
        public List<string> StatusTexts { get; set; } = new List<string>();
        public int Count { get; set; } = 0;
    }
}
