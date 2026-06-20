using Newtonsoft.Json;

public class FirebaseNotification
{
    [JsonProperty("message")]
    public Message Message { get; set; }
}

public class Message
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("notification")]
    public Notification Notification { get; set; }

    [JsonProperty("android")]
    public Android Android { get; set; }

    [JsonProperty("apns")]
    public Apns Apns { get; set; }

    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string> Data { get; set; }
}

public class Notification
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }
}

public class Android
{
    [JsonProperty("priority")]
    public string Priority { get; set; }
}

public class Apns
{
    [JsonProperty("headers")]
    public ApnsHeaders Headers { get; set; }
}

public class ApnsHeaders
{
    [JsonProperty("apns-priority")]
    public string ApnsPriority { get; set; }
}
