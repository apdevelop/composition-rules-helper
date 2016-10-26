namespace ScreenGrid.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class ScreenGridWindow : Window
    {
        // TODO: Better separation of concerns (MVVM pattern)
        // TODO: Disable rotate/flip buttons depending on grid type
        // TODO: Move window by cursor keys
        // TODO: Remove image files from resources
        // TODO: Resize window using sides handles

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

        private ContextMenu mainMenu;

        private ViewModels.ScreenGridViewModel vm;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CreateContextMenu();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                this.mainMenu.IsOpen = true;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                // http://stackoverflow.com/a/7418629/1182448
                this.DragMove();
            }
        }

        private void CreateContextMenu()
        {
            // TODO: use bindings to command with parameter
            this.mainMenu = new ContextMenu();

            foreach (var gr in Models.Grids.GridModeItem.List)
            {
                var menuItem = new MenuItem
                {
                    Header = gr.Title,
                    Tag = gr.GridMode,
                };

                menuItem.Click += (s, e) =>
                {
                    // Uncheck all others
                    foreach (var item in this.mainMenu.Items)
                    {
                        var mi = item as MenuItem;
                        if ((mi != null) && (mi.Tag != null))
                        {
                            mi.IsChecked = false;
                        }
                    }

                    menuItem.IsChecked = true;
                    this.vm.GridMode = (Models.Grids.GridType)((s as MenuItem).Tag);
                };

                this.mainMenu.Items.Add(menuItem);
            }

            this.mainMenu.Items.Add(new Separator());

            var itMinimize = new MenuItem { Header = "Minimize" };
            itMinimize.Click += this.btnMinimize_Click;
            this.mainMenu.Items.Add(itMinimize);

            var itClose = new MenuItem { Header = "Close" };
            itClose.Click += this.btnClose_Click;
            this.mainMenu.Items.Add(itClose);
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
            this.mainMenu.IsOpen = true;
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

        #region ResizeWindows

        private bool isResizeInProcess = false;

        private void ResizeInit(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                this.isResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void ResizeEnd(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                this.isResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void ResizingForm(object sender, MouseEventArgs e)
        {
            if (this.isResizeInProcess)
            {
                var senderRect = sender as Rectangle;
                if (senderRect != null)
                {
                    var window = senderRect.Tag as Window;
                    var width = e.GetPosition(window).X;
                    var height = e.GetPosition(window).Y;
                    senderRect.CaptureMouse();

                    var senderName = senderRect.Name.ToLower();

                    if (senderName.Contains("right"))
                    {
                        width++;
                        if (width > window.MinWidth)
                        {
                            window.Width = width;
                        }
                    }

                    if (senderName.Contains("left"))
                    {
                        width--;
                        
                        window.Left += width;
                        width = window.Width - width;
                        if (width > window.MinWidth)
                        {
                            window.Width = width;
                        }
                    }

                    if (senderName.Contains("bottom"))
                    {
                        height++;
                        if (height > window.MinHeight)
                        {
                            window.Height = height;
                        }
                    }

                    if (senderName.Contains("top"))
                    {
                        height -= ViewModels.ScreenGridViewModel.HeaderHeight;
                        height--;
                        window.Top += height;
                        height = window.Height - height;
                        if (height > window.MinHeight)
                        {
                            window.Height = height;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
