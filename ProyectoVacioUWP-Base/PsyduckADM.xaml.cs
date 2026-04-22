using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class PsyduckADM : UserControl, iPokemon
    {
        DispatcherTimer _timer;
        double healthIncrement = 0;

        public PsyduckADM()
        {
            this.InitializeComponent();
        }

        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += increaseHealth;
            _timer.Start();
            this.imRedPotion.Visibility = Visibility.Collapsed;
        }

        private void increaseHealth(object sender, object e)
        {
            this.pbHealth.Value += 0.5;
            this.healthIncrement += 0.5;
            if (pbHealth.Value >= 100 || healthIncrement >= 40)
            {
                _timer.Stop();
                healthIncrement = 0;
            }
        }

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

        public string Nombre { get; set; } = "Psyduck";
        public string Categoría { get; set; } = "Pato";
        public string Tipo { get; set; } = "Agua";
        public double Altura { get; set; } = 0.8;
        public double Peso { get; set; } = 19.6;
        public string Evolucion { get; set; } = "Golduck";
        public string Descripcion { get; set; } = "Padece continuamente dolores de cabeza. Cuando son muy fuertes, usa poderes psíquicos.";

        public void verFondo(bool ver) { if (imFondo != null) imFondo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verFilaVida(bool ver)
        {
            if (pbHealth != null) pbHealth.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
            if (imCorazon != null) imCorazon.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }
        public void verFilaEnergia(bool ver)
        {
            if (pbEnergy != null) pbEnergy.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
            if (imRayo != null) imRayo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }
        public void verPocionVida(bool ver) { if (imRedPotion != null) imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verPocionEnergia(bool ver) { if (imYellowPotion != null) imYellowPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }
        public void verNombre(bool ver) { if (txtNombre != null) txtNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed; }

        public void verEscudo(bool ver)
        {
            if (Escudo != null) Escudo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
            // ARREGLADO: Iniciamos o paramos la animación del estado
            if (ver) { ((Storyboard)this.Resources["EstEscudo"]).Begin(); }
            else { ((Storyboard)this.Resources["EstEscudo"]).Stop(); }
        }

        public void activarAniIdle(bool activar)
        {
            if (activar) { ((Storyboard)this.Resources["AniIdle"]).Begin(); }
            else { ((Storyboard)this.Resources["AniIdle"]).Stop(); }
        }

        public void animacionAtaqueFlojo() { ((Storyboard)this.Resources["AniAtaqueDebil"]).Begin(); }
        public void animacionAtaqueFuerte() { ((Storyboard)this.Resources["AniAtaqueFuerte"]).Begin(); }

        public void animacionDefensa()
        {
            // ARREGLADO: Nos aseguramos de que el escudo sea visible para poder parpadear
            if (Escudo != null) Escudo.Visibility = Visibility.Visible;
            ((Storyboard)this.Resources["AniDefensa"]).Begin();
        }

        public void animacionDescasar() { ((Storyboard)this.Resources["AniDescanso"]).Begin(); }
        public void animacionCansado() { ((Storyboard)this.Resources["EstCansado"]).Begin(); }
        public void animacionNoCansado() { ((Storyboard)this.Resources["EstCansado"]).Stop(); }
        public void animacionHerido() { ((Storyboard)this.Resources["EstHerido"]).Begin(); }
        public void animacionNoHerido() { ((Storyboard)this.Resources["EstHerido"]).Stop(); }

        // FUNCIÓN NUEVA PARA LA CAÍDA
        public void animacionDerrota() { ((Storyboard)this.Resources["EstDerrotado"]).Begin(); }
    }
}