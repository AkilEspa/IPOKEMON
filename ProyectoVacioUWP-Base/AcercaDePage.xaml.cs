using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ProyectoVacioUWP_Base
{
    public sealed partial class AcercaDePage : Page
    {
        private Page miPadre;

        public AcercaDePage()
        {
            this.InitializeComponent();
            this.Loaded += AcercaDePage_Loaded;
        }

        private void AcercaDePage_Loaded(object sender, RoutedEventArgs e)
        {
            iPokemon[] misDevs = { PokemonJoseFelix, PokemonDario, PokemonAkil };

            foreach (var poke in misDevs)
            {
                if (poke != null)
                {
                    poke.verFondo(false);
                    poke.verFilaVida(false);
                    poke.verFilaEnergia(false);
                    poke.verPocionVida(false);
                    poke.verPocionEnergia(false);
                    poke.verNombre(false);
                    poke.verEscudo(false);
                    poke.activarAniIdle(true);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            miPadre = e.Parameter as MainPage;
        }

        private async void CapturarDev_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StackPanel contenedor = (StackPanel)sender;
            Viewbox vb = contenedor.Children[0] as Viewbox;
            if (vb == null) return;

            iPokemon pokeInterface = vb.Child as iPokemon;
            Image bola = contenedor.Children[2] as Image;
            string infoDev = contenedor.Tag?.ToString() ?? "Desconocido";

            contenedor.IsHitTestVisible = false;

            if (pokeInterface != null)
            {
                pokeInterface.activarAniIdle(false);
                pokeInterface.animacionDerrota();
            }

            await Task.Delay(800);

            // Animación de Succión mejorada para que sea independiente del tamaño
            CompositeTransform trans = new CompositeTransform();
            vb.RenderTransform = trans;
            vb.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

            Storyboard sbSuccion = new Storyboard();
            DoubleAnimation animEscalaX = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(600), EasingFunction = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseIn } };
            DoubleAnimation animEscalaY = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(600), EasingFunction = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseIn } };
            DoubleAnimation animGiro = new DoubleAnimation { To = 720, Duration = TimeSpan.FromMilliseconds(600) };

            Storyboard.SetTarget(animEscalaX, vb);
            Storyboard.SetTargetProperty(animEscalaX, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(animEscalaY, vb);
            Storyboard.SetTargetProperty(animEscalaY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(animGiro, vb);
            Storyboard.SetTargetProperty(animGiro, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");

            sbSuccion.Children.Add(animEscalaX);
            sbSuccion.Children.Add(animEscalaY);
            sbSuccion.Children.Add(animGiro);
            sbSuccion.Begin();

            await Task.Delay(600);
            vb.Visibility = Visibility.Collapsed;

            if (bola != null)
            {
                bola.Visibility = Visibility.Visible;
                await VibrarBola(bola);
            }

            TxtLog.Text = $"¡HAS CAPTURADO A {infoDev.ToUpper()}!";
        }

        private async Task VibrarBola(Image bola)
        {
            if (!(bola.RenderTransform is CompositeTransform))
                bola.RenderTransform = new CompositeTransform();

            Storyboard sbVibrar = new Storyboard();
            DoubleAnimation animVibra = new DoubleAnimation
            {
                From = -25,
                To = 25,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3)
            };

            Storyboard.SetTarget(animVibra, bola);
            Storyboard.SetTargetProperty(animVibra, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");

            sbVibrar.Children.Add(animVibra);
            sbVibrar.Begin();

            await Task.Delay(700);

            DoubleAnimation salto = new DoubleAnimation
            {
                To = -20,
                Duration = TimeSpan.FromMilliseconds(200),
                AutoReverse = true
            };
            Storyboard.SetTarget(salto, bola);
            Storyboard.SetTargetProperty(salto, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            Storyboard sbSalto = new Storyboard();
            sbSalto.Children.Add(salto);
            sbSalto.Begin();
        }
    }
}