namespace ro.TeamsMqttBridge;

partial class SettingsForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
        save = new Button();
        useAuthentication = new CheckBox();
        teamsTokenLabel = new Label();
        teamsToken = new TextBox();
        mqttServer = new TextBox();
        mqttServerLabel = new Label();
        mqttUsername = new TextBox();
        mqttUsernameLabel = new Label();
        mqttPassword = new TextBox();
        mqttPasswordLabel = new Label();
        statusStrip = new StatusStrip();
        teamsStatusLabel = new ToolStripStatusLabel();
        teamsStatus = new ToolStripStatusLabel();
        mqttStatusLabel = new ToolStripStatusLabel();
        mqttStatus = new ToolStripStatusLabel();
        autodiscover = new CheckBox();
        reconnect = new Button();
        exit = new Button();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // save
        //
        save.Location = new Point(292, 295);
        save.Name = "save";
        save.Size = new Size(112, 34);
        save.TabIndex = 0;
        save.Text = "Save";
        save.UseVisualStyleBackColor = true;
        save.Click += Save;
        //
        // useAuthentication
        //
        useAuthentication.AutoSize = true;
        useAuthentication.Location = new Point(12, 136);
        useAuthentication.Name = "useAuthentication";
        useAuthentication.Size = new Size(187, 29);
        useAuthentication.TabIndex = 1;
        useAuthentication.Text = "Use Authentication";
        useAuthentication.UseVisualStyleBackColor = true;
        useAuthentication.CheckedChanged += SelectAuthentication;
        //
        // teamsTokenLabel
        //
        teamsTokenLabel.AutoSize = true;
        teamsTokenLabel.Location = new Point(12, 9);
        teamsTokenLabel.Name = "teamsTokenLabel";
        teamsTokenLabel.Size = new Size(148, 25);
        teamsTokenLabel.TabIndex = 2;
        teamsTokenLabel.Text = "Teams API Token:";
        //
        // teamsToken
        //
        teamsToken.Location = new Point(12, 37);
        teamsToken.Name = "teamsToken";
        teamsToken.Size = new Size(392, 31);
        teamsToken.TabIndex = 3;
        teamsToken.UseSystemPasswordChar = true;
        //
        // mqttServer
        //
        mqttServer.Location = new Point(12, 99);
        mqttServer.Name = "mqttServer";
        mqttServer.Size = new Size(392, 31);
        mqttServer.TabIndex = 5;
        //
        // mqttServerLabel
        //
        mqttServerLabel.AutoSize = true;
        mqttServerLabel.Location = new Point(12, 71);
        mqttServerLabel.Name = "mqttServerLabel";
        mqttServerLabel.Size = new Size(153, 25);
        mqttServerLabel.TabIndex = 4;
        mqttServerLabel.Text = "MQTT Server URL:";
        //
        // mqttUsername
        //
        mqttUsername.Location = new Point(12, 196);
        mqttUsername.Name = "mqttUsername";
        mqttUsername.Size = new Size(392, 31);
        mqttUsername.TabIndex = 7;
        //
        // mqttUsernameLabel
        //
        mqttUsernameLabel.AutoSize = true;
        mqttUsernameLabel.Location = new Point(12, 168);
        mqttUsernameLabel.Name = "mqttUsernameLabel";
        mqttUsernameLabel.Size = new Size(147, 25);
        mqttUsernameLabel.TabIndex = 6;
        mqttUsernameLabel.Text = "MQTT Username:";
        //
        // mqttPassword
        //
        mqttPassword.Location = new Point(12, 258);
        mqttPassword.Name = "mqttPassword";
        mqttPassword.Size = new Size(392, 31);
        mqttPassword.TabIndex = 9;
        mqttPassword.UseSystemPasswordChar = true;
        //
        // mqttPasswordLabel
        //
        mqttPasswordLabel.AutoSize = true;
        mqttPasswordLabel.Location = new Point(12, 230);
        mqttPasswordLabel.Name = "mqttPasswordLabel";
        mqttPasswordLabel.Size = new Size(143, 25);
        mqttPasswordLabel.TabIndex = 8;
        mqttPasswordLabel.Text = "MQTT Password:";
        //
        // statusStrip
        //
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { teamsStatusLabel, teamsStatus, mqttStatusLabel, mqttStatus });
        statusStrip.Location = new Point(0, 339);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(416, 32);
        statusStrip.TabIndex = 10;
        statusStrip.Text = "Status";
        //
        // teamsStatusLabel
        //
        teamsStatusLabel.Name = "teamsStatusLabel";
        teamsStatusLabel.Size = new Size(65, 25);
        teamsStatusLabel.Text = "Teams:";
        //
        // teamsStatus
        //
        teamsStatus.Name = "teamsStatus";
        teamsStatus.Size = new Size(132, 25);
        teamsStatus.Text = "Not Connected";
        //
        // mqttStatusLabel
        //
        mqttStatusLabel.Name = "mqttStatusLabel";
        mqttStatusLabel.Size = new Size(63, 25);
        mqttStatusLabel.Text = "MQTT:";
        //
        // mqttStatus
        //
        mqttStatus.Name = "mqttStatus";
        mqttStatus.Size = new Size(132, 25);
        mqttStatus.Text = "Not Connected";
        //
        // autodiscover
        //
        autodiscover.AutoSize = true;
        autodiscover.Location = new Point(205, 136);
        autodiscover.Name = "autodiscover";
        autodiscover.Size = new Size(200, 29);
        autodiscover.TabIndex = 11;
        autodiscover.Text = "Enable Autodiscover";
        autodiscover.UseVisualStyleBackColor = true;
        //
        // reconnect
        //
        reconnect.Location = new Point(174, 295);
        reconnect.Name = "reconnect";
        reconnect.Size = new Size(112, 34);
        reconnect.TabIndex = 12;
        reconnect.Text = "Reconnect";
        reconnect.UseVisualStyleBackColor = true;
        reconnect.Click += reconnect_Click;
        //
        // exit
        //
        exit.Location = new Point(12, 295);
        exit.Name = "exit";
        exit.Size = new Size(112, 34);
        exit.TabIndex = 13;
        exit.Text = "Exit";
        exit.UseVisualStyleBackColor = true;
        exit.Click += exit_Click;
        //
        // SettingsForm
        //
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(416, 371);
        Controls.Add(exit);
        Controls.Add(reconnect);
        Controls.Add(autodiscover);
        Controls.Add(statusStrip);
        Controls.Add(mqttPassword);
        Controls.Add(mqttPasswordLabel);
        Controls.Add(mqttUsername);
        Controls.Add(mqttUsernameLabel);
        Controls.Add(mqttServer);
        Controls.Add(mqttServerLabel);
        Controls.Add(teamsToken);
        Controls.Add(teamsTokenLabel);
        Controls.Add(useAuthentication);
        Controls.Add(save);
        Icon = Resources.Icon;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Settings";
        TopMost = true;
        Load += SettingsForm_Load;
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button save;
    private CheckBox useAuthentication;
    private Label teamsTokenLabel;
    private TextBox teamsToken;
    private TextBox mqttServer;
    private Label mqttServerLabel;
    private TextBox mqttUsername;
    private Label mqttUsernameLabel;
    private TextBox mqttPassword;
    private Label mqttPasswordLabel;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel teamsStatusLabel;
    private ToolStripStatusLabel teamsStatus;
    private ToolStripStatusLabel mqttStatusLabel;
    private ToolStripStatusLabel mqttStatus;
    private CheckBox autodiscover;
    private Button reconnect;
    private Button exit;
}