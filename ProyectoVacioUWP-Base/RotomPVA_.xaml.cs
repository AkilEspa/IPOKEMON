
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace ProyectoVacioUWP_Base
{
   
    public sealed partial class RotomXYZ : UserControl, iPokemon
    {
      
        private string _categoria = "Plasma";
        private string _tipo = "Eléctrico / Fantasma";
        private double _altura = 0.3;
        private double _peso = 0.3;
        private string _evolucion = "No tiene";
        private string _descripcion = "Su cuerpo está compuesto de plasma. Es conocido por infiltrarse en dispositivos electrónicos para causar todo tipo de travesuras.";

        public RotomXYZ()
        {
            this.InitializeComponent();
        }

      
        public double Vida
        {
            get => this.pbVida.Value;
            set => this.pbVida.Value = value;
        }

        public double Energia
        {
            get => this.pbEnergia.Value;
            set => this.pbEnergia.Value = value;
        }

        public string Nombre
        {
            get => this.txtNombre.Text;
            set => this.txtNombre.Text = value;
        }
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }
        // Devolvemos y actualizamos la información básica
        public string Categoría { get => _categoria; set => _categoria = value; }
        public string Tipo { get => _tipo; set => _tipo = value; }
        public double Altura { get => _altura; set => _altura = value; }
        public double Peso { get => _peso; set => _peso = value; }
        public string Evolucion { get => _evolucion; set => _evolucion = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }


   
      // MÉTODOS DE VISIBILIDAD DE LA INTERFAZ
       
        public void verFilaVida(bool ver) => this.panelVida.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

        public void verFilaEnergia(bool ver) => this.panelEnergia.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

        public void verNombre(bool ver) => this.txtNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;

        public void verEscudo(bool ver)
        {
            if (ver) animacionDefensa();
            else ((Storyboard)this.Resources["SinEscudo"]).Begin();
        }

        // Métodos vacíos (por si en el futuro añades fondo o pociones a Rotom)
        public void verFondo(bool ver) { /* this.FondoRotom.Visibility = ... */ }
        public void verPocionVida(bool ver) { }
        public void verPocionEnergia(bool ver) { }


        
        // MÉTODOS DE ANIMACIÓN (Llaman a los Storyboards)
       

        
        public void activarAniIdle(bool activar)
        {
            if (activar)
            {
                // Buscamos la nueva animación "AniIdle" y la iniciamos en bucle
                ((Storyboard)this.Resources["AniIdle"]).Begin();
            }
            else
            {
                // Detenemos la animación y Rotom se queda quieto
                ((Storyboard)this.Resources["AniIdle"]).Stop();
            }
        }

        public void animacionAtaqueFlojo()
        {
            // Arranca el movimiento
            ((Storyboard)this.Resources["AtaqueFlojo"]).Begin();

            // Reproduce el sonido
            if (sonidoAtaque != null)
            {
                sonidoAtaque.Play();
            }
        }

        public void animacionAtaqueFuerte()
        {
            // Arranca el movimiento y los rayos
            ((Storyboard)this.Resources["AtaqueFuerte"]).Begin();

            // Reproduce el sonido fuerte
            if (sonidoAtaqueFuerte != null)
            {
                sonidoAtaqueFuerte.Play();
            }
        }

        public void animacionDefensa() => ((Storyboard)this.Resources["Protegido"]).Begin();

        public void animacionDescasar() => ((Storyboard)this.Resources["Dormido"]).Begin();

        public void animacionCansado() => ((Storyboard)this.Resources["Cansado"]).Begin();

        public void animacionNoCansado() => ((Storyboard)this.Resources["Cansado"]).Stop();

        public void animacionHerido() => ((Storyboard)this.Resources["Herido"]).Begin();

        public void animacionNoHerido() => ((Storyboard)this.Resources["NoHerido"]).Begin();

        public void animacionDerrota()
        {
            // Usamos la animación de herido temporalmente para la derrota
            ((Storyboard)this.Resources["Herido"]).Begin();
        }
    }
}