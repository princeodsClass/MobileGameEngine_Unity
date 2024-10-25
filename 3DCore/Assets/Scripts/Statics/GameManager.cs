using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : SingletonMono<GameManager>
{
	public enum GameState
	{
		none = 0,
		title,
		lobby,
		battle,
	}

	public GameState _gameState;

	public struct DeviceSpec
	{
		public int processorFrequency;
		public int processorCount;
		public int systemMemorySize;
		public int graphicsMemorySize;

		public string processorType;
		public GraphicsDeviceType graphicsDeviceType;
	}

	public static DeviceSpec _deviceSpec;

	public void Awake()
	{
		CheckDevice();
		Screen.sleepTimeout = SleepTimeout.SystemSetting;

		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
	}

	void CheckDevice()
	{
#if DEBUG
		_deviceSpec.processorCount = SystemInfo.processorCount;
		_deviceSpec.processorFrequency = SystemInfo.processorFrequency;

		_deviceSpec.systemMemorySize = SystemInfo.systemMemorySize;
		_deviceSpec.graphicsMemorySize = SystemInfo.graphicsMemorySize;

		_deviceSpec.processorType = SystemInfo.processorType;
		_deviceSpec.graphicsDeviceType = SystemInfo.graphicsDeviceType;

		Log($"processorCount : {_deviceSpec.processorCount}");
		Log($"processorFrequency : {_deviceSpec.processorFrequency}");
		Log($"systemMemorySize : {_deviceSpec.systemMemorySize}");
		Log($"graphicsMemorySize : {_deviceSpec.graphicsMemorySize}");
		Log($"processorType : {_deviceSpec.processorType}");
		Log($"graphicsDeviceType : {_deviceSpec.graphicsDeviceType}");
#endif
	}

	public static void Log(string msg, string color = "white")
	{
#if DEBUG || DEVELOPMENT_BUILD
		Debug.Log($"<color={color}>{msg}</color>");
#endif
	}

	public static void Log(string titleColor, string title, string msgColor, string msg)
	{
#if DEBUG || DEVELOPMENT_BUILD
		Debug.Log($"<color={titleColor}>[{title}]</color> : <color={msgColor}>{msg}</color>");
#endif
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
	}

	public void Restart()
	{
		Awake();
		MenuManager.Singleton.SceneEnd();
		MenuManager.Singleton.SceneNext(ESceneType.MemuScene);
	}

#if DEBUG
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftCurlyBracket)) Time.timeScale -= 0.05f;
		if (Input.GetKeyDown(KeyCode.RightCurlyBracket)) Time.timeScale += 0.05f;
		if (Input.GetKeyDown(KeyCode.Backspace)) Time.timeScale = 1.0f;
	}
#endif
}
