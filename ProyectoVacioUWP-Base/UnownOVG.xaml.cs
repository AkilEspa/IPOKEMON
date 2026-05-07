using ProyectoVacioUWP_Base;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace ProyectoUWPenBlanco
{
    public sealed partial class UnownOVG : UserControl, iPokemon
    {
        private DispatcherTimer timerPocionVida;
        private DispatcherTimer timerPocionEnergia;
        private DispatcherTimer timerDefensa;

        private double incrementoVidaActual = 0;
        private double incrementoEnergiaActual = 0;
        private double objetivoDescansoVida = 0;
        private double objetivoDescansoEnergia = 0;

        private double stepVida = 1.0;
        private double stepEnergia = 1.0;

        private bool estaDerrotado = false;
        private bool estaDescansando = false;

        // Reproductores de Audio
        private MediaPlayer mpEfectos;
        private MediaPlayer mpEstados;

        public UnownOVG()
        {
            this.InitializeComponent();

            this.Vida = 100;
            this.Energia = 100;
            this.Nombre = "Unown";

            // Inicializar reproductores de sonido
            mpEfectos = new MediaPlayer();
            mpEstados = new MediaPlayer();

            timerPocionVida = new DispatcherTimer();
            timerPocionVida.Interval = TimeSpan.FromMilliseconds(50);
            timerPocionVida.Tick += IncrementoVida_Tick;

            timerPocionEnergia = new DispatcherTimer();
            timerPocionEnergia.Interval = TimeSpan.FromMilliseconds(50);
            timerPocionEnergia.Tick += IncrementoEnergia_Tick;

            timerDefensa = new DispatcherTimer();
            timerDefensa.Interval = TimeSpan.FromSeconds(7);
            timerDefensa.Tick += TimerDefensa_Tick;
        }

        // --- MÉTODOS AUXILIARES DE AUDIO ---
        private void ReproducirEfecto(string nombreArchivo)
        {
            mpEfectos.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///AssetsUnownOVG/{nombreArchivo}"));
            mpEfectos.Play();
        }

        private void ReproducirEstado(string nombreArchivo, bool loop)
        {
            mpEstados.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///AssetsUnownOVG/{nombreArchivo}"));
            mpEstados.IsLoopingEnabled = loop;
            mpEstados.Play();
        }
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }
        private void DetenerEstado()
        {
            mpEstados.Pause();
        }
        // -----------------------------------

        public double Vida
        {
            get { return pbVida.Value; }
            set
            {
                if (pbVida != null)
                {
                    if (value > 100) value = 100;
                    if (value < 0) value = 0;
                    pbVida.Value = value;

                    if (pbVida.Value <= 0 && !estaDerrotado)
                    {
                        animacionDerrota();
                    }
                    else if (pbVida.Value > 0 && estaDerrotado)
                    {
                        estaDerrotado = false;
                        ReproducirEfecto("revivir.mp3"); // <--- SONIDO DE REVIVIR AÑADIDO AQUÍ
                        sbRevivir.Begin();

                        if (this.Vida <= 20) animacionHerido();
                    }
                }
            }
        }

        public double Energia
        {
            get { return pbEnergia.Value; }
            set
            {
                if (pbEnergia != null)
                {
                    if (value > 100) value = 100;
                    if (value < 0) value = 0;
                    pbEnergia.Value = value;
                }
            }
        }

        public string Nombre
        {
            get { return tbNombre.Text; }
            set { if (tbNombre != null) tbNombre.Text = value; }
        }

        public string Categoría { get; set; } = "Símbolo";
        public string Tipo { get; set; } = "Psíquico";
        public double Altura { get; set; } = 0.5;
        public double Peso { get; set; } = 5.0;
        public string Evolucion { get; set; } = "Ninguna";
        public string Descripcion { get; set; } = "Se comunica mediante ondas telepáticas.";

        private void usarPocionVida(object sender, PointerRoutedEventArgs e)
        {
            imPocionVida.Visibility = Visibility.Collapsed;
            incrementoVidaActual = 0;
            objetivoDescansoVida = 50;
            stepVida = 2.0;
            ReproducirEfecto("pocion.mp3");
            timerPocionVida.Start();
        }

        private void IncrementoVida_Tick(object sender, object e)
        {
            this.Vida += stepVida;
            incrementoVidaActual += stepVida;

            if (incrementoVidaActual >= objetivoDescansoVida || this.Vida >= 100)
            {
                timerPocionVida.Stop();
                if (this.Vida > 20 && !estaDerrotado) animacionNoHerido();
                ComprobarFinDescanso();
            }
        }

        private void usarPocionEnergia(object sender, PointerRoutedEventArgs e)
        {
            imPocionEnergia.Visibility = Visibility.Collapsed;
            incrementoEnergiaActual = 0;
            objetivoDescansoEnergia = 50;
            stepEnergia = 2.0;
            ReproducirEfecto("pocion.mp3");
            timerPocionEnergia.Start();
        }

        private void IncrementoEnergia_Tick(object sender, object e)
        {
            this.Energia += stepEnergia;
            incrementoEnergiaActual += stepEnergia;

            if (incrementoEnergiaActual >= objetivoDescansoEnergia || this.Energia >= 100)
            {
                timerPocionEnergia.Stop();
                if (this.Energia > 20 && !estaDerrotado) animacionNoCansado();
                ComprobarFinDescanso();
            }
        }

        private void ComprobarFinDescanso()
        {
            if (estaDescansando && !timerPocionVida.IsEnabled && !timerPocionEnergia.IsEnabled)
            {
                Despertar();
            }
        }

        private void Despertar()
        {
            estaDescansando = false;
            DetenerEstado(); // Detiene el ronquido
            if (parpado != null) parpado.Visibility = Visibility.Collapsed;
            if (texto_zzz != null) texto_zzz.Visibility = Visibility.Collapsed;
            sbDescansando.Stop();

            // Si despierta y sigue herido, le devolvemos el latido
            if (this.Vida <= 20 && !estaDerrotado) ReproducirEstado("herido.mp3", true);
        }

        private void TimerDefensa_Tick(object sender, object e)
        {
            timerDefensa.Stop();
            sbDefensaOut.Begin();
        }

        public void verFondo(bool ver) { if (imFondo != null) imFondo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }

        public void verFilaVida(bool ver)
        {
            Visibility v = ver ? Visibility.Visible : Visibility.Collapsed;
            if (pbVida != null) pbVida.Visibility = v;
            if (imIconoVida != null) imIconoVida.Visibility = v;
        }

        public void verFilaEnergia(bool ver)
        {
            Visibility v = ver ? Visibility.Visible : Visibility.Collapsed;
            if (pbEnergia != null) pbEnergia.Visibility = v;
            if (imIconoEnergia != null) imIconoEnergia.Visibility = v;
        }

        public void verPocionVida(bool ver) { if (imPocionVida != null) imPocionVida.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verPocionEnergia(bool ver) { if (imPocionEnergia != null) imPocionEnergia.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verNombre(bool ver) { if (tbNombre != null) tbNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }

        public void verEscudo(bool ver)
        {
            if (escudo != null)
            {
                if (ver)
                {
                    escudo.Visibility = Visibility.Visible;
                    escudo.Opacity = 0.8;
                    sbDefensaOut.Stop();
                    sbDefensa.Stop();
                }
                else
                {
                    escudo.Visibility = Visibility.Collapsed;
                    escudo.Opacity = 0;
                }
            }
        }

        public void activarAniIdle(bool activar)
        {
            if (activar) sbIdle.Begin(); else sbIdle.Stop();
        }

        public void animacionAtaqueFlojo()
        {
            Despertar();
            ReproducirEfecto("ataque_flojo.mp3");
            sbAtaqueFlojo.Stop();
            sbAtaqueFlojo.Begin();
            this.Energia -= 5;
        }

        public void animacionAtaqueFuerte()
        {
            Despertar();
            ReproducirEfecto("ataque_fuerte.mp3");
            sbAtaqueFuerte.Stop();
            sbAtaqueFuerte.Begin();
            this.Energia -= 15;
        }

        public void animacionDefensa()
        {
            Despertar();
            ReproducirEfecto("escudo.mp3");
            sbDefensaOut.Stop();
            sbDefensa.Stop();

            if (escudo != null) escudo.Visibility = Visibility.Visible;
            sbDefensa.Begin();

            timerDefensa.Stop();
            timerDefensa.Start();
        }

        public void animacionDescasar()
        {
            estaDescansando = true;
            ReproducirEstado("ronquido.mp3", true);

            if (parpado != null) parpado.Visibility = Visibility.Visible;
            if (texto_zzz != null) texto_zzz.Visibility = Visibility.Visible;

            sbCansado.Stop();
            sbDescansando.Begin();

            incrementoVidaActual = 0;
            incrementoEnergiaActual = 0;
            objetivoDescansoVida = 20;
            objetivoDescansoEnergia = 20;

            // Al descansar la recuperación es muy lenta (0.1 por tick)
            stepVida = 0.1;
            stepEnergia = 0.1;

            timerPocionVida.Start();
            timerPocionEnergia.Start();
        }

        public void animacionCansado()
        {
            if (parpado != null) parpado.Visibility = Visibility.Visible;
            sbCansado.Stop();
            sbCansado.Begin();
        }

        public void animacionNoCansado()
        {
            if (!estaDescansando && parpado != null) parpado.Visibility = Visibility.Collapsed;
            sbCansado.Stop();
        }

        public void animacionHerido()
        {
            if (!estaDescansando && !estaDerrotado) ReproducirEstado("herido.mp3", true);
            sbHerido.Stop();
            sbHerido.Begin();
        }

        public void animacionNoHerido()
        {
            if (!estaDescansando) DetenerEstado(); // Detiene el latido
            sbHerido.Stop();

            // Restaura los colores a la normalidad si se interrumpe la animación herido
            if (cuerpo_rojo != null) cuerpo_rojo.Opacity = 0;
            if (scBrazo != null) scBrazo.Color = Color.FromArgb(255, 128, 131, 137);
            if (scPataDer != null) scPataDer.Color = Color.FromArgb(255, 128, 131, 137);
            if (scPataIzq != null) scPataIzq.Color = Color.FromArgb(255, 128, 131, 137);
            if (scCuerno != null) scCuerno.Color = Color.FromArgb(255, 128, 131, 137);
        }

        public void animacionDerrota()
        {
            estaDerrotado = true;
            Despertar(); // Detiene cualquier ronquido o latido
            ReproducirEfecto("derrotado.mp3");

            sbIdle.Stop();
            sbHerido.Stop();
            sbDerrota.Stop();
            sbDerrota.Begin();
            this.Vida = 0;
        }
    }
}