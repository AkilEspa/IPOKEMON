using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class EmpoleonARS : UserControl, iPokemon
    {
        // ... (resto de variables igual) ...
        // Timers para pociones y dormir
        private DispatcherTimer timerVida;
        private DispatcherTimer timerEnergia;
        private DispatcherTimer timerDormir;

        private double incrementoVida = 1.0;
        private double incrementoEnergia = 1.0;

        // Reproductor de sonido
        private MediaPlayer mpSonidos;

        // flags estado 
        private bool esDerrotado = false;
        private bool estaHerido = false;
        private bool estaCansado = false;
        private bool estaEscudo = false;

        // ... (Campos privados de datos igual) ...
        private string categoria = "Emperador";
        private string tipo = "Agua / Acero";
        private double altura = 1.7;
        private double peso = 84.5;
        private string evolucion = "Piplup -> Prinplup -> Empoleon";
        private string descripcion = "Empoleon nada tan rápido como una lancha. Sus alas cortan el hielo como cuchillas.";

        public EmpoleonARS()
        {
            this.InitializeComponent();
            this.IsTabStop = true;
            this.KeyDown += ControlTeclas;

            mpSonidos = new MediaPlayer();

            InicializarTimerDormir();
        }

        // ============================================
        // LÓGICA AUTOMÁTICA (Setters)
        // ============================================
        public double Vida
        {
            get => pbVida.Value;
            set
            {
                // Si ya está derrotado, no permitimos ningún cambio (bloqueo total)
                if (esDerrotado) return;

                pbVida.Value = value;

                // Si la vida baja de 0 (o es 0), disparamos la derrota automáticamente
                if (pbVida.Value <= 0)
                {
                    animacionDerrota();
                }
                /* TODO: DESCOMENTAR EN TRABAJO GRUPAL 
                else
                {
                    // Lógica de estados herido/no herido
                    if (pbVida.Value < 30 && !estaHerido) animacionHerido();
                    else if (pbVida.Value >= 30 && estaHerido) animacionNoHerido();
                }
                */
            }
        }

        public double Energia
        {
            get => pbEnergia.Value;
            set
            {
                if (esDerrotado) return;

                pbEnergia.Value = value;
                if (pbEnergia.Value < 30 && !estaCansado) animacionCansado();
                else if (pbEnergia.Value >= 30 && estaCansado) animacionNoCansado();
            }
        }
        // ... (Resto de getter u setter) ...

        public string Nombre { get => txtNombre.Text; set => txtNombre.Text = value; }
        public string Categoría { get => categoria; set => categoria = value; }
        public string Tipo { get => tipo; set => tipo = value; }
        public double Altura { get => altura; set => altura = value; }
        public double Peso { get => peso; set => peso = value; }
        public string Evolucion { get => evolucion; set => evolucion = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }

        // ... (Métodos visuales, ej verFilaVida, etc) 
        public void verFondo(bool ver) { if (image != null) image.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verFilaVida(bool ver)
        {
            var visibilidad = ver ? Visibility.Visible : Visibility.Collapsed;

            if (pbVida != null) pbVida.Visibility = visibilidad;
            if (imgPocionRoja != null) imgPocionRoja.Visibility = visibilidad;
            if (imgCorazon != null) imgCorazon.Visibility = visibilidad; 
        }

        public void verFilaEnergia(bool ver)
        {
            var visibilidad = ver ? Visibility.Visible : Visibility.Collapsed;

            if (pbEnergia != null) pbEnergia.Visibility = visibilidad;
            if (imgPocionAmarilla != null) imgPocionAmarilla.Visibility = visibilidad;
            if (imgEnergia != null) imgEnergia.Visibility = visibilidad; 
        }
        public void verPocionVida(bool ver) { if (imgPocionRoja != null) { imgPocionRoja.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; imgPocionRoja.IsHitTestVisible = ver; } }
        public void verPocionEnergia(bool ver) { if (imgPocionAmarilla != null) { imgPocionAmarilla.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; imgPocionAmarilla.IsHitTestVisible = ver; } }
        public void verNombre(bool ver) { if (txtNombre != null) txtNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        // public void verEscudo(bool ver) { if (this.FindName("img_destello") is UIElement escudoVisual) escudoVisual.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verEscudo(bool ver)
        {
            if (esDerrotado) return;

            if (ver)
            {
                estaEscudo = true;
                PararStoryboard("quitar_escudo");
                PlayStoryboard("poner_escudo");
            }
            else
            {
                estaEscudo = false;
                PararStoryboard("poner_escudo");
                PlayStoryboard("quitar_escudo");
            }
        }
        // ============================================
        // Animaciones Pokemon
        // ============================================

        public void animacionAtaqueFlojo()
        {
            if (esDerrotado) return;
            
            PlayStoryboard("ataquedebil");
            ReproducirSonido("burbujas.mp3");
        }

        public void animacionAtaqueFuerte()
        {
            if (esDerrotado) return;

            // Buscamos el Storyboard en los recursos
            if (this.Resources["ataquefuerte"] is Storyboard sbAtaqueFuerte)
            {
                // Nos desuscribimos primero para evitar que el evento se acumule si se pulsa varias veces
                sbAtaqueFuerte.Completed -= AtaqueFuerte_Completed;
                sbAtaqueFuerte.Completed += AtaqueFuerte_Completed;
                sbAtaqueFuerte.Begin();
            }

            // Reproducimos el sonido de las olas
            ReproducirSonido("sonido olas del mar.mp3");
        }

        private void AtaqueFuerte_Completed(object sender, object e)
        {
            // Cortamos el sonido al finalizar la animación
            if (sender is Storyboard sb) sb.Completed -= AtaqueFuerte_Completed;
            PararSonido();
        }
        public void animacionDefensa()
        {
            if (esDerrotado) return;
            estaEscudo = true;
            PararStoryboard("quitar_escudo");
            PlayStoryboard("poner_escudo");
            ReproducirSonido("escudo laser.wav");
            verEscudo(true);
        }

        public void animacionDescasar()
        {
            if (esDerrotado) return;
            Descansar();
            //PlayStoryboard("descanso");
        }
        public void activarAniIdle(bool activar)
        {
            if (esDerrotado) return;

            if (activar)
            {
                // detenemos cualquier animacion que pueda estar activa
                PararStoryboard("ataquedebil");
                PararStoryboard("ataquefuerte");
                PararStoryboard("dormir");
                PararStoryboard("despertar");

                PararStoryboard("poner_escudo");
                PararStoryboard("quitar_escudo");
                estaEscudo = false;

                PararStoryboard("estado_cansado");
                PararStoryboard("quitar_cansado");
                estaCansado = false;

                PararStoryboard("estado_herido");
                PararStoryboard("quitar_herido");
                estaHerido = false;

                PlayStoryboard("iddle");
            }
            else
            {
                PararStoryboard("iddle");
            }
        }
        public void animacionCansado()
        {
            if (esDerrotado) return;
            if (estaCansado) return;
            estaCansado = true;
            PararStoryboard("quitar_cansado");
            PlayStoryboard("estado_cansado");
        }

        public void animacionNoCansado()
        {
            if (esDerrotado) return;
            if (!estaCansado) return;
            estaCansado = false;
            PararStoryboard("estado_cansado");
            PlayStoryboard("quitar_cansado");
        }

        public void animacionHerido()
        {
            if (esDerrotado) return;
            if (estaHerido) return;
            estaHerido = true;
            PararStoryboard("quitar_herido");
            PlayStoryboard("estado_herido");
        }

        public void animacionNoHerido()
        {
            if (esDerrotado) return;
            if (!estaHerido) return;
            estaHerido = false;
            PararStoryboard("estado_herido");
            PlayStoryboard("quitar_herido");
        }

        public void animacionDerrota()
        { 
            if (esDerrotado) return;
            esDerrotado = true;
            PararSonido();
            if (timerDormir.IsEnabled) timerDormir.Stop();

            PararStoryboard("ataquedebil");
            PararStoryboard("ataquefuerte");
            PararStoryboard("poner_escudo");
            PararStoryboard("quitar_escudo");
            PararStoryboard("descanso"); // no debe ser necesario, descanso null
            PararStoryboard("dormir");
            PararStoryboard("despertar");

            // paramos el idle si muere
            PararStoryboard("iddle");

            PlayStoryboard("estado_derrotado");
            ReproducirSonido("muerte.wav");
        }

        // ============================================
        // LÓGICA INTERNA Y TECLAS (testeo, no tiene que ver con el visor)
        // ============================================
        private void ControlTeclas(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Number1: animacionAtaqueFlojo(); break;
                case Windows.System.VirtualKey.Number2: animacionAtaqueFuerte(); break;
                case Windows.System.VirtualKey.Number3: animacionDefensa(); break;
                case Windows.System.VirtualKey.Number4: animacionDerrota(); break;
                case Windows.System.VirtualKey.Number5: Descansar(); break; 
                case Windows.System.VirtualKey.Number6: animacionHerido(); break;
                case Windows.System.VirtualKey.Number7: EjecutarDespertar(); break;
                case Windows.System.VirtualKey.Number8: EjecutarQuitarEscudo(); break;
                case Windows.System.VirtualKey.Number9: animacionNoHerido(); break;
            }
        }

        private void EjecutarQuitarEscudo()
        {
            if (esDerrotado) return;
            if (!estaEscudo) return;
            estaEscudo = false;
            PararStoryboard("poner_escudo");
            PlayStoryboard("quitar_escudo");
        }

        private void Descansar()
        {
            if (esDerrotado) return;
            timerDormir.Start();
            if (this.Resources["dormir"] is Storyboard sbDormir)
            {
                sbDormir.Completed -= Dormir_Completado;
                sbDormir.Completed += Dormir_Completado;
                sbDormir.Begin();
            }
        }

        private void Dormir_Completado(object sender, object e)
        {
            if (sender is Storyboard sb) sb.Completed -= Dormir_Completado;
            EjecutarDespertar();
        }

        private void EjecutarDespertar()
        {
            if (esDerrotado) return;
            PararStoryboard("dormir");
            PlayStoryboard("despertar");
        }

        private void InicializarTimerDormir()
        {
            timerDormir = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timerDormir.Tick += (s, e) => {
                bool vidaLlena = false;
                bool energiaLlena = false;
                if (this.Vida < pbVida.Maximum) this.Vida += 2; else vidaLlena = true;
                if (this.Energia < pbEnergia.Maximum) this.Energia += 2; else energiaLlena = true;
                if (vidaLlena && energiaLlena) timerDormir.Stop();
            };
        }

        private void PlayStoryboard(string nombre) { if (this.Resources.ContainsKey(nombre)) (this.Resources[nombre] as Storyboard)?.Begin(); }
        private void PararStoryboard(string nombre) { if (this.Resources.ContainsKey(nombre)) (this.Resources[nombre] as Storyboard)?.Stop(); }
        private void ReproducirSonido(string nombreArchivo) { try { var uri = new Uri($"ms-appx:///AssetsE/{nombreArchivo}"); mpSonidos.Source = MediaSource.CreateFromUri(uri); mpSonidos.Play(); } catch { } }
        private void PararSonido() { try { mpSonidos.Pause(); } catch { } }

        private void imgPocionRoja_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (esDerrotado) return;
            if (timerVida == null)
            {
                timerVida = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                timerVida.Tick += (s, args) =>
                {
                    if (this.Vida < pbVida.Maximum) this.Vida += incrementoVida;
                    else { timerVida.Stop(); imgPocionRoja.Visibility = Visibility.Collapsed; }
                };
            }
            imgPocionRoja.IsHitTestVisible = false;
            timerVida.Start();
            ReproducirSonido("pocion.wav");
        }

        private void imgPocionAmarilla_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (esDerrotado) return;
            if (timerEnergia == null)
            {
                timerEnergia = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                timerEnergia.Tick += (s, args) =>
                {
                    if (this.Energia < pbEnergia.Maximum) this.Energia += incrementoEnergia;
                    else { timerEnergia.Stop(); imgPocionAmarilla.Visibility = Visibility.Collapsed; }
                };
            }
            imgPocionAmarilla.IsHitTestVisible = false;
            timerEnergia.Start();
            ReproducirSonido("pocion.wav");
        }
    }
}