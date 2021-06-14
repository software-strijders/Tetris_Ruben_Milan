using System.Windows;
using System.Windows.Input;

namespace TetrisClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private TetrisEngine _engine = new();
        
        public MainWindow()
        { 
            InitializeComponent();
            _engine.StartGame();
        }

        /// <summary>
        /// Checks if a rotation would cross the border and if so, corrects the position accordingly.
        /// </summary>
        /// <summary>
        /// C# function that triggers when a key is pressed.
        /// This is how the user controls the game
        /// </summary>
        /// <param name="e">pressed key</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Non in-game actions related keyboard controls
            switch (e.Key)
            {
                case Key.Enter:
                    _engine.Restart();
                    break;
                case Key.P:
                    _engine.Pause();
                    break;
                case Key.Escape:
                    Quit(null, null);
                    break;
            }

            //if (!_dpt.IsEnabled) return;

            // In-game actions
            switch (e.Key)
            {
                case Key.Right:
                    _engine.MoveRight();
                    break;
                case Key.Left:
                    _engine.MoveLeft();
                    break;
                //Rotate clockwise
                case Key.Up:
                    _engine.HandleRotation("UP");
                    break;
                //Rotate counter clockwise
                case Key.Down:
                    _engine.HandleRotation("DOWN");
                    break;
                //fully move down (hard drop)
                case Key.Space:
                    _engine.HardDrop();
                    break;
                //move down by one (soft drop)
                case Key.LeftShift:
                    _engine.SoftDrop();
                    break;
                default:
                    return;
            }
            _engine.RenderGrid();
        }
        
        private void Pause(object sender, RoutedEventArgs routedEventArgs)
        {
            pauseButton.Content = (string) pauseButton.Content == "Pause" ? "Resume" : "Pause";
            _engine.Pause();
        }
        
        private void Quit(object sender, RoutedEventArgs routedEventArgs) => Application.Current.Shutdown();
    }
}
