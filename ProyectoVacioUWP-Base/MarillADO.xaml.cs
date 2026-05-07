using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;


namespace ProyectoVacioUWP_Base
{
    public enum PokemonState
    {
        Idle,
        AttackingLight,
        AttackingHeavy,
        Hurt,
        Defending,
        Resting,
        Defeated
    }

    public sealed partial class MarillADO : UserControl, iPokemon
    {
        DispatcherTimer dtRestRecovery;
        DispatcherTimer dtTime;
        DispatcherTimer dtTime_e;

        Storyboard currentStoryboard;
        MediaPlayer mp;

        private PokemonState currentState = PokemonState.Idle;

        private bool cansado = false;
        private bool escudado = false;
        private bool isInitialized = false;

        // PROPIEDADES DE INTERFAZ
        public string Nombre { get; set; } = "Marill";
        public string Categoría { get; set; } = "Ratón Agua";
        public string Tipo { get; set; } = "Agua/Hada";
        public double Altura { get; set; } = 0.4;
        public double Peso { get; set; } = 8.5;
        public void silenciar(bool silenciar)
        {
            // Dejar vacío si este pokemon no tiene sonidos automáticos
        }
        public string Evolucion { get; set; } = "Azumarill";
        public string Descripcion { get; set; } = "La punta de su cola actúa como flotador.";

        public bool Cansado { get => cansado; set => cansado = value; }
        public bool Escudado { get => escudado; set => escudado = value; }

        public double Vida { get => pbHealth.Value; set => pbHealth.Value = value; }
        public double Energia { get => pbEnergy.Value; set => pbEnergy.Value = value; }

        public MarillADO()
        {
            InitializeComponent();

            mp = new MediaPlayer();

            dtRestRecovery = new DispatcherTimer();
            dtRestRecovery.Interval = TimeSpan.FromMilliseconds(200);
            dtRestRecovery.Tick += (s, e) =>
            {
                if (Energia < 100) Energia += 1;
                else dtRestRecovery.Stop();
            };

            pbEnergy.ValueChanged += pbEnergy_ValueChanged;
            pbHealth.ValueChanged += pbHealth_ValueChanged;

            isInitialized = true;

            EvaluateState();
        }

        private void EvaluateState()
        {
            if (!isInitialized) return;

            if (Vida <= 0)
            {
                ChangeState(PokemonState.Defeated);
                return;
            }

            if (currentState == PokemonState.Resting)
                return;

            if (Vida <= 30)
            {
                ChangeState(PokemonState.Hurt);
                return;
            }

            ChangeState(PokemonState.Idle);
        }

        private void ChangeState(PokemonState newState)
        {
            if (currentState == newState) return;

            currentState = newState;

            StopAllAnimations();
            ResetVisualesCaras();

            switch (newState)
            {
                case PokemonState.Idle:
                    PlayAnimation("Idle", true);
                    ApplyIdleSpeed();
                    break;

                case PokemonState.AttackingLight:
                    PlayAnimation("LightAttack", false, EvaluateState);
                    break;

                case PokemonState.AttackingHeavy:
                    PlayAnimation("HeavyAttack", false, EvaluateState);
                    break;

                case PokemonState.Hurt:
                    PlayAnimation("Hurt", false, EvaluateState);
                    break;

                case PokemonState.Resting:
                    dtRestRecovery.Start();
                    PlayAnimation("Defeated", true);
                    ApplySleepFace();
                    break;

                case PokemonState.Defeated:
                    dtRestRecovery.Stop();
                    PlayAnimation("Defeated", false);
                    break;
            }

            ApplyIdleSpeed();
        }

        private void StopAllAnimations()
        {
            currentStoryboard?.Stop();
        }

        private void PlayAnimation(string name, bool loop = false, Action onComplete = null)
        {
            try
            {
                var sb = (Storyboard)Resources[name];
                currentStoryboard = sb;

                if (!loop && onComplete != null)
                {
                    EventHandler<object> handler = null;
                    handler = (s, e) =>
                    {
                        sb.Completed -= handler;
                        onComplete();
                    };
                    sb.Completed += handler;
                }

                sb.Begin();
            }
            catch { }
        }

        private void ApplyIdleSpeed()
        {
            if (currentStoryboard != null)
                currentStoryboard.SpeedRatio = Cansado ? 1 : 1.5;
        }

        private void ResetVisualesCaras()
        {
            right_eye_shine.Visibility = Visibility.Visible;
            left_eye_shine.Visibility = Visibility.Visible;

            if (right_eye.RenderTransform is CompositeTransform ctRE)
            {
                ctRE.ScaleX = 1;
                ctRE.ScaleY = 1;
                ctRE.Rotation = 0;
                ctRE.TranslateX = 0;
                ctRE.TranslateY = 0;
            }

            if (left_eye.RenderTransform is CompositeTransform ctLE)
            {
                ctLE.ScaleX = 1;
                ctLE.ScaleY = 1;
                ctLE.Rotation = 0;
                ctLE.TranslateX = 0;
                ctLE.TranslateY = 0;
            }

            if (mouth.RenderTransform is CompositeTransform ctM)
            {
                ctM.ScaleX = 1;
                ctM.ScaleY = 1;
                ctM.TranslateX = 0;
                ctM.TranslateY = 0;
                ctM.Rotation = 0;
            }

            // Fuerza redibujado limpio
            right_eye.InvalidateArrange();
            left_eye.InvalidateArrange();
            mouth.InvalidateArrange();
        }

        private void ApplySleepFace()
        {
            right_eye_shine.Visibility = Visibility.Collapsed;
            left_eye_shine.Visibility = Visibility.Collapsed;

            if (right_eye.RenderTransform is CompositeTransform ctRE)
                ctRE.ScaleY = 0.2;

            if (left_eye.RenderTransform is CompositeTransform ctLE)
                ctLE.ScaleY = 0.2;

            if (mouth.RenderTransform is CompositeTransform ctM)
            {
                ctM.ScaleY = 0.2;
                ctM.TranslateY = 5;
            }
        }

        // MÉTODOS DE INTERFAZ

        public void animacionAtaqueFlojo()
        {
            if (Energia < 10 || currentState == PokemonState.Resting) return;
            Energia -= 10;
            ChangeState(PokemonState.AttackingLight);
        }

        public void animacionAtaqueFuerte()
        {
            if (Energia < 30 || currentState == PokemonState.Resting) return;
            Energia -= 30;
            ChangeState(PokemonState.AttackingHeavy);
        }

        public void animacionHerido()
        {
            if (currentState == PokemonState.Defeated) return;
            ChangeState(PokemonState.Hurt);
        }

        public void animacionNoHerido()
        {
            EvaluateState();
        }

        public void animacionDescasar()
        {
            if (currentState == PokemonState.Defeated) return;

            // Si ya está descansando → salir de descanso
            if (currentState == PokemonState.Resting)
            {
                dtRestRecovery.Stop();

                // Forzamos transición limpia
                if (Vida <= 30)
                    ChangeState(PokemonState.Hurt);
                else
                    ChangeState(PokemonState.Idle);

                return;
            }

            // Si no está descansando → entrar en descanso
            ChangeState(PokemonState.Resting);
        }

        public void animacionDefensa()
        {
            if (currentState == PokemonState.Defeated) return;

            Escudado = !Escudado;
            verEscudo(Escudado);
        }

        public void animacionDerrota()
        {
            ChangeState(PokemonState.Defeated);
        }

        public void activarAniIdle(bool activar)
        {
            if (activar) ChangeState(PokemonState.Idle);
            else StopAllAnimations();
        }

        public void animacionCansado()
        {
            Cansado = true;
            EvaluateState();
        }

        public void animacionNoCansado()
        {
            Cansado = false;
            EvaluateState();
        }

        public void Derrotado(bool valor)
        {
            if (valor) ChangeState(PokemonState.Defeated);
            else EvaluateState();
        }

        private void pbHealth_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isInitialized) return;
            EvaluateState();
        }

        private void pbEnergy_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isInitialized) return;

            Cansado = Energia < 30;

            if (currentState == PokemonState.Idle)
                ApplyIdleSpeed();
        }

        private void imRPotion_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            dtTime = new DispatcherTimer();
            dtTime.Interval = TimeSpan.FromMilliseconds(40);
            dtTime.Tick += (s, args) =>
            {
                pbHealth.Value += 1;
                if (pbHealth.Value >= 100) dtTime.Stop();
            };
            dtTime.Start();
            imRPotion.Visibility = Visibility.Collapsed;
        }

        private void imYPotion_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            dtTime_e = new DispatcherTimer();
            dtTime_e.Interval = TimeSpan.FromMilliseconds(40);
            dtTime_e.Tick += (s, args) =>
            {
                pbEnergy.Value += 1;
                if (pbEnergy.Value >= 100) dtTime_e.Stop();
            };
            dtTime_e.Start();
            imYPotion.Visibility = Visibility.Collapsed;
        }

        public void verFondo(bool ver)
        {
            imFondo.Source = ver ? new BitmapImage(new Uri("ms-appx:///AssetsMarillADO/background.png")) : null;
        }

        public void verFilaVida(bool ver)
        {
            pbHealth.Visibility = imCorazon.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verFilaEnergia(bool ver)
        {
            pbEnergy.Visibility = imEnergia.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verPocionVida(bool ver)
        {
            imRPotion.Source = ver ? new BitmapImage(new Uri("ms-appx:///AssetsMarillADO/red-potion.png")) : null;
        }

        public void verPocionEnergia(bool ver)
        {
            imYPotion.Source = ver ? new BitmapImage(new Uri("ms-appx:///AssetsMarillADO/yellow-potion.png")) : null;
        }

        public void verNombre(bool ver)
        {
            tbNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            Escudado = ver;
            imEscudo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}