using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class MonfernoCRS : UserControl, iPokemon
    {
        private DispatcherTimer _timerVida;
        private DispatcherTimer _timerEnergia;

        private double healthIncrement = 0;
        private double energyIncrement = 0;

        private bool estaCansado = false;
        private bool estaHerido = false;
        private bool tieneEscudo = false;
        private bool estaDerrotado = false;

        public double Vida
        {
            get { return pbHealth.Value; }
            set { pbHealth.Value = value; }
        }

        public double Energia
        {
            get { return pbEnergy.Value; }
            set { pbEnergy.Value = value; }
        }

        public string Nombre
        {
            get { return tbPokemonName.Text; }
            set { tbPokemonName.Text = value; }
        }

        public string Categoría { get => "Juguetón"; set => throw new NotImplementedException(); }
        public string Tipo { get => "Fuego/Lucha"; set => throw new NotImplementedException(); }
        public double Altura { get => 0.9; set => throw new NotImplementedException(); }
        public double Peso { get => 22.0; set => throw new NotImplementedException(); }
        public string Evolucion { get => "Infernape"; set => throw new NotImplementedException(); }
        public string Descripcion { get => "Controla el fuego de su cola para mantener a sus rivales a raya."; set => throw new NotImplementedException(); }


        public MonfernoCRS()
        {
            this.InitializeComponent();
            this.IsTabStop = true;
            this.KeyDown += ControlTeclas;

            this.Loaded += (sender, e) =>
            {
                this.Focus(FocusState.Programmatic);
                tbPokemonName.Text = "Monferno";
                restaurarBaseVisual();

                activarAniIdle(true);
            };
        }

        private void ControlTeclas(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Number1 || e.Key == Windows.System.VirtualKey.NumberPad1)
            {
                animacionAtaqueFlojo();
            }
            else if (e.Key == Windows.System.VirtualKey.Number2 || e.Key == Windows.System.VirtualKey.NumberPad2)
            {
                animacionAtaqueFuerte();
            }
            else if (e.Key == Windows.System.VirtualKey.Number3 || e.Key == Windows.System.VirtualKey.NumberPad3)
            {
                animacionDefensa(); 
            }
            else if (e.Key == Windows.System.VirtualKey.Number4 || e.Key == Windows.System.VirtualKey.NumberPad4)
            {
                animacionDescansar();
            }
        }

        // --- POCIONES ---
        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_timerVida != null && _timerVida.IsEnabled)
                return;

            _timerVida = new DispatcherTimer();
            _timerVida.Interval = TimeSpan.FromMilliseconds(100);
            _timerVida.Tick += increaseHealth;
            _timerVida.Start();

            imRedPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseHealth(object sender, object e)
        {
            pbHealth.Value += 0.5;
            healthIncrement += 0.5;

            if (pbHealth.Value >= 100 || healthIncrement >= 40)
            {
                if (pbHealth.Value > 100) pbHealth.Value = 100;
                _timerVida.Stop();
                healthIncrement = 0;
            }
        }

        private void useYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_timerEnergia != null && _timerEnergia.IsEnabled)
                return;

            _timerEnergia = new DispatcherTimer();
            _timerEnergia.Interval = TimeSpan.FromMilliseconds(100);
            _timerEnergia.Tick += increaseEnergy;
            _timerEnergia.Start();

            imYellowPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseEnergy(object sender, object e)
        {
            pbEnergy.Value += 0.5;
            energyIncrement += 0.5;

            if (pbEnergy.Value >= 100 || energyIncrement >= 40)
            {
                if (pbEnergy.Value > 100) pbEnergy.Value = 100;
                _timerEnergia.Stop();
                energyIncrement = 0;
            }
        }

        public async void animacionAtaqueFuerte()
        {
            tbPokemonName.Text = "¡LLAMARADA!";

            ojo_der.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 180, 180));
            ojo_izq.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 180, 180));
            pupila_der1.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            pupila_izq.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            collar.Fill = new SolidColorBrush(Windows.UI.Colors.Red);

            llamarada.Opacity = 0;
            transFuego.ScaleX = 0.15;
            transFuego.ScaleY = 0.8;
            transFuego.TranslateX = 0;
            transFuego.TranslateY = 0;

            soundLanzallamas.Play();

            Storyboard sbFuego = (Storyboard)this.Resources["sbLlamarada"];
            sbFuego.Begin();

            Storyboard sbAtaque = (Storyboard)this.Resources["sbAtaqueFuerte"];
            sbAtaque.Begin();

            await System.Threading.Tasks.Task.Delay(450);

            llamarada.Opacity = 0;
            transFuego.ScaleX = 0.15;
            transFuego.ScaleY = 0.8;
            transFuego.TranslateX = 0;
            transFuego.TranslateY = 0;

            ojo_der.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 250, 250, 250));
            ojo_izq.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 250, 250, 250));
            pupila_der1.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 53, 48, 43));
            pupila_izq.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 53, 48, 43));
            collar.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 251, 251, 251));

            actualizarTextoEstado();
        }

        public void animacionAtaqueFlojo()
        {
            tbPokemonName.Text = "¡Placaje!";

            soundPlacaje.Play();

            Storyboard sbaux = (Storyboard)this.Resources["sbAtaqueFlojo"];
            sbaux.Begin();
        }

        public async void animacionDefensa()
        {
            tbPokemonName.Text = "¡Defensa Total!";
            tieneEscudo = true;
            actualizarEstadoVisual();

            await System.Threading.Tasks.Task.Delay(800);

            tieneEscudo = false;
            actualizarEstadoVisual();
        }

        public void animacionDescansar()
        {
            tbPokemonName.Text = "Descansando...";

            energiaDescanso.Opacity = 0;
            transEnergiaDescanso.ScaleX = 0.8;
            transEnergiaDescanso.ScaleY = 0.8;
            transEnergiaDescanso.TranslateY = 20;

            soundDescansar.Play();

            Storyboard sbEnergia = (Storyboard)this.Resources["sbDescansarEnergia"];
            sbEnergia.Begin();
            Storyboard sbDescanso = (Storyboard)this.Resources["sbDescansar"];
            sbDescanso.Begin();

            energiaDescanso.Opacity = 0;
            actualizarTextoEstado();
        }

        public void animacionDescasar()
        {
            animacionDescansar();
        }
        public void activarEscudoVisual() { tieneEscudo = true; actualizarEstadoVisual(); }
        public void desactivarEscudoVisual() { tieneEscudo = false; actualizarEstadoVisual(); }

        public void verEscudo(bool ver)
        {
            if (ver) activarEscudoVisual();
            else desactivarEscudoVisual();
        }

        public void activarCansado() { estaCansado = true; actualizarEstadoVisual(); }
        public void desactivarCansado() { estaCansado = false; actualizarEstadoVisual(); }

        public void animacionCansado() { activarCansado(); }
        public void animacionNoCansado() { desactivarCansado(); }

        public void activarHerido() { estaHerido = true; actualizarEstadoVisual(); }
        public void desactivarHerido() { estaHerido = false; actualizarEstadoVisual(); }

        public void animacionHerido() { activarHerido(); }
        public void animacionNoHerido() { desactivarHerido(); }

        public void activarDerrotado()
        {
            estaDerrotado = true;
            activarAniIdle(false); 
            actualizarEstadoVisual();
        }

        public void desactivarDerrotado()
        {
            estaDerrotado = false;
            activarAniIdle(true);
            actualizarEstadoVisual();
        }

        public void animacionDerrota() { activarDerrotado(); }

        public void activarAniIdle(bool activar)
        {
            Storyboard sb = (Storyboard)this.Resources["sbIdle"];
            if (activar)
            {
                actualizarTextoEstado();
                sb.Begin();
            }
            else
            {
                sb.Stop();
                tbPokemonName.Text = "Parado";
            }
        }

        public void verFondo(bool ver)
        {
            if (ver) this.imgFondo.Visibility = Visibility.Visible;
            else this.imgFondo.Visibility = Visibility.Collapsed;
        }

        public void verFilaVida(bool ver)
        {
            if (ver)
            {
                this.pbHealth.Visibility = Visibility.Visible;
                this.imCorazon.Visibility = Visibility.Visible;
            }
            else
            {
                this.pbHealth.Visibility = Visibility.Collapsed;
                this.imCorazon.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaEnergia(bool ver)
        {
            if (ver)
            {
                this.pbEnergy.Visibility = Visibility.Visible;
                this.imEnergia.Visibility = Visibility.Visible;
            }
            else
            {
                this.pbEnergy.Visibility = Visibility.Collapsed;
                this.imEnergia.Visibility = Visibility.Collapsed;
            }
        }

        public void verPocionVida(bool ver)
        {
            this.imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verPocionEnergia(bool ver)
        {
            this.imYellowPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            this.tbPokemonName.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        private void actualizarEstadoVisual()
        {
            restaurarBaseVisual();

            if (estaDerrotado)
            {
                aplicarDerrotado();
                actualizarTextoEstado();
                return;
            }

            if (tieneEscudo) aplicarEscudo();
            if (estaCansado) aplicarCansado();
            if (estaHerido) aplicarHerido();

            resolverBocaYDientes();
            actualizarTextoEstado();
        }

        private void restaurarBaseVisual()
        {
            ___boca_.Visibility = Visibility.Visible;
            bocaCansado.Visibility = Visibility.Collapsed;
            bocaHerido.Visibility = Visibility.Collapsed;

            diente_izq.Visibility = Visibility.Visible;
            diente_der.Visibility = Visibility.Visible;

            sudorCansado.Opacity = 0;
            escudoVisual.Opacity = 0;

            ((Storyboard)this.Resources["sbSudorCaida"]).Stop();
            ((Storyboard)this.Resources["sbColaCansada"]).Stop();
            ((Storyboard)this.Resources["sbColaHerido"]).Stop();
            ((Storyboard)this.Resources["sbTemblorHerido"]).Stop();
            ((Storyboard)this.Resources["sbQuitarDefensa"]).Stop();

            transSudor.TranslateY = 0;
            transCola.Rotation = 0;
            MonfernoTransform.TranslateX = 120;
            MonfernoTransform.TranslateY = 40;

            Storyboard sbHeridoOff = (Storyboard)this.Resources["sbDesactivarHerido"];
            sbHeridoOff.Begin();

            ojoEspiralIzq.Visibility = Visibility.Collapsed;
            ojoEspiralDer.Visibility = Visibility.Collapsed;

            pupila_der.Visibility = Visibility.Visible;
            pupila_der1.Visibility = Visibility.Visible;
            pupila_izq.Visibility = Visibility.Visible;
            pupila_izq1.Visibility = Visibility.Visible;

            ((Storyboard)this.Resources["sbDerrotado"]).Stop();
            ((Storyboard)this.Resources["sbQuitarDerrotado"]).Stop();
        }

        private void aplicarEscudo()
        {
            Storyboard sb = (Storyboard)this.Resources["sbDefensa"];
            sb.Begin();
        }

        private void aplicarCansado()
        {
            sudorCansado.Opacity = 1;
            Storyboard sbCola = (Storyboard)this.Resources["sbColaCansada"];
            sbCola.Begin();
            Storyboard sbSudor = (Storyboard)this.Resources["sbSudorCaida"];
            sbSudor.Begin();
        }

        private void aplicarHerido()
        {
            Storyboard sbColaHerido = (Storyboard)this.Resources["sbColaHerido"];
            sbColaHerido.Begin();
            Storyboard sbTemblorHerido = (Storyboard)this.Resources["sbTemblorHerido"];
            sbTemblorHerido.Begin();
            Storyboard sbHerido = (Storyboard)this.Resources["sbActivarHerido"];
            sbHerido.Begin();
        }

        private void aplicarDerrotado()
        {
            ___boca_.Visibility = Visibility.Collapsed;
            bocaCansado.Visibility = Visibility.Collapsed;
            bocaHerido.Visibility = Visibility.Collapsed;
            diente_izq.Visibility = Visibility.Collapsed;
            diente_der.Visibility = Visibility.Collapsed;
            sudorCansado.Opacity = 0;
            escudoVisual.Opacity = 0;
            pupila_der.Visibility = Visibility.Collapsed;
            pupila_der1.Visibility = Visibility.Collapsed;
            pupila_izq.Visibility = Visibility.Collapsed;
            pupila_izq1.Visibility = Visibility.Collapsed;

            ojoEspiralIzq.Visibility = Visibility.Visible;
            ojoEspiralDer.Visibility = Visibility.Visible;
            transCola.Rotation = -10;

            Storyboard sb = (Storyboard)this.Resources["sbDerrotado"];
            sb.Begin();
        }

        private void resolverBocaYDientes()
        {
            ___boca_.Visibility = Visibility.Collapsed;
            bocaCansado.Visibility = Visibility.Collapsed;
            bocaHerido.Visibility = Visibility.Collapsed;
            diente_izq.Visibility = Visibility.Visible;
            diente_der.Visibility = Visibility.Visible;

            if (estaHerido)
            {
                bocaHerido.Visibility = Visibility.Visible;
                diente_izq.Visibility = Visibility.Collapsed;
                diente_der.Visibility = Visibility.Collapsed;
            }
            else if (estaCansado)
            {
                bocaCansado.Visibility = Visibility.Visible;
                diente_izq.Visibility = Visibility.Collapsed;
                diente_der.Visibility = Visibility.Collapsed;
            }
            else
            {
                ___boca_.Visibility = Visibility.Visible;
                diente_izq.Visibility = Visibility.Visible;
                diente_der.Visibility = Visibility.Visible;
            }
        }

        private void actualizarTextoEstado()
        {
            if (estaDerrotado) tbPokemonName.Text = "Derrotado...";
            else if (estaHerido && estaCansado) tbPokemonName.Text = "Herido y cansado...";
            else if (estaHerido) tbPokemonName.Text = "Herido...";
            else if (estaCansado) tbPokemonName.Text = "Cansado...";
            else if (tieneEscudo) tbPokemonName.Text = "Escudo activo";
            else tbPokemonName.Text = "Monferno";
        }

        private void MonfernoCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            MonfernoTransform.TranslateX += e.Delta.Translation.X;
            MonfernoTransform.TranslateY += e.Delta.Translation.Y;
            MonfernoTransform.ScaleX *= e.Delta.Scale;
            MonfernoTransform.ScaleY *= e.Delta.Scale;

            if (MonfernoTransform.ScaleX < 0.2) MonfernoTransform.ScaleX = 0.2;
            if (MonfernoTransform.ScaleY < 0.2) MonfernoTransform.ScaleY = 0.2;
            if (MonfernoTransform.ScaleX > 6) MonfernoTransform.ScaleX = 6;
            if (MonfernoTransform.ScaleY > 6) MonfernoTransform.ScaleY = 6;

            e.Handled = true;
        }
    }
}