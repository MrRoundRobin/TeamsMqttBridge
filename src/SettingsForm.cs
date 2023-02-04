namespace ro.TeamsMqttBridge;

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

        bool mqttChanged = false;

        if (useAuthentication.Checked)
        {
            if (string.IsNullOrEmpty(mqttUsername.Text))
            {
                mqttUsernameLabel.ForeColor = Color.Red;

                return;
            }
            else
            {
                mqttUsernameLabel.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrEmpty(mqttPassword.Text))
            {
                mqttPasswordLabel.ForeColor = Color.Red;

                return;
            }
            else
            {
                mqttPasswordLabel.ForeColor = SystemColors.ControlText;
            }

            if (Default.MqttUsername != mqttUsername.Text)
            {
                Default.MqttUsername = mqttUsername.Text;
                mqttChanged = true;
            }

            if (mqttPassword.Text != "Password")
            {
                Default.MqttPassword = mqttPassword.Text;
                mqttChanged = true;
            }
        }
        else
        {
            Default.MqttPassword = string.Empty;
        }

        var teamsChanded = false;

        if (token != Guid.Empty && Default.TeamsToken != token.ToString())
        {
            Default.TeamsToken = token.ToString();
            teamsChanded = true;
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

        if (teamsChanded)
            Program.TryReconnectTeams();

        if (mqttChanged)
            Program.TryReconnectMqtt();

        Program.SendUpdate();
        UpdateStatus();
    }

    private void SelectAuthentication(object sender, EventArgs e)
    {
        mqttUsername.Enabled = useAuthentication.Checked;
        mqttPassword.Enabled = useAuthentication.Checked;
    }

    private void SettingsForm_Load(object sender, EventArgs e)
        => UpdateStatus();

    private void UpdateStatus()
    {
        if (!string.IsNullOrWhiteSpace(Default.MqttPassword) && !string.IsNullOrWhiteSpace(Default.MqttPassword))
        {
            useAuthentication.Checked = true;
            mqttUsername.Text = Default.MqttUsername;
            mqttPassword.Text = "Password";
        }
        else
        {
            useAuthentication.Checked = false;
        }

        if (!string.IsNullOrEmpty(Default.TeamsToken))
        {
            teamsToken.Text = Guid.Empty.ToString();
        }

        mqttServer.Text = Default.MqttUrl.ToString();

        mqttUsername.Enabled = useAuthentication.Checked;
        mqttPassword.Enabled = useAuthentication.Checked;

        teamsStatus.Text = Program.TeamsClient is not null && Program.TeamsClient.IsConnected ? "Connected" : "Not Connected";
        teamsStatus.ForeColor = Program.TeamsClient is not null && Program.TeamsClient.IsConnected ? Color.Green : Color.Red;

        mqttStatus.Text = Program.MqttClient is not null && Program.MqttClient.IsConnected ? "Connected" : "Not Connected";
        mqttStatus.ForeColor = Program.MqttClient is not null && Program.MqttClient.IsConnected ? Color.Green : Color.Red;
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