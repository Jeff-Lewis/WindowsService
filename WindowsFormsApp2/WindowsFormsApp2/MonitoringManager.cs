using NAudio.CoreAudioApi;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsFormsApp2
{
    sealed class MonitoringManager
    {
        private const string focusChangeFile = "C:\\logger\\log_app.txt";
        private const string cameraFile = "C:\\logger\\log-camera.txt";
        private const string logOpenedFile = "C:\\logger\\log-open-log-file.txt";

        private static MonitoringManager instance;
        private AudioDeviceManager audiomngr;
        private FileSystemWatcher fileWatcher;
        //private VideoDeviceManager capture;

        private MonitoringManager()
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(focusChangeFile);
            fileWatcher.NotifyFilter = NotifyFilters.LastAccess;
            fileWatcher.Filter = Path.GetFileName(focusChangeFile);
            audiomngr = new AudioDeviceManager(DataFlow.Capture, DeviceState.Active);
        }

        public static MonitoringManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MonitoringManager();
                }
                return instance;
            }
        }

        public void RegisterEventHandlers()
        {
            fileWatcher.Changed += HandleLogFileOpened;
            fileWatcher.EnableRaisingEvents = true;

            WindowsApiWrapper.HookFocusChangeEvent(HandleFocusChanges);

            audiomngr.HookDeviceLevelChange(HandleMicLevelChange);

           
        }
        
        private void HandleCameraChange()
        {
            /*
            if (!VideoDeviceManager.CameraConnection())
            {
                Utils.LogToFile("Camera on", cameraFile);
            }
            */
        }
        
        private void HandleMicLevelChange(AudioVolumeNotificationData data)
        {
            if (data.Muted)
                Utils.LogToFile("Microphone has been muted", cameraFile);
            else
                Utils.LogToFile("Microphone has been unmuted", cameraFile);
        }

        private void HandleLogFileOpened(object source, FileSystemEventArgs e)
        {
            Utils.LogToFile("Log file opened", logOpenedFile);
        }

        private void HandleFocusChanges(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string title = WindowsApiWrapper.GetWindowTitle(hwnd);
            if (title.Length > 0)
            {
                Utils.LogToFile(title, focusChangeFile);
            }
        }
    }
}