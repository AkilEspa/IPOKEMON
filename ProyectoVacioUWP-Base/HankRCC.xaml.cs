using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProyectoVacioUWP_Base
{
    public sealed partial class HankRCC : UserControl, iPokemon
    {
        DispatcherTimer _timer;
        double healthIncrement = 0;

        Storyboard sbaux;

        Storyboard idle;

        // Interfaz iPokemon valores
        string categoria = "Pokemon Asesino";
        string tipo = "Lucha";
        double altura = 1.85;
        double peso = 85;
        string evolucion = "Mag Hank";
        string descripcion = "Un mercenario psicópata en una misión de no tomar prisioneros para acabar con una Agencia todopoderosa... una que mantiene un dominio absoluto sobre Nevada con su suministro aparentemente ilimitado de soldados leales y desechables.";

        public double Vida { get => this.progbarVida.Value; set => this.progbarVida.Value = value; }
        public double Energia { get => this.progbarEnergia.Value; set => this.progbarEnergia.Value = value; }
        public string Nombre { get => this.tbNombre.Text; set => this.tbNombre.Text = value; }
        public string Categoría { get => this.categoria; set => this.categoria = value; }
        public string Tipo { get => this.tipo; set => this.tipo = value; }
        public double Altura { get => this.altura; set => this.altura = value; }
        public double Peso { get => this.peso; set => this.peso = value; }
        public string Evolucion { get => this.evolucion; set => this.evolucion = value; }
        public string Descripcion { get => this.descripcion; set => this.descripcion = value; }

        public HankRCC()
        {
            this.InitializeComponent();

            this.idle = (Storyboard)this.Resources["Idle"];
        }


        private void imgPocionVida_usePotion(object sender, PointerRoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += subirBarraVida;
            _timer.Start();
            imgPocionVida.Visibility = Visibility.Collapsed;
        }

        private void ImgPocionEnergia_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += subirBarraEnergia;
            _timer.Start();
            imgPocionEnergia.Visibility = Visibility.Collapsed;
        }

        private void subirBarraVida(object sender, object e)
        {
            this.progbarVida.Value += 0.5;
            this.healthIncrement += 0.5;

            if (progbarVida.Value >= 100 || healthIncrement >= 40)
            {
                healthIncrement = 0;
                _timer.Stop();
            }

        }

        private void subirBarraEnergia(object sender, object e)
        {
            this.progbarEnergia.Value += 0.5;
            this.healthIncrement += 0.5;

            if (progbarEnergia.Value >= 100 || healthIncrement >= 40)
            {
                healthIncrement = 0;
                _timer.Stop();
            }
        }

        // Método para que siempre siempre vuelva a sonar el sonido y simplificar
        private void reproducirSonido(string nombreSonido)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsHankRCC/Sonidos/" + nombreSonido));
            mediaPlayer.Play();
        }

        // Método para reproducir un sonido desde inicio hasta fin
        private async void reproducirSonido(string nombreSonido, TimeSpan inicio, TimeSpan fin)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsHankRCC/Sonidos/" + nombreSonido));

            mediaPlayer.PlaybackSession.Position = inicio;
            mediaPlayer.Play();

            // Tiempo que dura
            await System.Threading.Tasks.Task.Delay(fin - inicio);

            mediaPlayer.Pause();
        }

        /* Método para reinciar personaje
         * animaciones que cambian el estado del personaje */
        private void reiniciarPersonaje()
        {
            ((Storyboard)this.Resources["Dodge"]).Stop();
            ((Storyboard)this.Resources["Defeat"]).Stop();
            ((Storyboard)this.Resources["Tired"]).Stop();
        }

        // Ejecución de las animaciones

        private void animIdle()
        {
            this.idle.Begin();
        }

        private void animIdleStop()
        {
            this.idle.Stop();
            this.reiniciarPersonaje();
        }

        private async void animLightAttack()
        {
            sbaux = (Storyboard)this.Resources["LightAttack"];
            sbaux.Begin();

            // Esperamos 0.5 segundos desde el inicio
            //Golpe 1
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Sword Whip 01.wav");

            // Esperamos 0.5 segundos más (0.5 + 0.5 = 1)
            // Golpe 2 - 1.0
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Sword Whip 02.wav");

            // 1.7
            // Golpe 3 - 1.7
            await System.Threading.Tasks.Task.Delay(700);
            reproducirSonido("Sword Whip 03.wav");
        }

        private async void animStrongAttack()
        {
            sbaux = (Storyboard)this.Resources["StrongAttack"];
            sbaux.Begin();

            // Golpe 1
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Sword Whip 01.wav");

            // Golpe 2 - 1.0
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Sword Whip 02.wav");

            // Disparo 1 - 2.2
            await System.Threading.Tasks.Task.Delay(1200);
            reproducirSonido("Baretta-1.wav");

            // Dispoaro 2 - 2.7
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Baretta-2.wav");

            // Disparo 3 - 3.2
            await System.Threading.Tasks.Task.Delay(500);
            reproducirSonido("Baretta-3.wav");
        }

        private async void animDodge()
        {
            sbaux = (Storyboard)this.Resources["Dodge"];
            sbaux.Begin();
            reproducirSonido("Sound - Woosh1.mp3.wav");
        }

        private async void animRestDance()
        {
            sbaux = (Storyboard)this.Resources["RestDance"];
            sbaux.Begin();

            TimeSpan inicio = new TimeSpan(0, 60, 60);
            TimeSpan fin = new TimeSpan(0, 60, 60);
            reproducirSonido("madness_combat_1_soundtrack.mp3", TimeSpan.FromSeconds(13), TimeSpan.FromSeconds(19));
        }

        private async void animTired()
        {
            sbaux = (Storyboard)this.Resources["Tired"];
            sbaux.Begin();
        }

        private async void animDefeat()
        {
            sbaux = (Storyboard)this.Resources["Defeat"];
            sbaux.Begin();

            reproducirSonido("Madness-Death-Sound02.mp3");

            await System.Threading.Tasks.Task.Delay(1900);
            reproducirSonido("Impact - Bodyfall02.wav");
        }

        private async void animDamage()
        {
            sbaux = (Storyboard)this.Resources["Damage"];
            sbaux.Begin();

            reproducirSonido("Impact - punch01.wav");
        }

        // Interfaz iPokemon
        public void verFondo(bool ver)
        {
            if (ver)
            {
                this.imgFondo.Visibility = Visibility.Visible;

            }
            else
            {
                this.imgFondo.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaVida(bool ver)
        {
            if (ver)
            {
                this.progbarVida.Visibility = Visibility.Visible;

            }
            else
            {
                this.progbarVida.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaEnergia(bool ver)
        {
            if (ver)
            {
                this.progbarEnergia.Visibility = Visibility.Visible;
            }
            else
            {
                this.progbarEnergia.Visibility = Visibility.Collapsed;
            }
        }

        public void verPocionVida(bool ver)
        {
            if (ver)
            {
                this.imgPocionVida.Visibility = Visibility.Visible;
            }
            else
            {
                this.imgPocionVida.Visibility = Visibility.Collapsed;
            }
        }

        public void verPocionEnergia(bool ver)
        {
            if (ver)
            {
                this.imgPocionEnergia.Visibility = Visibility.Visible;
            }
            else
            {
                this.imgPocionEnergia.Visibility = Visibility.Collapsed;
            }
        }

        public void verNombre(bool ver)
        {
            if (ver)
            {
                this.tbNombre.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbNombre.Visibility = Visibility.Collapsed;
            }
        }

        public void verEscudo(bool ver)
        {
            if (ver)
            {
                this.halo.Visibility = Visibility.Visible;
            }
            else
            {
                this.halo.Visibility = Visibility.Collapsed;
            }
        }

        public void activarAniIdle(bool activar)
        {
            if (activar)
            {
                this.animIdle();
            }
            else
            {
                this.animIdleStop();
            }
        }

        public void animacionAtaqueFlojo()
        {
            this.reiniciarPersonaje();
            this.animLightAttack();
        }

        public void animacionAtaqueFuerte()
        {
            this.reiniciarPersonaje();
            this.animStrongAttack();
        }

        public void animacionDefensa()
        {
            this.reiniciarPersonaje();
            this.animDodge();

            this.halo.Visibility = Visibility.Visible;
            this.halo.Opacity = 100;
        }

        public void animacionDescasar()
        {
            this.reiniciarPersonaje();
            this.animRestDance();
        }

        public void animacionCansado()
        {
            this.reiniciarPersonaje();
            this.animTired();
        }

        public void animacionNoCansado()
        {
            this.reiniciarPersonaje();
        }

        public void animacionHerido()
        {
            this.reiniciarPersonaje();
            this.animDamage();
        }

        public void animacionNoHerido()
        {
            this.reiniciarPersonaje();
            this.bloodBody.Visibility = Visibility.Collapsed;
        }

        public void animacionDerrota()
        {
            this.reiniciarPersonaje();
            this.animDefeat();
        }
    }
}
