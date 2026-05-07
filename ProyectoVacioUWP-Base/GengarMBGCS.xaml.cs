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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProyectoVacioUWP_Base
{
    public sealed partial class GengarMBGCS : UserControl, iPokemon 
    {
        DispatcherTimer _timer;
        double healthIncrement = 0;

        public double Vida { get => this.pbHealth.Value; set => this.pbHealth.Value = value; }

        public double Energia { get => this.pbEnergy.Value; set => this.pbEnergy.Value = value; }

        //No los usaremos aún
        public string Categoría { get => "Sombra"; set => throw new NotImplementedException(); }
        public string Nombre { get => "Gengar"; set => throw new NotImplementedException(); }
        public string Tipo { get => "Fantasma/Veneno"; set => throw new NotImplementedException(); }
        public double Altura { get => 1.5; set => throw new NotImplementedException(); }
        public double Peso { get => 40.5; set => throw new NotImplementedException(); }
        public string Evolucion { get => "Gastly-Haunter-Gengar"; set => throw new NotImplementedException(); }
        public string Descripcion { get => "Para quitarle la vida a su presa, se desliza en su sombra y espera su oportunidad en silencio."; set => throw new NotImplementedException(); }

        //Sonidos
        MediaPlayer mpCansado = new MediaPlayer();
        MediaPlayer mpAtaqueFuerte = new MediaPlayer();
        MediaPlayer mpRisa = new MediaPlayer();
        //MediaPlayer mpAmbiente = new MediaPlayer(); // este se ejecuta como principal continuamente

        public GengarMBGCS()
        {
            this.InitializeComponent();
            mpCansado.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsGengarMBGCS/gengar_cansado.mp3"));
            mpAtaqueFuerte.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsGengarMBGCS/gengar_ataquefuerte.mp3"));
            mpRisa.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsGengarMBGCS/gengar_risa.mp3"));
            //mpAmbiente.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsGengarMBGCS/gengar_pokemon.mp3"));
            //mpAmbiente.IsLoopingEnabled = true;
            //mpAmbiente.Volume = 0.3;
            //mpAmbiente.Play();
        }

        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += increaseHealth;
            _timer.Start();

            imRedPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseHealth(object sender, object e)
        {
            //Vida += 1;
            pbHealth.Value += 1;
            healthIncrement += 1;

            if (pbHealth.Value >= 100 || healthIncrement >= 40)
            {
                _timer.Stop();
                healthIncrement = 0;
            }
        }

        public void verFondo(bool ver)
        {
            if (!ver)
            imFondo.Visibility = Visibility.Collapsed; //también puede hacerse de esta forma: this.imFondo.Source = null;
            else
            imFondo.Visibility = Visibility.Visible; //this.imFondo.Source = new BitmapImage(new Uri("ms-appx:///AssetsGengarMBGCS/mansion.png"));
        }

        public void verFilaVida(bool ver)
        {
            if (!ver)
                this.gridGeneral.RowDefinitions[0].Height = new GridLength(0);
            else
                this.gridGeneral.RowDefinitions[0].Height = new GridLength(50);
        }

        public void verFilaEnergia(bool ver)
        {
            if (!ver)
                this.gridGeneral.RowDefinitions[1].Height = new GridLength(0);
            else
                this.gridGeneral.RowDefinitions[1].Height = new GridLength(50);
        }

        public void verPocionVida(bool ver)
        {
            if (ver)
                imRedPotion.Visibility = Visibility.Visible;
            else
                imRedPotion.Visibility = Visibility.Collapsed;
        }
        public void silenciar(bool silenciar)
        {
            if (silenciar)
            {
                //mpAmbiente.Pause();
                //mpAmbiente.Volume = 0;
                mpRisa.Volume = 0;
                mpAtaqueFuerte.Volume = 0;
                mpCansado.Volume = 0;
            }
            else
            {
                //mpAmbiente.Volume = 0.3;
                mpRisa.Volume = 1;
                mpAtaqueFuerte.Volume = 1;
                mpCansado.Volume = 1;
                // Solo reproducimos si no se ha llamado a activarAniIdle(false)
                //mpAmbiente.Play();
            }
        }

        public void verPocionEnergia(bool ver)
        {
            if (ver)
                imYellowPotion.Visibility = Visibility.Visible;
            else
                imYellowPotion.Visibility = Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            if (ver)
                txtNombre.Visibility = Visibility.Visible;
            else
                txtNombre.Visibility = Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            if (ver)
                Escudo.Opacity = 1;
            else
                Escudo.Opacity = 0;
        }

        public void activarAniIdle(bool activar)
        {
            Storyboard respirar = (Storyboard)this.Resources["respirar"];

            if (activar)
                respirar.Begin();
            else
                respirar.Stop();
        }

        public void animacionAtaqueFlojo()
        {
            Storyboard moverLengua = (Storyboard)this.Resources["moverLengua"];
            moverLengua.Begin();
            mpRisa.Play();
        }

        public void animacionAtaqueFuerte()
        {
            Storyboard ataqueGengar = (Storyboard)this.Resources["ataqueGengar"];
            ataqueGengar.Begin();
            mpAtaqueFuerte.Play();
        }

        public void animacionDefensa()
        {
            Storyboard animDefensa = (Storyboard)this.Resources["animDefensa"];
            animDefensa.Begin();
        }

        public void animacionDescasar()
        {
            Storyboard animDescansar = (Storyboard)this.Resources["animDescansar"];
            animDescansar.Begin();
        }

        public void animacionCansado()
        {
            ((Storyboard)Resources["animCansado"]).Begin();
            mpCansado.Play();
        }

        public void animacionNoCansado()
        {
            ((Storyboard)Resources["animCansado"]).Stop();
            mpCansado.Pause();
        }

        public void animacionHerido()
        {
            ((Storyboard)Resources["animHerido"]).Begin();
        }

        public void animacionNoHerido()
        {
            ((Storyboard)Resources["animHerido"]).Stop();
        }

        public void animacionDerrota()
        {
            ((Storyboard)Resources["animDerrota"]).Begin();
        }
    }
}
