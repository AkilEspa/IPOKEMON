using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProyectoVacioUWP_Base
{
    public sealed partial class HoothootJDCF : UserControl, iPokemon
    {
        DispatcherTimer dtTime;

        public double Vida { get => this.barraVida.Value; set => this.barraVida.Value = value; }
        
        private double vidaSubida { get; set; }
        public double Energia { get => this.barraEnergia.Value; set => this.barraEnergia.Value = value; }
        public string Nombre { get => txtNombre.Text; }
        public string Categoría { get => "Buho"; }
        public string Tipo { get => "Volador"+"Normal";}
        public double Altura { get => 0.7; }
        public double Peso { get => 21.2;}
        public string Evolucion { get => "Noctowl";}
        public string Descripcion { get => "Cuenta con un órgano interno en la cabeza, con el que percibe la rotación de la tierra. Gracias a él, ulula todos los días a la misma hora, como un reloj"; }
        string iPokemon.Nombre { get => Nombre; set => throw new NotImplementedException(); }
        string iPokemon.Categoría { get => Categoría; set => throw new NotImplementedException(); }
        string iPokemon.Tipo { get => Tipo; set => throw new NotImplementedException(); }
        double iPokemon.Altura { get => Altura; set => throw new NotImplementedException(); }
        double iPokemon.Peso { get => Peso; set => throw new NotImplementedException(); }
        string iPokemon.Evolucion { get => Evolucion; set => throw new NotImplementedException(); }
        string iPokemon.Descripcion { get => Descripcion; set => throw new NotImplementedException(); }

        bool pocionVida = true;
        bool pocionEnergia = true;
        public HoothootJDCF()
        {
            this.InitializeComponent();
        }
        private void usarPocion(object sender, PointerRoutedEventArgs e)
        {
            dtTime = new DispatcherTimer();
            dtTime.Interval = TimeSpan.FromMilliseconds(100);
            dtTime.Tick += subirVida;
            dtTime.Start();
            this.imgPozCor.Visibility = Visibility.Collapsed;
            pocionVida = false;
        }

        private void UsarPozEnergia(object sender, PointerRoutedEventArgs e)
        {
            dtTime = new DispatcherTimer();
            dtTime.Interval = TimeSpan.FromMilliseconds(50);
            dtTime.Tick += subirEnergia;
            dtTime.Start();
            this.imgPozEner.Visibility = Visibility.Collapsed;
            pocionEnergia = false;
        }

        private void subirVida(object sender, object e)
        {
            this.barraVida.Value += 0.5;
            if (barraVida.Value >= 75)
            {
                this.dtTime.Stop();
            }
        }
        private void subirEnergia(object sender, object e)
        {
            this.barraEnergia.Value += 0.5;
            if (barraEnergia.Value >= 65)
            {
                this.dtTime.Stop();
            }
        }

        private void hacerCosquillasPata(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            // Animación para picoAbajo: hacia abajo
            DoubleAnimation daPico = new DoubleAnimation();
            daPico.From = 0;
            daPico.To = 40;  // Píxeles hacia abajo
            picoAbajo.RenderTransform = new TranslateTransform(); //Para escalar se usa ScaleTransform, para mover se usa TranslateTransform
            Storyboard.SetTarget(daPico, picoAbajo.RenderTransform);
            Storyboard.SetTargetProperty(daPico, "Y");
            sb.Children.Add(daPico);

            // Animación para Hoothoot: hacia arriba
            DoubleAnimation daHoothoot = new DoubleAnimation();
            daHoothoot.From = 0;
            daHoothoot.To = -80;  // Píxeles hacia arriba (negativo)
            Hoothoot.RenderTransform = new TranslateTransform();  // Asume Hoothoot es tu Viewbox
            Storyboard.SetTarget(daHoothoot, Hoothoot.RenderTransform);
            Storyboard.SetTargetProperty(daHoothoot, "Y");
            sb.Children.Add(daHoothoot);

            sb.AutoReverse = true;
            sb.RepeatBehavior = new RepeatBehavior(5);
            sb.Begin();
        }
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }

        private void PulsarCuerpo(object sender, PointerRoutedEventArgs e)
        {
            // 1. EJECUTA el Storyboard de alas EXISTENTE
            Storyboard sbAlas = (Storyboard)this.Resources["MoverAlas"];
            sbAlas.Begin();

            // 2. CREA NUEVO Storyboard para Hoothoot
            Storyboard sbHoothoot = new Storyboard();
            DoubleAnimation daHoothoot = new DoubleAnimation();
            daHoothoot.From = -20;
            daHoothoot.To = -70;
            daHoothoot.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Hoothoot.RenderTransform = new TranslateTransform();
            Storyboard.SetTarget(daHoothoot, Hoothoot.RenderTransform);
            Storyboard.SetTargetProperty(daHoothoot, "Y");

            sbHoothoot.Children.Add(daHoothoot);
            sbHoothoot.AutoReverse = true;
            sbHoothoot.RepeatBehavior = new RepeatBehavior(10);
            sbHoothoot.Begin();  // ← SOLO UNA vez
        }

        /*private void Atacar(object sender, RoutedEventArgs e)
        {
            Storyboard atqAlas = (Storyboard)this.Resources["AtacarPico"];
            atqAlas.Begin();

        }*/

        public void verFondo(bool ver)
        {
            if (ver){
                imgFondo.Visibility = Visibility.Visible;
            } else
            {
                imgFondo.Visibility = Visibility.Collapsed;
            }
        }

        public void verFilaVida(bool ver)
        {
            if (!ver) this.gdGeneral.RowDefinitions[0].Height = new GridLength(0);
            else this.gdGeneral.RowDefinitions[0].Height = new GridLength(50, GridUnitType.Pixel);
        }

        public void verFilaEnergia(bool ver)
        {
            if (!ver) this.gdGeneral.RowDefinitions[1].Height = new GridLength(0);
            else this.gdGeneral.RowDefinitions[1].Height = new GridLength(50, GridUnitType.Pixel);
        }

        public void verPocionVida(bool ver)
        {
            if (!ver)
            {
                imgPozCor.Visibility = Visibility.Collapsed;
                imgPozCor.IsHitTestVisible = false;
            }
            else if(pocionVida)
            {
                imgPozCor.Visibility = Visibility.Visible;
                imgPozCor.IsHitTestVisible = true;
            }
        }


        public void verPocionEnergia(bool ver)
        {
            if (!ver)
            {
                imgPozEner.Visibility = Visibility.Collapsed;
                imgPozEner.IsHitTestVisible = false;
            }
            else if (pocionEnergia)
            {
                imgPozEner.Visibility = Visibility.Visible;
                imgPozEner.IsHitTestVisible = true;
            }
        }

        public void verNombre(bool ver)
        {
            if (!ver) this.gdGeneral.RowDefinitions[3].Height = new GridLength(0);
            else this.gdGeneral.RowDefinitions[3].Height = new GridLength(50, GridUnitType.Pixel);
        }

        public void verEscudo(bool ver)
        {
            Storyboard verEscudo = (Storyboard)this.Resources["ActivarEscudo"];

            if (ver) {
                verEscudo.Begin();
            } else
            {
                verEscudo.Stop();
            }
            
        }

        public void activarAniIdle(bool activar)
        {
            Storyboard idle = (Storyboard)this.Resources["AnimIdle"];

            if (activar)
            {
                idle.Begin();
            }
            else
            {
                idle.Stop();
            }
        }

        public void animacionAtaqueFlojo()
        {
            PararTodasLasAnimaciones();

            Storyboard aturdir = (Storyboard)this.Resources["AnimAturdir"];
            aturdir.Begin();
        }

        public void animacionAtaqueFuerte()
        {
            PararTodasLasAnimaciones();
            Storyboard sbAlas = (Storyboard)this.Resources["MoverAlasLento"];
            Storyboard sbAtqFuerte = (Storyboard)this.Resources["AtqFuerte"];
            sbAlas.Begin();
            sbAtqFuerte.Begin();
        }

        public void animacionDefensa()
        {
            PararTodasLasAnimaciones();
            Storyboard defensa = (Storyboard)this.Resources["AnimDefensa"];
            defensa.Begin();
        }

        public void animacionDescasar()
        {
            PararTodasLasAnimaciones();
            Storyboard cerrarOjos = (Storyboard)this.Resources["AnimCerrarOjos"];
            Storyboard dormir = (Storyboard)this.Resources["AnimDormir"];
            cerrarOjos.Begin();
            dormir.Begin();
        }

        public void animacionCansado()
        {
            Storyboard cansado = (Storyboard)this.Resources["AnimCansado"];
            cansado.Begin();
        }

        public void animacionNoCansado()
        {
            Storyboard cansado = (Storyboard)this.Resources["AnimCansado"];
            cansado.Stop();
        }

        public void animacionHerido()
        {
            Storyboard herido = (Storyboard)this.Resources["AnimHerido"];
            herido.Begin();
        }

        public void animacionNoHerido()
        {
            Storyboard herido = (Storyboard)this.Resources["AnimHerido"];
            herido.Stop();
        }

        public void animacionDerrota()
        {
            PararTodasLasAnimaciones();
            Storyboard derrota = (Storyboard)this.Resources["AnimDerrota"];
            derrota.Begin();
        }

        private void PararTodasLasAnimaciones()
        {
            Storyboard alasLento = (Storyboard)this.Resources["MoverAlasLento"];
            Storyboard idle = (Storyboard)this.Resources["AnimIdle"];
            Storyboard cansado = (Storyboard)this.Resources["AnimCansado"];
            Storyboard herido = (Storyboard)this.Resources["AnimHerido"];
            Storyboard atqFuerte = (Storyboard)this.Resources["AtqFuerte"];
            Storyboard cerrarOjos = (Storyboard)this.Resources["AnimCerrarOjos"];
            Storyboard derrota = (Storyboard)this.Resources["AnimDerrota"];
            Storyboard dormir = (Storyboard)this.Resources["AnimDormir"];
            Storyboard defensa = (Storyboard)this.Resources["AnimDefensa"];

            string[] nombres = { "AnimAturdir", "MoverAlasLento", "AnimIdle", "AnimCansado", "AnimHerido", "AtqFuerte", "AnimCerrarOjos"
            , "AnimDerrota", "AnimDormir", "AnimDefensa"};

            foreach (string nombre in nombres)
            {
                if (this.Resources[nombre] is Storyboard sb)
                    sb.Stop();
            }
        }

    }
}
