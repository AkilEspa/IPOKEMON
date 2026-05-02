using System;
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

            // Cuando la página esté cargada, preparamos a los "sujetos"
            this.Loaded += AcercaDePage_Loaded;
        }

        private void AcercaDePage_Loaded(object sender, RoutedEventArgs e)
        {
            // Creamos un array para iterar sobre los 3 y no repetir código
            iPokemon[] misDevs = {
                PokemonJoseFelix,
                PokemonDario,
                PokemonAkil
            };

            foreach (var poke in misDevs)
            {
                if (poke != null)
                {
                    // 1. Limpieza de interfaz (usando los métodos de tu interfaz)
                    poke.verFondo(false);
                    poke.verFilaVida(false);
                    poke.verFilaEnergia(false);
                    poke.verPocionVida(false);
                    poke.verPocionEnergia(false);
                    poke.verNombre(false);
                    poke.verEscudo(false);

                    // 2. Activamos el modo relax
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

            // Buscamos el Pokémon dentro del Viewbox
            Viewbox vb = (Viewbox)contenedor.Children[0];
            iPokemon pokeInterface = vb.Child as iPokemon;
            FrameworkElement visualPokemon = (FrameworkElement)vb;

            Image bola = (Image)contenedor.Children[2];
            string infoDev = contenedor.Tag.ToString();

            // Bloqueamos el clic para evitar spam
            contenedor.IsHitTestVisible = false;

            // --- MOMENTO ÉPICO ---
            if (pokeInterface != null)
            {
                // El Pokémon se da cuenta de que va a ser capturado
                pokeInterface.activarAniIdle(false);
                pokeInterface.animacionDerrota();
            }

            // Esperamos un poco para que se vea el inicio de la animación de derrota
            await Task.Delay(800);

            // 1. Animación de Succión (Giro + Escala a 0)
            CompositeTransform trans = new CompositeTransform();
            visualPokemon.RenderTransform = trans;
            visualPokemon.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

            Storyboard sbSuccion = new Storyboard();
            DoubleAnimation animEscalaX = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(600) };
            DoubleAnimation animEscalaY = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(600) };
            DoubleAnimation animGiro = new DoubleAnimation { To = 1080, Duration = TimeSpan.FromMilliseconds(600) };

            Storyboard.SetTarget(animEscalaX, visualPokemon);
            Storyboard.SetTargetProperty(animEscalaX, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(animEscalaY, visualPokemon);
            Storyboard.SetTargetProperty(animEscalaY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(animGiro, visualPokemon);
            Storyboard.SetTargetProperty(animGiro, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");

            sbSuccion.Children.Add(animEscalaX);
            sbSuccion.Children.Add(animEscalaY);
            sbSuccion.Children.Add(animGiro);

            sbSuccion.Begin();

            await Task.Delay(600);
            visualPokemon.Visibility = Visibility.Collapsed;

            // 2. Aparece la bola y vibra
            bola.Visibility = Visibility.Visible;
            await VibrarBola(bola);

            // 3. Mensaje Final
            TxtLog.Text = $"¡HAS CAPTURADO A {infoDev}!";
        }

        private async Task VibrarBola(Image bola)
        {
            if (bola.RenderTransform == null || !(bola.RenderTransform is CompositeTransform))
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

            // Un pequeño salto final de la bola para indicar éxito
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