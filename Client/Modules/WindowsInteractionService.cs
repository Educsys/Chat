using System;
using System.Collections.Generic;
using System.Windows;

namespace Client.Modules
{
    /// <summary>
    /// Реализация паттерна MVVM при работе с окнами
    /// </summary>
    public class WindowsInteractionService
    {
        Dictionary<Type, Type> RegisteredVMandWindows = new Dictionary<Type, Type>();
        Dictionary<object, Window> OpenedWindows = new Dictionary<object, Window>();


        public void RegisterVMandWindow<VM, Win>() where VM : class where Win : Window, new()
        {
            var VMType = typeof(VM);
            var WindowType = typeof(Win);
            if (RegisteredVMandWindows.ContainsKey(VMType))
                throw new InvalidOperationException($"{VMType.FullName} Already Registered");
            RegisteredVMandWindows[VMType] = WindowType;
        }

        public void UnRegisterVMandWindow<VM>() where VM: class
        {
            var VMType = typeof(VM);
            if (!RegisteredVMandWindows.ContainsKey(VMType))
                throw new InvalidOperationException($"{VMType.FullName} Not Registered");
            RegisteredVMandWindows.Remove(VMType);

        }

        private Window CreateWindowInstance(object vm)
        {
            if (vm == null)
                throw new NullReferenceException("VM cannot be null");
            
            RegisteredVMandWindows.TryGetValue(vm.GetType(), out Type WindowType);

            if (WindowType == null)
                throw new NullReferenceException($"Cannot find window. Window not registered {vm.GetType().FullName}");
            var window = (Window)Activator.CreateInstance(WindowType);
            window.DataContext = vm;
            if (window != App.Current.MainWindow)
            {
                window.Owner = App.Current.MainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            return window;
        }

        public void ShowModalWindow(object vm, Point? point = null)
        {
            if (vm == null)
                throw new NullReferenceException("VM cannot be null");
            var window = CreateWindowInstance(vm);
            if (point.HasValue)
            {
                window.Top = point.Value.Y;
                window.Left = point.Value.X;
            }             
            App.Current.Dispatcher.Invoke(() => window.ShowDialog());
        }

        public void ShowWindow(object vm)
        {
            var TypeVM = vm.GetType();
            if (!RegisteredVMandWindows.ContainsKey(TypeVM)) 
                throw new InvalidOperationException($"Dictionary not contains {TypeVM.FullName}");
            var window = CreateWindowInstance(vm);
            OpenedWindows.Add(vm, window);
            window.Show();
        }

        public void CloseWindow(object vm)
        {
            var TypeVM = vm.GetType();
            if (!RegisteredVMandWindows.ContainsKey(TypeVM))
                throw new InvalidOperationException($"Dictionary not contains {TypeVM.FullName}");
            if (!OpenedWindows.ContainsKey(vm))
                throw new InvalidOperationException($"Window is not openned");

            var window = OpenedWindows[vm];
            window.Close();
            OpenedWindows.Remove(vm);
            
        }
    }
}
