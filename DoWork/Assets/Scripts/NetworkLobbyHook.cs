﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook{

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, 
														GameObject lobbyPlayer, GameObject gamePlayer)
	{
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer> ();
		PlayerControlScripts localPlayer = gamePlayer.GetComponent<PlayerControlScripts> ();

		localPlayer.pname = lobby.playerName;
		localPlayer.playerColor = lobby.playerColor;
	}
}