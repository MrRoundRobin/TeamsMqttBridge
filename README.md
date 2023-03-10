# Teams MQTT Bridge

Using this tool, you can send Teams Meetings information to MQTT. With the Autodiscover option, these information will automatically be available in Home Assistant.

<img width="210" alt="Home Assistant" src="https://user-images.githubusercontent.com/948771/216786978-a753ec26-f939-4a5f-aa40-7abcc41ea58d.png"> <img width="210" alt="Settings" src="https://user-images.githubusercontent.com/948771/216786857-dbdfeb36-07e2-4e9d-ae19-986b7d09a9c5.png">

## Supported features

- Start and stop a meeting recording
- Raise and lower your hand
- Send reactions
- Mute and unmute
- Leave a meeting
- Turn your camera on and off
- Blur or unblur your background

## Prerequisites

- Teams third-party app API key ([see docs](https://support.microsoft.com/en-us/office/connect-third-party-devices-to-teams-aabca9f2-47bb-407f-9f9b-81a104a883d6))
- MQTT setup and integrated in Home Assistant ([see docs](https://www.home-assistant.io/integrations/mqtt/))

## Setup

- [Download](https://github.com/MrRoundRobin/TeamsMqttBridge/releases/latest) Executable
- Start the Executable
- Install .NET Runtime if requested
- Configure Settings:
  - Set Teams API token
  - Set MQTT Server address
  - Set credentials if needed
- After settings are saved application will start in the background
