namespace ro.TeamsMqttBridge;

internal class Daemon : ApplicationContext
{
    private readonly NotifyIcon icon;
    private bool SettingsOpen = false;

    internal Daemon()
    {
        icon = new()
        {
            Icon = new Icon("Home_Assistant.ico"),
            Visible = true,
            Text = "Home Assistant Bridge"
        };

        icon.Click += ShowSettings;

        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.NodeName))
        {
            Properties.Settings.Default.NodeName = Environment.GetEnvironmentVariable("USERNAME") ?? "user";
            Properties.Settings.Default.Save();
        }

        if (string.IsNullOrEmpty(Properties.Settings.Default.TeamsToken))
            ShowSettings(this, EventArgs.Empty);
    }

    private void ShowSettings(object? sender, EventArgs e)
    {
        if (SettingsOpen) return;

        SettingsOpen = true;
        new SettingsForm().ShowDialog();
        SettingsOpen = false;
    }
}
