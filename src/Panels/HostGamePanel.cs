﻿using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class HostGamePanel : UIPanel
    {
        private UITextField _portField;
        private UITextField _passwordField;

        private UILabel _connectionStatus;

        private UIButton _createButton;
        private UIButton _closeButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPHostGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 200.0f);

            width = 360;
            height = 400;

            // Title Label
            this.CreateTitleLabel("Host Server", new Vector2(120, -20));

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -65));
            // Port field
            _portField = this.CreateTextField("4230", new Vector2(10, -90));

            // Password label
            this.CreateLabel("Password:", new Vector2(10, -145));
            // Password field
            _passwordField = this.CreateTextField("", new Vector2(10, -170));

            _connectionStatus = this.CreateLabel("", new Vector2(10, -230));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            // Create Server Button
            _createButton = this.CreateButton("Create Server", new Vector2(10, -260));
            _createButton.eventClick += OnCreateServerClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -330));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };
        }

        /// <summary>
        ///     Check inputs and create server
        /// </summary>
        private void OnCreateServerClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Setting up server...";

            // Port must be filled in or be a number
            if (string.IsNullOrEmpty(_portField.text) || !int.TryParse(_portField.text, out var port))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Port must be a number";
                return;
            }

            // Check port is valid to use
            if (int.Parse(_portField.text) < 1 || int.Parse(_portField.text) > 49151)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid port. Choose from a range of 1 - 49151";
                return;
            }

            // Password validation
            if (!string.IsNullOrEmpty(_passwordField.text) /*&& (!int.TryParse(_portField.text, out var ) || _passwordField.text == " ")*/)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Password. Must be text or numeric.";
                return;
            }

            // Check already running server
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Already Running Server";
                return;
            }

            // Start server and check for errors
            if (MultiplayerManager.Instance.StartGameServer(int.Parse(_portField.text)) != true)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Error: Could not start server";
                return;
            }

            // Clear warnings/errors and hide panel
            _connectionStatus.text = "";
            isVisible = false;
        }
    }
}