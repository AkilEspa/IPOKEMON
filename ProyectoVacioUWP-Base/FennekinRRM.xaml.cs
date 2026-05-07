using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class FennekinRRM : UserControl, iPokemon
    {
        // VARIABLES GLOBALES
        DispatcherTimer _healthTimer;
        DispatcherTimer _energyTimer;
        MediaPlayer mpSonidos = new MediaPlayer(); // Reproductor
        // --- NUEVA MEMORIA DE ESTADOS ---
        bool isCansado = false;
        bool isHerido = false;
        bool isDerrotado = false;
        // Metas para las pociones
        double targetHealth;
        double targetEnergy;

        // --- PROPIEDADES DE LA INTERFAZ IPOKEMON ---
        public double Vida
        {
            get { return this.pbHealth.Value; }
            set { this.pbHealth.Value = value; }
        }

        public double Energia
        {
            get { return this.pbEnergy.Value; }
            set { this.pbEnergy.Value = value; }
        }

        public string Nombre
        {
            get { return this.tbPokemon.Text; }
            set { this.tbPokemon.Text = value; }
        }

        // Datos fijos de Fennekin
        public string Categoría { get => "Zorro"; set => throw new NotImplementedException(); }
        public string Tipo { get => "Fuego"; set => throw new NotImplementedException(); }
        public double Altura { get => 0.4; set => throw new NotImplementedException(); }
        public double Peso { get => 9.4; set => throw new NotImplementedException(); }
        public string Evolucion { get => "Braixen"; set => throw new NotImplementedException(); }
        public string Descripcion { get => "Tras masticar e ingerir pequeñas ramas se siente pletórico y expulsa aire caliente por sus grandes orejas."; set => throw new NotImplementedException(); }

        public FennekinRRM()
        {
            this.InitializeComponent();
            AniIdle.Begin(); // Fennekin empieza a respirar al cargar el control
        }

        // --- POCIONES Y TIMERS ---
        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            // Calculamos la meta: la vida actual + 40 (sin pasarnos de 100)
            targetHealth = Math.Min(100, this.pbHealth.Value + 40);

            if (_healthTimer == null)
            {
                _healthTimer = new DispatcherTimer();
                _healthTimer.Interval = TimeSpan.FromMilliseconds(50); // Lo he puesto a 50ms para que suba un pelín más fluido
                _healthTimer.Tick += increaseHealth;
            }

            _healthTimer.Start();
            this.imRedPotion.Visibility = Visibility.Collapsed;
        }

        private void useYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            // Calculamos la meta: la energía actual + 40 (sin pasarnos de 100)
            targetEnergy = Math.Min(100, this.pbEnergy.Value + 40);

            if (_energyTimer == null)
            {
                _energyTimer = new DispatcherTimer();
                _energyTimer.Interval = TimeSpan.FromMilliseconds(50);
                _energyTimer.Tick += increaseEnergy;
            }

            _energyTimer.Start();
            this.imYellowPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseHealth(object sender, object e)
        {
            this.pbHealth.Value += 1;
            // Se para si llega a la meta calculada (los +40) o si llega a 100
            if (this.pbHealth.Value >= targetHealth || this.pbHealth.Value >= 100)
            {
                _healthTimer.Stop();
            }
        }

        private void increaseEnergy(object sender, object e)
        {
            this.pbEnergy.Value += 1;
            // Se para si llega a la meta calculada (los +40) o si llega a 100
            if (this.pbEnergy.Value >= targetEnergy || this.pbEnergy.Value >= 100)
            {
                _energyTimer.Stop();
            }
        }
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }
        // --- MÉTODOS DE ESTADOS INTERNOS ---
        // --- ESTADO CANSADO ---
        private void activarCansado(object sender, RoutedEventArgs e)
        {
            isCansado = true;
            if (!isDerrotado)
            {
                AniIdle.Pause();
                AniCansado.Begin();
            }
        }

        private void desactivarCansado(object sender, RoutedEventArgs e)
        {
            isCansado = false;
            AniCansado.Stop(); 

            if (isDerrotado) return; 

            if (!isHerido)
            {
                AniRestaurar.Begin();
                AniIdle.Resume();
            }
        }

        // --- ESTADO HERIDO ---
        private void activarHerido(object sender, RoutedEventArgs e)
        {
            isHerido = true;
            if (!isDerrotado)
            {
                AniIdle.Pause();
                AniHeridoEstatico.Begin();
                AniHeridoBucle.Begin();
            }
        }

        private void desactivarHerido(object sender, RoutedEventArgs e)
        {
            isHerido = false;
            AniHeridoBucle.Stop();
            AniHeridoEstatico.Stop(); 

            if (!isCansado)
            {
                AniRestaurar.Begin();
                AniIdle.Resume();
            } else
            {
                AniCansado.Begin();
            }
        }

        // --- ESTADO DERROTADO ---
        public void animacionDerrota()
        {
            isDerrotado = true;
            AniIdle.Pause();

            AniCansado.Stop();
            AniHeridoBucle.Stop();
            AniHeridoEstatico.Stop();

            AniDerrotado.Begin();
        }

        public void animacionNoDerrota()
        {
            isDerrotado = false;
            AniDerrotado.Stop(); 
            AniRestaurar.Begin(); 

            if (isHerido)
            {
                AniHeridoEstatico.Begin();
                AniHeridoBucle.Begin();
            }

            if (isCansado)
            {
                AniCansado.Begin();
            }

            if (!isHerido && !isCansado)
            {
                AniIdle.Resume(); 
            }
        }
        private void Click_AtaqueFlojo(object sender, RoutedEventArgs e)
        {
            AniIdle.Pause();
            AniAtaqueFlojo.Completed += FinAtaqueFlojo;
            AniAtaqueFlojo.Begin();
        }

        private void FinAtaqueFlojo(object sender, object e)
        {
            AniAtaqueFlojo.Completed -= FinAtaqueFlojo;
            AniIdle.Begin();
        }

        private void Click_AtaqueFuerte(object sender, RoutedEventArgs e)
        {
            AniIdle.Stop();
            mpSonidos.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsFennekinRRM/llama.mp3"));
            mpSonidos.Play();
            AniAtaqueFuerte.Completed += FinAtaqueFuerte;
            AniAtaqueFuerte.Begin();
        }

        private void FinAtaqueFuerte(object sender, object e)
        {
            AniAtaqueFuerte.Completed -= FinAtaqueFuerte;
            mpSonidos.Pause();
            AniIdle.Begin();
        }

        private void Click_Descansar(object sender, RoutedEventArgs e)
        {
            AniIdle.Pause();

            cvOjoDer.RenderTransformOrigin = new Windows.Foundation.Point(0.6, 0.75);
            cvOjoIzq.RenderTransformOrigin = new Windows.Foundation.Point(0.3, 0.75);

            mpSonidos.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsFennekinRRM/curacion.mp3"));
            mpSonidos.Play();

            AniDescansar.Completed += FinDescansar;
            AniDescansar.Begin();
        }

        private void FinDescansar(object sender, object e)
        {
            AniDescansar.Completed -= FinDescansar;
            mpSonidos.Pause();

            ((CompositeTransform)elCuracion.RenderTransform).ScaleX = 0.5;
            ((CompositeTransform)elCuracion.RenderTransform).ScaleY = 0.5;

            cvOjoDer.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 1);
            cvOjoIzq.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 1);

            AniIdle.Begin();
        }

        // ==========================================================
        // --- IMPLEMENTACIÓN DE LOS MÉTODOS DE LA INTERFAZ ---
        // ==========================================================

        public void verFondo(bool ver)
        {
            if (ver) this.imgFondoBrush.Opacity = 1;
            else this.imgFondoBrush.Opacity = 0;
        }

        public void verFilaVida(bool ver)
        {
            if (ver)
            {
                this.pbHealth.Visibility = Visibility.Visible;
                this.imHeart.Visibility = Visibility.Visible;
            }
            else
            {
                this.pbHealth.Visibility = Visibility.Collapsed;
                this.imHeart.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaEnergia(bool ver)
        {
            if (ver)
            {
                this.pbEnergy.Visibility = Visibility.Visible;
                this.imLightning.Visibility = Visibility.Visible;
            }
            else
            {
                this.pbEnergy.Visibility = Visibility.Collapsed;
                this.imLightning.Visibility = Visibility.Collapsed;
            }
        }

        public void verPocionVida(bool ver)
        {
            if (ver) this.imRedPotion.Visibility = Visibility.Visible;
            else this.imRedPotion.Visibility = Visibility.Collapsed;
        }

        public void verPocionEnergia(bool ver)
        {
            if (ver) this.imYellowPotion.Visibility = Visibility.Visible;
            else this.imYellowPotion.Visibility = Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            if (ver) this.tbPokemon.Visibility = Visibility.Visible;
            else this.tbPokemon.Visibility = Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        { 
            if (!ver)
            {
                AniQuitarEscudo.Begin();
            }
            else
            {
                this.imEscudo.Opacity = 0.8;
                ((CompositeTransform)imEscudo.RenderTransform).ScaleX = 1;
                ((CompositeTransform)imEscudo.RenderTransform).ScaleY = 1;
            }
        }

        public void activarAniIdle(bool activar)
        {
            if (activar) AniIdle.Begin();
            else AniIdle.Stop();
        }

        public void animacionAtaqueFlojo()
        {
            Click_AtaqueFlojo(null, null);
        }

        public void animacionAtaqueFuerte()
        {
            Click_AtaqueFuerte(null, null);
        }

        public void animacionDefensa()
        {
            AniDefensa.Begin();
        }

        public void animacionDescasar()
        {
            Click_Descansar(null, null);
        }

        public void animacionCansado()
        {
            activarCansado(null, null);
        }

        public void animacionNoCansado()
        {
            desactivarCansado(null, null);
        }

        public void animacionHerido()
        {
            activarHerido(null, null);
        }

        public void animacionNoHerido()
        {
            desactivarHerido(null, null);
        }

    }
}