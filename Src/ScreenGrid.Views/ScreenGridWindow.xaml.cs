namespace ScreenGrid.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class ScreenGridWindow : Window
    {
        // TODO: Better separation of concerns (MVVM pattern)

        // TODO: Target to browser window
        // TODO: Frame proportions detection
        // TODO: Fix 'Golden Spiral' grid
        // TODO: Snap to TotalCommander Lister window
        // TODO: Disable rotate/flip buttons depends on grid type

        // TODO: Find corners by colors change in uniform color
        // TODO: 'Auto-snap' option on/off
        // TODO: Move window by cursor keys
        // TODO: Remove image files from resources
        // TODO: add grid color selection

        public ScreenGridWindow()
        {
            // TODO: check in high DPI modes

            // Making text sharp
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);

            // TODO: external ViewModel
            this.InitializeComponent();
            this.vm = new ViewModels.ScreenGridViewModel();
            this.DataContext = this.vm;
        }

        private ContextMenu cntxMenu;

        private ViewModels.ScreenGridViewModel vm;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cntxMenu = CreateContextMenu();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                this.cntxMenu.IsOpen = true;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                // http://stackoverflow.com/a/7418629/1182448
                this.DragMove();
            }
        }

        private ContextMenu CreateContextMenu()
        {
            var cntMenu = new ContextMenu();

            foreach (var gr in Models.Grids.GridModeItem.List)
            {
                var menuItem = new MenuItem() { Header = gr.Title, Tag = gr.GridMode, };
                menuItem.Click += (s, e) =>
                {
                    this.vm.GridMode = (Models.Grids.GridType)((s as MenuItem).Tag);
                };

                cntMenu.Items.Add(menuItem);
            }

            cntMenu.Items.Add(new Separator());

            var itMinimize = new MenuItem { Header = "Minimize" };
            itMinimize.Click += this.btnMinimize_Click;
            cntMenu.Items.Add(itMinimize);

            var itClose = new MenuItem { Header = "Close" };
            itClose.Click += this.btnClose_Click;
            cntMenu.Items.Add(itClose);

            return cntMenu;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            this.cntxMenu.IsOpen = true;
        }

        // TODO: public method
        public bool forceClosing = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.forceClosing)
            {
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
            }
        }
    }
}
