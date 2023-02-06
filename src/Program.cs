namespace Ro.Teams.MqttBridge;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using Ro.Teams.LocalApi;
using Ro.Teams.MqttBridge.HomeAssistant;
using Ro.Teams.MqttBridge.Utils;
using System.Reflection;
using System.Text;
using System.Text.Json;

using static Properties.Settings;

internal static class Program
{
    internal static Client? TeamsClient { get; set; }
    internal static IManagedMqttClient? MqttClient { get; set; }

    internal static event EventHandler? MqttConnected;
    internal static event EventHandler? MqttDisconnected;
    internal static event EventHandler? TeamsConnected;
    internal static event EventHandler? TeamsDisconnected;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        if (!Version.TryParse(Default.SettingsVersion, out Version? settingsVersion) || settingsVersion < Assembly.GetExecutingAssembly().GetName().Version)
        {
            Default.Upgrade();
            
            if (settingsVersion < new Version("0.2.0.0"))
            {
                if (!string.IsNullOrEmpty(Default.TeamsToken))
                    Default.TeamsToken = Default.TeamsToken.Encrypt();

                if (!string.IsNullOrEmpty(Default.MqttPassword))
                    Default.MqttPassword = Default.MqttPassword.Encrypt();
            }

            Default.SettingsVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            Default.Save();
        }

        TryReconnectTeams();
        TryReconnectMqtt();
        ApplicationConfiguration.Initialize();
        Application.Run(new Daemon());
    }

    internal static void TryReconnectTeams()
    {
        if (TeamsClient is not null && TeamsClient.IsConnected)
            TeamsClient.Disconnect();

        if (string.IsNullOrEmpty(Default.TeamsToken))
            return;

        TeamsClient = new(Default.TeamsToken.Decrypt(), true);
        TeamsClient.PropertyChanged += (_, e) => SendUpdate(e.PropertyName);
        TeamsClient.Connected += (o,e) => TeamsConnected?.Invoke(o, e);
        TeamsClient.Disconnected += (o, e) => TeamsDisconnected?.Invoke(o, e);
    }

    internal async static void TryReconnectMqtt()
    {
        if (MqttClient is not null && MqttClient.IsConnected)
            await MqttClient.StopAsync();

        MqttClient?.Dispose();

        if (!Uri.TryCreate(Default.MqttUrl, UriKind.Absolute, out Uri? uri) || uri is null)
            return;

        MqttClientOptionsBuilder clientOptions;

        switch (uri.Scheme)
        {
            case "mqtt":
                clientOptions = new MqttClientOptionsBuilder().WithTcpServer(uri.Host, uri.Port);
                break;

            case "mqtts":
                clientOptions = new MqttClientOptionsBuilder().WithTcpServer(uri.Host, uri.Port).WithTls();
                break;

            case "ws":
                clientOptions = new MqttClientOptionsBuilder().WithWebSocketServer(uri.ToString());
                break;

            case "wss":
                clientOptions = new MqttClientOptionsBuilder().WithWebSocketServer(uri.ToString()).WithTls();
                break;

            default:
                return;
        }

        if (!string.IsNullOrWhiteSpace(Default.MqttUsername) && !string.IsNullOrEmpty(Default.MqttPassword))
        {
            clientOptions = clientOptions.WithCredentials(Default.MqttUsername, Default.MqttPassword.Decrypt());
        }

        var managedClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(clientOptions.Build())
                .Build();

        MqttClient = new MqttFactory().CreateManagedMqttClient();

        MqttClient.ConnectedAsync    += (e) => { MqttConnected?.Invoke(MqttClient, EventArgs.Empty); return Task.CompletedTask; };
        MqttClient.DisconnectedAsync += (e) => { MqttDisconnected?.Invoke(MqttClient, EventArgs.Empty); return Task.CompletedTask; };

        await MqttClient.StartAsync(managedClientOptions);

        MqttClient.ApplicationMessageReceivedAsync += MqttUpdateReceived;
        await MqttClient.SubscribeAsync($"teams/{Default.NodeName}/state/+/command");
        await MqttClient.SubscribeAsync($"teams/{Default.NodeName}/action/+");

        if (Default.Autodiscover)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            var device = new Device()
            {
                Identifiers = Default.NodeName,
                Name = Default.NodeName,
                SwVersion = "1.0.0",
                Model = "Teams HA Link",
                Manufacturer = "MrRoundRobin",
            };

            await MqttClient.EnqueueAsync($"homeassistant/switch/{Default.NodeName}/mute/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Mute",
                StateTopic = $"teams/{Default.NodeName}/state/isMuted",
                CommandTopic = $"teams/{Default.NodeName}/state/isMuted/command",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:microphone",
                UniqueId = $"TeamsMeetingStatusMute{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/switch/{Default.NodeName}/camera/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Camera",
                StateTopic = $"teams/{Default.NodeName}/state/isCameraOn",
                CommandTopic = $"teams/{Default.NodeName}/state/isCameraOn/command",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:video",
                UniqueId = $"TeamsMeetingStatusCamera{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/switch/{Default.NodeName}/hand/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Hand",
                StateTopic = $"teams/{Default.NodeName}/state/isHandRaised",
                CommandTopic = $"teams/{Default.NodeName}/state/isHandRaised/command",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:hand-front-left",
                UniqueId = $"TeamsMeetingStatusHand{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/binary_sensor/{Default.NodeName}/in_meeting/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "In Meeting",
                StateTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:human-male-board",
                UniqueId = $"TeamsMeetingStatusInMeeting{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/switch/{Default.NodeName}/recording/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Recording",
                StateTopic = $"teams/{Default.NodeName}/state/isRecordingOn",
                CommandTopic = $"teams/{Default.NodeName}/state/isRecordingOn/command",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:movie-roll",
                UniqueId = $"TeamsMeetingStatusRecording{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/switch/{Default.NodeName}/blur/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Background Blur",
                StateTopic = $"teams/{Default.NodeName}/state/isBackgroundBlurred",
                CommandTopic = $"teams/{Default.NodeName}/state/isBackgroundBlurred/command",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:blur-linear",
                UniqueId = $"TeamsMeetingStatusBlur{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/leave/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Leave",
                CommandTopic = $"teams/{Default.NodeName}/action/leaveCall",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:run",
                UniqueId = $"TeamsMeetingActionLeave{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/applause/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Applause",
                CommandTopic = $"teams/{Default.NodeName}/action/reactApplause",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:hand-clap",
                UniqueId = $"TeamsMeetingActionApplause{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/laugh/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Laugh",
                CommandTopic = $"teams/{Default.NodeName}/action/reactLaugh",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:emoticon-excited-outline",
                UniqueId = $"TeamsMeetingActionLaugh{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/like/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Like",
                CommandTopic = $"teams/{Default.NodeName}/action/reactLike",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:thumb-up-outline",
                UniqueId = $"TeamsMeetingActionLike{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/love/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Love",
                CommandTopic = $"teams/{Default.NodeName}/action/reactLove",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:heart-outline",
                UniqueId = $"TeamsMeetingActionLove{Default.NodeName}",
                Device = device,
            }, options), retain: true);

            await MqttClient.EnqueueAsync($"homeassistant/button/{Default.NodeName}/wow/config", JsonSerializer.Serialize(new Discovery()
            {
                Name = "Wow",
                CommandTopic = $"teams/{Default.NodeName}/action/reactWow",
                AvailabilityTopic = $"teams/{Default.NodeName}/state/isInMeeting",
                Icon = "mdi:emoticon-dead-outline",
                UniqueId = $"TeamsMeetingActionWow{Default.NodeName}",
                Device = device,
            }, options), retain: true);
        }

        SendUpdate();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    internal async static Task MqttUpdateReceived(MqttApplicationMessageReceivedEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (TeamsClient is null || !TeamsClient.IsConnected)
            return;

        var action = e.ApplicationMessage.Topic.Split('/')[3];

        var stateString = Encoding.UTF8.GetString(e.ApplicationMessage.Payload, 0, e.ApplicationMessage.Payload.Length).ToLowerInvariant();

        if (!bool.TryParse(stateString, out bool state))
        {
            state = stateString == "on";
        }

        switch (action)
        {
            case "isMuted":
                TeamsClient.IsMuted = state;
                break;

            case "isCameraOn":
                TeamsClient.IsCameraOn = state;
                break;

            case "isHandRaised":
                TeamsClient.IsHandRaised = state;
                break;

            case "isRecordingOn":
                TeamsClient.IsRecordingOn = state;
                break;

            case "isBackgroundBlurred":
                TeamsClient.IsBackgroundBlurred = state;
                break;

            case "leaveCall":
                _ = TeamsClient.LeaveCall();
                break;

            case "reactApplause":
                _ = TeamsClient.ReactApplause();
                break;

            case "reactLaugh":
                _ = TeamsClient.ReactLaugh();
                break;

            case "reactLike":
                _ = TeamsClient.ReactLike();
                break;

            case "reactLove":
                _ = TeamsClient.ReactLove();
                break;

            case "reactWow":
                _ = TeamsClient.ReactLove();
                break;
        }
    }

    internal async static void SendUpdate(string? propertyName = null)
    {
        if (TeamsClient is null || !TeamsClient.IsConnected || MqttClient is null)
            return;

        if (propertyName is null || propertyName == nameof(TeamsClient.IsMuted))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isMuted", TeamsClient.IsMuted ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.IsCameraOn))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isCameraOn", TeamsClient.IsCameraOn ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.IsHandRaised))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isHandRaised", TeamsClient.IsHandRaised ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.IsInMeeting))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isInMeeting", TeamsClient.IsInMeeting ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.IsRecordingOn))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isRecordingOn", TeamsClient.IsRecordingOn ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.IsBackgroundBlurred))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/state/isBackgroundBlurred", TeamsClient.IsBackgroundBlurred ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanToggleMute))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canToggleMute", TeamsClient.CanToggleMute ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanToggleVideo))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canToggleVideo", TeamsClient.CanToggleVideo ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanToggleHand))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canToggleHand", TeamsClient.CanToggleHand ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanToggleBlur))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canToggleBlur", TeamsClient.CanToggleBlur ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanToggleRecord))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canToggleRecord", TeamsClient.CanToggleRecord ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanLeave))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canLeave", TeamsClient.CanLeave ? "ON" : "OFF", retain: true);

        if (propertyName is null || propertyName == nameof(TeamsClient.CanReact))
            await MqttClient.EnqueueAsync($"teams/{Default.NodeName}/permission/canReact", TeamsClient.CanReact ? "ON" : "OFF", retain: true);
    }
}