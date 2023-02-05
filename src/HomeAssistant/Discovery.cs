namespace Ro.Teams.MqttBridge.HomeAssistant;

internal class Discovery
{
    public string? Name { get; set; }
    public string? StateTopic { get; set; }
    public string? CommandTopic { get; set; }
    public string? PayloadAvailable { get; set; } = "ON";
    public string? PayloadNotAvailable { get; set; } = "OFF";
    public string? AvailabilityTopic { get; set; }
    public string? Icon { get; set; }
    public string? UniqueId { get; set; }
    public Device? Device { get; set; }
}
