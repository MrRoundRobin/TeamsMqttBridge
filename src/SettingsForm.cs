namespace Ro.Teams.MqttBridge;

using Ro.Teams.MqttBridge.Utils;
using static Properties.Settings;

public partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();
    }

    private void Save(object sender, EventArgs e)
    {
        if (!Guid.TryParse(teamsToken.Text, out Guid token))
        {
            teamsTokenLabel.ForeColor = Color.Red;

            return;
        }
        else
        {
            teamsTokenLabel.ForeColor = SystemColors.ControlText;
        }

        if (!Uri.TryCreate(mqttServer.Text, UriKind.Absolute, out Uri? uri))
        {
            mqttServerLabel.ForeColor = Color.Red;

            return;
        }
        else
        {
            mqttServerLabel.ForeColor = SystemColors.ControlText;
        }

        if (useAuthentication.Checked && string.IsNullOrEmpty(mqttUsername.Text))
        {
            mqttUsernameLabel.ForeColor = Color.Red;

            return;
        }
        else
        {
            mqttUsernameLabel.ForeColor = SystemColors.ControlText;
        }

        if (useAuthentication.Checked && string.IsNullOrEmpty(mqttPassword.Text))
        {
            mqttPasswordLabel.ForeColor = Color.Red;

            return;
        }
        else
        {
            mqttPasswordLabel.ForeColor = SystemColors.ControlText;
        }

        bool mqttChanged = false;

        if (useAuthentication.Checked)
        {
            var encryptedPassword = mqttPassword.Text.Encrypt();
            if (mqttPassword.Text != "*********" && encryptedPassword != Default.MqttPassword)
            {
                Default.MqttPassword = encryptedPassword;
                mqttChanged = true;
            }
        }
        else
        {
            if (Default.MqttPassword != string.Empty)
            {
                Default.MqttPassword = string.Empty;
                mqttChanged = true;
            }
        }

        if (Default.MqttUsername != mqttUsername.Text)
        {
            Default.MqttUsername = mqttUsername.Text;

            mqttChanged = Default.MqttPassword != string.Empty;
        }

        var teamsChanged = false;

        var encryptedToken = token.ToString().Encrypt();
        if (token != Guid.Empty && Default.TeamsToken != encryptedToken)
        {
            Default.TeamsToken = encryptedToken;
            teamsChanged = true;
        }

        if (Default.MqttUrl != uri.AbsoluteUri)
        {
            Default.MqttUrl = uri.AbsoluteUri;
            mqttChanged = true;
        }

        if (Default.Autodiscover != autodiscover.Checked)
        {
            Default.Autodiscover = autodiscover.Checked;
            mqttChanged = true;
        }

        Default.Save();

        if (teamsChanged)
            Program.TryReconnectTeams();

        if (mqttChanged)
            Program.TryReconnectMqtt();

        Program.SendUpdate();
        UpdateForm();
    }

    private void SelectAuthentication(object sender, EventArgs e)
    {
        mqttUsername.Enabled = useAuthentication.Checked;
        mqttPassword.Enabled = useAuthentication.Checked;
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        UpdateForm();

        Program.MqttConnected    += (_, _) => UpdateMqttStatus();
        Program.MqttDisconnected += (_, _) => UpdateMqttStatus();
        UpdateMqttStatus();

        Program.TeamsConnected    += (_, _) => UpdateTeamsStatus();
        Program.TeamsDisconnected += (_, _) => UpdateTeamsStatus();
        UpdateTeamsStatus();
    }

    private void UpdateForm()
    {
        mqttUsername.Text = Default.MqttUsername;

        if (!string.IsNullOrWhiteSpace(Default.MqttPassword))
        {
            mqttPassword.Text = "*********";
            useAuthentication.Checked = true;
        }
        else
        {
            mqttPassword.Text = string.Empty;
            useAuthentication.Checked = false;
        }

        if (!string.IsNullOrEmpty(Default.TeamsToken))
        {
            teamsToken.Text = Guid.Empty.ToString();
        }

        autodiscover.Checked = Default.Autodiscover;

        mqttServer.Text = Default.MqttUrl.ToString();
    }

    private void UpdateMqttStatus()
    {
        var status = Program.MqttClient is not null && Program.MqttClient.IsConnected;

        if (InvokeRequired)
            Invoke(() =>
            {
                mqttStatus.Text = status ? "Connected" : "Not Connected";
                mqttStatus.ForeColor = status ? Color.Green : Color.Red;
            });
        else
            mqttStatus.Text = status ? "Connected" : "Not Connected";
            mqttStatus.ForeColor = status ? Color.Green : Color.Red;
    }

    private void UpdateTeamsStatus()
    {
        var status = Program.TeamsClient is not null && Program.TeamsClient.IsConnected;

        teamsStatus.Text = status ? "Connected" : "Not Connected";
        teamsStatus.ForeColor = status ? Color.Green : Color.Red;
    }

    private void reconnect_Click(object sender, EventArgs e)
    {
        Program.TryReconnectTeams();
        Program.TryReconnectMqtt();
    }

    private void exit_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}