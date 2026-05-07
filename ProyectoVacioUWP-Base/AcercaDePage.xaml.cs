using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
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
        private bool esIngles = false;

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
                    poke.activarAniIdle(true);

                    // HACK DE SILENCIO (Reflexión) para que los Pokémon no griten al entrar
                    try
                    {
                        var campos = poke.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        foreach (var campo in campos)
                        {
                            if (campo.FieldType == typeof(Windows.Media.Playback.MediaPlayer))
                            {
                                var mp = campo.GetValue(poke) as Windows.Media.Playback.MediaPlayer;
                                if (mp != null) { mp.IsMuted = true; mp.Volume = 0; }
                            }
                            if (campo.Name == "timerSonido")
                            {
                                var timer = campo.GetValue(poke) as DispatcherTimer;
                                timer?.Stop();
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            miPadre = e.Parameter as MainPage;
        }

        // --- LÓGICA DE INTERNACIONALIZACIÓN ---
        private void BtnIdioma_Click(object sender, RoutedEventArgs e)
        {
            esIngles = !esIngles;
            ActualizarTextos();
        }

        private void ActualizarTextos()
        {
            if (esIngles)
            {
                TxtTitulo.Text = "CATCH THE DEVELOPERS";
                BtnIdioma.Label = "Español";
                if (TxtLog.Text.Contains("HAZ CLICK")) TxtLog.Text = "CLICK TO CATCH THEM...";
                else if (TxtLog.Text.Contains("HAS CAPTURADO")) TxtLog.Text = TxtLog.Text.Replace("¡HAS CAPTURADO A", "YOU CAUGHT").Replace("!", "");
            }
            else
            {
                TxtTitulo.Text = "CAPTURA LOS DESARROLLADORES";
                BtnIdioma.Label = "English";
                if (TxtLog.Text.Contains("CLICK TO")) TxtLog.Text = "HAZ CLICK PARA ATRAPARLOS...";
                else if (TxtLog.Text.Contains("YOU CAUGHT")) TxtLog.Text = TxtLog.Text.Replace("YOU CAUGHT", "¡HAS CAPTURADO A") + "!";
            }
        }

        private async void CapturarDev_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StackPanel contenedor = (StackPanel)sender;
            Viewbox vb = contenedor.Children[0] as Viewbox;
            Image bola = (contenedor.Children[2] as Grid)?.Children[0] as Image;
            string infoDev = contenedor.Tag?.ToString() ?? "Desconocido";

            if (vb == null || vb.Opacity == 0) return;

            contenedor.IsHitTestVisible = false;

            // 1. Animación de derrota
            if (vb.Child is iPokemon poke)
            {
                poke.activarAniIdle(false);
                poke.animacionDerrota();
            }

            await Task.Delay(700);

            // 2. Animación de succión
            CompositeTransform trans = new CompositeTransform();
            vb.RenderTransform = trans;
            vb.RenderTransformOrigin = new Point(0.5, 0.5);

            Storyboard sbSuccion = new Storyboard();
            DoubleAnimation animX = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(500) };
            DoubleAnimation animY = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(500) };
            DoubleAnimation animR = new DoubleAnimation { To = 360, Duration = TimeSpan.FromMilliseconds(500) };

            Storyboard.SetTarget(animX, vb);
            Storyboard.SetTargetProperty(animX, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(animY, vb);
            Storyboard.SetTargetProperty(animY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(animR, vb);
            Storyboard.SetTargetProperty(animR, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");

            sbSuccion.Children.Add(animX); sbSuccion.Children.Add(animY); sbSuccion.Children.Add(animR);
            sbSuccion.Begin();

            await Task.Delay(500);
            vb.Opacity = 0;

            // 3. Mostrar Pokéball y vibrar
            if (bola != null)
            {
                bola.Visibility = Visibility.Visible;
                await VibrarBola(bola);
            }

            // Texto de log con idioma aplicado
            if (esIngles) TxtLog.Text = $"YOU CAUGHT {infoDev.ToUpper()}";
            else TxtLog.Text = $"¡HAS CAPTURADO A {infoDev.ToUpper()}!";
        }

        private async Task VibrarBola(Image bola)
        {
            if (!(bola.RenderTransform is CompositeTransform)) bola.RenderTransform = new CompositeTransform();

            Storyboard sbVibrar = new Storyboard();
            DoubleAnimation vibra = new DoubleAnimation { From = -20, To = 20, Duration = TimeSpan.FromMilliseconds(100), AutoReverse = true, RepeatBehavior = new RepeatBehavior(3) };
            Storyboard.SetTarget(vibra, bola);
            Storyboard.SetTargetProperty(vibra, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");

            sbVibrar.Children.Add(vibra);
            sbVibrar.Begin();
            await Task.Delay(600);
        }
    }
}