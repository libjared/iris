using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using Lidgren.Network;

namespace Iris
{
    public class SoundInstance
    {
        public float baseVolume = 2;
        public float volume = 1f;
        public Sound sound;
        float basePitch;
        float pitchVariance;
        public bool started = false;

        public SoundInstance(SoundBuffer sound, float basePitch, float pitchVariance)
        {
            this.sound = new Sound(sound);
            this.basePitch = basePitch;
            this.pitchVariance = pitchVariance;
        }

        public SoundInstance(SoundBuffer sound, float basePitch, float pitchVariance, float volume)
        {
            this.sound = new Sound(sound);
            this.basePitch = basePitch;
            this.pitchVariance = pitchVariance;
            this.volume = volume;
        }

        public void Update()
        {

            if (!started)
            {
                started = true;
                float finalVariance =
                   ((float)(MainGame.rand.Next((int)(pitchVariance * 100)) / 100f));

                float pitch = basePitch + finalVariance;
                if (pitch > 2)
                    pitch = 2;
                if (pitch < -1)
                    pitch = -1;

                sound.Loop = false;
                sound.Pitch = pitch;
                sound.Volume = baseVolume * volume;
                sound.Play();
            }
            if (sound.Status.Equals(SoundStatus.Stopped))
            {
                MainGame.soundInstances.Remove(this);
            }

        }

    }
}


