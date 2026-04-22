using System;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class ZapdosRGS : UserControl, iPokemon
    {
        private DispatcherTimer _healthTimer;
        private DispatcherTimer _energyTimer;

        private double _healthIncrement;
        private double _energyIncrement;

        private MediaPlayer mpAleteo;
        private MediaPlayer mpMuerte;
        private MediaPlayer mpAtaqueDebil;
        private MediaPlayer mpAtaqueFuerte;
        private MediaPlayer mpEscudoArriba;
        private MediaPlayer mpEscudoAbajo;
        private MediaPlayer mpDescanso;

        private DispatcherTimer sbTimer;

        private bool estadoNormal = false;
        private bool estadoCansado = false;
        private bool estadoHerido = false;
        private bool estadoDefensa = false;
        private bool estadoDescanso = false;
        private bool estadoMuerte = false;

        private string _categoria = "Legendario";
        private string _tipo = "Eléctrico / Volador";
        private double _altura = 1.6;
        private double _peso = 52.6;
        private string _evolucion = "";
        private string _descripcion = "Zapdos es un Pokémon legendario de tipo Eléctrico y Volador." +
            " Se dice que vive entre nubes de tormenta y que obtiene energía de los rayos. Cuando bate sus alas, puede generar descargas eléctricas muy poderosas a su alrededor.";

        public double Vida
        {
            get { return pbHealth.Value; }
            set
            {
                double nuevoValor = value;

                if (nuevoValor < pbHealth.Minimum)
                    nuevoValor = pbHealth.Minimum;

                if (nuevoValor > pbHealth.Maximum)
                    nuevoValor = pbHealth.Maximum;

                pbHealth.Value = nuevoValor;
            }
        }

        public double Energia
        {
            get { return pbEnergy.Value; }
            set
            {
                double nuevoValor = value;

                if (nuevoValor < pbEnergy.Minimum)
                    nuevoValor = pbEnergy.Minimum;

                if (nuevoValor > pbEnergy.Maximum)
                    nuevoValor = pbEnergy.Maximum;

                pbEnergy.Value = nuevoValor;
            }
        }

        public string Nombre
        {
            get { return tbNombre.Text; }
            set { tbNombre.Text = value; }
        }

        public string Categoría
        {
            get { return _categoria; }
            set { _categoria = value; }
        }

        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        public double Altura
        {
            get { return _altura; }
            set { _altura = value; }
        }

        public double Peso
        {
            get { return _peso; }
            set { _peso = value; }
        }

        public string Evolucion
        {
            get { return _evolucion; }
            set { _evolucion = value; }
        }

        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }

        public ZapdosRGS()
        {
            InitializeComponent();
            Loaded += PokemonControl_Loaded;
            Unloaded += PokemonControl_Unloaded;
        }

        private void PokemonControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(canvas.RenderTransform is CompositeTransform))
                canvas.RenderTransform = new CompositeTransform();

            if (!(PataIzquierdaConTodo.RenderTransform is CompositeTransform))
                PataIzquierdaConTodo.RenderTransform = new CompositeTransform();

            if (!(PataDerechaConTodo.RenderTransform is CompositeTransform))
                PataDerechaConTodo.RenderTransform = new CompositeTransform();

            PataIzquierdaConTodo.RenderTransformOrigin = new Point(0.5, 0.1);
            PataDerechaConTodo.RenderTransformOrigin = new Point(0.5, 0.1);

            if (mpAleteo == null)
            {
                mpAleteo = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/aleteo.mp3")),
                    IsLoopingEnabled = true,
                    Volume = 0.6
                };
            }

            if (mpMuerte == null)
            {
                mpMuerte = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/grito.mp3")),
                    Volume = 0.25
                };
            }

            if (mpAtaqueDebil == null)
            {
                mpAtaqueDebil = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/ataque-debil.mpeg")),
                    Volume = 0.8
                };
            }

            if (mpAtaqueFuerte == null)
            {
                mpAtaqueFuerte = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/ataque-fuerte.mpeg")),
                    Volume = 0.8
                };
            }

            if (mpEscudoArriba == null)
            {
                mpEscudoArriba = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/escudo_arriba.mpeg")),
                    Volume = 0.8
                };
            }

            if (mpEscudoAbajo == null)
            {
                mpEscudoAbajo = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/escudo_abajo.mpeg")),
                    Volume = 0.8
                };
            }

            if (mpDescanso == null)
            {
                mpDescanso = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri("ms-appx:///AssetsZapdosRGS/descanso-bucle.mpeg")),
                    IsLoopingEnabled = true,
                    Volume = 0.18
                };
            }

            if (sbTimer == null)
            {
                sbTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(30)
                };
            }

            if (imFondo != null)
            {
                imFondo.Source = new BitmapImage(new Uri("ms-appx:///AssetsZapdosRGS/fondo3.jpg"));
            }

            sBDespertar.Completed += SBDespertar_Completed;
            sBNoCansado.Completed += SBNocansado_Completed;
            sBNoHerido.Completed += SBNoHerido_Completed;
            sBDefensa.Completed += SBDefensa_Completed;
            sBQuitarEscudo.Completed += SBQuitarEscudo_Completed;

            ResetToIdle();
        }

        private void PokemonControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sbTimer != null)
                sbTimer.Stop();

            sBDespertar.Completed -= SBDespertar_Completed;
            sBNoCansado.Completed -= SBNocansado_Completed;
            sBNoHerido.Completed -= SBNoHerido_Completed;
            sBDefensa.Completed -= SBDefensa_Completed;
            sBQuitarEscudo.Completed -= SBQuitarEscudo_Completed;

            StopAllAudio();
        }

        private void SBDespertar_Completed(object sender, object e)
        {
            ApplyStates(skipFullReset: true);
        }

        private void SBNocansado_Completed(object sender, object e)
        {
            ApplyStates(skipFullReset: true);
        }

        private void SBNoHerido_Completed(object sender, object e)
        {
            OcultarHeridas();
            ApplyStates(skipFullReset: true);
        }

        private void SBDefensa_Completed(object sender, object e)
        {
            if (estadoDefensa && !estadoMuerte)
            {
                sBMantenerEscudo.Stop();
                sBMantenerEscudo.Begin();
            }
        }

        private void SBQuitarEscudo_Completed(object sender, object e)
        {
            EscudoHexagonal.Opacity = 0;
            BrilloEscudo.Opacity = 0;
            Hex1.Opacity = 0;
            Hex2.Opacity = 0;
            Hex3.Opacity = 0;
            Hex4.Opacity = 0;
            Hex5.Opacity = 0;

            ApplyStates(skipFullReset: true);
        }

        private void StopAllStoryboards()
        {
            sBAleteo?.Stop();
            sBCansado?.Stop();
            sBNoCansado?.Stop();
            sBHerido?.Stop();
            sBNoHerido?.Stop();
            sBAtaqueDebil?.Stop();
            sBAtaqueFuerte?.Stop();
            sBDefensa?.Stop();
            sBMantenerEscudo?.Stop();
            sBQuitarEscudo?.Stop();
            sBDescanso?.Stop();
            sBDespertar?.Stop();
            sBMuerte?.Stop();
        }

        private void StopCryTimer()
        {
            sbTimer?.Stop();
        }

        private void StopAllAudio()
        {
            mpAleteo?.Pause();
            mpMuerte?.Pause();
            mpAtaqueDebil?.Pause();
            mpAtaqueFuerte?.Pause();
            mpEscudoArriba?.Pause();
            mpEscudoAbajo?.Pause();
            mpDescanso?.Pause();

            if (mpAleteo != null)
                mpAleteo.PlaybackSession.Position = TimeSpan.Zero;

            if (mpMuerte != null)
                mpMuerte.PlaybackSession.Position = TimeSpan.Zero;

            if (mpAtaqueDebil != null)
                mpAtaqueDebil.PlaybackSession.Position = TimeSpan.Zero;

            if (mpAtaqueFuerte != null)
                mpAtaqueFuerte.PlaybackSession.Position = TimeSpan.Zero;

            if (mpEscudoArriba != null)
                mpEscudoArriba.PlaybackSession.Position = TimeSpan.Zero;

            if (mpEscudoAbajo != null)
                mpEscudoAbajo.PlaybackSession.Position = TimeSpan.Zero;

            if (mpDescanso != null)
                mpDescanso.PlaybackSession.Position = TimeSpan.Zero;
        }

        private void StartAtaqueDebilSound()
        {
            if (mpAtaqueDebil == null)
                return;

            mpAtaqueDebil.Pause();
            mpAtaqueDebil.PlaybackSession.Position = TimeSpan.Zero;
            mpAtaqueDebil.Play();
        }

        private void StartAtaqueFuerteSound()
        {
            if (mpAtaqueFuerte == null)
                return;

            mpAtaqueFuerte.Pause();
            mpAtaqueFuerte.PlaybackSession.Position = TimeSpan.Zero;
            mpAtaqueFuerte.Play();
        }

        private void StartEscudoArribaSound()
        {
            if (mpEscudoArriba == null)
                return;

            mpEscudoArriba.Pause();
            mpEscudoArriba.PlaybackSession.Position = TimeSpan.Zero;
            mpEscudoArriba.Play();
        }

        private void StartEscudoAbajoSound()
        {
            if (mpEscudoAbajo == null)
                return;

            mpEscudoAbajo.Pause();
            mpEscudoAbajo.PlaybackSession.Position = TimeSpan.Zero;
            mpEscudoAbajo.Play();
        }

        private void StartDescansoSound()
        {
            if (mpDescanso == null)
                return;

            mpDescanso.Pause();
            mpDescanso.PlaybackSession.Position = TimeSpan.Zero;
            mpDescanso.Play();
        }

        private void StopDescansoSound()
        {
            if (mpDescanso == null)
                return;

            mpDescanso.Pause();
            mpDescanso.PlaybackSession.Position = TimeSpan.Zero;
        }

        private void StartAleteoSound()
        {
            if (mpAleteo == null)
                return;

            if (mpAleteo.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                mpAleteo.PlaybackSession.Position = TimeSpan.Zero;
                mpAleteo.Play();
            }
        }

        private void StartDeathSound()
        {
            if (mpMuerte == null)
                return;

            mpMuerte.Pause();
            mpMuerte.PlaybackSession.Position = TimeSpan.Zero;
            mpMuerte.Play();
        }

        private void ResetBaseTransforms()
        {
            if (canvas.RenderTransform is CompositeTransform ct)
            {
                ct.TranslateX = 0;
                ct.TranslateY = 0;
                ct.ScaleX = 1;
                ct.ScaleY = 1;
                ct.Rotation = 0;
                ct.SkewX = 0;
                ct.SkewY = 0;
            }

            if (PataIzquierdaConTodo.RenderTransform is CompositeTransform ctPataIzq)
            {
                ctPataIzq.TranslateX = 0;
                ctPataIzq.TranslateY = 0;
                ctPataIzq.ScaleX = 1;
                ctPataIzq.ScaleY = 1;
                ctPataIzq.Rotation = 0;
                ctPataIzq.SkewX = 0;
                ctPataIzq.SkewY = 0;
            }

            if (PataDerechaConTodo.RenderTransform is CompositeTransform ctPataDer)
            {
                ctPataDer.TranslateX = 0;
                ctPataDer.TranslateY = 0;
                ctPataDer.ScaleX = 1;
                ctPataDer.ScaleY = 1;
                ctPataDer.Rotation = 0;
                ctPataDer.SkewX = 0;
                ctPataDer.SkewY = 0;
            }

            canvas.Opacity = 1;

            AlaIzquierdaCompleta.Opacity = 1;
            AlaDerechaCompleta.Opacity = 1;

            OjoDerechoMuerto.Opacity = 0;
            OjoIzquierdoMuerto.Opacity = 0;
            DetalleBlandoOjo.Opacity = 1;
            DetalleOjoIzquierdoBlanco.Opacity = 1;

            DescansoVisual.Opacity = 0;
            ZDescanso1.Opacity = 0;
            ZDescanso2.Opacity = 0;
            ZDescanso3.Opacity = 0;

            AtaqueDebilVisual.Opacity = 0;
            AtaqueFuerteVisual.Opacity = 0;

            GotaSudor1.Opacity = 0;
            GotaSudor2.Opacity = 0;
            GotaSudor3.Opacity = 0;
            GotaSudor4.Opacity = 0;
            GotaSudor5.Opacity = 0;
            GotaSudor6.Opacity = 0;

            EscudoHexagonal.Opacity = 0;
            BrilloEscudo.Opacity = 0;
            Hex1.Opacity = 0;
            Hex2.Opacity = 0;
            Hex3.Opacity = 0;
            Hex4.Opacity = 0;
            Hex5.Opacity = 0;

            OcultarHeridas();
        }

        private void StopAll()
        {
            StopAllStoryboards();
            StopAllAudio();
            ResetBaseTransforms();
        }

        private bool DebeSonarAleteo()
        {
            return !estadoMuerte &&
                   !estadoDescanso &&
                   (estadoNormal || estadoCansado || estadoHerido || estadoDefensa);
        }

        private void ApplyStates(bool skipFullReset = false)
        {
            StopAllStoryboards();
            StopCryTimer();

            if (!skipFullReset)
                ResetBaseTransforms();

            if (estadoMuerte)
            {
                mpAleteo?.Pause();
                StopDescansoSound();
                sBMuerte.Begin();
                StartDeathSound();
                return;
            }

            if (estadoNormal)
                sBAleteo.Begin();

            if (estadoCansado)
                sBCansado.Begin();

            if (estadoHerido)
            {
                MostrarHeridas();
                sBHerido.Begin();
            }

            if (estadoDefensa)
                sBMantenerEscudo.Begin();

            if (estadoDescanso)
            {
                DescansoVisual.Opacity = 1;
                sBDescanso.Begin();
                StartDescansoSound();
                mpAleteo?.Pause();
                StopCryTimer();
                return;
            }
            else
            {
                StopDescansoSound();
            }

            if (DebeSonarAleteo())
                StartAleteoSound();
            else
                mpAleteo?.Pause();
        }

        private void ResetToIdle()
        {
            estadoNormal = false;
            estadoCansado = false;
            estadoHerido = false;
            estadoDefensa = false;
            estadoDescanso = false;
            estadoMuerte = false;

            StopAll();
        }

        public void ActivarIdle()
        {
            if (estadoMuerte)
                return;

            estadoNormal = true;
            ApplyStates();
        }

        public void DesactivarIdle()
        {
            if (estadoMuerte)
                return;

            estadoNormal = false;
            ApplyStates();
        }

        public void ToggleNormal()
        {
            if (estadoMuerte)
                return;

            estadoNormal = !estadoNormal;
            ApplyStates();
        }

        public void ToggleMuerte()
        {
            estadoMuerte = !estadoMuerte;
            ApplyStates();
        }

        public void ToggleCansado()
        {
            if (estadoMuerte)
                return;

            if (!estadoCansado)
            {
                estadoCansado = true;
                ApplyStates();
            }
            else
            {
                estadoCansado = false;
                sBCansado.Stop();
                sBNoCansado.Stop();
                sBNoCansado.Begin();
            }
        }

        public void ToggleHerido()
        {
            if (estadoMuerte)
                return;

            if (!estadoHerido)
            {
                estadoHerido = true;
                MostrarHeridas();
                ApplyStates();
            }
            else
            {
                estadoHerido = false;
                sBHerido.Stop();
                sBNoHerido.Stop();
                sBNoHerido.Begin();
            }
        }

        public void ToggleDefensa()
        {
            if (estadoMuerte)
                return;

            if (!estadoDefensa)
            {
                estadoDefensa = true;

                sBMantenerEscudo.Stop();
                sBQuitarEscudo.Stop();
                sBDefensa.Stop();

                StartEscudoArribaSound();
                sBDefensa.Begin();
            }
            else
            {
                estadoDefensa = false;

                sBMantenerEscudo.Stop();
                sBDefensa.Stop();
                sBQuitarEscudo.Stop();

                StartEscudoAbajoSound();
                sBQuitarEscudo.Begin();
            }
        }

        public void ToggleDescanso()
        {
            if (estadoMuerte)
                return;

            if (!estadoDescanso)
            {
                estadoDescanso = true;
                ApplyStates();
            }
            else
            {
                estadoDescanso = false;

                StopDescansoSound();

                sBDescanso.Stop();
                sBDespertar.Stop();
                sBDespertar.Begin();
            }
        }

        public void LanzarAtaqueDebil()
        {
            if (estadoMuerte)
                return;

            sBAtaqueDebil.Stop();
            sBAtaqueDebil.Begin();
            StartAtaqueDebilSound();
        }

        public void LanzarAtaqueFuerte()
        {
            if (estadoMuerte)
                return;

            sBAtaqueFuerte.Stop();
            sBAtaqueFuerte.Begin();
            StartAtaqueFuerteSound();
        }

        private void MostrarHeridas()
        {
            HeridaAlaIzquierda.Opacity = 1;
            HeridaPecho.Opacity = 1;
            HeridaCara.Opacity = 1;
        }

        private void OcultarHeridas()
        {
            HeridaAlaIzquierda.Opacity = 0;
            HeridaPecho.Opacity = 0;
            HeridaCara.Opacity = 0;
        }

        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_healthTimer != null && _healthTimer.IsEnabled)
                return;

            _healthIncrement = 0;

            _healthTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _healthTimer.Tick += increaseHealth;
            _healthTimer.Start();

            imRedPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseHealth(object sender, object e)
        {
            const double step = 0.5;
            const double maxGain = 40.0;

            pbHealth.Value = Math.Min(pbHealth.Maximum, pbHealth.Value + step);
            _healthIncrement += step;

            if (pbHealth.Value >= pbHealth.Maximum || _healthIncrement >= maxGain)
            {
                _healthTimer.Stop();
                _healthTimer.Tick -= increaseHealth;
                _healthTimer = null;
                _healthIncrement = 0;
            }
        }

        private void useYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_energyTimer != null && _energyTimer.IsEnabled)
                return;

            _energyIncrement = 0;

            _energyTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _energyTimer.Tick += increaseEnergy;
            _energyTimer.Start();

            imYellowPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseEnergy(object sender, object e)
        {
            const double step = 0.5;
            const double maxGain = 40.0;

            pbEnergy.Value = Math.Min(pbEnergy.Maximum, pbEnergy.Value + step);
            _energyIncrement += step;

            if (pbEnergy.Value >= pbEnergy.Maximum || _energyIncrement >= maxGain)
            {
                _energyTimer.Stop();
                _energyTimer.Tick -= increaseEnergy;
                _energyTimer = null;
                _energyIncrement = 0;
            }
        }

        private void molestar(object sender, PointerRoutedEventArgs e)
        {
        }

        private void cosquillas(object sender, PointerRoutedEventArgs e)
        {
        }

        public void verFondo(bool ver)
        {
            if (imFondo == null)
                return;

            if (ver)
                imFondo.Source = new BitmapImage(new Uri("ms-appx:///AssetsZapdosRGS/fondo3.jpg"));
            else
                imFondo.Source = null;
        }

        public void verFilaVida(bool ver)
        {
            if (imHeart != null)
                imHeart.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

            if (pbHealth != null && pbHealth.Parent is Border)
                ((Border)pbHealth.Parent).Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

            if (imRedPotion != null)
                imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verFilaEnergia(bool ver)
        {
            if (imEnergy != null)
                imEnergy.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

            if (pbEnergy != null && pbEnergy.Parent is Border)
                ((Border)pbEnergy.Parent).Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

               if (imYellowPotion != null)
                imYellowPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }



        public void verPocionVida(bool ver)
        {
            if (imRedPotion != null)
                imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verPocionEnergia(bool ver)
        {
            if (imYellowPotion != null)
                imYellowPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            if (tbNombre != null)
                tbNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            if (ver)
            {
                if (!estadoDefensa)
                    ToggleDefensa();
            }
            else
            {
                if (estadoDefensa)
                    ToggleDefensa();
                else
                {
                    EscudoHexagonal.Opacity = 0;
                    BrilloEscudo.Opacity = 0;
                    Hex1.Opacity = 0;
                    Hex2.Opacity = 0;
                    Hex3.Opacity = 0;
                    Hex4.Opacity = 0;
                    Hex5.Opacity = 0;
                }
            }
        }

        public void cambiarTamano(double porcentaje)
        {
            double escala = porcentaje / 100.0;

            if (escala < 0.1)
                escala = 0.1;

            if (!(canvas.RenderTransform is CompositeTransform))
                canvas.RenderTransform = new CompositeTransform();

            canvas.RenderTransformOrigin = new Point(0.5, 0.5);

            CompositeTransform transform = (CompositeTransform)canvas.RenderTransform;
            transform.ScaleX = escala;
            transform.ScaleY = escala;
        }


        public void activarAniIdle(bool activar)
        {
            if (activar)
                ActivarIdle();
            else
                DesactivarIdle();
        }

        public void animacionAtaqueFlojo()
        {
            LanzarAtaqueDebil();
        }

        public void animacionAtaqueFuerte()
        {
            LanzarAtaqueFuerte();
        }

        public void animacionDefensa()
        {
            if (!estadoDefensa)
                ToggleDefensa();
        }

        public void animacionDescasar()
        {
                ToggleDescanso();
        }

        public void animacionCansado()
        {
            if (!estadoCansado)
                ToggleCansado();
        }

        public void animacionNoCansado()
        {
            if (estadoCansado)
                ToggleCansado();
        }

        public void animacionHerido()
        {
            if (!estadoHerido)
                ToggleHerido();
        }

        public void animacionNoHerido()
        {
            if (estadoHerido)
                ToggleHerido();
        }

        public void animacionDerrota()
        {
            if (!estadoMuerte)
                ToggleMuerte();
        }

        public void ResetearDerrota()
        {
            if (estadoMuerte)
                ToggleMuerte();
        }


        public void animacionNoDerrota()
        {
            if (estadoMuerte)
                ToggleMuerte();
        }
    }
}