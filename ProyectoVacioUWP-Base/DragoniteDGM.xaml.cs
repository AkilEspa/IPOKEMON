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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProyectoVacioUWP_Base
{
    public sealed partial class DragoniteDGM : UserControl, iPokemon
    {
        DispatcherTimer _timerHealth;
        DispatcherTimer _timerEnergy;
        DispatcherTimer _timerDescansar;

        double healthIncrement = 0;
        double energyIncrement = 0;

        bool _idleActivo = false;
        bool _cansadoActivo = false;
        bool _heridoActivo = false;

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
            get { return this.NombrePokemon.Text; }
            set { this.NombrePokemon.Text = value; }
        }
        public string Categoría { get { return "Dragón"; } set => throw new NotImplementedException(); }
        public string Tipo { get { return "Dragón/Volador"; } set => throw new NotImplementedException(); }
        public double Altura { get { return 2.2; } set => throw new NotImplementedException(); }
        public double Peso { get { return 210; } set => throw new NotImplementedException(); }
        public string Evolucion { get { return "Dratini -> Dragonair -> Dragonite"; } set => throw new NotImplementedException(); }
        public string Descripcion
        {
            get
            {
                return "Dragonite es un Pokémon de tipo Dragón y Volador introducido en la primera generación. " +
                       "Es la evolución final de Dratini y la evolución de Dragonair. Dragonite es conocido por su apariencia " +
                       "amigable y su gran poder, a pesar de ser un Pokémon de tipo Dragón, que a menudo se asocia con criaturas " +
                       "más feroces. Es capaz de volar a velocidades increíbles y se dice que puede circunnavegar el mundo en 16 horas.";
            }
            set => throw new NotImplementedException();
        }

        private bool _verEnergia = true;
        public bool VerEnergia
        {
            get { return _verEnergia; }
            set
            {
                _verEnergia = value;
                if (!_verEnergia) this.gridGeneral.RowDefinitions[1].Height = new GridLength(0);
                else this.gridGeneral.RowDefinitions[1].Height = new GridLength(10, GridUnitType.Star);
            }
        }

        public DragoniteDGM()
        {
            this.InitializeComponent();
            pbEnergy.Value = pbEnergy.Maximum / 2;
            pbHealth.Value = pbHealth.Maximum / 2;
            this.IsTabStop = true;
            this.Focus(FocusState.Programmatic);
        }

        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            _timerHealth = new DispatcherTimer();
            _timerHealth.Interval = TimeSpan.FromMilliseconds(100);
            _timerHealth.Tick += increaseHealth;
            _timerHealth.Start();
            this.imRedPotion.Visibility = Visibility.Collapsed;
        }
        private void increaseHealth(object sender, object e)
        {
            this.pbHealth.Value += 0.5;
            this.healthIncrement += 0.5;
            if (pbHealth.Value >= 100 || healthIncrement >= 40)
            {
                _timerHealth.Stop();
                healthIncrement = 0;
            }
        }
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }
        private void useYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            _timerEnergy = new DispatcherTimer();
            _timerEnergy.Interval = TimeSpan.FromMilliseconds(100);
            _timerEnergy.Tick += increaseEnergy;
            _timerEnergy.Start();
            this.imYellowPotion.Visibility = Visibility.Collapsed;
        }
        private void increaseEnergy(object sender, object e)
        {
            this.pbEnergy.Value += 0.5;
            this.energyIncrement += 0.5;
            if (pbEnergy.Value >= 100 || energyIncrement >= 40)
            {
                _timerEnergy.Stop();
                energyIncrement = 0;
            }
        }

        public void verFondo(bool verfondo)
        {
            this.Fondo.Source = verfondo
                ? new BitmapImage(new Uri("ms-appx:///AssetsDragoniteDGM/Fondo.jpg"))
                : null;
        }

        public void verFilaVida(bool ver)
        {
            Visibility visibility = ver ? Visibility.Visible : Visibility.Collapsed;
            red_heart.Visibility = visibility;
            pbHealth.Visibility = visibility;
            imRedPotion.Visibility = visibility;
        }

        public void verFilaEnergia(bool ver)
        {
            Visibility visibility = ver ? Visibility.Visible : Visibility.Collapsed;
            energy.Visibility = visibility;
            pbEnergy.Visibility = visibility;
            imYellowPotion.Visibility = visibility;
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
            NombrePokemon.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            if (ver)
            {
                DejarDeVerEscudo.Stop();
                Escudo.Visibility = Visibility.Visible;
                VerEscudo.Begin();
            }
            else
            {
                VerEscudo.Stop();
                DejarDeVerEscudo.Begin();
            }
        }

        private void DetenerAcciones()
        {
            AtaqueDebil?.Stop();
            AtaqueFuerte?.Stop();
            DefensaEscudo?.Stop();
            AccionDescansar?.Stop();
            _timerDescansar?.Stop();
        }

        private void DetenerEstados()
        {
            AniIdle?.Stop();
            AniIdleAlas?.Stop(); 
            EstadoCansado?.Stop();
            QuitarEstadoCansado?.Stop();
            EstadoHerido?.Stop();
            DejarEstadoHerido?.Stop();
        }

        private void ReanudarEstados()
        {
            if (_idleActivo) AniIdle.Begin();
            if (_cansadoActivo) EstadoCansado.Begin();
            if (_heridoActivo) EstadoHerido.Begin();
        }

        public void activarAniIdle(bool activar)
        {
            _idleActivo = activar;
            if (activar)
            {
                AniIdle.Begin();
                AniIdleAlas.Begin();
            }
            else
            {
                AniIdle.Stop();
                AniIdleAlas.Stop();
            }
        }

        public void animacionCansado()
        {
            _cansadoActivo = true;
            QuitarEstadoCansado.Stop();
            EstadoCansado.Begin();
            if (_idleActivo)
                AniIdleAlas.Begin();
        }

        public void animacionNoCansado()
        {
            _cansadoActivo = false;
            EstadoCansado.Stop();
            QuitarEstadoCansado.Begin();
        }

        public void animacionHerido()
        {
            _heridoActivo = true;
            DejarEstadoHerido.Stop();
            EstadoHerido.Begin();
            if (_idleActivo)
                AniIdleAlas.Begin();
        }

        public void animacionNoHerido()
        {
            _heridoActivo = false;
            EstadoHerido.Stop();
            DejarEstadoHerido.Begin();
        }

        public void animacionAtaqueFlojo()
        {
            DetenerAcciones();
            AtaqueDebil.Begin();
            AtaqueDebil.Completed += (s, e) => ReanudarEstados();
        }

        public void animacionAtaqueFuerte()
        {
            DetenerAcciones();
            AtaqueFuerte.Begin();
            AtaqueFuerte.Completed += (s, e) => ReanudarEstados();
        }

        public void animacionDefensa()
        {
            DetenerAcciones();
            DefensaEscudo.Begin();
            DefensaEscudo.Completed += (s, e) =>
            {
                _timerDescansar = new DispatcherTimer();
                _timerDescansar.Interval = TimeSpan.FromSeconds(1);
                _timerDescansar.Tick += (ts, te) =>
                {
                    _timerDescansar.Stop();
                    QuitarEscudo.Begin();
                    QuitarEscudo.Completed += (qs, qe) => ReanudarEstados();
                };
                _timerDescansar.Start();
            };
        }

        public void animacionDescasar()
        {
            DetenerAcciones();
            AccionDescansar.Begin();

            AccionDescansar.Completed += (s, e) =>
            {
                _timerDescansar = new DispatcherTimer();
                _timerDescansar.Interval = TimeSpan.FromSeconds(1);
                _timerDescansar.Tick += (ts, te) =>
                {
                    _timerDescansar.Stop();
                    DejarDeDescansar.Begin();
                    DejarDeDescansar.Completed += (qs, qe) => ReanudarEstados();
                };
                _timerDescansar.Start();
            };
        }

        public void animacionDerrota()
        {
            DetenerAcciones();
            DetenerEstados();
            EstadoDerrotado.Begin();
        }
    }

}
