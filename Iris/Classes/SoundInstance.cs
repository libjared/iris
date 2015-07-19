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
        public static float volume = 10f;
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

        public void Update()
        {

            if (!started)
            {
                started = true;
                float finalVariance = basePitch +
                   ((float)(MainGame.rand.Next((int)(pitchVariance * 100)) / 100f));

                sound.Loop = false;
                sound.Pitch = 1;
                sound.Volume = volume;
                sound.Attenuation = 1f;
                sound.Play();
            }
            if (sound.Status.Equals(SoundStatus.Stopped))
            {
                MainGame.soundInstances.Remove(this);
            }

        }

    }
}


