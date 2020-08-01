using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp2
{
    class AudioDeviceManager
    {
        private List<MMDevice> devices;

        public AudioDeviceManager(DataFlow flow, DeviceState state)
        {
            MMDeviceEnumerator en = new MMDeviceEnumerator();
            devices = en.EnumerateAudioEndPoints(flow, state).ToList();

            MuteAllDevices();
        }

        public void MuteAllDevices()
        {
            foreach (MMDevice device in devices)
            {
                AudioEndpointVolume audioEndpointVolume = device.AudioEndpointVolume;
                audioEndpointVolume.Mute = true;
                audioEndpointVolume.MasterVolumeLevel = 0;
            }
        }

        public void HookDeviceLevelChange(AudioEndpointVolumeNotificationDelegate handler)
        {
            foreach (MMDevice device in devices)
            {
                AudioEndpointVolume audioEndpointVolume = device.AudioEndpointVolume;
                audioEndpointVolume.OnVolumeNotification += handler;
            }
        }
    }
}
