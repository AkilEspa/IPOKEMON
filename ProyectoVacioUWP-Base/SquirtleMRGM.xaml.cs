using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Media.Playback;
using Windows.Media.Core;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class SquirtleMRGM : UserControl, iPokemon
    {
        private DispatcherTimer _healthTimer;
        private DispatcherTimer _energyTimer;

        private bool estaHerido = false;
        private bool estaCansado = false;
        private bool tieneEscudo = false;
        private bool estaDerrotado = false;

        private double targetHealth;
        private double targetEnergy;

        private string categoria = "Tortuguita";
        private string tipo = "Agua";
        private double altura = 0.5;
        private double peso = 9.0;
        private string evolucion = "Wartortle / Blastoise";
        private string descripcion = "Squirtle es un Pokémon de tipo Agua con caparazón resistente. Puede lanzar chorros de agua con distinta intensidad, protegerse con escudo y mostrar distintos estados visuales como cansancio, heridas o derrota.";
        
        private MediaPlayer sonido = new MediaPlayer();

        public SquirtleMRGM()
        {
            this.InitializeComponent();

            AniAtaqueFlojo.Completed += AnimacionTemporalTerminada;
            AniAtaqueFuerte.Completed += AnimacionTemporalTerminada;
            AniDescanso.Completed += AnimacionTemporalTerminada;
            AniDefensa.Completed += DefensaTerminada;

            AplicarEstados();
        }

        // =========================
        // PROPIEDADES iPokemon
        // =========================
        public double Vida
        {
            get { return pbHealth.Value; }
            set { pbHealth.Value = Math.Max(pbHealth.Minimum, Math.Min(pbHealth.Maximum, value)); }
        }

        public double Energia
        {
            get { return pbEnergy.Value; }
            set { pbEnergy.Value = Math.Max(pbEnergy.Minimum, Math.Min(pbEnergy.Maximum, value)); }
        }

        public string Nombre
        {
            get { return tbPokemon.Text; }
            set { tbPokemon.Text = value; }
        }

        public string Categoría
        {
            get { return categoria; }
            set { categoria = value; }
        }

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public double Altura
        {
            get { return altura; }
            set { altura = value; }
        }

        public double Peso
        {
            get { return peso; }
            set { peso = value; }
        }

        public string Evolucion
        {
            get { return evolucion; }
            set { evolucion = value; }
        }

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        private void ReproducirSonido(string archivo)
        {
            sonido.Source = MediaSource.CreateFromUri(
                new Uri($"ms-appx:///AssetsSquirtleMRGM/{archivo}")
            );

            sonido.Volume = 0.6; 
            sonido.Play();
        }
        // =========================
        // CONTROL GENERAL
        // =========================
        private void AnimacionTemporalTerminada(object sender, object e)
        {
            AplicarEstados();
        }

        private void DefensaTerminada(object sender, object e)
        {
            ReproducirSonido("defensa.mp3");

            AplicarEstados();

            if (tieneEscudo && !estaDerrotado)
            {
                AniEscudoLatido.Stop();
                AniEscudoLatido.Begin();
            }
        }

        private void PararAccionesTemporales()
        {
            AniAtaqueFlojo.Stop();
            AniAtaqueFuerte.Stop();
            AniDescanso.Stop();
        }

        private void PararStoryboardsDeEstado()
        {
            AniIdle.Stop();

            AniHerido.Stop();
            AniHeridoCansado.Stop();
            AniNoHerido.Stop();

            AniCansado.Stop();
            AniNoCansado.Stop();

            AniDerrotado.Stop();
            AniNoDerrotado.Stop();

            AniDefensa.Stop();
            AniQuitarEscudo.Stop();
            AniEscudoLatido.Stop();

            ResetVisualManual();
        }

        private void ResetVisualManual()
        {
            // Caras
            pBoca.Opacity = 1;
            pBocaAbierta.Opacity = 0;

            cvCaraHerido.Opacity = 0;
            cvCaraCansado.Opacity = 0;
            cvCaraDerrotado.Opacity = 0;
            cvCaraDescanso.Opacity = 0;

            if (pBocaCansado != null)
                pBocaCansado.Opacity = 1;

            // Ojos normales visibles
            cvOjoDer.Opacity = 1;
            cvOjoIzq.Opacity = 1;

            // Escudo y sudores especiales
            cvEscudo.Opacity = 0;
            imSudorCombo.Opacity = 0;

            // Sudor cansado
            imSudorCansado.Visibility = Visibility.Collapsed;
            imSudorCansado.Opacity = 0;

            CompositeTransform trSudorCansado = imSudorCansado.RenderTransform as CompositeTransform;
            if (trSudorCansado != null)
            {
                trSudorCansado.TranslateY = 0;
                trSudorCansado.ScaleX = 0.6;
                trSudorCansado.ScaleY = 0.6;
            }

            // Sudor general por si acaso
            if (imSudor != null)
            {
                imSudor.Visibility = Visibility.Collapsed;
                imSudor.Opacity = 0;
            }

            // Heridas visibles solo si toca
            imTiritaPrincipal.Opacity = 0;
            imHeridaSuave.Opacity = 0;
            imHeridaCara.Opacity = 0;

            CompositeTransform trTirita = imTiritaPrincipal.RenderTransform as CompositeTransform;
            if (trTirita != null)
            {
                trTirita.ScaleX = 0.60;
                trTirita.ScaleY = 0.60;
            }

            CompositeTransform trHeridaSuave = imHeridaSuave.RenderTransform as CompositeTransform;
            if (trHeridaSuave != null)
            {
                trHeridaSuave.ScaleX = 0.78;
                trHeridaSuave.ScaleY = 0.78;
            }

            CompositeTransform trHeridaCara = imHeridaCara.RenderTransform as CompositeTransform;
            if (trHeridaCara != null)
            {
                trHeridaCara.ScaleX = 0.70;
                trHeridaCara.ScaleY = 0.70;
            }
        }

        private void MostrarCaraNormal()
        {
            pBoca.Opacity = 1;
            pBocaAbierta.Opacity = 0;

            cvCaraHerido.Opacity = 0;
            cvCaraCansado.Opacity = 0;
            cvCaraDerrotado.Opacity = 0;
            cvCaraDescanso.Opacity = 0;
        }

        private void MostrarCaraHerido()
        {
            pBoca.Opacity = 0;
            pBocaAbierta.Opacity = 0;

            cvCaraHerido.Opacity = 1;
            cvCaraCansado.Opacity = 0;
            cvCaraDerrotado.Opacity = 0;
            cvCaraDescanso.Opacity = 0;
        }

        private void MostrarCaraCansado()
        {
            pBoca.Opacity = 0;
            pBocaAbierta.Opacity = 0;

            cvCaraHerido.Opacity = 0;
            cvCaraCansado.Opacity = 1;
            cvCaraDerrotado.Opacity = 0;
            cvCaraDescanso.Opacity = 0;
        }

        private void MostrarCaraDerrotado()
        {
            pBoca.Opacity = 0;
            pBocaAbierta.Opacity = 0;

            cvCaraHerido.Opacity = 0;
            cvCaraCansado.Opacity = 0;
            cvCaraDerrotado.Opacity = 1;
            cvCaraDescanso.Opacity = 0;
        }

        private void MostrarSudorCansado()
        {
            imSudorCansado.Visibility = Visibility.Visible;
            imSudorCansado.Opacity = 1;
        }

        private void OcultarSudorCansado()
        {
            imSudorCansado.Visibility = Visibility.Collapsed;
            imSudorCansado.Opacity = 0;
        }

        private void MostrarSoloSudorDeCansado()
        {
            MostrarSudorCansado();
        }

        private void AplicarEstados()
        {
            // parar animaciones de estado antes de recomponer
            AniIdle.Stop();
            AniHerido.Stop();
            AniHeridoCansado.Stop();
            AniNoHerido.Stop();
            AniCansado.Stop();
            AniNoCansado.Stop();
            AniEscudoLatido.Stop();

            // =========================
            // DERROTADO MANDA SOBRE TODO
            // =========================
            if (estaDerrotado)
            {
                // ocultar otros visuales mientras esté derrotado
                cvEscudo.Opacity = 0;

                imTiritaPrincipal.Opacity = 0;
                imHeridaSuave.Opacity = 0;
                imHeridaCara.Opacity = 0;
                OcultarSudorCansado();
                imSudorCombo.Opacity = 0; 

                AniDerrotado.Stop();
                AniDerrotado.Begin();

                MostrarCaraDerrotado();
                return;
            }

            // si ya no está derrotado, limpiamos esa pose visual
            AniDerrotado.Stop();

            // =========================
            // ESCUDO
            // =========================
            if (tieneEscudo)
            {
                cvEscudo.Opacity = 0.55;
            }
            else
            {
                cvEscudo.Opacity = 0;
            }

            // =========================
            // HERIDO + CANSADO
            // =========================
            if (estaHerido && estaCansado)
            {
                AniNoHerido.Stop();
                AniNoCansado.Stop();

                AniHerido.Stop();
                AniCansado.Stop();
                AniHeridoCansado.Stop();
                AniHeridoCansado.Begin();

                MostrarCaraHerido();
                OcultarSudorCansado();

                imHeridaCara.Opacity = 0;

                return;
            }

            // =========================
            // SOLO HERIDO
            // =========================
            if (estaHerido)
            {
                AniNoCansado.Stop();
                AniNoCansado.Begin();

                AniHerido.Stop();
                AniHerido.Begin();

                MostrarCaraHerido();
                OcultarSudorCansado();
                imSudorCombo.Opacity = 0; 
                return;
            }

            // =========================
            // SOLO CANSADO
            // =========================
            if (estaCansado)
            {
                AniNoHerido.Stop();
                AniNoHerido.Begin();

                AniCansado.Stop();
                AniCansado.Begin();

                MostrarCaraCansado();
                MostrarSudorCansado();
                imSudorCombo.Opacity = 0; 
                return;
            }

            // =========================
            // NORMAL
            // =========================
            AniNoHerido.Stop();
            AniNoHerido.Begin();

            AniNoCansado.Stop();
            AniNoCansado.Begin();

            MostrarCaraNormal();
            OcultarSudorCansado();
            imSudorCombo.Opacity = 0; 

            if (!tieneEscudo)
            {
                AniIdle.Stop();
                AniIdle.Begin();
            }
        }

        // =========================
        // DERROTADO
        // =========================
        private void ActivarDerrotado()
        {
            estaDerrotado = true;
            AplicarEstados();
        }

        private void DesactivarDerrotado()
        {
            estaDerrotado = false;
            AplicarEstados();
        }

        // =========================
        // ESCUDO
        // =========================
        private void ActivarEscudo()
        {
            if (estaDerrotado)
                return;

            tieneEscudo = true;
            AplicarEstados();
        }

        private void DesactivarEscudo()
        {
            tieneEscudo = false;
            AplicarEstados();
        }

        // =========================
        // HERIDO
        // =========================
        private void ActivarHerido()
        {
            if (estaDerrotado)
                return;

            estaHerido = true;
            AplicarEstados();
        }

        private void DesactivarHerido()
        {
            estaHerido = false;
            AplicarEstados();
        }

        // =========================
        // CANSADO
        // =========================
        private void ActivarCansado()
        {
            if (estaDerrotado)
                return;

            estaCansado = true;
            AplicarEstados();
        }

        private void DesactivarCansado()
        {
            estaCansado = false;
            AplicarEstados();
        }

        // =========================
        // POCIÓN ROJA
        // =========================
        private void UseRedPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_healthTimer != null)
                return;

            targetHealth = Math.Min(pbHealth.Maximum, pbHealth.Value + 40);

            imRedPotion.Visibility = Visibility.Collapsed;

            _healthTimer = new DispatcherTimer();
            _healthTimer.Interval = TimeSpan.FromMilliseconds(40);
            _healthTimer.Tick += IncreaseHealth;
            _healthTimer.Start();
        }

        private void IncreaseHealth(object sender, object e)
        {
            if (pbHealth.Value < targetHealth)
            {
                pbHealth.Value += 1;
                return;
            }

            _healthTimer.Stop();
            _healthTimer.Tick -= IncreaseHealth;
            _healthTimer = null;

            imRedPotion.Visibility = Visibility.Visible;
        }

        // =========================
        // POCIÓN AMARILLA
        // =========================
        private void UseYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_energyTimer != null)
                return;

            targetEnergy = Math.Min(pbEnergy.Maximum, pbEnergy.Value + 40);

            imYellowPotion.Visibility = Visibility.Collapsed;

            _energyTimer = new DispatcherTimer();
            _energyTimer.Interval = TimeSpan.FromMilliseconds(40);
            _energyTimer.Tick += IncreaseEnergy;
            _energyTimer.Start();
        }

        private void IncreaseEnergy(object sender, object e)
        {
            if (pbEnergy.Value < targetEnergy)
            {
                pbEnergy.Value += 1;
                return;
            }

            _energyTimer.Stop();
            _energyTimer.Tick -= IncreaseEnergy;
            _energyTimer = null;

            imYellowPotion.Visibility = Visibility.Visible;
        }

        // =========================
        // MÉTODOS iPokemon
        // =========================
        public void verFondo(bool ver)
        {
            imgFondoBrush.Opacity = ver ? 1 : 0;
        }

        public void verFilaVida(bool ver)
        {
            if (ver)
            {
                gridGeneral.RowDefinitions[0].Height = new GridLength(60);
                imHeart.Visibility = Visibility.Visible;
                pbHealth.Visibility = Visibility.Visible;
                imRedPotion.Visibility = Visibility.Visible;
            }
            else
            {
                gridGeneral.RowDefinitions[0].Height = new GridLength(0);
                imHeart.Visibility = Visibility.Collapsed;
                pbHealth.Visibility = Visibility.Collapsed;
                imRedPotion.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaEnergia(bool ver)
        {
            if (ver)
            {
                gridGeneral.RowDefinitions[1].Height = new GridLength(60);
                imLightning.Visibility = Visibility.Visible;
                pbEnergy.Visibility = Visibility.Visible;
                imYellowPotion.Visibility = Visibility.Visible;
            }
            else
            {
                gridGeneral.RowDefinitions[1].Height = new GridLength(0);
                imLightning.Visibility = Visibility.Collapsed;
                pbEnergy.Visibility = Visibility.Collapsed;
                imYellowPotion.Visibility = Visibility.Collapsed;
            }
        }

        public void verPocionVida(bool ver)
        {
            imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verPocionEnergia(bool ver)
        {
            imYellowPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            tbPokemon.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            if (ver) ActivarEscudo();
            else DesactivarEscudo();
        }

        public void activarAniIdle(bool activar)
        {
            if (estaDerrotado)
                return;

            if (activar)
            {
                if (!estaHerido && !estaCansado && !tieneEscudo)
                {
                    AniIdle.Stop();
                    AniIdle.Begin();
                }
            }
            else
            {
                AniIdle.Stop();
            }
        }

        public void animacionAtaqueFlojo()
        {
            if (estaDerrotado)
                return;

            PararAccionesTemporales();
            PararStoryboardsDeEstado();

            ReproducirSonido("ataque_flojo.mp3"); 

            AniAtaqueFlojo.Stop();
            AniAtaqueFlojo.Begin();
        }

        public void animacionAtaqueFuerte()
        {
            if (estaDerrotado)
                return;

            PararAccionesTemporales();
            PararStoryboardsDeEstado();

            ReproducirSonido("ataque_fuerte.mp3"); 

            AniAtaqueFuerte.Stop();
            AniAtaqueFuerte.Begin();
        }

        public void animacionDefensa()
        {
            if (estaDerrotado)
                return;

            PararAccionesTemporales();
            PararStoryboardsDeEstado();

            tieneEscudo = true;

            ReproducirSonido("defensa.mp3");

            AniDefensa.Stop();
            AniDefensa.Begin();
        }

        public void animacionDescasar()
        {
            if (estaDerrotado)
                return;

            PararAccionesTemporales();
            PararStoryboardsDeEstado();

            ReproducirSonido("descanso.mp3");

            AniDescanso.Stop();
            AniDescanso.Begin();
        }

        public void animacionCansado()
        {
            ActivarCansado();
        }

        public void animacionNoCansado()
        {
            DesactivarCansado();
        }

        public void animacionHerido()
        {
            ActivarHerido();
        }

        public void animacionNoHerido()
        {
            DesactivarHerido();
        }

        public void animacionDerrota()
        {
            estaDerrotado = true; 

            ReproducirSonido("derrotado.mp3");

            ActivarDerrotado();
        }

        // Extra útil para el tester
        public void animacionNoDerrotado()
        {
            DesactivarDerrotado();
        }
    }
}